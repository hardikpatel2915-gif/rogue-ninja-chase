using System;

namespace RogueNinjaChase.Pool
{
    /// <summary>
    /// Fixed free list. TryTake fails when empty instead of growing.
    /// </summary>
    public sealed class IndexPool
    {
        readonly int[] _free;
        int _freeCount;

        public IndexPool(int capacity)
        {
            if (capacity < 1)
                throw new ArgumentOutOfRangeException(nameof(capacity));

            _free = new int[capacity];
            _freeCount = capacity;
            for (int i = 0; i < capacity; i++)
                _free[i] = i;
        }

        public int Capacity => _free.Length;
        public int Available => _freeCount;

        public bool TryTake(out int index)
        {
            if (_freeCount == 0)
            {
                index = -1;
                return false;
            }

            _freeCount--;
            index = _free[_freeCount];
            return true;
        }

        public void Return(int index)
        {
            _free[_freeCount] = index;
            _freeCount++;
        }
    }
}
