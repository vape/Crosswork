using UnityEngine;

namespace Crosswork.Core
{
    public interface IBoardModel
    {
        int Width
        { get; }
        int Height
        { get; }

        bool TryGetCellModel(int x, int y, out ICellModel model);
    }

    public interface IWritableBoardModel
    {
        void SetCells(Cell[,] cells);
        void SetElements(int x, int y, IElementModel[] elements, int size);
    }
}
