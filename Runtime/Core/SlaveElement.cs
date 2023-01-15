namespace Crosswork.Core
{
    public class SlaveElement : Element
    {
        public virtual Element Master
        {
            get
            {
                return masterElement;
            }
        }

        protected Element masterElement;

        public SlaveElement(Element master, IElementModel model)
            : base(model)
        {
            masterElement = master;
        }

        public override ulong GetCollisionMask()
        {
            return masterElement.GetCollisionMask();
        }
    }
}
