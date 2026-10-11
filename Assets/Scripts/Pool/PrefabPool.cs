using UnityEngine;

namespace RogueNinjaChase.Pool
{
    /// <summary>
    /// Instances exist from the start. Spawn never calls Instantiate.
    /// </summary>
    public sealed class PrefabPool
    {
        readonly GameObject[] _items;
        readonly IndexPool _slots;

        public PrefabPool(GameObject[] items)
        {
            _items = items;
            _slots = new IndexPool(items.Length);
            for (int i = 0; i < items.Length; i++)
                items[i].SetActive(false);
        }

        public bool TrySpawn(Vector3 position, Quaternion rotation, out GameObject item)
        {
            if (!_slots.TryTake(out int index))
            {
                item = null;
                return false;
            }

            item = _items[index];
            item.transform.SetPositionAndRotation(position, rotation);
            item.SetActive(true);
            return true;
        }

        public void Release(GameObject item)
        {
            int index = IndexOf(item);
            if (index < 0)
                return;

            item.SetActive(false);
            _slots.Return(index);
        }

        int IndexOf(GameObject item)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                if (_items[i] == item)
                    return i;
            }

            return -1;
        }
    }
}
