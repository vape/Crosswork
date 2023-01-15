namespace Crosswork.View.Sorting
{
    public static class CommonSortingMethod
    {
        public static readonly UpSortingMethod UpSorting = new UpSortingMethod();
        public static readonly DownSortingMethod DownSorting = new DownSortingMethod();

        public class UpSortingMethod : ISortingMethod
        {
            public int GetOrder(int x, int y)
            {
                return ((y * 512) + (x * 8));
            }
        }

        public class DownSortingMethod : ISortingMethod
        {
            public int GetOrder(int x, int y)
            {
                return ((-y * 256) + (x * 8));
            }
        }
    }
}
