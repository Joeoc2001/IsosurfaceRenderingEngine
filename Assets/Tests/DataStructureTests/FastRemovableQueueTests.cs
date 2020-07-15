using NUnit.Framework;

namespace DataStructureTests
{
    public class FastRemovableQueueTests
    {
        [Test]
        public void FastRemovableQueue_PopOnEmpty_ThrowsException()
        {
            // ARANGE
            FastRemovableQueue<int?> fastRemovableQueue = new FastRemovableQueue<int?>();

            // ACT

            // ASSERT
            Assert.Catch(() => fastRemovableQueue.Pop());
        }

        [Test]
        public void FastRemovableQueue_PushOnEmpty_DoesNotThrowException()
        {
            // ARANGE
            FastRemovableQueue<int?> fastRemovableQueue = new FastRemovableQueue<int?>();

            // ACT

            // ASSERT
            Assert.DoesNotThrow(() => fastRemovableQueue.Push(1));
        }

        [Test]
        public void FastRemovableQueue_PushNull_ThrowsException()
        {
            // ARANGE
            FastRemovableQueue<string> fastRemovableQueue = new FastRemovableQueue<string>();

            // ACT

            // ASSERT
            Assert.Catch(() => fastRemovableQueue.Push(null));
        }

        [Test]
        public void FastRemovableQueue_PushDefault_ThrowsException()
        {
            // ARANGE
            FastRemovableQueue<int> fastRemovableQueue = new FastRemovableQueue<int>();

            // ACT

            // ASSERT
            Assert.Catch(() => fastRemovableQueue.Push(0));
        }

        [Test]
        public void FastRemovableQueue_PushPop_GetsValue()
        {
            // ARANGE
            FastRemovableQueue<int> fastRemovableQueue = new FastRemovableQueue<int>();

            // ACT
            fastRemovableQueue.Push(1006);
            int result = fastRemovableQueue.Pop();

            // ASSERT
            Assert.AreEqual(1006, result);
        }

        [Test]
        public void FastRemovableQueue_PushRemove_DoesNotThrowException()
        {
            // ARANGE
            FastRemovableQueue<int> fastRemovableQueue = new FastRemovableQueue<int>();

            // ACT
            fastRemovableQueue.Push(1006);

            // ASSERT
            Assert.DoesNotThrow(() => fastRemovableQueue.Remove(1006));
        }

        [Test]
        public void FastRemovableQueue_PushRemovePop_ThrowsException()
        {
            // ARANGE
            FastRemovableQueue<int> fastRemovableQueue = new FastRemovableQueue<int>();

            // ACT
            fastRemovableQueue.Push(1006);
            fastRemovableQueue.Remove(1006);

            // ASSERT
            Assert.Catch(() => fastRemovableQueue.Pop());
        }

        [Test]
        public void FastRemovableQueue_PushPopPop_ThrowsException()
        {
            // ARANGE
            FastRemovableQueue<int> fastRemovableQueue = new FastRemovableQueue<int>();

            // ACT
            fastRemovableQueue.Push(1006);
            fastRemovableQueue.Pop();

            // ASSERT
            Assert.Catch(() => fastRemovableQueue.Pop());
        }

        [Test]
        public void FastRemovableQueue_PushPushPopPop_ReturnsCorrect()
        {
            // ARANGE
            FastRemovableQueue<int> fastRemovableQueue = new FastRemovableQueue<int>();

            // ACT
            fastRemovableQueue.Push(81);
            fastRemovableQueue.Push(82);
            int v1 = fastRemovableQueue.Pop();
            int v2 = fastRemovableQueue.Pop();

            // ASSERT
            Assert.AreEqual(81, v1);
            Assert.AreEqual(82, v2);
        }

        [Test]
        public void FastRemovableQueue_PushPopPushPop_ReturnsCorrect()
        {
            // ARANGE
            FastRemovableQueue<int> fastRemovableQueue = new FastRemovableQueue<int>();

            // ACT
            fastRemovableQueue.Push(81);
            int v1 = fastRemovableQueue.Pop();
            fastRemovableQueue.Push(82);
            int v2 = fastRemovableQueue.Pop();

            // ASSERT
            Assert.AreEqual(81, v1);
            Assert.AreEqual(82, v2);
        }

        [Test]
        public void FastRemovableQueue_PushPushPushPopPopPop_ReturnsCorrect()
        {
            // ARANGE
            FastRemovableQueue<int> fastRemovableQueue = new FastRemovableQueue<int>();

            // ACT
            fastRemovableQueue.Push(81);
            fastRemovableQueue.Push(82);
            fastRemovableQueue.Push(83);
            int v1 = fastRemovableQueue.Pop();
            int v2 = fastRemovableQueue.Pop();
            int v3 = fastRemovableQueue.Pop();

            // ASSERT
            Assert.AreEqual(81, v1);
            Assert.AreEqual(82, v2);
            Assert.AreEqual(83, v3);
        }

        [Test]
        public void FastRemovableQueue_PushPushPopPushPopPop_ReturnsCorrect()
        {
            // ARANGE
            FastRemovableQueue<int> fastRemovableQueue = new FastRemovableQueue<int>();

            // ACT
            fastRemovableQueue.Push(81);
            fastRemovableQueue.Push(82);
            int v1 = fastRemovableQueue.Pop();
            fastRemovableQueue.Push(83);
            int v2 = fastRemovableQueue.Pop();
            int v3 = fastRemovableQueue.Pop();

            // ASSERT
            Assert.AreEqual(81, v1);
            Assert.AreEqual(82, v2);
            Assert.AreEqual(83, v3);
        }

        [Test]
        public void FastRemovableQueue_PushPopPushPushPopPop_ReturnsCorrect()
        {
            // ARANGE
            FastRemovableQueue<int> fastRemovableQueue = new FastRemovableQueue<int>();

            // ACT
            fastRemovableQueue.Push(81);
            int v1 = fastRemovableQueue.Pop();
            fastRemovableQueue.Push(82);
            fastRemovableQueue.Push(83);
            int v2 = fastRemovableQueue.Pop();
            int v3 = fastRemovableQueue.Pop();

            // ASSERT
            Assert.AreEqual(81, v1);
            Assert.AreEqual(82, v2);
            Assert.AreEqual(83, v3);
        }

        [Test]
        public void FastRemovableQueue_PushPopPushPopPushPop_ReturnsCorrect()
        {
            // ARANGE
            FastRemovableQueue<int> fastRemovableQueue = new FastRemovableQueue<int>();

            // ACT
            fastRemovableQueue.Push(81);
            int v1 = fastRemovableQueue.Pop();
            fastRemovableQueue.Push(82);
            int v2 = fastRemovableQueue.Pop();
            fastRemovableQueue.Push(83);
            int v3 = fastRemovableQueue.Pop();

            // ASSERT
            Assert.AreEqual(81, v1);
            Assert.AreEqual(82, v2);
            Assert.AreEqual(83, v3);
        }

        [Test]
        public void FastRemovableQueue_PushPushRemoveFirstPop_ReturnsCorrect()
        {
            // ARANGE
            FastRemovableQueue<int> fastRemovableQueue = new FastRemovableQueue<int>();

            // ACT
            fastRemovableQueue.Push(81);
            fastRemovableQueue.Push(82);
            fastRemovableQueue.Remove(81);
            int v1 = fastRemovableQueue.Pop();

            // ASSERT
            Assert.AreEqual(82, v1);
        }

        [Test]
        public void FastRemovableQueue_PushPushRemoveSecondPop_ReturnsCorrect()
        {
            // ARANGE
            FastRemovableQueue<int> fastRemovableQueue = new FastRemovableQueue<int>();

            // ACT
            fastRemovableQueue.Push(81);
            fastRemovableQueue.Push(82);
            fastRemovableQueue.Remove(82);
            int v1 = fastRemovableQueue.Pop();

            // ASSERT
            Assert.AreEqual(81, v1);
        }

        [Test]
        public void FastRemovableQueue_PushRemovePushPop_ReturnsCorrect()
        {
            // ARANGE
            FastRemovableQueue<int> fastRemovableQueue = new FastRemovableQueue<int>();

            // ACT
            fastRemovableQueue.Push(81);
            fastRemovableQueue.Remove(81);
            fastRemovableQueue.Push(82);
            int v1 = fastRemovableQueue.Pop();

            // ASSERT
            Assert.AreEqual(82, v1);
        }

        [Test]
        public void FastRemovableQueue_PushPushPopRemove_DoesntThrow()
        {
            // ARANGE
            FastRemovableQueue<int> fastRemovableQueue = new FastRemovableQueue<int>();

            // ACT
            fastRemovableQueue.Push(81);
            fastRemovableQueue.Push(82);
            int v1 = fastRemovableQueue.Pop();

            // ASSERT
            Assert.AreEqual(81, v1);
            Assert.DoesNotThrow(() => fastRemovableQueue.Remove(82));
        }
    }
}
