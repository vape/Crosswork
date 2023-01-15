using System.ComponentModel;
using UnityEngine;

namespace Crosswork.Core
{
    public struct Cell
    {
        public readonly Vector2Int Position;
        public readonly bool Active;

        public Cell(Vector2Int position, bool active)
        {
            Position = position;
            Active = active;
        }

        public override string ToString()
        {
            return Active ? $"{Position}" : $"{Position} inactive";
        }
    }
}
