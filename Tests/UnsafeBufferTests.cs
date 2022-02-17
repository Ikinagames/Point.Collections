using NUnit.Framework;
using Point.Collections.Buffer.LowLevel;
using System;

namespace Point.Collections.Tests
{
    public class UnsafeBufferTests
    {
        [Test]
        public void UnsafeLinearHashMapTest()
        {
            UnsafeLinearHashMap<Guid, Guid> temp = new UnsafeLinearHashMap<Guid, Guid>(128, Unity.Collections.Allocator.Persistent);

            Guid
                g1 = Guid.NewGuid(),
                g2 = Guid.NewGuid(),
                g3 = Guid.NewGuid(),
                g4 = Guid.NewGuid();

            temp.Add(g1, g1);
            temp.Add(g2, g2);
            temp.Add(g3, g3);
            temp.Add(g4, g4);

            Assert.IsTrue(temp.ContainsKey(g1));
            Assert.IsTrue(temp.ContainsKey(g2));
            Assert.IsTrue(temp.ContainsKey(g3));
            Assert.IsTrue(temp.ContainsKey(g4));

            Assert.AreEqual(g1, temp[g1]);
            Assert.AreEqual(g2, temp[g2]);
            Assert.AreEqual(g3, temp[g3]);
            Assert.AreEqual(g4, temp[g4]);

            temp.Dispose();
        }

        [Test]
        public void a0_UnsafeMemoryPoolTest()
        {
            MemoryPool pool = new MemoryPool(1024, Unity.Collections.Allocator.Temp);

            bool sucess1 = pool.TryGet(512, out MemoryBlock block1);
            UnityEngine.Debug.Log($"block1: {sucess1}, {block1.Length}");

            bool sucess2 = pool.TryGet(512, out MemoryBlock block2);
            UnityEngine.Debug.Log($"block2: {sucess2}, {block2.Length}");

            pool.Reserve(block1);

            bool sucess3 = pool.TryGet(512, out MemoryBlock block3);
            UnityEngine.Debug.Log($"block3: {sucess3}, {block3.Length}");
            Assert.IsTrue(block3.Ptr.Equals(block1.Ptr));
        }
        [Test]
        public void a1_UnsafeMemoryPoolTest()
        {
            MemoryPool pool = new MemoryPool(1024, Unity.Collections.Allocator.Temp);

            bool sucess1 = pool.TryGet(256, out MemoryBlock block1);
            UnityEngine.Debug.Log($"block1: {sucess1}, {block1.Length}");

            bool sucess2 = pool.TryGet(256, out MemoryBlock block2);
            UnityEngine.Debug.Log($"block2: {sucess2}, {block2.Length}");

            bool sucess3 = pool.TryGet(512, out MemoryBlock block3);
            UnityEngine.Debug.Log($"block3: {sucess3}, {block3.Length}");

            pool.Reserve(block2);
            pool.Reserve(block3);

            bool sucess5 = pool.TryGet(512, out MemoryBlock block5);
            UnityEngine.Debug.Log($"block5: {sucess5}, {block5.Length}");
            Assert.IsTrue(block5.Ptr.Equals(block2.Ptr));
        }
        [Test]
        public void a2_UnsafeMemoryPoolTest()
        {
            MemoryPool pool = new MemoryPool(1024, Unity.Collections.Allocator.Temp);

            bool sucess1 = pool.TryGet(256, out MemoryBlock block1);
            UnityEngine.Debug.Log($"block1: {sucess1}, {block1.Length}");

            bool sucess2 = pool.TryGet(256, out MemoryBlock block2);
            UnityEngine.Debug.Log($"block2: {sucess2}, {block2.Length}");

            bool sucess3 = pool.TryGet(512, out MemoryBlock block3);
            UnityEngine.Debug.Log($"block3: {sucess3}, {block3.Length}");

            pool.Reserve(block2);

            bool sucess5 = pool.TryGet(128, out MemoryBlock block5);
            UnityEngine.Debug.Log($"block5: {sucess5}, {block5.Length}");
            Assert.IsTrue(block5.Ptr.Equals(block2.Ptr));
        }
    }
}
