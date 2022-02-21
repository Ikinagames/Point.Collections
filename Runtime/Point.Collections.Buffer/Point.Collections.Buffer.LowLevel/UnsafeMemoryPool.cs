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
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;

namespace Point.Collections.Buffer.LowLevel
{
    /// <summary>
    /// 매 Allocation 을 피하기 위한 메모리 공간 재사용 구조체입니다. 
    /// </summary>
    [BurstCompatible]
    public struct UnsafeMemoryPool : INativeDisposable, IDisposable
    {
        public const int INITBUCKETSIZE = 8;

        internal readonly Hash m_Hash;
        private UnsafeAllocator<UnsafeBuffer> m_Buffer;

        internal ref UnsafeBuffer Buffer => ref m_Buffer[0];

        /// <summary>
        /// 이 풀이 가진 메모리 크기입니다.
        /// </summary>
        public int Length => Buffer.buffer.Length;
        /// <summary>
        /// 반환 가능한 최대 메모리 포인터 갯수입니다.
        /// </summary>
        public int BlockCapacity => m_Buffer[0].blocks.Capacity;
        /// <summary>
        /// 현재 사용 중인 메모리 포인터 갯수입니다.
        /// </summary>
        public int BlockCount => m_Buffer[0].blocks.Length;

        /// <summary>
        /// 새로운 Memory Pool 을 생성합니다.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="allocator"></param>
        public UnsafeMemoryPool(int size, Allocator allocator, int bucketSize = INITBUCKETSIZE)
        {
            m_Hash = Hash.NewHash();

            m_Buffer = new UnsafeAllocator<UnsafeBuffer>(1, allocator);
            m_Buffer[0] = new UnsafeBuffer(new UnsafeAllocator<byte>(size, allocator), new UnsafeAllocator<UnsafeMemoryBlock>(bucketSize, allocator));
        }
        /// <summary>
        /// 이미 존재하는 버퍼를 Memory Pool 로 Wrapping 합니다.
        /// </summary>
        /// <param name="buffer"></param>
        public UnsafeMemoryPool(UnsafeAllocator<byte> buffer, int bucketSize = INITBUCKETSIZE)
        {
            m_Hash = Hash.NewHash();

            m_Buffer = new UnsafeAllocator<UnsafeBuffer>(1, buffer.m_Buffer.m_Allocator);
            m_Buffer[0] = new UnsafeBuffer(buffer, new UnsafeAllocator<UnsafeMemoryBlock>(bucketSize, buffer.m_Buffer.m_Allocator));
        }

        private void SortBucket()
        {
            BucketComparer comparer = new BucketComparer(m_Buffer[0].buffer);
            m_Buffer[0].blocks.Sort(comparer);
        }

        private UnsafeMemoryBlock GetBucket(in int length)
        {
            UnsafeMemoryBlock block;
            if (m_Buffer[0].blocks.IsEmpty)
            {
                block = new UnsafeMemoryBlock(m_Hash, m_Buffer[0].buffer.Ptr, 
                    0, length);
                m_Buffer[0].blocks.AddNoResize(block);

                return block;
            }

#if DEBUG_MODE
            if (IsMaxCapacity())
            {
                PointHelper.LogError(Channel.Collections,
                    $"You\'re trying to get memory size({length}) from {nameof(UnsafeMemoryPool)} " +
                    $"that exceeding max memory block capacity. " +
                    $"You can increase capacity with {nameof(ResizeBucket)}.");
                return default(UnsafeMemoryBlock);
            }
#endif
            SortBucket();

            if (m_Buffer[0].blocks[0].Ptr - Buffer.buffer.Ptr >= length)
            {
                block = new UnsafeMemoryBlock(m_Hash, m_Buffer[0].buffer.Ptr, 
                    0, length);
                m_Buffer[0].blocks.AddNoResize(block);

                return block;
            }

            for (int i = 1; i < m_Buffer[0].blocks.Length; i++)
            {
                if (!IsAllocatableBetween(m_Buffer[0].blocks[i - 1], m_Buffer[0].blocks[i].Ptr, length, out _))
                {
                    continue;
                }

                UnsafeReference<byte> temp = m_Buffer[0].blocks[i - 1].Last();
                block = new UnsafeMemoryBlock(m_Hash, temp, temp - m_Buffer[0].buffer.Ptr, length);
                m_Buffer[0].blocks.AddNoResize(block);

                return block;
            }

            var last = m_Buffer[0].blocks.Last;
            var p = last.Last();

#if DEBUG_MODE
            if (IsExceedingAllocator(m_Buffer[0].buffer, p, length))
            {
                PointHelper.LogError(Channel.Collections,
                    $"You\'re trying to get memory size({length}) from {nameof(UnsafeMemoryPool)} " +
                    $"that doesn\'t have free memory.");
                return default(UnsafeMemoryBlock);
            }
#endif

            block = new UnsafeMemoryBlock(m_Hash, p, p - m_Buffer[0].buffer.Ptr, length);
            return block;
        }
        private bool TryGetBucket(in int length, out UnsafeMemoryBlock block)
        {
            if (m_Buffer[0].blocks.IsEmpty)
            {
                block = new UnsafeMemoryBlock(m_Hash, m_Buffer[0].buffer.Ptr, 0, length);
                m_Buffer[0].blocks.AddNoResize(block);

                return true;
            }

#if DEBUG_MODE
            if (IsMaxCapacity())
            {
                PointHelper.LogError(Channel.Collections,
                    $"You\'re trying to get memory size({length}) from {nameof(UnsafeMemoryPool)} " +
                    $"that exceeding max memory block capacity. " +
                    $"You can increase capacity with {nameof(ResizeBucket)}.");
                block = default(UnsafeMemoryBlock);
                return false;
            }
#endif
            SortBucket();

            if (m_Buffer[0].blocks[0].Ptr - Buffer.buffer.Ptr >= length)
            {
                block = new UnsafeMemoryBlock(m_Hash, m_Buffer[0].buffer.Ptr, 0, length);
                m_Buffer[0].blocks.AddNoResize(block);

                return true;
            }

            for (int i = 1; i < m_Buffer[0].blocks.Length; i++)
            {
                if (!IsAllocatableBetween(m_Buffer[0].blocks[i - 1], m_Buffer[0].blocks[i].Ptr, length, out _))
                {
                    continue;
                }

                UnsafeReference<byte> temp = m_Buffer[0].blocks[i - 1].Last();
                block = new UnsafeMemoryBlock(m_Hash, temp, temp - m_Buffer[0].buffer.Ptr, length);
                m_Buffer[0].blocks.AddNoResize(block);

                return true;
            }

            var last = m_Buffer[0].blocks.Last;
            var p = last.Last();

#if DEBUG_MODE
            if (IsExceedingAllocator(m_Buffer[0].buffer, p, length))
            {
                PointHelper.LogError(Channel.Collections,
                    $"You\'re trying to get memory size({length}) from {nameof(UnsafeMemoryPool)} " +
                    $"that doesn\'t have free memory.");
                block = default(UnsafeMemoryBlock);
                return false;
            }
#endif

            block = new UnsafeMemoryBlock(m_Hash, p, p - m_Buffer[0].buffer.Ptr, length);
            return true;
        }

        public bool IsMaxCapacity() => BlockCount >= BlockCapacity;
        public void ResizeMemoryPool(int length)
        {
            Buffer.ResizeBuffer(m_Hash, in length);
        }
        public void ResizeBucket(int length)
        {
            m_Buffer[0].ResizeMemoryBlock(length);
        }

        /// <summary>
        /// <paramref name="length"/> bytes 만큼 메모리 주소를 할당받습니다.
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public UnsafeMemoryBlock Get(in int length)
        {
            PointHelper.AssertMainThread();

            UnsafeMemoryBlock p = GetBucket(in length);
            return p;
        }
        /// <summary>
        /// <inheritdoc cref="Get(int)"/>
        /// </summary>
        /// <param name="length"></param>
        /// <param name="block"></param>
        /// <returns></returns>
        public bool TryGet(in int length, out UnsafeMemoryBlock block)
        {
            PointHelper.AssertMainThread();

            if (!TryGetBucket(length, out var p))
            {
                block = default(UnsafeMemoryBlock);
                return false;
            }

            //block = new MemoryBlock(m_Hash, p, length);
            block = p;
            return true;
        }
        /// <summary>
        /// 이 풀에서 할당받은 메모리를 반환합니다.
        /// </summary>
        /// <remarks>
        /// 이 메모리 풀이 아닌 곳에서 할당받은 메모리를 반환하려하면 에러가 발생합니다.
        /// </remarks>
        /// <param name="block"></param>
        public void Reserve(UnsafeMemoryBlock block)
        {
            PointHelper.AssertMainThread();
#if DEBUG_MODE
            if (!block.ValidateOwnership(in m_Hash))
            {
                PointHelper.LogError(Channel.Collections, $"");
                return;
            }
#endif
            //BucketComparer comparer = new BucketComparer(m_Buffer[0].m_Buffer);
            //m_Buffer[0].m_Blocks.Sort(comparer);
            //m_Buffer[0].buckets.Sort(comparer, m_Buffer[0].bucketCount);
            SortBucket();
            m_Buffer[0].blocks.RemoveSwapback(block);

            //int index = UnsafeBufferUtility.IndexOf(m_Buffer[0].buckets.Ptr, m_Buffer[0].bucketCount, block.m_Block);
            //UnsafeBufferUtility.RemoveAtSwapBack(m_Buffer[0].buckets.Ptr, m_Buffer[0].bucketCount, index);

            //m_Buffer[0].bucketCount--;
        }

        public bool TryGetMemoryBlockFromID(in Hash id, out UnsafeMemoryBlock block)
        {
            return Buffer.TryGetMemoryBlockFromID(id, out block);
        }

        public void Dispose()
        {
            PointHelper.AssertMainThread();

            m_Buffer[0].Dispose();
            m_Buffer.Dispose();
        }
        public JobHandle Dispose(JobHandle inputDeps)
        {
            var job = m_Buffer[0].Dispose(inputDeps);
            job = m_Buffer.Dispose(job);

            return job;
        }

        #region Inner Classes

        [BurstCompatible]
        internal struct UnsafeBuffer : IDisposable, INativeDisposable
        {
            public UnsafeAllocator<byte> buffer;
            private UnsafeAllocator<UnsafeMemoryBlock> m_MemoryBlockBuffer;
            
            public UnsafeFixedListWrapper<UnsafeMemoryBlock> blocks;

            public UnsafeBuffer(UnsafeAllocator<byte> buffer, UnsafeAllocator<UnsafeMemoryBlock> blocks)
            {
                this.buffer = buffer;
                m_MemoryBlockBuffer = blocks;
                this.blocks = new UnsafeFixedListWrapper<UnsafeMemoryBlock>(m_MemoryBlockBuffer, 0);
            }

            public void ResizeBuffer(Hash owner, in int length)
            {
                PointHelper.AssertMainThread();

                if (length < buffer.Length)
                {
                    throw new NotImplementedException();
                }

                Allocator allocator = buffer.m_Buffer.m_Allocator;
                buffer.Resize(length);

                UnsafeAllocator<UnsafeMemoryBlock> newBlockBuffer = new UnsafeAllocator<UnsafeMemoryBlock>(blocks.Capacity, allocator);
                UnsafeFixedListWrapper<UnsafeMemoryBlock> tempBlocks = new UnsafeFixedListWrapper<UnsafeMemoryBlock>(newBlockBuffer, 0);

                if (blocks.Length > 0)
                {
                    for (int i = 0; i < blocks.Length; i++)
                    {
                        UnsafeMemoryBlock current = blocks[i];
                        UnsafeMemoryBlock temp = new UnsafeMemoryBlock(
                            owner, buffer.Ptr + current.Index, 
                            current.Index, current.Length);

                        tempBlocks.AddNoResize(temp);
                    }
                }

                m_MemoryBlockBuffer.Dispose();
                m_MemoryBlockBuffer = newBlockBuffer;
                blocks = tempBlocks;
            }
            public void ResizeMemoryBlock(in int length)
            {
                PointHelper.AssertMainThread();

                m_MemoryBlockBuffer.Resize(length);
                var temp = new UnsafeFixedListWrapper<UnsafeMemoryBlock>(m_MemoryBlockBuffer, blocks.Length);
                blocks = temp;
            }

            public bool TryGetMemoryBlockFromID(in Hash id, out UnsafeMemoryBlock block)
            {
                for (int i = 0; i < blocks.Length; i++)
                {
                    if (blocks[i].Identifier.Equals(id))
                    {
                        block = blocks[i];
                        return true;
                    }
                }

                block = default(UnsafeMemoryBlock);
                return false;
            }
            public UnsafeMemoryBlock GetMemoryBlockFromID(in Hash id)
            {
                for (int i = 0; i < blocks.Length; i++)
                {
                    if (blocks[i].Identifier.Equals(id)) return blocks[i];
                }

                return default(UnsafeMemoryBlock);
            }

            public void Dispose()
            {
                buffer.Dispose();
                m_MemoryBlockBuffer.Dispose();
                //buckets.Dispose();
            }
            public JobHandle Dispose(JobHandle inputDeps)
            {
                var job = buffer.Dispose(inputDeps);
                job = m_MemoryBlockBuffer.Dispose(job);
                //job = buckets.Dispose(job);

                return job;
            }
        }
        //[BurstCompatible]
        //public struct Bucket : IEquatable<UnsafeReference<byte>>, IEquatable<Bucket>
        //{
        //    private KeyValue<UnsafeReference<byte>, int> m_Block;

        //    public Bucket(UnsafeReference<byte> p, int length)
        //    {
        //        m_Block = new KeyValue<UnsafeReference<byte>, int>(p, length);
        //    }

        //    public UnsafeReference<byte> Block => m_Block.Key;
        //    public int Length => m_Block.Value;

        //    public UnsafeReference<byte> GetNext()
        //    {
        //        return Block + Length;
        //    }

        //    public bool Equals(UnsafeReference<byte> other) => Block.Equals(other);
        //    public bool Equals(Bucket other) => Block.Equals(other.Block) && Length == other.Length;
        //}
        [BurstCompatible]
        private struct BucketComparer : IComparer<UnsafeMemoryBlock>
        {
            private UnsafeReference<byte> m_Buffer;

            public BucketComparer(UnsafeAllocator<byte> buffer)
            {
                m_Buffer = buffer.Ptr;
            }

            public int Compare(UnsafeMemoryBlock x, UnsafeMemoryBlock y)
            {
                long
                    a = x.Ptr - m_Buffer,
                    b = y.Ptr - m_Buffer;

                if (a < b) return -1;
                else if (a > b) return 1;
                return 0;
            }
        }

        #endregion

        #region Calculations

        private static bool IsAllocatableBetween(UnsafeMemoryBlock a, UnsafeReference<byte> b, int length, out long freeSpace)
        {
            freeSpace = UnsafeBufferUtility.CalculateFreeSpaceBetween(a.Ptr, a.Length, b);

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

        #endregion
    }
    public static class MemoryPoolExtensions
    {
        
    }
}
