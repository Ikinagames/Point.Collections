﻿// Copyright 2021 Ikina Games
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
#if UNITY_BURST
using Unity.Burst;
using Point.Collections.Burst;
#endif
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Point.Collections.Native;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Linq;
using UnityEngine;
using Unity.Jobs;

namespace Point.Collections.Buffer.LowLevel
{
#if UNITYENGINE && UNITY_BURST
    [BurstCompile(CompileSynchronously = true, DisableSafetyChecks = true)]
#endif
    public static unsafe class UnsafeBufferUtility
    {
#if UNITYENGINE && UNITY_BURST
        [BurstCompile]
#endif
        public static byte* AsBytes<T>(ref T t, out int length)
            where T : unmanaged
        {
            length = TypeHelper.SizeOf<T>();
            void* p = NativeUtility.AddressOf(ref t);

            return (byte*)p;
        }
        public static byte[] ToBytes(in UnsafeReference ptr, in int length)
        {
            if (!ptr.IsCreated || ptr.Ptr == null) return Array.Empty<byte>();

            byte[] arr = new byte[length];
            Marshal.Copy(ptr.IntPtr, arr, 0, length);

            return arr;
        }

        public static byte[] ObjectToByteArray(this object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }
        public static object ByteArrayToObject(byte[] arrBytes)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        /// <summary>
        /// <see cref="FNV1a64"/> 알고리즘으로 바이너리 해시 연산을 하여 반환합니다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="t"></param>
        /// <returns></returns>
#if UNITYENGINE && UNITY_BURST
        [BurstCompile]
#endif
        public static Hash Calculate<T>(this ref T t) where T : unmanaged
        {
            byte* bytes = AsBytes(ref t, out int length);
            uint output;
#if UNITYENGINE && UNITYBURST
            BurstFNV1a.fnv1a32_byte(bytes, &length, &output);
#elif POINT_COLLECTIONS_NATIVE
            NativeFNV1a.fnv1a32_byte(bytes, &length, &output);
#else
            output = FNV1a32.Calculate(bytes, length);
#endif
            Hash hash = new Hash(output);

            return hash;
        }

#if UNITYENGINE && UNITY_BURST
        [BurstCompile]
#endif
        public unsafe static int BinarySearch<T, U>(T* ptr, int length, T value, U comp) 
            where T : unmanaged 
            where U : IComparer<T>
        {
            int num = 0;
            for (int num2 = length; num2 != 0; num2 >>= 1)
            {
                int num3 = num + (num2 >> 1);
                T y = ptr[num3];
                int num4 = comp.Compare(value, y);
                if (num4 == 0)
                {
                    return num3;
                }

                if (num4 > 0)
                {
                    num = num3 + 1;
                    num2--;
                }
            }

            return ~num;
        }
#if UNITYENGINE && UNITY_BURST
        [BurstCompile]
#endif
        public static bool BinaryComparer<T, U>(ref T x, ref U y)
            where T : unmanaged
            where U : unmanaged
        {
            byte*
                a = AsBytes(ref x, out int length),
                b = (byte*)NativeUtility.AddressOf(ref y);
#if UNITYENGINE
            int index = 0;
            while (index < length && a[index].Equals(b[index]))
            {
                index++;
            }

            if (index != length) return false;
            return true;
#else
            bool result;
            NativeMath.binaryComparer(a, b, in length, &result);
            return result;
#endif
        }

#if UNITYENGINE && UNITY_BURST
        [BurstCompile]
#endif
        public static void Sort<T, U>(in UnsafeReference<T> buffer, in int length, U comparer)
            where T : unmanaged
            where U : unmanaged, IComparer<T>
        {
            for (int i = 0; i + 1 < length; i++)
            {
                int compare = comparer.Compare(buffer[i], buffer[i + 1]);
                if (compare > 0)
                {
                    Swap(buffer, i, i + 1);
                    Sort(buffer, in i, comparer);
                }
            }
        }

#if UNITYENGINE && UNITY_BURST
        [BurstCompile]
#endif
        public static void Swap<T>(in UnsafeReference<T> buffer, in int from, in int to)
            where T : unmanaged
        {
            T temp = buffer[from];
            buffer[from] = buffer[to];
            buffer[to] = temp;
        }

        public static bool Contains<T, U>(in UnsafeReference<T> buffer, in int length, U item)
            where T : unmanaged, IEquatable<U>
            where U : unmanaged
        {
            int index = IndexOf(buffer, length, item);

            return index >= 0;
        }

#if UNITYENGINE && UNITY_BURST
        [BurstCompile]
#endif
        public static int IndexOf<T, U>(in UnsafeReference<T> array, in int length, U item)
            where T : unmanaged, IEquatable<U>
            where U : unmanaged
        {
            for (int i = 0; i < length; i++)
            {
                if (array[i].Equals(item)) return i;
            }
            return -1;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int IndexOf<T>(this T[] array, T element)
            where T : IEquatable<T>
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i].Equals(element)) return i;
            }
            return -1;
        }

#if UNITYENGINE && UNITY_BURST
        [BurstCompile]
#endif
        public static bool RemoveForSwapBack<T, U>(UnsafeReference<T> array, int length, U element)
            where T : unmanaged, IEquatable<U>
            where U : unmanaged
        {
            int index = IndexOf(array, length, element);
            if (index < 0) return false;

            for (int i = index + 1; i < length; i++)
            {
                array[i - 1] = array[i];
            }

            return true;
        }
#if UNITYENGINE && UNITY_BURST
        [BurstCompile]
#endif
        public static bool RemoveAtSwapBack<T>(UnsafeReference<T> array, int length, int index)
           where T : unmanaged
        {
            if (index < 0) return false;

            for (int i = index + 1; i < length; i++)
            {
                array[i - 1] = array[i];
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveForSwapBack<T>(this T[] array, T element)
            where T : IEquatable<T>
        {
            int index = array.IndexOf(element);
            if (index < 0) return false;

            for (int i = index + 1; i < array.Length; i++)
            {
                array[i - 1] = array[i];
            }

            return true;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool RemoveAtSwapBack<T>(this T[] array, int index)
        {
            if (index < 0) return false;

            for (int i = index + 1; i < array.Length; i++)
            {
                array[i - 1] = array[i];
            }

            return true;
        }

        #region Memory

#if UNITYENGINE && UNITY_BURST
        [BurstCompile]
#endif
        public static long CalculateFreeSpaceBetween(in UnsafeReference from, in int length, in UnsafeReference to)
        {
            UnsafeReference<byte> p = (UnsafeReference<byte>)from;

            return to - (p + length);
        }

        public static T DeepClone<T>(this T obj)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream, obj);
                stream.Position = 0;

                return (T)formatter.Deserialize(stream);
            }
        }

        #endregion

        #region Native Array

#if UNITYENGINE

        public static ref T ElementAtAsRef<T>(this NativeArray<T> t, in int index)
            where T : unmanaged
        {
            unsafe
            {
                T* buffer = (T*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(t);
                //return ref UnsafeUtility.ArrayElementAsRef<T>(buffer, index);

                return ref buffer[index];
            }
        }

#endif

        #endregion

        #region Safety Checks

#if UNITYENGINE && ENABLE_UNITY_COLLECTIONS_CHECKS
        private static readonly Dictionary<IntPtr, (DisposeSentinel, Allocator)> m_Safety
            = new Dictionary<IntPtr, (DisposeSentinel, Allocator)>();

        public static void CreateSafety(UnsafeReference ptr, Allocator allocator, out AtomicSafetyHandle handle)
        {
            DisposeSentinel.Create(out handle, out var sentinel, 1, allocator);

            IntPtr p = ptr.IntPtr;
            m_Safety.Add(p, (sentinel, allocator));
        }
        public static void RemoveSafety(UnsafeReference ptr, ref AtomicSafetyHandle handle)
        {
            IntPtr p = ptr.IntPtr;
            var sentinel = m_Safety[p];

            DisposeSentinel.Dispose(ref handle, ref sentinel.Item1);
            m_Safety.Remove(p);
        }

        public static void DisposeAllSafeties()
        {
            foreach (var item in m_Safety)
            {
                UnsafeUtility.Free(item.Key.ToPointer(), item.Value.Item2);
            }
            m_Safety.Clear();
        }
#endif

        #endregion

#if UNITYENGINE

        private static readonly Dictionary<Type, ExportFieldInfo> s_ParsedExportFields = new Dictionary<Type, ExportFieldInfo>();
        private sealed class ExportFieldInfo
        {
            public FieldInfo[] fieldInfos;
            public int size;
            public int alignment;
        }
        private struct ExportDataComparer : IComparer<int>
        {
            public int Compare(int x, int y)
            {
                if (x == y) return 0;
                else if (x < y) return -1;
                else return 1;
            }
        }
        private static ExportFieldInfo GetExportFieldInfo(Type t)
        {
            if (!s_ParsedExportFields.TryGetValue(t, out var fields))
            {
                var temp = t
                    .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy)
                    .Where(fieldInfo =>
                    {
                        if (fieldInfo.GetCustomAttribute<ExportDataAttribute>() == null) return false;
                        else if (!fieldInfo.IsPublic)
                        {
                            return fieldInfo.GetCustomAttribute<SerializeField>() != null;
                        }
                        return fieldInfo.GetCustomAttribute<NonSerializedAttribute>() == null;
                    })
                    .OrderBy(fieldInfo => TypeHelper.SizeOf(fieldInfo.FieldType), new ExportDataComparer())
                    .ToArray();
                int size = 0, alignment = 0;
                foreach (var item in temp)
                {
                    int itemSize = TypeHelper.SizeOf(item.FieldType);
                    size += itemSize;

                    alignment = Mathf.Max(alignment, itemSize);
                    alignment = alignment > 4 ? 4 : alignment;
                }
                fields = new ExportFieldInfo
                {
                    fieldInfos = temp,
                    size = size,
                    alignment = alignment
                };

                s_ParsedExportFields.Add(t, fields);
            }
            return fields;
        }
        public static unsafe UnsafeExportedData ExportData<T>(in T t, Allocator allocator)
             where T : unmanaged
        {
            ExportFieldInfo fields = GetExportFieldInfo(TypeHelper.TypeOf<T>.Type);

            UnsafeAllocator alloc = new UnsafeAllocator(fields.size, fields.alignment, allocator);
            UnsafeReference p = alloc.Ptr;

            foreach (FieldInfo field in fields.fieldInfos)
            {
                object obj = field.GetValue(t);
                byte[] bytes = obj.ObjectToByteArray();

                fixed (byte* ptr = bytes)
                {
                    UnsafeUtility.MemCpy(p, ptr, bytes.Length);
                    p += bytes.Length;
                }
            }

            return new UnsafeExportedData(TypeHelper.TypeOf<T>.Type.ToTypeInfo(), alloc);
        }
        public static unsafe T ReadData<T>(this in UnsafeExportedData t, int index)
            where T : unmanaged
        {
            ExportFieldInfo fields = GetExportFieldInfo(t.Type);

            UnsafeAllocator alloc = t.Allocator;

            //FieldInfo field = fields.fieldInfos[index];
            UnsafeReference p = alloc.Ptr;
            for (int i = 0; i < index; i++)
            {
                p += TypeHelper.SizeOf(fields.fieldInfos[i].FieldType);
            }

            T data = Marshal.PtrToStructure<T>(p);
            return data;
        }
#endif
    }
#if UNITYENGINE && UNITY_COLLECTIONS
    [BurstCompatible]
#endif
    public struct UnsafeExportedData : IValidation, IDisposable
#if UNITY_COLLECTIONS
        , INativeDisposable
#endif
    {
        private readonly TypeInfo m_Type;
        private UnsafeAllocator m_Allocator;

        public TypeInfo Type => m_Type;
        public UnsafeAllocator Allocator => m_Allocator;

        internal UnsafeExportedData(TypeInfo type, UnsafeAllocator allocator)
        {
            m_Type = type;
            m_Allocator = allocator;
        }

        public bool IsValid() => m_Allocator.IsCreated;
        public void Dispose()
        {
            m_Allocator.Dispose();
        }
#if UNITY_COLLECTIONS
        public JobHandle Dispose(JobHandle inputDeps)
        {
            inputDeps = m_Allocator.Dispose(inputDeps);

            return inputDeps;
        }
#endif
    }
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class ExportDataAttribute : Attribute
    {
    }
}
