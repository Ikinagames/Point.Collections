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

using System;
#if UNITYENGINE
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
#endif

namespace Point.Collections.Buffer.LowLevel
{
    /// <summary>
    /// 미리 버퍼 사이즈를 정하고, 해당 버퍼만으로 FI-FO (First-In-First-Out), 선입 선출을 하는 컬렉션입니다.
    /// </summary>
    /// <remarks>
    /// 버퍼 사이즈를 초과하여 아이템을 넣는 경우, 에러 로그를 보내고 받아들여지지 않습니다.
    /// </remarks>
    /// <typeparam name="T"></typeparam>
#if UNITYENGINE
    [BurstCompatible]
#endif
    public struct UnsafeFixedQueue<T> :
#if UNITYENGINE
        INativeDisposable, 
#endif
        IDisposable, IEquatable<UnsafeFixedQueue<T>>
        where T : unmanaged
    {
        private struct Item
        {
            public bool Occupied;
            public T Data;
        }
        private struct List
        {
            public UnsafeAllocator<Item> Buffer;
            public int NextIndex, CurrentIndex;
        }

        private UnsafeAllocator<List> m_List;

        public bool IsCreated => m_List.IsCreated;
        public int Count
        {
            get
            {
                int
                    start = m_List[0].CurrentIndex,
                    end = m_List[0].NextIndex;

                if (end < start)
                {
                    return m_List[0].Buffer.Length - start + end;
                }
                return end - start;
            }
        }

        public UnsafeFixedQueue(int length
#if UNITYENGINE
            , Allocator allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory
#endif
            )
        {
            m_List = new UnsafeAllocator<List>(1
#if UNITYENGINE
                , allocator, options
#endif
                );
            m_List[0] = new List
            {
                Buffer = new UnsafeAllocator<Item>(length
#if UNITYENGINE
                , allocator, options
#endif
                ),

                NextIndex = 0,
                CurrentIndex = 0
            };
        }

        public void Enqueue(T item)
        {
            ref List list = ref m_List[0];
            ref Item temp = ref list.Buffer[list.NextIndex];
            if (temp.Occupied)
            {
                throw new Exception("Exceeding max count");
                //UnityEngine.Debug.LogError("Exceeding max count");
                //return;
            }

            temp.Occupied = true;
            temp.Data = item;

            list.NextIndex++;
            if (list.NextIndex >= list.Buffer.Length) list.NextIndex = 0;
        }
        public T Dequeue()
        {
            ref List list = ref m_List[0];
            ref Item temp = ref list.Buffer[list.CurrentIndex];
#if DEBUG_MODE
            if (!temp.Occupied)
            {
                UnityEngine.Debug.LogError(
                    $"{nameof(UnsafeFixedQueue<T>)} Doesn\'t have items.");
                return default(T);
            }
#endif

            list.CurrentIndex++;
            if (list.CurrentIndex >= list.Buffer.Length) list.CurrentIndex = 0;

            temp.Occupied = false;
            return temp.Data;
        }
        public bool TryDequeue(out T t)
        {
            ref List list = ref m_List[0];
            ref Item temp = ref list.Buffer[list.CurrentIndex];
            if (!temp.Occupied)
            {
                t = default(T);
                return false;
            }

            list.CurrentIndex++;
            if (list.CurrentIndex >= list.Buffer.Length) list.CurrentIndex = 0;

            temp.Occupied = false;
            t = temp.Data;

            return true;
        }

        public void Dispose()
        {
            m_List[0].Buffer.Dispose();
            m_List.Dispose();

            m_List = default(UnsafeAllocator<List>);
        }
#if UNITYENGINE
        public JobHandle Dispose(JobHandle inputDeps)
        {
            //#if ENABLE_UNITY_COLLECTIONS_CHECKS
            //            if (!UnsafeUtility.IsValidAllocator(m_Allocator))
            //            {
            //                UnityEngine.Debug.LogError(
            //                    $"{nameof(UnsafeFixedQueue<T>)} cannot be disposed. Allocator({m_Allocator}) is not valid.");
            //                throw new Exception();
            //            }
            //#endif
            JobHandle result = m_List[0].Buffer.Dispose(inputDeps);
            result = m_List.Dispose(result);

            m_List = default(UnsafeAllocator<List>);

            return result;
        }
#endif
        public bool Equals(UnsafeFixedQueue<T> other) => m_List.Equals(other.m_List);
    }
}
