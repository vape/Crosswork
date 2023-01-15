using System;
using System.Collections.Generic;
using UnityEngine;

namespace Crosswork.View.Sorting
{
    [CreateAssetMenu(fileName = AssetName, menuName = "Crosswork/View/Sorting Config")]
    public class CrossworkSortingConfig : ScriptableObject
    {
        [Serializable]
        public struct LayerData
        {
            public string Name;
            public int Id;
            public UnitySortingLayerWrapper SortingLayer;
        }

        public const string AssetName = "Crosswork Sorting Config";

        public static CrossworkSortingConfig Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Resources.Load<CrossworkSortingConfig>(AssetName);
                }

                return instance;
            }
        }

        private static CrossworkSortingConfig instance;
        private static System.Random random = new System.Random();
        private static HashSet<int> ids = new HashSet<int>();

        public LayerData[] Layers => layers;

        [SerializeField]
        private LayerData[] layers;

        private void OnValidate()
        {
            ids.Clear();

            for (int i = 0; i < layers.Length; ++i)
            {
                if (layers[i].Id == 0 || ids.Contains(layers[i].Id))
                {
                    layers[i].Id = random.Next(int.MinValue, int.MaxValue);
                }

                ids.Add(layers[i].Id);
            }

            CrossworkSortingConfigCache.Invalidate();
        }

        public LayerData GetLayerDataById(int id)
        {
            for (int i = 0; i < layers.Length; ++i)
            {
                if (layers[i].Id == id)
                {
                    return layers[i];
                }
            }

            return default;
        }

        public LayerData GetLayerDataByName(string name)
        {
            for (int i = 0; i < layers.Length; ++i)
            {
                if (layers[i].Name == name)
                {
                    return layers[i];
                }
            }

            return default;
        }
    }
}
