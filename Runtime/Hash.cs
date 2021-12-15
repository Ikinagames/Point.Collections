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

using System;
using Unity.Collections;
using UnityEngine;

namespace Point.Collections
{
    [Serializable]
    public struct Hash : IEquatable<Hash>
    {
        public static Hash NewHash()
        {
            Guid guid = Guid.NewGuid();
            return new Hash(FNV1a32.Calculate(guid.ToByteArray()));
        }

        [SerializeField] private uint m_Value;
#if DEBUG_MODE
        private Unity.Collections.FixedString512Bytes m_Key;
#endif

        public uint Value => m_Value;
#if DEBUG_MODE
        /// <summary>
        /// Editor only
        /// </summary>
        [NotBurstCompatible]
        public string Key => m_Key.ToString();
#endif

        public Hash(uint hash)
        {
            m_Value = hash;
#if DEBUG_MODE
            m_Key = "NULL";
#endif
        }
        public Hash(string key)
        {
            m_Value = FNV1a32.Calculate(key);
#if DEBUG_MODE
            m_Key = key;
#endif
        }

        public bool Equals(Hash other) => m_Value.Equals(other.m_Value);
        public override bool Equals(object obj)
        {
            if (obj is Hash y)
            {
                return m_Value.Equals(y.m_Value);
            }

            return false;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (int)m_Value;
            }
        }
        [NotBurstCompatible]
        public override string ToString()
        {
            string str = string.Empty;
            str += m_Value;
#if DEBUG_MODE
            str += $"({m_Key})";
#endif

            return str;
        }

        public static bool operator ==(Hash x, Hash y)
        {
            return x.Equals(y);
        }
        public static bool operator !=(Hash x, Hash y)
        {
            return !x.Equals(y);
        }

        public static Hash operator ^(Hash x, Hash y)
        {
            return new Hash(x.m_Value ^ y.m_Value);
        }
        public static Hash operator |(Hash x, Hash y)
        {
            return new Hash(x.m_Value | y.m_Value);
        }
    }
}
