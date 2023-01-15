using System;
using UnityEngine;

namespace Crosswork.View.Sorting
{
    [Serializable]
    public struct CrossworkSortingLayer
    {
        [SerializeField]
        public int Id;

        public CrossworkSortingLayer(string name)
        {
            Id = CrossworkSortingConfigCache.FindCrossworkLayer(name);
        }

        public CrossworkSortingLayer(int id)
        {
            Id = id;
        }

        public static implicit operator int(CrossworkSortingLayer layer)
        {
            return layer.Id;
        }

        public static implicit operator CrossworkSortingLayer(int layerId)
        {
            return new CrossworkSortingLayer(layerId);
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}
