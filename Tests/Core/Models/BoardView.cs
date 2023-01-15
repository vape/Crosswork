using Crosswork.Core;
using Crosswork.Core.Intents;

namespace Crosswork.Tests.Core.Models
{
    public class BoardView : IBoardView
    {
        public void Load(Cell[,] cells)
        { }

        public void Unload()
        { }

        public void CreateView(Element element, IIntent intent)
        { }

        public bool DestroyView(Element element, IIntent intent)
        {
            return true;
        }

        public bool ResetPosition(Element element)
        {
            return true;
        }
    }
}
