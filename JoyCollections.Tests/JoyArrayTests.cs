// FILEPATH: /N:/blafed/JoyCollections/JoyCollections/JoyArrayTests.cs
using NUnit.Framework;
using System;

namespace JoyCollections.Tests
{
    public class JoyArrayTests
    {
        private JoyArray<int> joyArray;

        [SetUp]
        public void Setup()
        {
            joyArray = new JoyArray<int>();
        }

        [Test]
        public void Test_Add()
        {
            int index = joyArray.Add(10);
            Assert.That(joyArray.Get(index), Is.EqualTo(10));
        }

        [Test]

        public void Test_AddMany()
        {
            for (int i = 0; i < 100; i++)
            {
                joyArray.Add(i);
            }
            Assert.That(joyArray.Get(99), Is.EqualTo(99));
        }

        [Test]
        public void Test_RemoveAt()
        {
            int index = joyArray.Add(20);
            joyArray.RemoveAt(index);
            Assert.IsFalse(joyArray.Exists(index));
        }

        [Test]
        public void Test_Exists()
        {
            int index = joyArray.Add(30);
            Assert.IsTrue(joyArray.Exists(index));
        }

        [Test]
        public void Test_Get_ThrowsException_WhenItemRemoved()
        {
            int index = joyArray.Add(40);
            joyArray.RemoveAt(index);
            Assert.Throws<Exception>(() => joyArray.Get(index));
        }

        [Test]
        public void Test_Iterate()
        {
            joyArray.Add(50);
            joyArray.Add(60);
            joyArray.Add(70);

            int sum = 0;
            foreach (var item in joyArray.Iterate())
            {
                sum += item;
            }

            Assert.That(sum, Is.EqualTo(180));
        }

        [Test]
        public void Test_Iterator()
        {
            joyArray.Add(80);
            joyArray.Add(90);
            joyArray.Add(100);

            var iterator = joyArray.CreateIterator();
            int sum = 0;
            while (iterator.MoveNext())
            {
                sum += iterator.GetCurrent();
            }

            Assert.That(sum, Is.EqualTo(270));
        }
        [Test]
        public void Test_MixedOperations()
        {
            //do bunch of operations from adding to removing
            for (int i = 0; i < 100; i++)
            {
                joyArray.Add(i);
            }
            for (int i = 0; i < 100; i++)
            {
                joyArray.RemoveAt(i);
            }
            for (int i = 0; i < 100; i++)
            {
                joyArray.Add(i);
            }
            for (int i = 0; i < 100; i++)
            {
                joyArray.RemoveAt(i);
            }
            for (int i = 0; i < 100; i++)
            {
                joyArray.Add(i);
            }
            for (int i = 0; i < 100; i++)
            {
                joyArray.RemoveAt(i);
            }
            for (int i = 0; i < 100; i++)
            {
                joyArray.Add(i);
            }
            for (int i = 0; i < 100; i++)
            {
                joyArray.RemoveAt(i);
            }
            for (int i = 0; i < 100; i++)
            {
                joyArray.Add(i);
            }
            Assert.That(joyArray.Get(99), Is.EqualTo(99));
        }

        [Test]
        public void Test_ExtremeConditions()
        {
            joyArray.increaseCapacityBy = 1000000;
            for (int i = 0; i < 1000000; i++)
            {
                joyArray.Add(i);
            }
            for (int i = 0; i < 1000000; i++)
            {
                joyArray.RemoveAt(i);
            }
            for (int i = 0; i < 1000000; i++)
            {
                joyArray.Add(i);
            }
            Assert.That(joyArray.Get(999999), Is.EqualTo(999999));

        }
    }
}