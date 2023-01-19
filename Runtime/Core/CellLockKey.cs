#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define CROSSWORK_DEBUG
#endif

using System;

namespace Crosswork.Core
{
    public struct CellLockKey : IEquatable<CellLockKey>
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

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                hash = hash * 23 + Flag.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is not CellLockKey)
            {
                return false;
            }

            return Equals((CellLockKey)obj);
        }

        public bool Equals(CellLockKey other)
        {
            return X == other.X && Y == other.Y && Flag == other.Flag;
        }
    }
}