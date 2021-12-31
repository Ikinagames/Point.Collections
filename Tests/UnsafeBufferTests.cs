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
    }
}
