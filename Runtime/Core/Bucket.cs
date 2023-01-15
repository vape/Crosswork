using System.Runtime.CompilerServices;

namespace Crosswork.Core
{
    public struct Bucket
    {
        public int Count;
        public readonly Element[] Elements;

        public Bucket(int size)
        {
            Count = 0;
            Elements = new Element[size];
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsFull()
        {
            return Count == Elements.Length;
        }

        public bool TryFind(int id, out int index)
        {
            for (int i = 0; i < Elements.Length; ++i)
            {
                if (Elements[i].Id == id)
                {
                    index = i;
                    return true;
                }
            }

            index = -1;
            return false;
        }

        internal void Add(Element element)
        {
            Elements[Count] = element;
            Count++;
        }

        internal void Remove(int index)
        {
            Elements[index] = Elements[Count - 1];
            Elements[Count - 1] = null;
            Count--;
        }

        internal bool RemoveById(int id)
        {
            for (int i = 0; i < Elements.Length; ++i)
            {
                if (Elements[i].Id == id)
                {
                    Remove(i);
                    return true;
                }
            }

            return false;
        }
    }
}
