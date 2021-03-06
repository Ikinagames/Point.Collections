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
#if UNITY_COLLECTIONS
using Unity.Collections;
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif


namespace Point.Collections
{
    public struct FixedChar512Bytes
    {
        private Char510 m_Buffer;
        private ushort m_Length;

        public int Length => m_Length;
        public bool IsEmpty => m_Length == 0;

        public FixedChar512Bytes(string str)
        {
            m_Buffer = str;
            m_Length = (ushort)str.Length;
        }

        public override string ToString() => m_Buffer.Read(0, m_Length);

        public static implicit operator FixedChar512Bytes(string t) => new FixedChar512Bytes(t);
        public static implicit operator string(FixedChar512Bytes t) => t.ToString();
    }
}
