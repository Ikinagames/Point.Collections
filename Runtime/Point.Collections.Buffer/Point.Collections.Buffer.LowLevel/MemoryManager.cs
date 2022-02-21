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
}
