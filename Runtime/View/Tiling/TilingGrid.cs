using Crosswork.Core;
using System;

namespace Crosswork.View.Tiling
{
    public struct TilingGrid
    {
        public static TilingGrid Create(Cell[,] data)
        {
            var height = data.GetLength(0);
            var width = data.GetLength(1);
            var grid = new TilingGrid(width, height);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    if (data[y, x].Active)
                    {
                        grid[x, y] = SlotFlag.Occupied;
                    }
                }
            }

            return grid;
        }

        public static TilingGrid Create(bool[,] data)
        {
            var height = data.GetLength(0);
            var width = data.GetLength(1);
            var grid = new TilingGrid(width, height);

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    if (data[y, x])
                    {
                        grid[x, y] = SlotFlag.Occupied;
                    }
                }
            }

            return grid;
        }

        [Flags]
        public enum SlotFlag : long
        {
            Empty,
            Occupied
        }

        public int Width => slots.GetLength(1);
        public int Height => slots.GetLength(0);

        private SlotFlag[,] slots;

        public SlotFlag this[int x, int y]
        {
            get
            {
                if (x < 0 || y < 0 || x >= slots.GetLength(1) || y >= slots.GetLength(0))
                {
                    return SlotFlag.Empty;
                }

                return slots[y, x];
            }
            set
            {
                if (x < 0 || y < 0 || x >= slots.GetLength(1) || y >= slots.GetLength(0))
                {
                    throw new Exception($"Invalid grid position ({x}, {y})");
                }

                slots[y, x] = value;
            }
        }

        public TilingGrid(int width, int height)
        {
            slots = new SlotFlag[height, width];
        }
    }
}
