using Crosswork.Core;

namespace Crosswork.Tests.Core.Models
{
    public class BoardFactory : IBoardFactory
    {
        public Element CreateElement(IElementModel model)
        {
            switch (model)
            {
                case TestElementModel testElementModel:
                    return new TestElement(testElementModel);
                case TestElementBigModel testElementBigModel:
                    return new TestElementBig(testElementBigModel);
                default:
                    return null;
            }
        }
    }
}
