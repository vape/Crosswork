using UnityEngine;

namespace Crosswork.View.Sorting
{
    public static class CrossworkSortingUtility
    {
        public static void SetLayerAndOrder(MonoBehaviour behaviour, CrossworkSortingLayer layer, int order)
        {
            SetLayerAndOrder(behaviour.transform, layer, order);
        }

        public static void SetLayerAndOrder(Transform transform, CrossworkSortingLayer layer, int order)
        {
            foreach (var component in transform.GetComponentsInChildren<ISortingHandler>(includeInactive: true))
            {
                component.SetOrder(order);
                component.SetLayer(layer);
            }
        }

        public static void SetOrder(MonoBehaviour behaviour, int order)
        {
            SetOrder(behaviour.transform, order);
        }

        public static void SetOrder(Transform transform, int order)
        {
            foreach (var component in transform.GetComponentsInChildren<ISortingHandler>(includeInactive: true))
            {
                component.SetOrder(order);
            }
        }

        public static void SetLayer(MonoBehaviour behaviour, CrossworkSortingLayer layer)
        {
            SetLayer(behaviour.transform, layer);
        }

        public static void SetLayer(Transform transform, CrossworkSortingLayer layer)
        {
            foreach (var component in transform.GetComponentsInChildren<ISortingHandler>(includeInactive: true))
            {
                component.SetLayer(layer);
            }
        }

        public static void SetDefaultLayer(MonoBehaviour behaviour, CrossworkSortingLayer layer)
        {
            SetDefaultLayer(behaviour.transform, layer);
        }

        public static void SetDefaultLayer(Transform transform, CrossworkSortingLayer layer)
        {
            foreach (var component in transform.GetComponentsInChildren<ISortingHandler>(includeInactive: true))
            {
                component.SetDefaultLayer(layer);
            }
        }

        public static void ResetLayer(MonoBehaviour behaviour)
        {
            ResetLayer(behaviour.transform);
        }

        public static void ResetLayer(Transform transform)
        {
            foreach (var component in transform.GetComponentsInChildren<ISortingHandler>(includeInactive: true))
            {
                component.ResetLayer();
            }
        }
    }
}
