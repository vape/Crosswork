using Crosswork.Core;
using System;

namespace Crosswork.Tests.Core.Models
{
    public class BoardModel : IBoardModel, IWritableBoardModel
    {
        public int Width => cells.GetLength(1);
        public int Height => cells.GetLength(0);

        private CellModel[,] cells;

        public BoardModel(int w, int h)
        {
            cells = new CellModel[h, w];
        }

        public void FillCells()
        {
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    cells[y, x] = new CellModel();
                }
            }
        }

        public void SetCell(int x, int y, bool active)
        {
            cells[y, x] = active ? cells[y, x] == null ? new CellModel() : cells[y, x] : null;
        }

        public void SetElement(int x, int y, IElementModel model)
        {
            cells[y, x] = new CellModel()
            {
                Elements = new IElementModel[1] { model }
            };
        }

        public bool TryGetCellModel(int x, int y, out ICellModel model)
        {
            model = cells[y, x];
            return model != null;
        }

        public void SetCells(Cell[,] cells)
        {
            this.cells = new CellModel[cells.GetLength(0), cells.GetLength(1)];

            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    if (cells[y, x].Active)
                    {
                        this.cells[y, x] = new CellModel();
                    }
                }
            }
        }

        public void SetElements(int x, int y, IElementModel[] elements, int size)
        {
            cells[y, x].Elements = new IElementModel[size];
            Array.Copy(elements, cells[y, x].Elements, size);
        }
    }
}
