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

using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Point.Collections.Buffer.LowLevel
{
    internal sealed class MemoryManager : CLRSingleTone<MemoryManager>
    {
        public const int
            SHARED_MEMORY_SIZE = 10240;

        private UnsafeMemoryPool m_SharedMemoryPool;
        private UnsafeLinearHashMap<uint, UnsafeMemoryBlock> m_LinearHashMap;

        protected override void OnInitialize()
        {
            m_SharedMemoryPool = new UnsafeMemoryPool(SHARED_MEMORY_SIZE, Allocator.Persistent);
        }
        protected override void OnDispose()
        {
            m_SharedMemoryPool.Dispose();
        }

        public static UnsafeMemoryBlock GetMemory(int length)
        {
            UnsafeMemoryBlock temp = Instance.m_SharedMemoryPool.Get(length);

            return temp;
        }
        public static UnsafeMemoryBlock<T> GetMemory<T>(int count)
            where T : unmanaged
        {
            UnsafeMemoryBlock temp = GetMemory(UnsafeUtility.SizeOf<T>() * count);

            return (UnsafeMemoryBlock<T>)temp;
        }

    }

    public struct NativeMemoryPool
    {
        private UnsafeAllocator<UnsafeBuffer> m_Buffer;

        internal ref UnsafeBuffer Buffer => ref m_Buffer[0];

        public NativeMemoryPool(int poolSize, Allocator allocator, int initialBucketSize = UnsafeMemoryPool.INITBUCKETSIZE)
        {
            m_Buffer = new UnsafeAllocator<UnsafeBuffer>(1, allocator);
            m_Buffer[0] = new UnsafeBuffer(poolSize, initialBucketSize, allocator);
        }

        public MemoryBlock GetMemory(in int length)
        {
            UnsafeMemoryBlock unsafeBlock = Buffer.Get(in length);

            MemoryBlock block = new MemoryBlock(m_HashMap, m_Pool.m_Hash, hash);

            return block;
        }

        internal struct UnsafeBuffer
        {
            private UnsafeMemoryPool m_Pool;
            
            public UnsafeBuffer(int poolSize, int bucketSize, Allocator allocator)
            {
                m_Pool = new UnsafeMemoryPool(poolSize, allocator, bucketSize);
            }

            private void ValidateBuffer()
            {
                if (m_Pool.IsMaxCapacity())
                {
                    int length = m_Pool.BlockCapacity * 2;
                    
                    m_Pool.ResizeBucket(length);
                }
            }

            public UnsafeMemoryBlock GetNoResize(in int length)
            {
                ValidateBuffer();

                UnsafeMemoryBlock block = m_Pool.Get(in length);
                
                return block;
            }
            public UnsafeMemoryBlock Get(in int length)
            {
                ValidateBuffer();

                if (!m_Pool.TryGet(in length, out UnsafeMemoryBlock block))
                {
                    m_Pool.ResizeMemoryPool(m_Pool.Length * 2);
                    return Get(in length);
                }

                return block;
            }
        }
    }

    public struct MemoryBlock
    {
        private UnsafeMemoryPool m_Pool;
        private Hash m_OwnerHash, m_Hash;

        internal MemoryBlock(UnsafeMemoryPool pool, 
            Hash ownerHash, Hash hash)
        {
            m_Pool = pool;
            m_OwnerHash = ownerHash;
            m_Hash = hash;
        }
    }
}
