using System;
using System.Collections;
using System.Collections.Generic;

namespace Mazo
{

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
        public int increaseCapacityBy
        {
            get => _increaseCapacityBy;
            set
            {
                if (value < 1)
                    value = 1;
                _increaseCapacityBy = value;
            }
        }


        public JoyArray()
        {
            array = new T[0];
            flags = new Flags[0];
        }


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
                // flags[index] &= ~Flags.Removed;
                _arrayEnd++;
            }
            else
            {
                //resize and add
                Array.Resize(ref array, array.Length + increaseCapacityBy);
                Array.Resize(ref flags, flags.Length + increaseCapacityBy);
                index = _arrayEnd;
                array[index] = item;
                // flags[index] &= ~Flags.Removed;
            }
            return index;
        }

        public void RemoveAt(int index)
        {
            flags[index] |= Flags.Removed;
            freeIndexes.Enqueue(index);
        }

        public ref T Get(int index)
        {
            if (index < 0 || index >= _arrayEnd)
                throw new System.Exception("Index out of range " + index);
            if ((flags[index] & Flags.Removed) != 0)
                throw new System.Exception("Item was removed " + index);
            return ref array[index];
        }

        public IEnumerable<T> Iterate()
        {
            for (int i = 0; i < _arrayEnd; i++)
            {
                if ((flags[i] & Flags.Removed) != 0)
                    continue;
                yield return array[i];
            }
        }

        public Iterator CreateIterator()
        {
            return Iterator.GetOne(this);
        }

        public class Iterator : IDisposable, System.Collections.Generic.IEnumerator<T>
        {
            public static Iterator GetOne(JoyArray<T> array)
            {
                if (pool.Count > 0)
                {
                    var it = pool.Dequeue();
                    it.array = array;
                    it.Reset();
                    return it;
                }
                return new Iterator(array);
            }
            static Queue<Iterator> pool = new Queue<Iterator>();
            JoyArray<T> array;
            int index;



            public Iterator(JoyArray<T> array)
            {
                this.array = array;
            }
            public void Reset()
            {
                index = 0;
            }

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

            public ref T GetCurrent()
            {
                return ref array.array[index];
            }

            T IEnumerator<T>.Current => GetCurrent();
            object IEnumerator.Current => GetCurrent();
            public void Dispose()
            {
                pool.Enqueue(this);
            }

            bool IEnumerator.MoveNext()
            {
                return this.MoveNext();
            }

            void IEnumerator.Reset()
            {
                this.Reset();
            }
        }
    }
}