using Crosswork.Core;
using UnityEngine;

namespace Crosswork.View
{
    public interface IBoardViewFactory
    {
        ElementView CreateView(Element element, Transform container);
        void PurgeView(ElementView view);
    }
}
