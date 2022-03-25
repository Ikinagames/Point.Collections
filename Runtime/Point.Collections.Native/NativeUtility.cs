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

#if UNITY_2020
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System;
using System.Runtime.InteropServices;
#if UNITYENGINE
using Unity.Collections.LowLevel.Unsafe;
#endif

namespace Point.Collections.Native
{
    public unsafe static class NativeUtility
    {
        public static void* AddressOf<T>(ref T t) where T : unmanaged
        {
#if UNITYENGINE
            return UnsafeUtility.AddressOf(ref t);
#else
            fixed (T* ptr = &t)
            {
                return ptr;
            }
#endif
        }

        public static void* Malloc(in long size, in int align
#if UNITYENGINE
            , Unity.Collections.Allocator allocator
#endif
            )
        {
#if !UNITYENGINE
            return (void*)Marshal.AllocHGlobal((int)size);
            //return malloc(size, align);
#else
            return UnsafeUtility.Malloc(size, align, allocator);
#endif
        }
        public static void Free(void* ptr
#if UNITYENGINE
            , Unity.Collections.Allocator allocator
#endif
            )
        {
#if UNITYENGINE
            UnsafeUtility.Free(ptr, allocator);
#else
            // TODO : 임시
            Marshal.FreeHGlobal((IntPtr)ptr);
            //throw new NotImplementedException();
#endif
        }
        public static void MemCpy(void* ptr, void* from, in long size)
        {
#if UNITYENGINE
            UnsafeUtility.MemCpy(ptr, from, size);
#else
            //Marshal.Copy((IntPtr)from, (IntPtr)ptr, (int)size);
            // TODO : 임시
            System.Buffer.MemoryCopy(from, ptr, size, size);
            //memCpy(ptr, from, size);
#endif
        }
        public static void MemClear(void* ptr, in long size)
        {
#if UNITYENGINE
            UnsafeUtility.MemClear(ptr, size);
#else
            // TODO : 임시
            int from = 0;
            unsafe
            {
                System.Buffer.MemoryCopy(&from, ptr, size, TypeHelper.SizeOf<int>());
            }
            //memClear(ptr, size);
#endif
        }

#if !UNITYENGINE
        [DllImport("Point.Collections.Native.Internal")]
        private static extern void memorypool_initialize();
        [DllImport("Point.Collections.Native.Internal")]
        private static extern void memorypool_uninitialize();

        [DllImport("Point.Collections.Native.Internal")]
        private static extern void* malloc(in long size, in int align);
        [DllImport("Point.Collections.Native.Internal")]
        private static extern void memCpy(void* ptr, void* from, in long size);
        [DllImport("Point.Collections.Native.Internal")]
        private static extern void memClear(void* ptr, in long size);
        [DllImport("Point.Collections.Native.Internal")]
        private static extern void free(void* ptr);
#endif
    }
}
