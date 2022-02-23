using NUnit.Framework;
using Point.Collections.Buffer;

namespace Point.Collections.Tests
{
    public sealed class NativeBufferTests
    {
        [Test]
        public void a0_NativeMemoryPoolTest()
        {
            NativeMemoryPool pool = new NativeMemoryPool(1024, Unity.Collections.Allocator.Temp);

            MemoryBlock temp1 = pool.Get(512);
            MemoryBlock temp2 = pool.Get(512);

            Assert.IsFalse(temp2.Ptr.Equals(temp1.Ptr));

            Buffer.LowLevel.UnsafeReference<byte> testPtr = temp2.Ptr;
            pool.Reserve(temp2);

            MemoryBlock temp3 = pool.Get(512);

            Assert.IsTrue(testPtr.Equals(temp3.Ptr));

            MemoryBlock temp4 = pool.Get(512);
        }
    }
}
