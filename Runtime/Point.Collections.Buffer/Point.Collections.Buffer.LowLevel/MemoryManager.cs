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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#if UNITY_2019 && !UNITY_2020_OR_NEWER
#define UNITYENGINE_OLD
#endif
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if !UNITYENGINE_OLD
// https://issuetracker.unity3d.com/issues/ecs-compiler-wrongly-detect-unmanaged-structs-as-containing-nullabe-fields

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
            m_SharedMemoryPool = new UnsafeMemoryPool(SHARED_MEMORY_SIZE
#if UNITYENGINE
                , Allocator.Persistent
#endif
                );
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
            UnsafeMemoryBlock temp = GetMemory(TypeHelper.SizeOf<T>() * count);

            return (UnsafeMemoryBlock<T>)temp;
        }

    }

    public struct NativeMemoryPool
    {
        private UnsafeMemoryPool m_Pool;
        private UnsafeLinearHashMap<Hash, UnsafeMemoryBlock> m_HashMap;

        //public NativeMemoryPool(Allocator allocator)
        //{

        //}

        public MemoryBlock GetMemory(in int length)
        {
            if (!m_Pool.TryGet(length, out UnsafeMemoryBlock unsafeBlock))
            {
                throw new System.Exception();
            }

            Hash hash = Hash.NewHash();
            m_HashMap.Add(hash, unsafeBlock);

            MemoryBlock block = new MemoryBlock(m_HashMap, m_Pool.m_Hash, hash);

            return block;
        }
    }

    public struct MemoryBlock
    {
        private UnsafeLinearHashMap<Hash, UnsafeMemoryBlock> m_HashMap;
        private Hash m_OwnerHash, m_Hash;

        internal MemoryBlock(UnsafeLinearHashMap<Hash, UnsafeMemoryBlock> hashMap, 
            Hash ownerHash, Hash hash)
        {
            m_HashMap = hashMap;
            m_OwnerHash = ownerHash;
            m_Hash = hash;
        }
    }
}

#endif