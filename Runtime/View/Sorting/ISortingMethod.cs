using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crosswork.View.Sorting
{
    public interface ISortingMethod
    {
        int GetOrder(int x, int y);
    }
}
