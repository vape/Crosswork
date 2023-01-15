using System;

namespace Crosswork.View.Tiling.Pattern
{
    internal enum QuadPosition
    {
        LeftBottom = 0,
        LeftTop = 1,
        RightTop = 2,
        RightBottom = 3,
    }

    internal struct Quad
    {
        private static bool IsSlotActive(TilingGrid grid, int x, int y) => (grid[x, y] & TilingGrid.SlotFlag.Occupied) != 0;

        public bool AnyActive => Self || Adjacent0 || Adjacent1 || Adjacent2;

        public readonly QuadPosition Position;
        public readonly bool Self;
        public readonly bool Adjacent0;
        public readonly bool Adjacent1;
        public readonly bool Adjacent2;
        public readonly int TileX;
        public readonly int TileY;

        public Quad(TilingGrid grid, int x, int y, QuadPosition position)
        {
            Position = position;
            Self = IsSlotActive(grid, x, y);
            TileX = x * 2 + 2 + ((position == QuadPosition.RightTop || position == QuadPosition.RightBottom) ? 1 : 0);
            TileY = y * 2 + 2 + ((position == QuadPosition.RightTop || position == QuadPosition.LeftTop) ? 1 : 0);

            switch (position)
            {
                case QuadPosition.LeftBottom:
                    Adjacent0 = IsSlotActive(grid, x + 0, y + -1);
                    Adjacent1 = IsSlotActive(grid, x + -1, y + -1);
                    Adjacent2 = IsSlotActive(grid, x + -1, y + 0);
                    break;

                case QuadPosition.LeftTop:
                    Adjacent0 = IsSlotActive(grid, x + -1, y + 0);
                    Adjacent1 = IsSlotActive(grid, x + -1, y + 1);
                    Adjacent2 = IsSlotActive(grid, x + 0, y + 1);
                    break;

                case QuadPosition.RightTop:
                    Adjacent0 = IsSlotActive(grid, x + 0, y + 1);
                    Adjacent1 = IsSlotActive(grid, x + 1, y + 1);
                    Adjacent2 = IsSlotActive(grid, x + 1, y + 0);
                    break;

                case QuadPosition.RightBottom:
                    Adjacent0 = IsSlotActive(grid, x + 1, y + 0);
                    Adjacent1 = IsSlotActive(grid, x + 1, y + -1);
                    Adjacent2 = IsSlotActive(grid, x + 0, y + -1);
                    break;

                default:
                    throw new Exception($"Unknown quad position: {position}");
            }
        }
    }
}