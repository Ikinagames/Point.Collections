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

#if UNITY_2020_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Point.Collections.Native;
using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Point.Collections.Buffer.LowLevel
{
    public unsafe readonly struct IntPtr<T> : IEmpty, IDisposable, IEquatable<IntPtr<T>>, IEquatable<IntPtr>
    {
        public static IntPtr<T> Zero => new IntPtr<T>(IntPtr.Zero);

        private readonly void* m_Ptr;
        private readonly bool m_IsUnmanaged, m_IsAllocated;

        #region Constructors

        public IntPtr(IntPtr t)
        {
            if (t.Equals(IntPtr.Zero))
            {
                m_Ptr = null;
                m_IsUnmanaged = false;
                m_IsAllocated = false;

                return;
            }

            m_Ptr = t.ToPointer();
            m_IsUnmanaged = true;
            m_IsAllocated = false;
        }
        /// <summary>
        /// <paramref name="obj"/> 를 메모리에 고정하여 포인터 주소로 변환합니다.
        /// </summary>
        /// <remarks>
        /// 만약 <typeparamref name="T"/> 가 Unmanaged 타입이 아니라면 <seealso cref="GCHandle.Alloc(object)"/> 을 통해 메모리에 고정합니다. Unmanaged 타입이라면 <seealso cref="NativeUtility.Malloc(in long, in int, Unity.Collections.Allocator)"/> 을 수행합니다. 사용이 끝난 후, 사용자는 수동으로 <seealso cref="Dispose"/> 를 호출하여 메모리를 해제하여야합니다.
        /// </remarks>
        /// <param name="obj"></param>
        public IntPtr(T obj)
        {
            if (obj == null)
            {
                this = Zero;
                return;
            }
            else if (!TypeHelper.IsUnmanaged(TypeHelper.TypeOf<T>.Type))
            {
                this = new IntPtr<T>(obj, GCHandleType.Pinned);
                return;
            }

            m_Ptr = NativeUtility.Malloc(Marshal.SizeOf<T>(), TypeHelper.AlignOf<T>(), Unity.Collections.Allocator.Persistent);

            m_IsUnmanaged = true;
            m_IsAllocated = true;
        }
        public IntPtr(T obj, GCHandleType handleType)
        {
            if (obj == null)
            {
                this = Zero;
                return;
            }

            GCHandle handle = GCHandle.Alloc(obj, handleType);
            m_Ptr = handle.AddrOfPinnedObject().ToPointer();

            m_IsUnmanaged = false;
            m_IsAllocated = true;
        }

        #endregion

        #region Base Methods

        public void* ToPointer() => m_Ptr;

        public bool IsEmpty()
        {
            if (m_Ptr == null) return true;

            return false;
        }
        public void Dispose()
        {
            if (m_IsAllocated)
            {
                if (m_IsUnmanaged) NativeUtility.Free(m_Ptr, Unity.Collections.Allocator.Persistent);
                else
                {
                    GCHandle.FromIntPtr((IntPtr)m_Ptr).Free();
                }
            }
        }

        public bool Equals(IntPtr<T> other) => m_Ptr == other.m_Ptr;
        public bool Equals(IntPtr other) => m_Ptr == other.ToPointer();
        public override bool Equals(object obj)
        {
            if (obj is IntPtr<T> t0) return Equals(t0);
            else if (obj is IntPtr t1) return Equals(t1);

            return false;
        }
        [SecuritySafeCritical]
        public override int GetHashCode() => (int)m_Ptr;

        #endregion

        public static explicit operator IntPtr<T>(IntPtr t) => new IntPtr<T>(t);
        public static implicit operator IntPtr(IntPtr<T> t) => (IntPtr)t.m_Ptr;

        public static bool operator ==(IntPtr<T> lptr, IntPtr<T> rptr) => lptr.Equals(rptr);
        public static bool operator !=(IntPtr<T> lptr, IntPtr<T> rptr) => !lptr.Equals(rptr);

        public static IntPtr<T> operator +(IntPtr<T> ptr, int offset)
        {
            return new IntPtr<T>(IntPtr.Add(ptr, offset));
        }
        public static IntPtr<T> operator -(IntPtr<T> ptr, int offset)
        {
            return new IntPtr<T>(IntPtr.Subtract(ptr, offset));
        }
    }
}
