#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define CROSSWORK_DEBUG
#endif

namespace Crosswork.Core
{
    public struct CellLockKey
    {
        public readonly int X;
        public readonly int Y;
        public readonly ulong Flag;

#if CROSSWORK_DEBUG
        public readonly string Description;
#endif

        public CellLockKey(int x, int y, ulong flag, string description = null)
        {
            X = x;
            Y = y;
            Flag = flag;

#if CROSSWORK_DEBUG
            Description = description;
#endif
        }
    }
}