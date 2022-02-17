// Copyright 2022 Ikina Games
// Author : Seung Ha Kim (Syadeu)
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

using System;
using System.Collections.Generic;
using Unity.Collections;

namespace Point.Collections.Buffer.LowLevel
{
    public struct MemoryPool : IDisposable
    {
        private const int c_InitialMemoryPoolSize = 10240;

        private Hash m_Hash;
        private UnsafeAllocator<byte> m_Buffer;
        private UnsafeAllocator<Bucket> m_Bucket;

        private int m_BucketCount;

        private struct Bucket : IEquatable<UnsafeReference<byte>>, IEquatable<Bucket>
        {
            private KeyValue<UnsafeReference<byte>, int> m_Block;

            public Bucket(UnsafeReference<byte> p, int length)
            {
                m_Block = new KeyValue<UnsafeReference<byte>, int>(p, length);
            }

            public UnsafeReference<byte> Block => m_Block.Key;
            public int Length => m_Block.Value;

            public UnsafeReference<byte> GetNext()
            {
                return Block + Length;
            }

            public bool Equals(UnsafeReference<byte> other) => Block.Equals(other);
            public bool Equals(Bucket other) => Block.Equals(other.Block) && Length == other.Length;
        }
        private struct BucketComparer : IComparer<Bucket>
        {
            private UnsafeReference<byte> m_Buffer;

            public BucketComparer(UnsafeAllocator<byte> buffer)
            {
                m_Buffer = buffer.Ptr;
            }

            public int Compare(Bucket x, Bucket y)
            {
                long
                    a = x.Block - m_Buffer,
                    b = y.Block - m_Buffer;

                if (a < b) return -1;
                else if (a > b) return 1;
                return 0;
            }
        }

        public MemoryPool(int size, Allocator allocator)
        {
            m_Hash = Hash.NewHash();

            m_Buffer = new UnsafeAllocator<byte>(size, allocator);

            m_Bucket = new UnsafeAllocator<Bucket>(4, m_Buffer.m_Allocator.m_Allocator);
            m_BucketCount = 0;
        }
        public MemoryPool(UnsafeAllocator<byte> buffer)
        {
            m_Hash = Hash.NewHash();

            m_Buffer = buffer;

            m_Bucket = new UnsafeAllocator<Bucket>(4, m_Buffer.m_Allocator.m_Allocator);
            m_BucketCount = 0;
        }

        private static long CalculateFreeSpaceBetween(Bucket a, UnsafeReference<byte> b)
        {
            long length = b - (a.Block + a.Length);

            return length;
        }
        private static bool IsAllocatableBetween(Bucket a, UnsafeReference<byte> b, int length, out long freeSpace)
        {
            freeSpace = CalculateFreeSpaceBetween(a, b);

            return freeSpace - 4 >= length;
        }
        private static bool IsExceedingAllocator(UnsafeAllocator<byte> allocator, UnsafeReference<byte> from, int length)
        {
            UnsafeReference<byte> 
                endPtr = allocator.Ptr + allocator.Length,
                temp = from + length;

            long st = endPtr - temp;
            //$"ex {st}".ToLog();
            return st < 0;
        }

        private void IncrementBucket()
        {
            int length = m_Bucket.Length;
            m_Bucket.Resize(length * 2);
        }

        private bool TryGetBucket(int length, out UnsafeReference<byte> p)
        {
            if (m_BucketCount == 0)
            {
                m_Bucket[0] = new Bucket(m_Buffer.Ptr, length);

                p = m_Buffer.Ptr;
                m_BucketCount++;

                //"count 0 true return".ToLog();
                return true;
            }

            if (m_BucketCount + 1 >= m_Bucket.Length)
            {
                IncrementBucket();
                //"increse".ToLog();
            }

            BucketComparer comparer = new BucketComparer(m_Buffer);
            m_Bucket.Sort(comparer, m_BucketCount);

            if (m_BucketCount > 0)
            {
                if (m_Bucket[0].Block - m_Buffer.Ptr >= length)
                {
                    p = m_Buffer.Ptr;
                    m_Bucket[m_BucketCount++] = new Bucket(p, length);

                    return true;
                }

                for (int i = 1; i < m_BucketCount; i++)
                {
                    if (!IsAllocatableBetween(m_Bucket[i - 1], m_Bucket[i].Block, length, out _))
                    {
                        continue;
                    }

                    p = m_Bucket[i - 1].GetNext();
                    m_Bucket[m_BucketCount++] = new Bucket(p, length);

                    //"btw return".ToLog();
                    return true;
                }
            }

            ref Bucket last = ref m_Bucket[m_BucketCount - 1];
            p = last.GetNext();

            if (IsExceedingAllocator(m_Buffer, p, length))
            {
                return false;
            }

            m_Bucket[m_BucketCount++] = new Bucket(p, length);
            //$"last return count?{m_BucketCount}".ToLog();

            return true;
        }

        public MemoryBlock Get(int length)
        {
            PointHelper.AssertMainThread();

            if (!TryGetBucket(length, out var p))
            {
                PointHelper.LogError(Channel.Collections,
                    $"You\'re trying to get memory size({length}) from {nameof(MemoryPool)} " +
                    $"that doesn\'t have free memory.");

                return default(MemoryBlock);
            }

            return new MemoryBlock(m_Hash, p, length);
        }
        public bool TryGet(int length, out MemoryBlock block)
        {
            PointHelper.AssertMainThread();

            if (!TryGetBucket(length, out var p))
            {
                block = default(MemoryBlock);
                return false;
            }

            block = new MemoryBlock(m_Hash, p, length);
            return true;
        }
        public void Reserve(MemoryBlock block)
        {
            PointHelper.AssertMainThread();

            if (!block.ValidateOwnership(in m_Hash))
            {
                PointHelper.LogError(Channel.Collections, $"");
                return;
            }

            BucketComparer comparer = new BucketComparer(m_Buffer);
            m_Bucket.Sort(comparer, m_BucketCount);

            int index = UnsafeBufferUtility.IndexOf(m_Bucket.Ptr, m_BucketCount, block.m_Block);
            UnsafeBufferUtility.RemoveAtSwapBack(m_Bucket.Ptr, m_BucketCount, index);

            m_BucketCount--;
        }

        public void Dispose()
        {
            m_Buffer.Dispose();
            m_Bucket.Dispose();
        }
    }
    public readonly struct MemoryBlock : IEquatable<MemoryBlock>
    {
        private readonly Hash m_Owner;

        internal readonly UnsafeReference<byte> m_Block;
        private readonly int m_Length;

        public ref byte this[int index] => ref m_Block[index];
        public UnsafeReference<byte> Ptr => m_Block;
        public int Length => m_Length;

        internal MemoryBlock(Hash owner, UnsafeReference<byte> p, int length)
        {
            m_Owner = owner;
            m_Block = p;
            m_Length = length;
        }

        internal bool ValidateOwnership(in Hash pool)
        {
            if (!pool.Equals(m_Owner)) return false;
            return true;
        }

        public bool Equals(MemoryBlock other) => m_Block.Equals(other.m_Block) && m_Length == other.m_Length;
    }
}
