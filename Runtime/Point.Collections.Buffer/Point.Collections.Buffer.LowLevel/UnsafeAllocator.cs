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
#if UNITY_2019 && !UNITY_2020_OR_NEWER
#define UNITYENGINE_OLD
using math = Point.Collections.Math;
#endif
#if UNITY_MATHEMATICS
using Unity.Mathematics;
#endif
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

#if !UNITY_COLLECTIONS
//using System.Runtime.InteropServices;
#endif
using Unity.Jobs;
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Point.Collections.Native;
using Point.Collections.Threading;
using System;
using System.Collections.Generic;
#if SYSTEM_BUFFER
using System.Buffers;
#endif

namespace Point.Collections.Buffer.LowLevel
{
    //////////////////////////////////////////////////////////////////////////////////////////
    /*                                   Critical Section                                   */
    /*                                       수정금지                                        */
    /*                                                                                      */
    /*                          Unsafe pointer를 포함하는 코드입니다                          */
    //////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// Unity native-side 에서 메모리 버퍼를 할당하는 Allocator 입니다.
    /// </summary>
#if UNITYENGINE && UNITY_COLLECTIONS
    [BurstCompatible]
#endif
    public struct UnsafeAllocator :
#if UNITYENGINE && UNITY_COLLECTIONS
        INativeDisposable, 
#endif
        IDisposable, IEquatable<UnsafeAllocator>
    {
#if UNITYENGINE && UNITY_COLLECTIONS
        [BurstCompatible]
#endif
        internal struct Buffer
        {
            internal UnsafeReference Ptr;
            /// <summary>
            /// <see cref="Ptr"/> 에 할당된 총 메모리 크기입니다.
            /// </summary>
            internal long Size;
        }

        internal UnsafeReference<Buffer> m_Buffer;
#if UNITYENGINE
        private JobHandle m_JobHandle;
        internal readonly Allocator m_Allocator;
#endif

#if UNITYENGINE && ENABLE_UNITY_COLLECTIONS_CHECKS
        internal AtomicSafetyHandle m_SafetyHandle;
#endif

        /// <summary>
        /// 이 메모리 버퍼의 메모리 주소입니다.
        /// </summary>
        public UnsafeReference Ptr => m_Buffer.Value.Ptr;
        /// <summary>
        /// 이 메모리 버퍼의 총 사이즈입니다.
        /// </summary>
        public long Size => m_Buffer.Value.Size;
        /// <summary>
        /// 이 메모리 버퍼가 생성되어 정상적으로 할당되었는지 반환합니다.
        /// </summary>
        public bool IsCreated => m_Buffer.IsCreated;

        public UnsafeAllocator(long size, int alignment
#if UNITYENGINE
            , Allocator allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory
#endif
            )
        {
            unsafe
            {
#if UNITYENGINE
                m_Buffer = (Buffer*)NativeUtility.Malloc(
                    TypeHelper.SizeOf<Buffer>(),
                    TypeHelper.AlignOf<Buffer>()
                    , allocator
                    );
#else
                // TODO : 임시
                m_Buffer = (Buffer*)Marshal.AllocHGlobal(TypeHelper.SizeOf<Buffer>());
#endif

                m_Buffer.Value = new Buffer
                {
#if UNITYENGINE
                    Ptr = NativeUtility.Malloc(size, alignment, allocator)
#else
                    // TODO : 임시
                    Ptr = Marshal.AllocHGlobal((int)size)
#endif
                    ,
                    Size = size
                };
                
#if UNITYENGINE
                if ((options & NativeArrayOptions.ClearMemory) == NativeArrayOptions.ClearMemory)
                {
                    UnsafeUtility.MemClear(m_Buffer.Value.Ptr, size);
                }
#endif
            }
#if UNITYENGINE
            m_JobHandle = default(JobHandle);
            m_Allocator = allocator;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            UnsafeBufferUtility.CreateSafety(m_Buffer, allocator, out m_SafetyHandle);
#endif
#endif
        }
        public UnsafeAllocator(UnsafeReference ptr, long size
#if UNITYENGINE
            , Allocator allocator
#endif
            )
        {
            unsafe
            {
#if UNITYENGINE
                m_Buffer = (Buffer*)NativeUtility.Malloc(
                    TypeHelper.SizeOf<Buffer>(),
                    TypeHelper.AlignOf<Buffer>()
                    , allocator
                    );
#else
                // TODO : 임시
                m_Buffer = (Buffer*)Marshal.AllocHGlobal(TypeHelper.SizeOf<Buffer>());
#endif

                m_Buffer.Value = new Buffer
                {
                    Ptr = ptr,
                    Size = size
                };
            }
#if UNITYENGINE
            m_JobHandle = default(JobHandle);
            m_Allocator = allocator;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            UnsafeBufferUtility.CreateSafety(m_Buffer, allocator, out m_SafetyHandle);
#endif
#endif
        }
        public ReadOnly AsReadOnly() => new ReadOnly(this);

        /// <summary>
        /// 이 메모리 버퍼의 메모리를 전부 초기화합니다.
        /// </summary>
        public void Clear()
        {
#if UNITY_COLLECTIONS
            CompleteAllJobs();
#endif
            unsafe
            {
                NativeUtility.MemClear(m_Buffer.Value.Ptr, m_Buffer.Value.Size);
            }
        }

        /// <summary>
        /// <paramref name="item"/> 을 이 메모리에 작성합니다.
        /// </summary>
        /// <remarks>
        /// 만약 이 메모리가 버퍼라면, 0 번째 인덱스에 작성합니다.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        public void Write<T>(T item) where T : unmanaged
        {
#if UNITY_COLLECTIONS
            CompleteAllJobs();
#endif
            unsafe
            {
                byte* bytes = UnsafeBufferUtility.AsBytes(ref item, out int length);
                NativeUtility.MemCpy(m_Buffer.Value.Ptr, bytes, length);
            }
        }
        public unsafe void Write(byte* bytes, int length)
        {
#if UNITY_COLLECTIONS
            CompleteAllJobs();
#endif
            NativeUtility.MemCpy(m_Buffer.Value.Ptr, bytes, length);
        }

        public void Dispose()
        {
#if UNITYENGINE && ENABLE_UNITY_COLLECTIONS_CHECKS
#if UNITY_COLLECTIONS
            CompleteAllJobs();
#endif
            if (!UnsafeUtility.IsValidAllocator(m_Allocator))
            {
                PointHelper.LogError(Channel.Collections,
                    $"{nameof(UnsafeAllocator)} that doesn\'t have valid allocator mark cannot be disposed. " +
                    $"Most likely this {nameof(UnsafeAllocator)} has been wrapped from NativeArray.");
                return;
            }
#endif
#if UNITYENGINE
            unsafe
            {
                NativeUtility.Free(m_Buffer.Value.Ptr, m_Allocator);
                NativeUtility.Free(m_Buffer, m_Allocator);
                
            }
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            UnsafeBufferUtility.RemoveSafety(m_Buffer, ref m_SafetyHandle);
#endif
#else
            Marshal.FreeHGlobal(m_Buffer.Value.Ptr.IntPtr);
            Marshal.FreeHGlobal(m_Buffer.IntPtr);
#endif
            m_Buffer = default(UnsafeReference<Buffer>);
        }
#if UNITYENGINE && UNITY_COLLECTIONS
        public JobHandle Dispose(JobHandle inputDeps)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            if (!UnsafeUtility.IsValidAllocator(m_Allocator))
            {
                PointHelper.LogError(Channel.Collections,
                    $"{nameof(UnsafeAllocator)} that doesn\'t have valid allocator mark cannot be disposed. " +
                    $"Most likely this {nameof(UnsafeAllocator)} has been wrapped from NativeArray.");
                return default(JobHandle);
            }
#endif
            DisposeJob disposeJob = new DisposeJob()
            {
                Buffer = m_Buffer,
                Allocator = m_Allocator
            };
            JobHandle result = disposeJob.Schedule(inputDeps);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            UnsafeBufferUtility.RemoveSafety(m_Buffer, ref m_SafetyHandle);
#endif
            m_Buffer = default(UnsafeReference<Buffer>);
            return result;
        }

        public void CompleteAllJobs()
        {
            m_JobHandle.Complete();
        }
        public JobHandle CombineDependencies(JobHandle inputDeps)
        {
            m_JobHandle = JobHandle.CombineDependencies(m_JobHandle, inputDeps);
            return m_JobHandle;
        }
#endif

        public bool Equals(UnsafeAllocator other) => m_Buffer.Equals(other.m_Buffer);

#if UNITYENGINE && UNITY_COLLECTIONS
        [BurstCompatible]
#endif
        public readonly struct ReadOnly
        {
            private readonly UnsafeReference m_Ptr;
            private readonly long m_Size;

            public UnsafeReference Ptr => m_Ptr;
            public long Size => m_Size;

            internal ReadOnly(UnsafeAllocator allocator)
            {
                m_Ptr = allocator.m_Buffer.Value.Ptr;
                m_Size = allocator.m_Buffer.Value.Size;
            }
        }
#if UNITYENGINE && UNITY_COLLECTIONS
        [BurstCompatible]
        private struct DisposeJob : IJob
        {
            public UnsafeReference<Buffer> Buffer;
            public Allocator Allocator;

            public void Execute()
            {
                unsafe
                {
                    UnsafeUtility.Free(Buffer.Value.Ptr, Allocator);
                    UnsafeUtility.Free(Buffer, Allocator);
                }
            }
        }
#endif
    }
    /// <summary><inheritdoc cref="UnsafeAllocator"/></summary>
    /// <typeparam name="T"></typeparam>
#if UNITYENGINE && UNITY_COLLECTIONS
    [BurstCompatible]
#endif
    public struct UnsafeAllocator<T> :
#if UNITYENGINE && UNITY_COLLECTIONS
        INativeDisposable, 
#endif
        IDisposable, IEquatable<UnsafeAllocator<T>>
        where T : unmanaged
    {
        internal UnsafeAllocator m_Buffer;

        /// <inheritdoc cref="UnsafeAllocator.Ptr"/>
        public UnsafeReference<T> Ptr => (UnsafeReference<T>)m_Buffer.Ptr;
        /// <inheritdoc cref="UnsafeAllocator.IsCreated"/>
        public bool IsCreated => m_Buffer.IsCreated;

        public ref T this[int index]
        {
            get
            {
#if DEBUG_MODE
                if (index < 0 || index >= Length)
                {
                    throw new IndexOutOfRangeException();
                }
#endif
                return ref Ptr[index];
            }
        }
        
        /// <inheritdoc cref="UnsafeAllocator.Size"/>
        public long Size => m_Buffer.Size;
        /// <summary>
        /// 이 메모리 버퍼의 사이즈를 <typeparamref name="T"/> 의 크기에 맞춘 최대 길이입니다.
        /// </summary>
        public int Length => Convert.ToInt32(m_Buffer.Size / TypeHelper.SizeOf<T>());

        /// <summary>
        /// <paramref name="length"/> 만큼 새로운 <typeparamref name="T"/> 의 버퍼를 할당합니다.
        /// </summary>
        /// <param name="length"></param>
        /// <param name="allocator"></param>
        /// <param name="options"></param>
        public UnsafeAllocator(int length
#if UNITYENGINE
            , Allocator allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory
#endif
            )
        {
            m_Buffer = new UnsafeAllocator(
                UnsafeUtility.SizeOf<T>() * length,
                UnsafeUtility.AlignOf<T>()
#if UNITYENGINE
                , allocator,
                options
#endif
                );
        }
        public unsafe UnsafeAllocator(T[] array, Allocator allocator) : this(array.Length, allocator, NativeArrayOptions.UninitializedMemory)
        {
            fixed (T* p = array)
            {
                UnsafeUtility.MemCpy(m_Buffer.Ptr, p, m_Buffer.Size);
            }
        }
        /// <summary>
        /// 이미 메모리가 할당된 포인터 <paramref name="ptr"/> 으로 wrapping 하여 반환합니다.
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="length"></param>
        /// <param name="allocator"></param>
        public UnsafeAllocator(UnsafeReference<T> ptr, int length
#if UNITYENGINE
            , Allocator allocator
#endif
            )
        {
            m_Buffer = new UnsafeAllocator(ptr, TypeHelper.SizeOf<T>() * length
#if UNITYENGINE
                , allocator
#endif
                );
        }
        /// <summary>
        /// 이 버퍼를 읽기 전용으로 반환합니다.
        /// </summary>
        /// <returns></returns>
        public ReadOnly AsReadOnly() => new ReadOnly(this);
        public ParallelWriter AsParallelWriter() => new ParallelWriter(this);

        /// <inheritdoc cref="UnsafeAllocator.Clear"/>
        public void Clear() => m_Buffer.Clear();

        /// <summary>
        /// 이 메모리 버퍼의 <paramref name="index"/> 번째 주소를 반환합니다.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        /// <exception cref="IndexOutOfRangeException"></exception>
        public UnsafeReference<T> ElementAt(in int index)
        {
#if DEBUG_MODE
            if (index < 0 || index >= Length)
            {
                throw new IndexOutOfRangeException();
            }
#endif
            return Ptr + index;
        }
        public UnsafeReference<T> ElementAtWithStride(in int index, in int stride)
        {
#if DEBUG_MODE
            if (index < 0 || index >= Length)
            {
                throw new IndexOutOfRangeException();
            }
#endif
            return Ptr + index + stride;
        }

        public void Dispose()
        {
            m_Buffer.Dispose();
        }
#if UNITYENGINE && UNITY_COLLECTIONS
        public JobHandle Dispose(JobHandle inputDeps)
        {
            JobHandle result = m_Buffer.Dispose(inputDeps);

            m_Buffer = default(UnsafeAllocator);
            return result;
        }
#endif

        public bool Equals(UnsafeAllocator<T> other) => m_Buffer.Equals(other.m_Buffer);

#if UNITYENGINE && UNITY_COLLECTIONS
        [BurstCompatible]
#endif
        public readonly struct ReadOnly
        {
            private readonly UnsafeReference<T>.ReadOnly m_Ptr;
            private readonly int m_Length;

            public int Length => m_Length;

            public T this[int index]
            {
                get
                {
#if DEBUG_MODE
                    if (index < 0 || index >= m_Length)
                    {
                        throw new IndexOutOfRangeException();
                    }
#endif
                    return m_Ptr[index];
                }
            }

            internal ReadOnly(UnsafeAllocator<T> allocator)
            {
                m_Ptr = allocator.Ptr.AsReadOnly();
                m_Length = allocator.Length;
            }
        }
#if UNITYENGINE && UNITY_COLLECTIONS
        [BurstCompatible]
#endif
        public struct ParallelWriter
        {
            private readonly UnsafeReference<T> m_Ptr;
            private readonly int m_Length;

            private AtomicOperator m_Op;

            public int Length => m_Length;

            public T this[int index]
            {
                set => SetValue(in index, in value);
            }

            internal ParallelWriter(UnsafeAllocator<T> allocator)
            {
                m_Ptr = allocator.Ptr;
                m_Length = allocator.Length;

                m_Op = new AtomicOperator();
            }

            public void SetValue(in int index, in T value)
            {
                m_Op.Enter(index);
                UnsafeReference<T> p = m_Ptr + index;
                p.Value = value;
                m_Op.Exit(index);
            }
        }

        public static implicit operator UnsafeAllocator(UnsafeAllocator<T> t) => t.m_Buffer;
        public static explicit operator UnsafeAllocator<T>(UnsafeAllocator t)
        {
            return new UnsafeAllocator<T>
            {
                m_Buffer = t
            };
        }
    }
    public static class UnsafeAllocatorExtensions
    {
        /// <summary>
        /// 이 메모리 버퍼를 <paramref name="size"/> 만큼 다시 재 할당합니다.
        /// </summary>
        /// <remarks>
        /// 이전에 작성된 데이터는 자동으로 복사되어 초기화됩니다. 만약 사이즈가 이전보다 증가하였으면 
        /// 증가한 메모리 사이즈의 초기화를 <paramref name="options"/> 에서 결정할 수 있습니다.
        /// </remarks>
        /// <param name="t"></param>
        /// <param name="size"></param>
        /// <param name="alignment"></param>
        /// <param name="options"></param>
        /// <exception cref="Exception"></exception>
        public static void Resize(this ref UnsafeAllocator t, long size, int alignment)
        {
            if (size < 0) throw new Exception();

            UnsafeAllocator newBuffer = new UnsafeAllocator(size, alignment, t.m_Allocator);
            t.CopyTo(newBuffer);
            t.Dispose();

            t = newBuffer;
        }
#if UNITYENGINE
        public static void Resize(this ref UnsafeAllocator t, long size, int alignment, NativeArrayOptions options)
        {
            if (size < 0) throw new Exception();

            UnsafeAllocator newBuffer = new UnsafeAllocator(size, alignment, t.m_Allocator, options);
            t.CopyTo(newBuffer);
            t.Dispose();

            t = newBuffer;

            //UnityEngine.Debug.Log($"re allocate from {t.m_Buffer.Value.Size} -> {size}");
            //unsafe
            //{
            //    void* ptr = UnsafeUtility.Malloc(size, alignment, t.m_Allocator);

            //    UnsafeUtility.MemCpy(ptr, t.Ptr, math.min(size, t.Size));
            //    UnsafeUtility.Free(t.Ptr, t.m_Allocator);

            //    t.m_Buffer.Value.Ptr = ptr;

            //    if (size > t.Size &&
            //        (options & NativeArrayOptions.ClearMemory) == NativeArrayOptions.ClearMemory)
            //    {
            //        UnsafeUtility.MemClear(t.Ptr[t.Size].ToPointer(), size - t.Size);
            //    }

            //    t.m_Buffer.Value.Size = size;
            //}
        }
#endif
        /// <summary>
        /// 이 메모리 버퍼를 <paramref name="length"/> 길이 만큼 다시 재 할당합니다.
        /// </summary>
        /// <remarks>
        /// 이전에 작성된 데이터는 자동으로 복사되어 초기화됩니다. 만약 사이즈가 이전보다 증가하였으면 
        /// 증가한 메모리 사이즈의 초기화를 <paramref name="options"/> 에서 결정할 수 있습니다.
        /// </remarks>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <param name="length"></param>
        /// <param name="options"></param>
        /// <exception cref="Exception"></exception>
        public static unsafe void Resize<T>(this ref UnsafeAllocator<T> t, int length, 
            NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
            where T : unmanaged
        {
            if (length < 0) throw new Exception();

            int targetSize = UnsafeUtility.SizeOf<T>() * length;
            UnityEngine.Debug.Log($"re allocate from {t.m_Buffer.m_Buffer.Value.Size} -> {targetSize}");

            void* ptr = UnsafeUtility.Malloc(
                targetSize,
                UnsafeUtility.AlignOf<T>(),
                t.m_Buffer.m_Allocator);

            UnsafeUtility.MemCpy(ptr, t.Ptr, math.min(targetSize, t.Size));
            UnsafeUtility.Free(t.Ptr, t.m_Buffer.m_Allocator);

            t.m_Buffer.m_Buffer.Value.Ptr = ptr;

            if (targetSize > t.Size &&
                (options & NativeArrayOptions.ClearMemory) == NativeArrayOptions.ClearMemory)
            {
                UnsafeUtility.MemClear(t.ElementAt(t.Length).IntPtr.ToPointer(), targetSize - t.Size);
            }

            t.m_Buffer.m_Buffer.Value.Size = targetSize;

            //UnsafeAllocator<T> newBuffer = new UnsafeAllocator<T>(length, t.m_Buffer.m_Allocator);
            //t.CopyTo(newBuffer);

            //var old = t;
            //t = newBuffer;

            //old.Dispose();
        }
//#if UNITYENGINE
//        public static void Resize<T>(this ref UnsafeAllocator<T> t, int length
//            , NativeArrayOptions options
//            )
//            where T : unmanaged
//        {
//            if (length < 0) throw new Exception();

//            t.m_Buffer.Resize(
//                TypeHelper.SizeOf<T>() * length,
//                TypeHelper.AlignOf<T>()
//                , options
//                );
//        }
//#endif

        public static unsafe bool Contains(this in UnsafeAllocator t, UnsafeReference ptr)
        {
            UnsafeReference start = t.Ptr;
            UnsafeReference end = ((byte*)t.Ptr) + t.Size;

            if (start.Ptr <= ptr.Ptr && ptr.Ptr < end.Ptr) return true;
            return false;
        }
        public static unsafe bool Contains<T>(this in UnsafeAllocator<T> t, UnsafeReference ptr)
            where T : unmanaged
        {
            UnsafeReference start = t.Ptr;
            UnsafeReference end = ((byte*)t.Ptr.Ptr) + t.Size;

            if (start.Ptr <= ptr.Ptr && ptr.Ptr < end.Ptr) return true;
            return false;
        }

        public static unsafe int IndexOf<T>(this in UnsafeAllocator<T> t, UnsafeReference ptr)
            where T : unmanaged
        {
            if (!t.Contains(ptr)) return -1;

            UnsafeReference<T> first = t.Ptr;
            for (int i = 0; i < t.Length; i++)
            {
                UnsafeReference target = first + i;
                if (target.Equals(ptr)) return i;
            }
            return -1;
        }

        public static void Sort<T, U>(this ref UnsafeAllocator<T> t, U comparer)
            where T : unmanaged
            where U : unmanaged, IComparer<T>
        {
            UnsafeBufferUtility.Sort<T, U>(t.Ptr, t.Length, comparer);
        }
        public static void Sort<T, U>(this ref UnsafeAllocator<T> t, U comparer, int length)
            where T : unmanaged
            where U : unmanaged, IComparer<T>
        {
            UnsafeBufferUtility.Sort<T, U>(t.Ptr, length, comparer);
        }

        public static bool Contains<T, U>(this in UnsafeAllocator<T> t, U item)
            where T : unmanaged, IEquatable<U>
            where U : unmanaged
        {
            int length = t.Length;
            bool result = false;

            for (int i = 0; i < length && !result; i++)
            {
                result |= t[i].Equals(item);
            }
            return result;
        }
        public static int IndexOf<T, U>(this in UnsafeAllocator<T> t, U item)
            where T : unmanaged, IEquatable<U>
            where U : unmanaged
        {
            return UnsafeBufferUtility.IndexOf(t.Ptr, t.Length, item);
        }
        public static bool RemoveForSwapBack<T, U>(this in UnsafeAllocator<T> t, U element)
            where T : unmanaged, IEquatable<U>
            where U : unmanaged
        {
            return UnsafeBufferUtility.RemoveForSwapBack(t.Ptr, t.Length, element);
        }
        public static bool RemoveAtSwapBack<T>(this in UnsafeAllocator<T> t, int index)
            where T : unmanaged
        {
            return UnsafeBufferUtility.RemoveAtSwapBack(t.Ptr, t.Length, index);
        }

#if UNITYENGINE
        /// <summary>
        /// <paramref name="other"/ 의 데이터들을 모아 새로운 <seealso cref="NativeArray{T}"/> 의 메모리 버퍼를 생성하여 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="other"></param>
        /// <param name="allocator"></param>
        /// <returns></returns>
        public static NativeArray<T> ToNativeArray<T>(this in UnsafeAllocator<T> other, Allocator allocator) where T : unmanaged
        {
            var arr = new NativeArray<T>(other.Length, allocator, NativeArrayOptions.UninitializedMemory);
            unsafe
            {
                T* buffer = (T*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(arr);
                UnsafeUtility.MemCpy(buffer, other.Ptr, other.Size);
            }

            return arr;
        }
        /// <summary>
        /// 임시 접근 어레이로 변환하여 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
        public static unsafe NativeArray<T> ConvertToNativeArray<T>(this in UnsafeAllocator<T> t)
            where T : unmanaged
        {
            NativeArray<T> arr = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(
                    t.Ptr, t.Length, t.m_Buffer.m_Allocator);

#if UNITY_EDITOR || ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Create(out var safety, out var sentinel, 1, Allocator.Temp);
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref arr, safety);
#endif

            return arr;
        }

        public static unsafe void CopyTo(this in UnsafeAllocator t, UnsafeAllocator target)
        {
            //UnsafeUtility.MemCpy(target.Ptr, t.Ptr, math.min(t.Size, target.Size));
            UnsafeUtility.MemCpy(target.Ptr, t.Ptr, t.Size);
        }
        public static unsafe void CopyTo<T>(this in UnsafeAllocator<T> t, UnsafeAllocator<T> target)
            where T : unmanaged
        {
            //UnsafeUtility.MemCpy(target.Ptr, t.Ptr, math.min(t.Size, target.Size));
            UnsafeUtility.MemCpy(target.Ptr, t.Ptr, t.Size);
        }

        public static void CopyToBuffer<T>(this in UnsafeAllocator<T> t, UnityEngine.GraphicsBuffer buffer)
            where T : unmanaged
        {
            buffer.SetData(t.ConvertToNativeArray());
        }
        public static void CopyToBuffer<T>(this in UnsafeAllocator<T> t, UnityEngine.ComputeBuffer buffer)
            where T : unmanaged
        {
            buffer.SetData(t.ConvertToNativeArray());
        }
#if UNITY_COLLECTIONS
        [NotBurstCompatible]
#endif
        public static unsafe void ReadFromBuffer<T>(this in UnsafeAllocator<T> t, UnityEngine.GraphicsBuffer buffer)
            where T : unmanaged
        {
            T[] arr
#if SYSTEM_BUFFER
                = ArrayPool<T>.Shared.Rent(t.Length);
#else
                = new T[t.Length];
#endif
            buffer.GetData(arr);

            fixed (T* ptr = arr)
            {
                UnsafeUtility.MemCpy(t.Ptr, ptr, t.Size);
            }

#if SYSTEM_BUFFER
            ArrayPool<T>.Shared.Return(arr);
#endif
        }
#if UNITY_COLLECTIONS
        [NotBurstCompatible]
#endif
        public static unsafe void ReadFromBuffer<T>(this in UnsafeAllocator<T> t, UnityEngine.ComputeBuffer buffer)
            where T : unmanaged
        {
            T[] arr
#if SYSTEM_BUFFER
                = ArrayPool<T>.Shared.Rent(t.Length);
#else
                = new T[t.Length];
#endif
            buffer.GetData(arr);

            fixed (T* ptr = arr)
            {
                UnsafeUtility.MemCpy(t.Ptr, ptr, t.Size);
            }

#if SYSTEM_BUFFER
            ArrayPool<T>.Shared.Return(arr);
#endif
        }

        public static UnsafeAllocator<T> ToUnsafeAllocator<T>(this in NativeArray<T> other, Allocator allocator) where T : unmanaged
        {
            unsafe
            {
                return new UnsafeAllocator<T>(
                    (T*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(other),
                    other.Length,
                    allocator
                    );
            }
        }
#endif

#if SYSTEM_BUFFER
        public struct SharedArray<T> : IDisposable
            where T : unmanaged
        {
            private T[] m_Array;
            public T[] Array => m_Array;

            public SharedArray(T[] arr)
            {
                m_Array = arr;
            }
            public void Dispose()
            {
                ArrayPool<T>.Shared.Return(m_Array);
                m_Array = null;
            }

            public static implicit operator T[](SharedArray<T> t) => t.Array;
        }
        public static SharedArray<T> ToSharedArray<T>(this in UnsafeAllocator<T> t)
            where T : unmanaged
        {
            SharedArray<T> arr = new SharedArray<T>(ArrayPool<T>.Shared.Rent(t.Length));
            return arr;
        }
#endif

        //////////////////////////////////////////////////////////////////////////////////////////
        /*                                End of Critical Section                               */
        //////////////////////////////////////////////////////////////////////////////////////////
    }
}
