using Crosswork.Core;
using Crosswork.Core.Intents;
using Crosswork.View.Sorting;
using Crosswork.View.Tiling;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Crosswork.View
{
    public class CrossworkBoardView : MonoBehaviour, IBoardView
    {
        private static readonly Vector2 half = new Vector2(0.5f, 0.5f);

        private Camera BoardCamera
        {
            get
            {
                if (boardCamera != null)
                {
                    return boardCamera;
                }

                if (cachedMainCamera == null)
                {
                    cachedMainCamera = Camera.main;
                }

                return cachedMainCamera;
            }
        }

        [SerializeField]
        private Vector2 boardPivot = half;
        [SerializeField]
        private Camera boardCamera;
        [SerializeField]
        private Vector2 cellSize = Vector2.one;
        [SerializeField]
        private Transform elementsContainer;
        [SerializeField]
        private TilingGridController cellGrid;

        private Camera cachedMainCamera;
        private int width;
        private int height;
        private Dictionary<int, ElementView> views;
        private IBoardViewFactory viewFactory;
        private ISortingMethod sortingMethod;
        private bool initialized;

        public void Initialize(IBoardViewFactory viewFactory)
        {
            if (viewFactory == null)
            {
                throw new ArgumentNullException(nameof(viewFactory));
            }

            this.viewFactory = viewFactory;
            SetSortingMethod(CommonSortingMethod.UpSorting);

            initialized = true;
        }

        [Conditional("DEBUG"), Conditional("UNITY_EDITOR")]
        private void AssertInitialized()
        {
            if (!initialized)
            {
                throw new InvalidOperationException("Not initialized");
            }
        }

        public void Unload()
        {
            AssertInitialized();

            foreach (var view in views.Values)
            {
                if (view != null)
                {
                    viewFactory.PurgeView(view);
                }
            }

            views.Clear();

            width = 0;
            height = 0;
            cellGrid.Clear();
        }

        public void Load(Cell[,] cells)
        {
            AssertInitialized();

            width = cells.GetLength(1);
            height = cells.GetLength(0);
            views = new Dictionary<int, ElementView>(width * height);
            cellGrid.Render(boardPivot, cellSize, TilingGrid.Create(cells));
        }

        public void SetSortingMethod(ISortingMethod method)
        {
            sortingMethod = method;

            if (views != null && views.Count > 0)
            {
                foreach (var view in views.Values)
                {
                    var cell = view.Element.GetCell();
                    if (cell.Active)
                    {
                        UpdateSortingOrder(view, cell.Position);
                    }
                }
            }
        }

        public bool TryGetView(Element element, out ElementView view)
        {
            return views.TryGetValue(element.Id, out view);
        }

        public bool TryGetView(int elementId, out ElementView view)
        {
            return views.TryGetValue(elementId, out view);
        }

        public ElementView GetView(Element element)
        {
            if (TryGetView(element, out var view))
            {
                return view;
            }

            return null;
        }

        public ElementView GetView(int elementId)
        {
            if (TryGetView(elementId, out var view))
            {
                return view;
            }

            return null;
        }

        public void CreateView(Element element, IIntent intent)
        {
            AssertInitialized();

            var view = viewFactory.CreateView(element, elementsContainer);
            view.OnCreated(element, intent);
            views.Add(element.Id, view);
            ResetPosition(view);
        }

        public bool DestroyView(Element element, IIntent intent)
        {
            AssertInitialized();

            if (!views.TryGetValue(element.Id, out var view))
            {
                return false;
            }

            if (view != null)
            {
                view.OnDestroying(intent);
                viewFactory.PurgeView(view);
            }

            return true;
        }

        public bool ResetPosition(Element element)
        {
            AssertInitialized();

            if (views.TryGetValue(element.Id, out var view))
            {
                ResetPosition(view);
                return true;
            }

            return false;
        }

        public void ResetPosition(ElementView view)
        {
            var cell = view.Element.GetCell();
            if (!cell.Active)
            {
                return;
            }

            SetPosition(view, cell.Position);
        }

        private void SetPosition(ElementView view, Vector2Int position)
        {
            view.transform.position = GridToWorldPosition(position);
            UpdateSortingOrder(view, position);
        }

        private void UpdateSortingOrder(ElementView view, Vector2Int position)
        {
            if (sortingMethod != null)
            {
                CrossworkSortingUtility.SetOrder(view, sortingMethod.GetOrder(position.x, position.y));
            }
        }

        #region Transforms

        public Bounds GetLocalBounds()
        {
            var a = GridToLocalPosition(0, 0, Vector2.zero);
            var b = GridToLocalPosition(width, height, Vector2.zero);

            return new Bounds(b + (a - b) * 0.5f, a - b);
        }

        public Bounds GetWorldBounds()
        {
            var a = LocalToWorldPosition(GridToLocalPosition(0, 0, Vector2.zero));
            var b = LocalToWorldPosition(GridToLocalPosition(width, height, Vector2.zero));

            return new Bounds(a + (b - a) * 0.5f, b - a);
        }

        // --------------

        public Vector3 ScreenToLocalPosition(Vector2 screenPosition)
        {
            return WorldToLocalPosition(BoardCamera.ScreenToWorldPoint(screenPosition));
        }

        public Vector3 ScreenToWorldPosition(Vector2 screenPosition)
        {
            var local = WorldToLocalPosition(BoardCamera.ScreenToWorldPoint(screenPosition));
            local.z = 0;
            return LocalToWorldPosition(local);
        }

        public Vector2Int ScreenToGridPosition(Vector2 screenPosition)
        {
            return WorldToGridPosition(BoardCamera.ScreenToWorldPoint(screenPosition));
        }

        // --------------

        public Vector3 WorldToLocalPosition(Vector3 worldPosition)
        {
            return transform.InverseTransformPoint(worldPosition);
        }

        public Vector2Int WorldToGridPosition(Vector3 worldPosition)
        {
            return LocalToGridPosition(WorldToLocalPosition(worldPosition));
        }

        // --------------

        public Vector3 LocalToWorldPosition(Vector3 local)
        {
            return transform.TransformPoint(local);
        }

        public Vector2Int LocalToGridPosition(Vector3 localPosition)
        {
            return new Vector2Int(
                Mathf.FloorToInt((localPosition.x / cellSize.x) + (width * boardPivot.x)),
                Mathf.FloorToInt((localPosition.y / cellSize.y) + (height * boardPivot.y)));
        }

        // --------------

        public static Vector3 GridToLocalPosition(float x, float y, int width, int height, Vector2 cellSize, Vector2 cellPivot, Vector2 boardPivot)
        {
            return new Vector3(
                (cellSize.x * cellPivot.x) + x * cellSize.x - (cellSize.x * width * boardPivot.x),
                (cellSize.y * cellPivot.y) + y * cellSize.y - (cellSize.y * height * boardPivot.y));
        }

        public static Vector3 GridToLocalPosition(int x, int y, int width, int height, Vector2 cellSize, Vector2 cellPivot, Vector2 boardPivot)
        {
            return new Vector3(
                (cellSize.x * cellPivot.x) + x * cellSize.x - (cellSize.x * width * boardPivot.x),
                (cellSize.y * cellPivot.y) + y * cellSize.y - (cellSize.y * height * boardPivot.y));
        }

        public Vector3 GridToLocalPosition(Vector2 gridPosition)
        {
            return GridToLocalPosition(gridPosition.x, gridPosition.y);
        }

        public Vector3 GridToLocalPosition(Vector2Int gridPosition)
        {
            return GridToLocalPosition(gridPosition.x, gridPosition.y);
        }

        public Vector3 GridToLocalPosition(int x, int y)
        {
            return GridToLocalPosition(x, y, width, height, cellSize, half, boardPivot);
        }

        public Vector3 GridToLocalPosition(float x, float y)
        {
            return GridToLocalPosition(x, y, width, height, cellSize, half, boardPivot);
        }

        public Vector3 GridToLocalPosition(int x, int y, Vector2 cellPivot)
        {
            return GridToLocalPosition(x, y, width, height, cellSize, cellPivot, boardPivot);
        }

        public Vector3 GridToWorldPosition(int x, int y)
        {
            return LocalToWorldPosition(GridToLocalPosition(x, y));
        }

        public Vector3 GridToWorldPosition(Vector2Int gridPosition)
        {
            return GridToWorldPosition(gridPosition.x, gridPosition.y);
        }

        #endregion
    }
}
