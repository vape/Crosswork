using Crosswork.Core.Intents;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Crosswork.Core
{
    public class CrossworkBoard
    {
        const int MaxElementsPerCell = 8;

        public int Width
        { get { return width; } }
        public int Height
        { get { return height; } }
        public bool Loaded
        { get { return loaded; } }

        public IBoardView View
        { get { return view; } }
        public Cell[,] Cells
        { get { return cells; } }
        public Bucket[,] Buckets
        { get { return buckets; } }

        private IBoardView view;
        private IBoardFactory factory;
        private bool loaded;
        private int width;
        private int height;
        private Bucket[,] buckets;
        private Cell[,] cells;
        private Dictionary<int, ulong> elementsLocks;
        private ulong[,] cellLocks;

        // start ids from '1' so '0' can be used as default value
        private int idGen = 1;

        public CrossworkBoard(IBoardView view, IBoardFactory factory)
        {
            this.view = view;
            this.factory = factory;
        }

        public void Load(IBoardModel model)
        {
            if (loaded)
            {
                throw new InvalidOperationException("Already loaded.");
            }

            width = model.Width;
            height = model.Height;
            buckets = new Bucket[model.Height, model.Width];
            cells = new Cell[model.Height, model.Width];
            cellLocks = new ulong[model.Height, model.Width];

            for (int y = 0; y < model.Height; ++y)
            {
                for (int x = 0; x < model.Width; ++x)
                {
                    if (!model.TryGetCellModel(x, y, out var data))
                    {
                        cells[y, x] = new Cell(new Vector2Int(x, y), false);
                        continue;
                    }

                    buckets[y, x] = new Bucket(MaxElementsPerCell);
                    cells[y, x] = new Cell(new Vector2Int(x, y), true);
                }
            }

            if (elementsLocks == null || elementsLocks.Count < width * height)
            {
                elementsLocks = new Dictionary<int, ulong>(capacity: width * height);
            }

            view.Load(cells);

            for (int y = 0; y < model.Height; ++y)
            {
                for (int x = 0; x < model.Width; ++x)
                {
                    if (!model.TryGetCellModel(x, y, out var cell) || cell.Elements == null)
                    {
                        continue;
                    }

                    for (int k = 0; k < cell.Elements.Length; ++k)
                    {
                        if (!TryCreateElement(cell.Elements[k], x, y, out _, LoadingIntent.Instance))
                        {
                            throw new Exception($"Failed to create element {cell.Elements[k]} at {{{x},{y}}}");
                        }
                    }
                }
            }

            loaded = true;
        }

        public void Unload()
        {
            if (!loaded)
            {
                throw new InvalidOperationException("Board not loaded.");
            }

            loaded = false;
        }

        public void Save(IWritableBoardModel model)
        {
            if (!loaded)
            {
                throw new Exception("Board is not loaded.");
            }

            model.SetCells(cells);

            var models = new IElementModel[MaxElementsPerCell];

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    if (!cells[y, x].Active)
                    {
                        continue;
                    }

                    ref var bucket = ref buckets[y, x];
                    var count = 0;

                    for (int i = 0; i < bucket.Count; ++i)
                    {
                        if (bucket.Elements[i] is SlaveElement)
                        {
                            continue;
                        }

                        models[count++] = bucket.Elements[i].GetModel();
                    }

                    model.SetElements(x, y, models, count);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Bucket GetBucketAt(int x, int y)
        {
            return ref buckets[y, x];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Bucket GetBucketAt(Vector2Int position)
        {
            return ref buckets[position.y, position.x];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref Cell GetCellAt(int x, int y)
        {
            return ref cells[y, x];
        }

        public IEnumerable<Element> GetElementsAt(int x, int y)
        {
            if (!InsideBounds(x, y) || !cells[y, x].Active)
            {
                yield break;
            }

            for (int i = 0; i < buckets[y, x].Count; ++i)
            {
                yield return buckets[y, x].Elements[i];
            }
        }

        public IEnumerable<Element> GetElementsAt(Vector2Int position)
        {
            return GetElementsAt(position.x, position.y);
        }

        private void InitializeElement(Element element)
        {
            element.id = idGen++;
            element.OnCreated(this);
        }

        private Element ProduceElement(IElementModel model)
        {
            var element = factory.CreateElement(model);
            InitializeElement(element);

            return element;
        }

        private SlaveElement ProduceSlaveElement(Element master, Vector2Int offset)
        {
            var element = master.CreateSlave(offset);
            InitializeElement(element);

            return element;
        }

        private void CreateElementInternal(Element element, int x, int y, IIntent intent)
        {
            AttachElement(element, x, y);
            view.CreateView(element, intent);
        }

        public bool CanCreateElement(IElementModel model, int x, int y)
        {
            return CanCreateElement(model, x, y, out _);
        }

        public bool CanCreateElement(IElementModel model, Vector2Int position)
        {
            return CanCreateElement(model, position.x, position.y, out _);
        }

        internal bool CanCreateElement(IElementModel model, int x, int y, out Element element)
        {
            if (!HasSpaceForElement(x, y))
            {
                element = default;
                return false;
            }

            element = ProduceElement(model);
            return CanAttachElement(element, x, y);
        }

        public Element CreateElement(IElementModel model, int x, int y, IIntent intent = default)
        {
            var element = ProduceElement(model);
            CreateElementInternal(element, x, y, intent);
            return element;
        }

        public Element CreateElement(IElementModel model, Vector2Int position, IIntent intent = default)
        {
            return CreateElement(model, position.x, position.y, intent);
        }

        public bool TryCreateElement(IElementModel model, int x, int y, out Element element, IIntent intent = default)
        {
            if (!CanCreateElement(model, x, y, out element))
            {
                return false;
            }

            CreateElementInternal(element, x, y, intent);
            return true;
        }

        public bool TryCreateElement<T>(IElementModel model, int x, int y, out T element, IIntent intent = null)
            where T : Element
        {
            if (!TryCreateElement(model, x, y, out var baseElement, intent))
            {
                element = default;
                return false;
            }

            element = baseElement as T;
            return true;
        }

        public bool TryCreateElement(IElementModel model, int x, int y, IIntent intent = default)
        {
            return TryCreateElement(model, x, y, out _, intent);
        }

        public bool TryCreateElement(IElementModel model, Vector2Int position, out Element element, IIntent intent = default)
        {
            return TryCreateElement(model, position.x, position.y, out element, intent);
        }

        public bool TryCreateElement(IElementModel model, Vector2Int position, IIntent intent = default)
        {
            return TryCreateElement(model, position.x, position.y, intent);
        }

        public bool CanDestroyElement(Element element)
        {
            if (element is SlaveElement)
            {
                return false;
            }

            return CanDetachElement(element);
        }

        public void DestroyElement(Element element, IIntent intent = default)
        {
            DetachElement(element);
            element.OnDestroyed();

            view.DestroyView(element, intent);
        }

        public bool TryDestroyElement(Element element, IIntent intent = default)
        {
            if (!CanDestroyElement(element))
            {
                return false;
            }

            DestroyElement(element, intent);
            return true;
        }

        private bool CanDetachElement(Element element)
        {
            var cell = element.GetCell();
            if (!cell.Active)
            {
                return false;
            }

            if (IsElementLocked(element))
            {
                return false;
            }

            var pattern = element.GetPattern();
            if (pattern.Length > 1 || pattern != Element.SingleCellPattern)
            {
                for (int i = 0; i < pattern.Length; ++i)
                {
                    ref var bucket = ref buckets[cell.Position.y + pattern[i].y, cell.Position.x + pattern[i].x];

                    if (TryFindSlaveInBucket(ref bucket, element.Id, out var index) && IsElementLocked(bucket.Elements[index]))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private void DetachElement(Element element)
        {
            void DetachSlave(ref Bucket bucket, int index)
            {
                var slave = bucket.Elements[index];
                bucket.Remove(index);
                slave.OnCellChanged(default);
                slave.OnDestroyed();
            }

            var position = element.GetCell().Position;

            buckets[position.y, position.x].RemoveById(element.Id);
            element.OnCellChanged(default);

            var pattern = element.GetPattern();
            if (pattern.Length > 1 || pattern != Element.SingleCellPattern)
            {
                for (int i = 0; i < pattern.Length; ++i)
                {
                    if (pattern[i] == Vector2Int.zero)
                    {
                        continue;
                    }

                    ref var bucket = ref buckets[position.y + pattern[i].y, position.x + pattern[i].x];

                    if (TryFindSlaveInBucket(ref bucket, element.Id, out var index))
                    {
                        DetachSlave(ref bucket, index);
                    }
                }
            }
        }

        private bool HasSpaceForElement(int x, int y)
        {
            return InsideBounds(x, y) && cells[y, x].Active && !IsCellLocked(x, y) && !buckets[y, x].IsFull();
        }

        private bool IsColliding(int x, int y, ulong mask)
        {
            ref var bucket = ref buckets[y, x];

            for (int i = 0; i < bucket.Count; ++i)
            {
                // TODO: Make it cache friendly somehow
                var testingMask = bucket.Elements[i].GetCollisionMask();
                if ((mask & testingMask) != 0)
                {
                    return true;
                }
            }

            return false;
        }

        private bool CanAttachElement(Element element, int x, int y)
        {
            var mask = element.GetCollisionMask();

            if (!HasSpaceForElement(x, y) || IsColliding(x, y, mask))
            {
                return false;
            }

            var pattern = element.GetPattern();
            if (pattern.Length > 1 || pattern != Element.SingleCellPattern)
            {
                for (int i = 0; i < pattern.Length; ++i)
                {
                    var xx = x + pattern[i].x;
                    var yy = y + pattern[i].y;

                    if (!HasSpaceForElement(xx, yy) || IsColliding(xx, yy, mask))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private bool CanAttachElement(Element element, Vector2Int position)
        {
            return CanAttachElement(element, position.x, position.y);
        }

        private void AttachElement(Element element, int x, int y)
        {
            void AttachSlave(ref Bucket bucket, in Cell cell, SlaveElement slave)
            {
                bucket.Add(slave);
                slave.OnCellChanged(cell);
            }

            buckets[y, x].Add(element);
            element.OnCellChanged(cells[y, x]);

            var pattern = element.GetPattern();
            if (pattern.Length > 1 || pattern != Element.SingleCellPattern)
            {
                for (int i = 0; i < pattern.Length; ++i)
                {
                    if (pattern[i] == Vector2Int.zero)
                    {
                        continue;
                    }

                    var sx = x + pattern[i].x;
                    var sy = y + pattern[i].y;

                    var slave = ProduceSlaveElement(element, pattern[i]);
                    ref var slaveBucket = ref buckets[sy, sx];

                    AttachSlave(ref slaveBucket, in cells[sy, sx], slave);
                }
            }
        }

        private void AttachElement(Element element, Vector2Int position)
        {
            AttachElement(element, position.x, position.y);
        }

        public bool CanMoveElement(Element element, int x, int y)
        {
            if (element is SlaveElement)
            {
                return false;
            }

            if (!CanDetachElement(element))
            {
                return false;
            }

            var originalPosition = element.GetCell().Position;
            DetachElement(element);

            var canAttach = CanAttachElement(element, x, y);
            AttachElement(element, originalPosition);

            return canAttach;
        }

        public void MoveElement(Element element, int x, int y)
        {
            DetachElement(element);
            AttachElement(element, x, y);

            view.ResetPosition(element);
        }

        public bool TryMoveElement(Element element, int x, int y)
        {
            if (!CanMoveElement(element, x, y))
            {
                return false;
            }

            MoveElement(element, x, y);
            return true;
        }

        public bool CanSwapElements(Element e0, Element e1)
        {
            if (e0 == e1 || e0 is SlaveElement || e1 is SlaveElement)
            {
                return false;
            }

            if (!CanDetachElement(e0) || !CanDetachElement(e1))
            {
                return false;
            }

            var p0 = e0.GetCell().Position;
            var p1 = e1.GetCell().Position;

            DetachElement(e0);
            DetachElement(e1);

            var result = false;

            if (CanAttachElement(e0, p1))
            {
                AttachElement(e0, p1);
                result = CanAttachElement(e1, p0);
                DetachElement(e0);
            }

            AttachElement(e0, p0);
            AttachElement(e1, p1);

            return result;
        }

        public void SwapElements(Element e0, Element e1)
        {
            var p0 = e0.GetCell().Position;
            var p1 = e1.GetCell().Position;

            DetachElement(e0);
            DetachElement(e1);

            AttachElement(e0, p1);
            AttachElement(e1, p0);

            view.ResetPosition(e0);
            view.ResetPosition(e1);
        }

        public bool TrySwapElements(Element e0, Element e1)
        {
            if (!CanSwapElements(e0, e1))
            {
                return false;
            }

            SwapElements(e0, e1);
            return true;
        }

        private bool TryFindSlaveInBucket(ref Bucket bucket, int masterId, out int index)
        {
            for (int i = 0; i < bucket.Elements.Length; ++i)
            {
                var slave = bucket.Elements[i] as SlaveElement;
                if (slave == null)
                {
                    continue;
                }

                if (slave.Master.Id == masterId)
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        private bool TryFindElement(int id, out Element element)
        {
            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    if (!buckets[y, x].TryFind(id, out var index))
                    {
                        continue;
                    }

                    element = buckets[y, x].Elements[index];
                    return true;
                }
            }

            element = default;
            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InsideBounds(Vector2Int position)
        {
            return InsideBounds(position.x, position.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool InsideBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < width && y < height;
        }

        public ElementLockKey LockElement(int id, [CallerMemberName] string descriptor = null)
        {
            const int MaxBit = sizeof(ulong) * 8;

            if (!elementsLocks.TryGetValue(id, out var flags))
            {
                elementsLocks.Add(id, 0);
            }

            var bit = 0;
            while ((flags & (1UL << bit)) != 0)
            {
                bit++;
            }


            if (bit >= MaxBit)
            {
                if (TryFindElement(id, out var element))
                {
                    throw new InvalidOperationException($"Failed to create lock on element {element}, too much locks");
                }
                else
                {
                    throw new InvalidOperationException($"Failed to create lock on element {id}, too much locks");
                }
            }

            var lockFlag = 1UL << bit;
            flags |= lockFlag;
            elementsLocks[id] = flags;

            return new ElementLockKey(id, lockFlag, descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ElementLockKey LockElement(Element element, [CallerMemberName] string descriptor = null)
        {
            return LockElement(element.Id, descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool UnlockElement(ElementLockKey key)
        {
            if (!elementsLocks.TryGetValue(key.ElementId, out var flags))
            {
                return false;
            }

            if ((flags & key.Flag) == 0)
            {
                return false;
            }

            flags &= ~key.Flag;
            elementsLocks[key.ElementId] = flags;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsElementLocked(int elementId)
        {
            return elementsLocks.TryGetValue(elementId, out var flag) && flag != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsElementLocked(Element element)
        {
            return IsElementLocked(element.Id);
        }

        public CellLockKey LockCell(int x, int y, [CallerMemberName] string descriptor = null)
        {
            const int MaxBit = sizeof(ulong) * 8;

            var flags = cellLocks[y, x];

            var bit = 0;
            while ((flags & (1UL << bit)) != 0)
            {
                bit++;
            }

            if (bit >= MaxBit)
            {
                throw new InvalidOperationException($"Failed to create lock on cell {cells[y, x]}, too much locks");
            }

            var lockFlag = 1UL << bit;
            flags |= lockFlag;
            cellLocks[y, x] = flags;

            return new CellLockKey(x, y, lockFlag, descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CellLockKey LockCell(Vector2Int position, [CallerMemberName] string descriptor = null)
        {
            return LockCell(position.x, position.y, descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public CellLockKey LockCell(Cell cell, [CallerMemberName] string descriptor = null)
        {
            return LockCell(cell.Position, descriptor);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool UnlockCell(CellLockKey key)
        {
            var flags = cellLocks[key.Y, key.X];

            if ((flags & key.Flag) == 0)
            {
                return false;
            }

            flags &= ~key.Flag;
            cellLocks[key.Y, key.X] = flags;

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCellLocked(int x, int y)
        {
            return cellLocks[y, x] != 0;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsCellLocked(Vector2Int position)
        {
            return cellLocks[position.y, position.x] != 0;
        }
    }
}
