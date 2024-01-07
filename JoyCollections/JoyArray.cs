using System;
using System.Collections;
using System.Collections.Generic;

namespace JoyCollections
{
    /// <summary>
    /// Represents a performant array implementation that is optimized for value types.
    /// </summary>
    /// <typeparam name="T">The type of elements in the array.</typeparam>
    public class JoyArray<T> where T : struct
    {
        [System.Flags]
        enum Flags : byte
        {
            None,
            Removed,
        }

        T[] array;
        Flags[] flags;
        Queue<int> freeIndexes = new Queue<int>();
        int _arrayEnd;
        int _increaseCapacityBy = 10;

        /// <summary>
        /// Gets or sets the amount by which the capacity of the array is increased when it is full upon adding an item.
        /// </summary>
        public int IncreaseCapacityBy
        {
            get => _increaseCapacityBy;
            set
            {
                if (value < 1)
                    value = 1;
                _increaseCapacityBy = value;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JoyArray{T}"/> class.
        /// </summary>
        public JoyArray()
        {
            array = new T[0];
            flags = new Flags[0];
        }

        /// <summary>
        /// Adds an item to the array and returns the index at which the item is added.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <returns>The index at which the item is added.</returns>
        public int Add(in T item)
        {
            int index;
            if (freeIndexes.Count > 0)
            {
                index = freeIndexes.Dequeue();
                array[index] = item;
                flags[index] &= ~Flags.Removed;
            }
            else if (_arrayEnd < array.Length)
            {
                index = _arrayEnd;
                array[index] = item;
                _arrayEnd++;
            }
            else
            {
                Array.Resize(ref array, array.Length + IncreaseCapacityBy);
                Array.Resize(ref flags, flags.Length + IncreaseCapacityBy);
                index = _arrayEnd;
                array[index] = item;
            }
            return index;
        }

        /// <summary>
        /// Removes the item at the specified index from the array.
        /// </summary>
        /// <param name="index">The index of the item to remove.</param>
        public void RemoveAt(int index)
        {
            flags[index] |= Flags.Removed;
            freeIndexes.Enqueue(index);
        }


        /// <summary>
        /// Checks if an element exists at the specified index in the JoyArray.
        /// </summary>
        public bool Exists(int index)
        {
            return (flags[index] & Flags.Removed) == 0;
        }

        /// <summary>
        /// Gets a reference to the item at the specified index in the array.
        /// </summary>
        /// <param name="index">The index of the item to get.</param>
        /// <returns>A reference to the item at the specified index.</returns>
        /// <exception cref="System.Exception">Thrown when the index is out of range or the item was removed.</exception>
        public ref T Get(int index)
        {
            if (index < 0 || index >= _arrayEnd)
                throw new System.Exception("Index out of range " + index);
            if ((flags[index] & Flags.Removed) != 0)
                throw new System.Exception("Item was removed " + index);
            return ref array[index];
        }

        /// <summary>
        /// Returns an enumerable that iterates over the items in the array.
        /// </summary>
        /// <returns>An enumerable that iterates over the items in the array.</returns>
        public IEnumerable<T> Iterate()
        {
            for (int i = 0; i < _arrayEnd; i++)
            {
                if ((flags[i] & Flags.Removed) != 0)
                    continue;
                yield return array[i];
            }
        }

        /// <summary>
        /// Creates an iterator for the array.
        /// </summary>
        /// <returns>An iterator for the array.</returns>
        public Iterator CreateIterator()
        {
            return new Iterator(this);
        }

        /// <summary>
        /// Represents an iterator for the <see cref="JoyArray{T}"/> class.
        /// </summary>
        public class Iterator
        {
            readonly JoyArray<T> array;
            int index;

            /// <summary>
            /// Initializes a new instance of the <see cref="Iterator"/> class.
            /// </summary>
            /// <param name="array">The <see cref="JoyArray{T}"/> instance to iterate over.</param>
            public Iterator(JoyArray<T> array)
            {
                this.array = array;
            }

            /// <summary>
            /// Resets the iterator to the beginning of the array.
            /// </summary>
            public void Reset()
            {
                index = 0;
            }

            /// <summary>
            /// Moves the iterator to the next item in the array.
            /// </summary>
            /// <returns><c>true</c> if the iterator successfully moved to the next item; otherwise, <c>false</c>.</returns>
            public bool MoveNext()
            {
                while (index < array._arrayEnd)
                {
                    if ((array.flags[index] & Flags.Removed) != 0)
                    {
                        index++;
                        continue;
                    }
                    return true;
                }
                return false;
            }

            /// <summary>
            /// Gets a reference to the current item in the array.
            /// </summary>
            public ref T GetCurrent()
            {
                return ref array.array[index];
            }
        }
    }
}