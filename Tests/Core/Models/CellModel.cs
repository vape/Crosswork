using Crosswork.Core;

namespace Crosswork.Tests.Core.Models
{
    public class CellModel : ICellModel
    {
        public IElementModel[] Elements
        { get; set; }
    }
}
