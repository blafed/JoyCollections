using System.Collections.Generic;
using System;

namespace JoyCollections
{
    /// <summary>
    /// Represents a unique identifier for an item in the JoyList.
    /// </summary>
    public struct JoyListId
    {
        /// <summary>
        /// The index of the item in the JoyList.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// The version of the item in the JoyList.
        /// </summary>
        public int Version { get; }

        /// <summary>
        /// Initializes a new instance of the JoyListId struct with the specified index and version.
        /// </summary>
        /// <param name="index">The index of the item.</param>
        /// <param name="version">The version of the item.</param>
        public JoyListId(int index, int version)
        {
            Index = index;
            Version = version;
        }
    }

    /// <summary>
    /// A list that maintains the order of the items.
    /// </summary>
    /// <typeparam name="T">The type of elements in the list.</typeparam>
    public class JoyList<T>
    {
        /// <summary>
        /// Represents an iterator for the JoyList.
        /// </summary>
        public class Iterator
        {
            private readonly JoyList<T> list;
            private int index;

            internal Iterator(JoyList<T> list)
            {
                this.list = list;
                index = -1;
            }

            /// <summary>
            /// Moves the iterator to the next item in the JoyList.
            /// </summary>
            /// <returns><see langword="true"/> if the iterator was successfully moved to the next item; otherwise, <see langword="false"/>.</returns>
            public bool MoveNext()
            {
                index++;
                if (index >= list.items.Count)
                    return false;
                if (list.items[index] == null)
                    return MoveNext();
                return true;
            }

            /// <summary>
            /// Gets the current item in the JoyList.
            /// </summary>
            public T Current => list.items[index];

            /// <summary>
            /// Resets the iterator to its initial state.
            /// </summary>
            public void Reset()
            {
                index = -1;
            }
        }

        private List<T> items = new List<T>();
        private List<int> versions = new List<int>();
        private Queue<int> freeIndices = new Queue<int>();

        /// <summary>
        /// Gets or sets the item at the specified JoyListId.
        /// </summary>
        /// <param name="id">The JoyListId of the item.</param>
        /// <returns>The item at the specified JoyListId.</returns>
        public T this[JoyListId id] => Get(id);

        /// <summary>
        /// Gets the item at the specified JoyListId.
        /// </summary>
        /// <param name="id">The JoyListId of the item.</param>
        /// <returns>The item at the specified JoyListId.</returns>
        public T Get(in JoyListId id)
        {
            if (!Contains(id))
                throw new System.Exception("Entity not found " + id);
            return items[id.Index];
        }

        /// <summary>
        /// Determines whether the JoyList contains the specified JoyListId.
        /// </summary>
        /// <param name="id">The JoyListId to locate.</param>
        /// <returns><see langword="true"/> if the JoyList contains the specified JoyListId; otherwise, <see langword="false"/>.</returns>
        public bool Contains(in JoyListId id)
        {
            if (id.Index < 0 || id.Index >= items.Count)
                return false;
            if (versions[id.Index] != id.Version)
                return false;
            return true;
        }

        /// <summary>
        /// Adds an item to the JoyList.
        /// </summary>
        /// <param name="entity">The item to add.</param>
        /// <returns>The JoyListId of the added item.</returns>
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

        /// <summary>
        /// Removes the item with the specified JoyListId from the JoyList.
        /// </summary>
        public void Remove(in JoyListId id)
        {
            if (id.Index < 0 || id.Index >= items.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Invalid entity ID");
            }
            if (versions[id.Index] != id.Version)
            {
                throw new InvalidOperationException("Entity has already been removed");
            }

            items[id.Index] = default;
            versions[id.Index]++;
            freeIndices.Enqueue(id.Index);
        }

        /// <summary>
        /// Returns an iterator that iterates over the items in the JoyList.
        /// </summary>
        public Iterator CreateIterator()
        {
            return new Iterator(this);
        }

        /// <summary>
        /// Enumerates over the items in the JoyList.
        /// </summary>
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

        /// <summary>
        /// Returns the list of items in the JoyList.
        /// </summary>
        [Obsolete("Unsafe, don't use it unless you are a performance-nerd. Also check for null while iterating. UPDATE: use CreateIterator() instead if you want to iterate over the list.")]
        public List<T> UnsafeGetItemList()
        {
            return items;
        }
    }
}