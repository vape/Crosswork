using Crosswork.Core;
using Crosswork.Core.Intents;
using UnityEngine;

namespace Crosswork.View
{
    public abstract class ElementView : MonoBehaviour
    {
        public Element Element
        {
            get
            {
                return element;
            }
        }

        protected Element element;

        public virtual void OnCreated(Element element, IIntent intent)
        {
            this.element = element;
        }
    }

    public abstract class ElementView<TElement> : ElementView
        where TElement : Element
    {
        protected new TElement element;

        public override void OnCreated(Element element, IIntent intent)
        {
            base.OnCreated(element, intent);

            OnCreated(element as TElement, intent);
        }

        protected virtual void OnCreated(TElement element, IIntent intent)
        {
            this.element = element;
        }
    }
}
