namespace Crosswork.View.Sorting
{
    public interface ISortingHandler
    {
        void SetLayer(CrossworkSortingLayer layer);
        void SetDefaultLayer(CrossworkSortingLayer layer);
        void ResetLayer();
        void SetOrder(int order);
    }
}
