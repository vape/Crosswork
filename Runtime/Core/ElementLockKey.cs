#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define CROSSWORK_DEBUG
#endif

using System;

namespace Crosswork.Core
{
    public struct ElementLockKey : IEquatable<ElementLockKey>
    {
        public readonly int ElementId;
        public readonly ulong Flag;

#if CROSSWORK_DEBUG
        public readonly string Description;
#endif

        public ElementLockKey(int elementId, ulong flag, string description = null)
        {
            ElementId = elementId;
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
                hash = hash * 23 + ElementId.GetHashCode();
                hash = hash * 23 + Flag.GetHashCode();
                return hash;
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is not ElementLockKey)
            {
                return false;
            }

            return Equals((ElementLockKey)obj);
        }

        public bool Equals(ElementLockKey other)
        {
            return ElementId == other.ElementId && Flag == other.Flag;
        }
    }
}
