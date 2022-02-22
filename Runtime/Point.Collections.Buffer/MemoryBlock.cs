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

namespace Point.Collections.Buffer
{
    public struct MemoryBlock
    {
        /// <summary>
        /// <see cref="LowLevel.UnsafeMemoryPool.Identifier"/>
        /// </summary>
        internal readonly Hash owner;
        /// <summary>
        /// <see cref="Buffer.LowLevel.UnsafeMemoryBlock.Identifier"/>
        /// </summary>
        internal readonly Hash identifier;

        private readonly UnsafeReference<NativeMemoryPool.UnsafeBuffer> m_Buffer;

        public UnsafeReference<byte> Ptr
        {
            get
            {
                if (!m_Buffer.Value.TryGetFromID(in identifier, out UnsafeMemoryBlock block))
                {
                    PointHelper.LogError(Channel.Collections,
                        $"");

                    return default(UnsafeReference<byte>);
                }

                return block.Ptr;
            }
        }

        internal MemoryBlock(Hash owner, Hash identifier, UnsafeReference<NativeMemoryPool.UnsafeBuffer> buffer)
        {
            this.owner = owner;
            this.identifier = identifier;
            m_Buffer = buffer;
        }
    }
}
