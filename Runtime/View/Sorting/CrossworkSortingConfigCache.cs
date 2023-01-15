using System.Collections.Generic;
using UnityEngine;

namespace Crosswork.View.Sorting
{
    public static class CrossworkSortingConfigCache
    {
        private static Dictionary<string, UnitySortingLayerWrapper> nameToSortingLayer;
        private static Dictionary<CrossworkSortingLayer, UnitySortingLayerWrapper> idToSortingLayer;
        private static Dictionary<string, CrossworkSortingLayer> nameToCrossworkLayer;

        public static void Invalidate()
        {
            nameToSortingLayer = null;
            idToSortingLayer = null;
            nameToCrossworkLayer = null;
        }

        private static void EnsureCache()
        {
            if (nameToSortingLayer != null && idToSortingLayer != null && nameToCrossworkLayer != null)
            {
                return;
            }

            var config = CrossworkSortingConfig.Instance;
            if (config == null)
            {
                Debug.LogError("Crosswork sorting confing not found. Make sure config exists and is inside Resources folder");
                return;
            }

            nameToSortingLayer = new Dictionary<string, UnitySortingLayerWrapper>(config.Layers.Length);
            idToSortingLayer = new Dictionary<CrossworkSortingLayer, UnitySortingLayerWrapper>(config.Layers.Length);
            nameToCrossworkLayer = new Dictionary<string, CrossworkSortingLayer>(config.Layers.Length);

            for (int i = 0; i < config.Layers.Length; ++i)
            {
                nameToSortingLayer.Add(config.Layers[i].Name, config.Layers[i].SortingLayer);
                idToSortingLayer.Add(config.Layers[i].Id, config.Layers[i].SortingLayer);
                nameToCrossworkLayer.Add(config.Layers[i].Name, new CrossworkSortingLayer(config.Layers[i].Id));
            }
        }

        public static CrossworkSortingLayer FindCrossworkLayer(string crossworkLayerName)
        {
            EnsureCache();

            if (nameToCrossworkLayer.TryGetValue(crossworkLayerName, out var crossworkLayer))
            {
                return crossworkLayer;
            }

            Debug.LogWarning($"Missing sorting layer {crossworkLayerName}");

            return new CrossworkSortingLayer(0);
        }

        public static UnitySortingLayerWrapper FindLayer(string crossworkLayerName)
        {
            EnsureCache();

            if (nameToSortingLayer.TryGetValue(crossworkLayerName, out var unitySortingLayer))
            {
                return unitySortingLayer;
            }

            Debug.LogWarning($"Missing sorting layer {crossworkLayerName}");

            return new UnitySortingLayerWrapper(0);
        }

        public static UnitySortingLayerWrapper FindLayer(CrossworkSortingLayer crossworkSortingLayer)
        {
            EnsureCache();

            if (idToSortingLayer.TryGetValue(crossworkSortingLayer, out var unitySortingLayer))
            {
                return unitySortingLayer;
            }

            Debug.LogWarning($"Missing sorting layer {crossworkSortingLayer}");

            return new UnitySortingLayerWrapper(0);
        }
    }
}
