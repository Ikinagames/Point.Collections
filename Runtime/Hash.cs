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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
using Unity.Collections;
using UnityEngine;
#if !UNITY_COLLECTIONS
using FixedString512Bytes = Point.Collections.FixedChar512Bytes;
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Newtonsoft.Json;
using System;
using System.Runtime.InteropServices;

namespace Point.Collections
{
    [Serializable]
    [JsonConverter(typeof(IO.Json.HashJsonConverter))]
    [Guid("acdb109b-3a13-4ea2-8835-ef97b416cbb7")]
    public struct Hash : IEquatable<Hash>, IConvertible, IEmpty
    {
        public static Hash Empty => new Hash(0);
        public static Hash NewHash()
        {
            Guid guid = Guid.NewGuid();
            return new Hash(FNV1a32.Calculate(guid.ToByteArray()));
        }

#if UNITYENGINE
        [SerializeField]
#endif
        private uint m_Value;
#if DEBUG_MODE
        private FixedString512Bytes m_Key;
#endif

        public uint Value => m_Value;
#if DEBUG_MODE
        /// <summary>
        /// Editor only
        /// </summary>
#if UNITY_COLLECTIONS
        [NotBurstCompatible]
#endif
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
#if UNITYENGINE && UNITY_COLLECTIONS
        public Hash(FixedString4096Bytes key)
        {
            m_Value = FNV1a32.Calculate(key);
#if DEBUG_MODE
            m_Key = new FixedString512Bytes(key);
#endif
        }
        public Hash(FixedString512Bytes key)
        {
            m_Value = FNV1a32.Calculate(key);
#if DEBUG_MODE
            m_Key = key;
#endif
        }
        public Hash(FixedString128Bytes key)
        {
            m_Value = FNV1a32.Calculate(key);
#if DEBUG_MODE
            m_Key = key;
#endif
        }
#endif

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
#if UNITYENGINE && UNITY_COLLECTIONS
        [NotBurstCompatible]
#endif
        public override string ToString()
        {
            string str = string.Empty;
            str += m_Value;
#if DEBUG_MODE
            str += $"({m_Key})";
#endif

            return str;
        }

        public TypeCode GetTypeCode() => TypeCode.UInt32;

        public bool ToBoolean(IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public sbyte ToSByte(IFormatProvider provider) => unchecked(Convert.ToSByte(m_Value));
        public byte ToByte(IFormatProvider provider) => unchecked(Convert.ToByte(m_Value));
        public char ToChar(IFormatProvider provider) => unchecked(Convert.ToChar(m_Value));
        public short ToInt16(IFormatProvider provider) => unchecked(Convert.ToInt16(m_Value));
        public ushort ToUInt16(IFormatProvider provider) => unchecked(Convert.ToUInt16(m_Value));
        public int ToInt32(IFormatProvider provider) => unchecked(Convert.ToInt32(m_Value));
        public float ToSingle(IFormatProvider provider) => unchecked(Convert.ToSingle(m_Value));
        public long ToInt64(IFormatProvider provider) => Convert.ToInt64(m_Value);
        public uint ToUInt32(IFormatProvider provider) => m_Value;
        public ulong ToUInt64(IFormatProvider provider) => Convert.ToUInt64(m_Value);
        public DateTime ToDateTime(IFormatProvider provider) => Convert.ToDateTime(m_Value);
        public decimal ToDecimal(IFormatProvider provider) => Convert.ToDecimal(m_Value);
        public double ToDouble(IFormatProvider provider) => Convert.ToDouble(m_Value);
        public string ToString(IFormatProvider provider) => ToString();

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public bool IsEmpty() => m_Value == 0;

        public static bool operator ==(Hash x, Hash y) => x.Equals(y);
        public static bool operator !=(Hash x, Hash y) => !x.Equals(y);

        public static Hash operator ^(Hash x, Hash y) => new Hash(x.m_Value ^ y.m_Value);
        public static Hash operator |(Hash x, Hash y) => new Hash(x.m_Value | y.m_Value);

        public static implicit operator uint(Hash hash) => hash.m_Value;
    }
}
