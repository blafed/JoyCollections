using System.Collections.Generic;
using System;
namespace Mazo
{
    public struct JoyListId
    {
        public int index { get; }
        public int version { get; }

        public JoyListId(int index, int version)
        {
            this.index = index;
            this.version = version;
        }
    }

    public class JoyList<T>
    {
        private List<T> items = new List<T>();
        private List<int> versions = new List<int>();
        private Queue<int> freeIndices = new Queue<int>();

        public T this[JoyListId id]
        {
            get => Get(id);
        }

        public T Get(JoyListId id)
        {
            if (!Contains(id))
                throw new System.Exception("Entity not found " + id);
            return items[id.index];
        }

        public bool Contains(JoyListId id)
        {
            if (id.index < 0 || id.index >= items.Count)
                return false;
            if (versions[id.index] != id.version)
                return false;
            return true;
        }


        public JoyListId Add(T entity)
        {
            int index;
            int version;

            if (freeIndices.Count > 0)
            {
                index = freeIndices.Dequeue();
                items[index] = entity;
                version = ++versions[index];
            }
            else
            {
                items.Add(entity);
                versions.Add(0);
                index = items.Count - 1;
                version = 0;
            }

            return new JoyListId(index, version);
        }

        public void Remove(JoyListId id)
        {
            if (id.index < 0 || id.index >= items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Invalid entity ID");
            }
            if (versions[id.index] != id.version)
            {
                throw new InvalidOperationException("Entity has already been removed");
            }

            items[id.index] = default;
            versions[id.index]++;
            freeIndices.Enqueue(id.index);
        }

        public IEnumerable<T> Iterate()
        {
            for (int i = 0; i < items.Count; i++)
            {
                if (items[i] != null)
                {
                    yield return items[i];
                }
            }
        }
        [Obsolete("Unsafe, dont use it unless you are performance-nerd. also check for the null while iterating")]
        public List<T> UnsafeGetEntities()
        {
            return items;
        }
    }
}