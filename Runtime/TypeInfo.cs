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

#if UNITY_2020
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Point.Collections.Native;
using System;
using System.Runtime.InteropServices;
#if UNITYENGINE
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
#endif

namespace Point.Collections
{
    /// <summary>
    /// Runtime 중 기본 <see cref="System.Type"/> 의 정보를 저장하고, 해당 타입의 binary 크기, alignment를 저장합니다.
    /// </summary>
#if UNITYENGINE
    [BurstCompatible]
#endif
    [Guid("f090b023-0343-4436-811b-7ea2360503d0")]
    public readonly struct TypeInfo : IValidation, IEquatable<TypeInfo>
    {
        private readonly RuntimeTypeHandle m_TypeHandle;
        private readonly int m_Size;
        private readonly int m_Align;

        private readonly int m_HashCode;
        private readonly bool m_IsUnmanaged;

#if UNITYENGINE
        [NotBurstCompatible]
#endif
        public Type Type
        {
            get
            {
                if (m_TypeHandle.Value.Equals(IntPtr.Zero))
                {
                    return null;
                }
                return Type.GetTypeFromHandle(m_TypeHandle);
            }
        }
        public int Size => m_Size;
        public int Align => m_Align;
        public bool IsUnmanaged => m_IsUnmanaged;

#if UNITYENGINE
        [NotBurstCompatible]
#endif
        internal TypeInfo(Type type)
        {
            if (!TypeHelper.IsUnmanaged(type))
            {
                this = default(TypeInfo);

                m_TypeHandle = type.TypeHandle;

                m_IsUnmanaged = false;
                return;
            }

            this = new TypeInfo(type, TypeHelper.SizeOf(type), TypeHelper.AlignOf(type), CollectionUtility.CreateHashCode());
        }
#if UNITYENGINE
        [NotBurstCompatible]
#endif
        internal TypeInfo(Type type, int size, int align, int hashCode)
        {
            m_TypeHandle = type.TypeHandle;
            m_Size = size;
            m_Align = align;

            unchecked
            {
                // https://stackoverflow.com/questions/102742/why-is-397-used-for-resharper-gethashcode-override
                m_HashCode = hashCode * 397;
            }
            m_IsUnmanaged = true;
        }

        public override int GetHashCode() => m_HashCode;

        public bool Equals(TypeInfo other) => m_TypeHandle.Equals(other.m_TypeHandle);

        public bool IsValid()
        {
            if (m_TypeHandle.Value == IntPtr.Zero ||
                m_Size == 0 || m_HashCode == 0) return false;

            return true;
        }

        public static implicit operator Type(TypeInfo t) => t.Type;
    }
}
