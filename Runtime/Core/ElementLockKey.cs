#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define CROSSWORK_DEBUG
#endif

namespace Crosswork.Core
{
    public struct ElementLockKey
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
    }
}
