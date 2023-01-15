using UnityEngine;
using UnityEngine.Rendering;

namespace Crosswork.View.Sorting
{
    [ExecuteAlways]
    public class CrossworkSorting : MonoBehaviour, ISortingHandler
    {
        [SerializeField]
        private SortingGroup sortingGroup;
        [SerializeField]
        private CrossworkSortingLayer layer;
        [SerializeField]
        private int orderOffset;

        private int order;
        private CrossworkSortingLayer defaultLayer;

        private void Awake()
        {
            defaultLayer = layer;
        }

        private void OnEnable()
        {
            Sync();
        }

        private void OnDisable()
        {
            if (sortingGroup != null && (sortingGroup.hideFlags | HideFlags.NotEditable) != 0)
            {
                sortingGroup.hideFlags &= ~HideFlags.NotEditable;
            }
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                defaultLayer = layer;
            }
#endif

            Sync();
        }

        private void Sync()
        {
            if (sortingGroup == null)
            {
                return;
            }

            if ((sortingGroup.hideFlags & HideFlags.NotEditable) == 0)
            {
                sortingGroup.hideFlags |= HideFlags.NotEditable;
            }

            sortingGroup.sortingLayerID = CrossworkSortingConfigCache.FindLayer(layer).Id;
            sortingGroup.sortingOrder = order + orderOffset;
        }

        public void SetOrder(int order)
        {
            this.order = order;

            Sync();
        }

        public void SetLayer(CrossworkSortingLayer layer)
        {
            this.layer = layer;

            Sync();
        }

        public void ResetLayer()
        {
            layer = defaultLayer;

            Sync();
        }

        public void SetDefaultLayer(CrossworkSortingLayer layer)
        {
            defaultLayer = layer;
        }
    }
}
