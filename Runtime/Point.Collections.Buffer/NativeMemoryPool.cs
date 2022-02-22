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

using Point.Collections.Buffer.LowLevel;
using Unity.Collections;

namespace Point.Collections.Buffer
{
    public struct NativeMemoryPool
    {
        private UnsafeAllocator<UnsafeBuffer> m_Buffer;

        internal ref UnsafeBuffer Buffer => ref m_Buffer[0];

        public NativeMemoryPool(int poolSize, Allocator allocator, int initialBucketSize = UnsafeMemoryPool.INITBUCKETSIZE)
        {
            m_Buffer = new UnsafeAllocator<UnsafeBuffer>(1, allocator);
            m_Buffer[0] = new UnsafeBuffer(poolSize, initialBucketSize, allocator);
        }

        //public bool Validate(in MemoryBlock memory)
        //{
            
        //}
        public MemoryBlock Get(in int length)
        {
            UnsafeMemoryBlock unsafeBlock = Buffer.Get(in length);
            MemoryBlock block = new MemoryBlock(Buffer.Pool.Identifier, unsafeBlock.Identifier, m_Buffer.Ptr);

            return block;
        }
        public void Reserve(in MemoryBlock block)
        {
#if DEBUG_MODE
            // MemoryPool 이 Resizing 되었을 경우, 이전 레퍼런스는 경고를 뱉는다.
            if (!Buffer.Pool.Identifier.Equals(block.owner))
            {
                PointHelper.LogWarning(Channel.Collections,
                    $"You\'re trying to access memory block with old reference.");
            }
#endif
            if (!Buffer.TryGetFromID(block.identifier, out var unsafeBlock))
            {
                PointHelper.LogError(Channel.Collections, $"");
                return;
            }

            Buffer.Reserve(unsafeBlock);
        }

        internal struct UnsafeBuffer
        {
            private UnsafeMemoryPool m_Pool;

            public UnsafeMemoryPool Pool => m_Pool;

            public UnsafeBuffer(int poolSize, int bucketSize, Allocator allocator)
            {
                m_Pool = new UnsafeMemoryPool(poolSize, allocator, bucketSize);
            }

            private void ValidateBuffer()
            {
                if (m_Pool.IsMaxCapacity())
                {
                    int length = m_Pool.BlockCapacity * 2;

                    m_Pool.ResizeMemoryBlock(length);
                }
            }

            //public bool Validate()
            //{

            //}
            public UnsafeMemoryBlock GetNoResize(in int length)
            {
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

            public void Reserve(UnsafeMemoryBlock block)
            {
                m_Pool.Reserve(block);
            }

            public bool TryGetFromID(in Hash id, out UnsafeMemoryBlock block)
            {
                return m_Pool.TryGetMemoryBlockFromID(in id, out block);
            }
        }
    }
}
