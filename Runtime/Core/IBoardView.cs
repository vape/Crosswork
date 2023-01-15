using Crosswork.Core.Intents;

namespace Crosswork.Core
{
    public interface IBoardView
    {
        void Unload();
        void Load(Cell[,] cells);

        void CreateView(Element element, IIntent intent);
        bool DestroyView(Element element, IIntent intent);
        bool ResetPosition(Element element);
    }
}
