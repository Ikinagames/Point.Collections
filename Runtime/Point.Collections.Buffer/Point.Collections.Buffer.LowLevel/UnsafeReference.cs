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
using Unity.Collections.LowLevel.Unsafe;
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System;
using System.Runtime.InteropServices;

namespace Point.Collections.Buffer.LowLevel
{
    //////////////////////////////////////////////////////////////////////////////////////////
    /*                                   Critical Section                                   */
    /*                                       수정금지                                        */
    /*                                                                                      */
    /*                          Unsafe pointer를 포함하는 코드입니다                          */
    //////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// <inheritdoc cref="IUnsafeReference"/>
    /// </summary>
#if UNITYENGINE && UNITY_COLLECTIONS
    [BurstCompatible]
#endif
    [StructLayout(LayoutKind.Sequential
#if POINT_COLLECTIONS_NATIVE
        , Pack = 1
#endif
        )]
    public struct UnsafeReference : IUnsafeReference, IEquatable<UnsafeReference>
    {
        //[MarshalAs(UnmanagedType.U1)]
        //private bool m_IsCreated;
#if UNITYENGINE
        [NativeDisableUnsafePtrRestriction]
#endif
        private unsafe void* m_Ptr;

        public IntPtr this[int offset]
        {
            get
            {
                IntPtr ptr;
                unsafe
                {
                    ptr = (IntPtr)m_Ptr;
                }
                return IntPtr.Add(ptr, offset);
            }
        }
        public IntPtr this[long offset]
        {
            get
            {
                IntPtr ptr;
                unsafe
                {
                    ptr = (IntPtr)m_Ptr;
                }

                return IntPtr.Add(ptr, (int)offset);
            }
        }

        /// <summary>
        /// 실제 메모리 주소입니다.
        /// </summary>
        public unsafe void* Ptr => m_Ptr;
        /// <summary>
        /// 실제 메모리 주소를 <seealso cref="System.IntPtr"/> 값으로 반환합니다.
        /// </summary>
        public IntPtr IntPtr { get { unsafe { return (IntPtr)m_Ptr; } } }

        /// <summary>
        /// 이 포인터가 사용자에 의헤 할당되었는지 반환합니다.
        /// </summary>
        public bool IsCreated => !IntPtr.Equals(IntPtr.Zero);

        public unsafe UnsafeReference(void* ptr)
        {
            m_Ptr = ptr;
            //m_IsCreated = true;
        }
        public UnsafeReference(IntPtr ptr)
        {
            unsafe
            {
                m_Ptr = ptr.ToPointer();
            }
            //m_IsCreated = true;
        }

        public bool Equals(UnsafeReference other)
        {
            unsafe
            {
                return m_Ptr == other.m_Ptr;
            }
        }

        public static UnsafeReference operator +(UnsafeReference a, int b)
        {
            unsafe
            {
                return new UnsafeReference(IntPtr.Add(a.IntPtr, b));
            }
        }
        public static UnsafeReference operator -(UnsafeReference a, int b)
        {
            unsafe
            {
                return new UnsafeReference(IntPtr.Subtract(a.IntPtr, b));
            }
        }

        public static unsafe implicit operator UnsafeReference(void* p)
        {
            if (p == null)
            {
                return default(UnsafeReference);
            }

            return new UnsafeReference(p);
        }
        public static implicit operator UnsafeReference(IntPtr p) => new UnsafeReference(p);
        public static unsafe implicit operator void*(UnsafeReference p) => p.m_Ptr;
        public static unsafe implicit operator IntPtr(UnsafeReference p) => (IntPtr)p.m_Ptr;

#if UNITY_COLLECTIONS
        [NotBurstCompatible]
#endif
        public override unsafe string ToString() => ((long)m_Ptr).ToString("X4");
    }
    /// <summary>
    /// <inheritdoc cref="IUnsafeReference"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
#if UNITYENGINE && UNITY_COLLECTIONS
    [BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
#endif
    [StructLayout(LayoutKind.Sequential
#if POINT_COLLECTIONS_NATIVE
        , Pack = 1
#endif
        )]
    public struct UnsafeReference<T> : IUnsafeReference,
        IEquatable<UnsafeReference<T>>, IEquatable<UnsafeReference>
        where T : unmanaged
    {
        //[MarshalAs(UnmanagedType.U1)]
        //private bool m_IsCreated;
#if UNITYENGINE
        [NativeDisableUnsafePtrRestriction]
#endif
        private readonly unsafe void* m_Ptr;

        IntPtr IUnsafeReference.this[int offset]
        {
            get
            {
                IntPtr ptr;
                unsafe
                {
                    ptr = (IntPtr)m_Ptr;
                }
                return IntPtr.Add(ptr, offset);
            }
        }
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
                unsafe
                {
                    return ref *(Ptr + index);
                }
            }
        }
        /// <inheritdoc cref="this[int]"/>
        public ref T this[uint index]
        {
            get
            {
                unsafe
                {
                    return ref *(Ptr + index);
                }
            }
        }

        /// <inheritdoc cref="UnsafeReference.Ptr"/>
        public unsafe T* Ptr
        {
            get
            {
                if (m_Ptr == null)
                {
                    return null;
                }

                return (T*)m_Ptr;
            }
        }
        /// <inheritdoc cref="UnsafeReference.IntPtr"/>
        public IntPtr IntPtr { get { unsafe { return (IntPtr)m_Ptr; } } }
        /// <inheritdoc cref="GetValue"/>
        public ref T Value => ref GetValue();

        /// <inheritdoc cref="UnsafeReference.IsCreated"/>
        public bool IsCreated => !IntPtr.Equals(IntPtr.Zero);

        public UnsafeReference(IntPtr ptr)
        {
            unsafe
            {
                m_Ptr = ptr.ToPointer();
            }
            //m_IsCreated = true;
        }
        public unsafe UnsafeReference(T* ptr)
        {
            m_Ptr = ptr;
            //m_IsCreated = true;
        }
        public ReadOnly AsReadOnly() { unsafe { return new ReadOnly(m_Ptr); } }

        /// <summary>
        /// 포인터의 값을 <typeparamref name="T"/> 로 읽어서 반환합니다.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public ref T GetValue()
        {
            unsafe
            {
                if (Ptr == null) throw new InvalidOperationException();

                return ref *Ptr;
            }
        }

        public bool Equals(UnsafeReference<T> other)
        {
            unsafe
            {
                return m_Ptr == other.m_Ptr;
            }
        }
        public bool Equals(UnsafeReference other)
        {
            unsafe
            {
                return Ptr == other.Ptr;
            }
        }

        /// <summary>
        /// 포인터를 오른쪽으로 <paramref name="b"/> 만큼 밀어서 반환합니다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static UnsafeReference<T> operator +(UnsafeReference<T> a, int b)
        {
            unsafe
            {
                return new UnsafeReference<T>(a.Ptr + b);
            }
        }
        /// <summary>
        /// 포인터를 왼쪽으로 <paramref name="b"/> 만큼 밀어서 반환합니다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static UnsafeReference<T> operator -(UnsafeReference<T> a, int b)
        {
            unsafe
            {
                return new UnsafeReference<T>(a.Ptr - b);
            }
        }
        /// <summary>
        /// 두 포인터 간의 거리를 반환합니다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long operator -(UnsafeReference<T> a, UnsafeReference<T> b)
        {
            unsafe
            {
                return a.Ptr - b.Ptr;
            }
        }
        /// <summary>
        /// 두 포인터 간의 거리를 반환합니다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long operator -(UnsafeReference<T> a, UnsafeReference b)
        {
            unsafe
            {
                return (byte*)a.Ptr - (byte*)b.Ptr;
            }
        }
        /// <summary>
        /// 두 포인터 간의 거리를 반환합니다.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static long operator -(UnsafeReference a, UnsafeReference<T> b)
        {
            unsafe
            {
                return (byte*)a.Ptr - (byte*)b.Ptr;
            }
        }

        public static unsafe implicit operator UnsafeReference<T>(T* p)
        {
            if (p == null) return default(UnsafeReference<T>);

            return new UnsafeReference<T>(p);
        }
        public static unsafe implicit operator UnsafeReference(UnsafeReference<T> p) => new UnsafeReference(p.IntPtr);
        public static unsafe implicit operator T*(UnsafeReference<T> p) => p.Ptr;
        public static unsafe explicit operator UnsafeReference<T>(UnsafeReference p) => new UnsafeReference<T>(p.IntPtr);

        public static unsafe implicit operator IntPtr(UnsafeReference<T> p) => (IntPtr)p.m_Ptr;
        public static unsafe implicit operator IntPtr<T>(UnsafeReference<T> p) => new IntPtr<T>((IntPtr)p.m_Ptr);

#if UNITY_COLLECTIONS
        [NotBurstCompatible]
#endif
        public override unsafe string ToString() => ((long)m_Ptr).ToString("X4");

#if UNITYENGINE && UNITY_COLLECTIONS
        [BurstCompatible]
#endif
        public struct ReadOnly
        {
            private unsafe void* m_Ptr;
            private unsafe T* Ptr => (T*)m_Ptr;

            public T this[int index]
            {
                get
                {
                    unsafe
                    {
                        return Ptr[index];
                    }
                }
            }
            public T Value { get { unsafe { return *Ptr; } } }

            internal unsafe ReadOnly(void* ptr)
            {
                m_Ptr = ptr;
            }
        }
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    /*                                End of Critical Section                               */
    //////////////////////////////////////////////////////////////////////////////////////////
}
