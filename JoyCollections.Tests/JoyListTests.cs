// FILEPATH: /N:/blafed/JoyCollections/JoyCollections/JoyListTests.cs
using NUnit.Framework;
using System;

namespace JoyCollections.Tests
{
    public class JoyListTests
    {
        private JoyList<int> joyList;

        [SetUp]
        public void Setup()
        {
            joyList = new JoyList<int>();
        }

        [Test]
        public void Test_Add()
        {
            var id = joyList.Add(10);
            Assert.That(joyList[id], Is.EqualTo(10));
        }

        [Test]
        public void Test_Remove()
        {
            var id = joyList.Add(20);
            joyList.Remove(id);
            Assert.IsFalse(joyList.Contains(id));
        }

        [Test]
        public void Test_Contains()
        {
            var id = joyList.Add(30);
            Assert.IsTrue(joyList.Contains(id));
        }

        [Test]
        public void Test_Get_ThrowsException_WhenItemRemoved()
        {
            var id = joyList.Add(40);
            joyList.Remove(id);
            Assert.Throws<Exception>(() => joyList.Get(id));
        }

        [Test]
        public void Test_Iterator()
        {
            joyList.Add(50);
            joyList.Add(60);
            joyList.Add(70);

            var iterator = joyList.CreateIterator();
            int sum = 0;
            while (iterator.MoveNext())
            {
                sum += iterator.Current;
            }

            Assert.That(sum, Is.EqualTo(180));
        }

        [Test]
        public void Test_Iterator_SkipsRemovedItems()
        {
            var id1 = joyList.Add(80);
            joyList.Add(90);
            var id3 = joyList.Add(100);

            joyList.Remove(id1);
            joyList.Remove(id3);

            var iterator = joyList.CreateIterator();
            int sum = 0;
            while (iterator.MoveNext())
            {
                sum += iterator.Current;
            }

            Assert.That(sum, Is.EqualTo(90));
        }
    }
}