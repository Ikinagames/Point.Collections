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

using Point.Collections.Burst;
using System;

namespace Point.Collections
{
    /// <summary>
    /// CheckSum 알고리즘으로 데이터 무결성 검사를 하는 구조체입니다.
    /// </summary>
#if UNITYENGINE && UNITY_COLLECTIONS
    [BurstCompatible]
#endif
    public struct CheckSum : IEquatable<CheckSum>, IEquatable<int>, IEquatable<uint>
    {
        /// <summary>
        /// 데이터 <paramref name="data"/> 를 CheckSum 알고리즘으로 연산된 해시를 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static CheckSum Calculate<T>(T data) where T : unmanaged
        {
            return new CheckSum(CheckSumMathematics.Calculate(data));
        }
        /// <summary>
        /// <inheritdoc cref="Calculate{T}(T)"/>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static CheckSum Calculate(byte[] data)
        {
            return new CheckSum(CheckSumMathematics.Calculate(data));
        }

        private readonly uint m_Hash;

        /// <summary>
        /// 계산된 해시 값입니다.
        /// </summary>
        public uint Hash => m_Hash;

        private CheckSum(uint hash)
        {
            m_Hash = hash;
        }

        /// <summary>
        /// 데이터 <paramref name="data"/> 와 비교하여 무결성 검사를 수행합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool Validate<T>(in T data) where T : unmanaged
        {
            uint result = CheckSumMathematics.Validate(data, in m_Hash);
            return result == 0;
        }
        public bool Validate(in byte[] data)
        {
            uint result = CheckSumMathematics.Validate(data, in m_Hash);
            return result == 0;
        }

#if UNITYENGINE && UNITY_COLLECTIONS
        [NotBurstCompatible]
#endif
        public override string ToString() => m_Hash.ToString();
#if UNITYENGINE && UNITY_COLLECTIONS
        [NotBurstCompatible]
#endif
        public override bool Equals(
#if !UNITYENGINE
            [System.Diagnostics.CodeAnalysis.AllowNull]
#endif
            object obj)
        {
            if (obj == null || !(obj is CheckSum other)) return false;

            return Equals(other);
        }
        public bool Equals(CheckSum other) => m_Hash == other.m_Hash;
        public bool Equals(int other) => m_Hash == other;
        public bool Equals(uint other) => m_Hash == other;
        public override int GetHashCode() => Convert.ToInt32(m_Hash);
    }
}
