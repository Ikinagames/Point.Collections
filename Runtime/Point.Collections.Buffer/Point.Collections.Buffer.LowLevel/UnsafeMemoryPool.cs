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

#if UNITY_2020_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Jobs.LowLevel.Unsafe;
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System;
using System.Collections.Generic;

namespace Point.Collections.Buffer.LowLevel
{
    /// <summary>
    /// 매 Allocation 을 피하기 위한 메모리 공간 재사용 구조체입니다. 
    /// </summary>
#if UNITYENGINE
    [BurstCompatible]
#endif
    public struct UnsafeMemoryPool :
#if UNITYENGINE
        INativeDisposable,
#endif
        IDisposable
    {
        public const int INITBUCKETSIZE = 8;

        internal readonly Hash m_Hash;
        private UnsafeAllocator<UnsafeBuffer> m_Buffer;

        internal ref UnsafeBuffer Buffer => ref m_Buffer[0];

        /// <summary>
        /// 새로운 Memory Pool 을 생성합니다.
        /// </summary>
        /// <param name="size"></param>
        /// <param name="allocator"></param>
        public UnsafeMemoryPool(int size
#if UNITYENGINE
            , Allocator allocator
#endif
            , int bucketSize = INITBUCKETSIZE)
        {
            m_Hash = Hash.NewHash();

            m_Buffer = new UnsafeAllocator<UnsafeBuffer>(1
#if UNITYENGINE
                , allocator
#endif
                );
            m_Buffer[0] = new UnsafeBuffer(new UnsafeAllocator<byte>(size
#if UNITYENGINE
                , allocator
#endif
                ), new UnsafeAllocator<UnsafeMemoryBlock>(bucketSize
#if UNITYENGINE
                , allocator
#endif
                ));
        }
        /// <summary>
        /// 이미 존재하는 버퍼를 Memory Pool 로 Wrapping 합니다.
        /// </summary>
        /// <param name="buffer"></param>
        public UnsafeMemoryPool(UnsafeAllocator<byte> buffer, int bucketSize = INITBUCKETSIZE)
        {
            m_Hash = Hash.NewHash();

            m_Buffer = new UnsafeAllocator<UnsafeBuffer>(1
#if UNITYENGINE
                , buffer.m_Buffer.m_Allocator
#endif
                );
            m_Buffer[0] = new UnsafeBuffer(buffer, new UnsafeAllocator<UnsafeMemoryBlock>(bucketSize
#if UNITYENGINE
                , buffer.m_Buffer.m_Allocator
#endif
                ));
        }

        private void SortBucket()
        {
            BucketComparer comparer = new BucketComparer(m_Buffer[0].buffer);
            m_Buffer[0].blocks.Sort(comparer);
        }

        private void IncrementBucket()
        {
            int length = m_Buffer[0].blocks.Capacity;
            m_Buffer[0].ResizeMemoryBlock(length * 2);
        }
        private UnsafeMemoryBlock GetBucket(in int length)
        {
            UnsafeMemoryBlock block;
            if (m_Buffer[0].blocks.IsEmpty)
            {
                block = new UnsafeMemoryBlock(m_Hash, m_Buffer[0].buffer.Ptr, length);
                m_Buffer[0].blocks.AddNoResize(block);

                return block;
            }

            if (m_Buffer[0].blocks.Length + 1 >= m_Buffer[0].blocks.Capacity)
            {
                IncrementBucket();
            }
            SortBucket();

            if (m_Buffer[0].blocks[0].Ptr - Buffer.buffer.Ptr >= length)
            {
                block = new UnsafeMemoryBlock(m_Hash, m_Buffer[0].buffer.Ptr, length);
                m_Buffer[0].blocks.AddNoResize(block);

                return block;
            }

            for (int i = 1; i < m_Buffer[0].blocks.Length; i++)
            {
                if (!IsAllocatableBetween(m_Buffer[0].blocks[i - 1], m_Buffer[0].blocks[i].Ptr, length, out _))
                {
                    continue;
                }

                block = new UnsafeMemoryBlock(m_Hash, m_Buffer[0].blocks[i - 1].Last(), length);
                m_Buffer[0].blocks.AddNoResize(block);

                return block;
            }

            var last = m_Buffer[0].blocks.Last;
            var p = last.Last();

            if (IsExceedingAllocator(m_Buffer[0].buffer, p, length))
            {
#if DEBUG_MODE
                PointHelper.LogError(Channel.Collections,
                    $"You\'re trying to get memory size({length}) from {nameof(UnsafeMemoryPool)} " +
                    $"that doesn\'t have free memory.");
#endif
                return default(UnsafeMemoryBlock);
            }

            block = new UnsafeMemoryBlock(m_Hash, p, length);
            return block;
        }
        private bool TryGetBucket(in int length, out UnsafeMemoryBlock block)
        {
            if (m_Buffer[0].blocks.IsEmpty)
            {
                block = new UnsafeMemoryBlock(m_Hash, m_Buffer[0].buffer.Ptr, length);
                m_Buffer[0].blocks.AddNoResize(block);

                return true;
            }

            if (m_Buffer[0].blocks.Length + 1 >= m_Buffer[0].blocks.Capacity)
            {
                IncrementBucket();
            }
            SortBucket();

            if (m_Buffer[0].blocks[0].Ptr - Buffer.buffer.Ptr >= length)
            {
                block = new UnsafeMemoryBlock(m_Hash, m_Buffer[0].buffer.Ptr, length);
                m_Buffer[0].blocks.AddNoResize(block);

                return true;
            }

            for (int i = 1; i < m_Buffer[0].blocks.Length; i++)
            {
                if (!IsAllocatableBetween(m_Buffer[0].blocks[i - 1], m_Buffer[0].blocks[i].Ptr, length, out _))
                {
                    continue;
                }

                block = new UnsafeMemoryBlock(m_Hash, m_Buffer[0].blocks[i - 1].Last(), length);
                m_Buffer[0].blocks.AddNoResize(block);

                return true;
            }

            var last = m_Buffer[0].blocks.Last;
            var p = last.Last();

            if (IsExceedingAllocator(m_Buffer[0].buffer, p, length))
            {
#if DEBUG_MODE
                PointHelper.LogError(Channel.Collections,
                    $"You\'re trying to get memory size({length}) from {nameof(UnsafeMemoryPool)} " +
                    $"that doesn\'t have free memory.");
#endif
                block = default(UnsafeMemoryBlock);
                return false;
            }

            block = new UnsafeMemoryBlock(m_Hash, p, length);
            return true;
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
            SortBucket();
            m_Buffer[0].blocks.RemoveSwapback(block);
        }

        public void Dispose()
        {
            PointHelper.AssertMainThread();

            m_Buffer[0].Dispose();
            m_Buffer.Dispose();
        }
#if UNITYENGINE
        public JobHandle Dispose(JobHandle inputDeps)
        {
            var job = m_Buffer[0].Dispose(inputDeps);
            job = m_Buffer.Dispose(job);

            return job;
        }
#endif

        #region Inner Classes

#if UNITYENGINE
        [BurstCompatible]
#endif
        internal struct UnsafeBuffer : IDisposable
#if UNITYENGINE
            , INativeDisposable
#endif
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

            public void ResizeMemoryBlock(in int length)
            {
                m_MemoryBlockBuffer.Resize(length);
                var temp = new UnsafeFixedListWrapper<UnsafeMemoryBlock>(m_MemoryBlockBuffer, blocks.Length);
                blocks = temp;
            }

            public void Dispose()
            {
                buffer.Dispose();
                m_MemoryBlockBuffer.Dispose();
            }
#if UNITYENGINE
            public JobHandle Dispose(JobHandle inputDeps)
            {
                var job = buffer.Dispose(inputDeps);
                job = m_MemoryBlockBuffer.Dispose(job);

                return job;
            }
#endif
        }

#if UNITYENGINE
        [BurstCompatible]
#endif
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
            return st < 0;
        }

        #endregion
    }
    public static class MemoryPoolExtensions
    {
        
    }
}
