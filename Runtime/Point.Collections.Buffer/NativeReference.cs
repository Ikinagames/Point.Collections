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

#if UNITY_2020
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Point.Collections.Buffer.LowLevel;
using Point.Collections.Threading;
using System;

namespace Point.Collections.Buffer
{
#if UNITYENGINE
    [BurstCompatible]
#endif
    public struct NativeReference<T> : IEquatable<NativeReference<T>>
        where T : unmanaged
    {
        private UnsafeReference<T> m_Ptr;
#if DEBUG_MODE
        private ThreadInfo m_Owner;
#endif

        /// <summary>
        /// <typeparamref name="T"/> 의 size * <paramref name="index"/> 만큼 
        /// 포인터를 오른쪽으로 밀어서 반환합니다.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ref T this[int index]
        {
            get
            {
#if DEBUG_MODE
                PointHelper.AssertThreadAffinity(in m_Owner);
#endif
                return ref m_Ptr[index];
            }
        }

        public bool IsCreated => m_Ptr.IsCreated;
        public ref T Value
        {
            get
            {
#if DEBUG_MODE
                PointHelper.AssertThreadAffinity(in m_Owner);
#endif
                return ref m_Ptr.Value;
            }
        }

        public NativeReference(UnsafeReference<T> ptr)
        {
            m_Ptr = ptr;
#if DEBUG_MODE
            m_Owner = ThreadInfo.CurrentThread;
#endif
        }
#if UNITYENGINE
        public NativeReference(NativeArray<T> array, int elementIndex)
        {
            unsafe
            {
                T* buffer = (T*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(array);

                m_Ptr = new UnsafeReference<T>(buffer + elementIndex);
            }
#if DEBUG_MODE
            m_Owner = ThreadInfo.CurrentThread;
#endif
        }
#endif

        public bool Equals(NativeReference<T> other) => m_Ptr.Equals(other.m_Ptr);

        public static NativeReference<T> operator +(NativeReference<T> a, int b) => a.m_Ptr + b;
        public static NativeReference<T> operator -(NativeReference<T> a, int b) => a.m_Ptr - b;

        public static implicit operator NativeReference<T>(UnsafeReference<T> t) => new NativeReference<T>(t);
        public static implicit operator NativeReference<T>(IntPtr t) => new NativeReference<T>(new UnsafeReference<T>(t));

        public static explicit operator UnsafeReference<T>(NativeReference<T> t) => t.m_Ptr;
    }
}
