// Copyright 2021 Ikina Games
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

namespace Point.Collections.Buffer
{
    public struct MemoryBlock
    {
        private UnsafeMemoryPool.UnsafeBuffer m_Pool;
        private Hash m_OwnerHash, m_Identifier;

        public Hash Identifier => m_Identifier;
        public UnsafeReference<byte> Ptr
        {
            get
            {
                var block = m_Pool.GetMemoryBlockFromID(in m_Identifier);
                return block.Ptr;
            }
        }

        internal MemoryBlock(UnsafeMemoryPool pool,
            Hash ownerHash, Hash hash)
        {
            m_Pool = pool.Buffer;
            m_OwnerHash = ownerHash;
            m_Identifier = hash;
        }
    }
}
