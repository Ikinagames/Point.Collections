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
#if UNITY_2019 && !UNITY_2020_OR_NEWER
#define UNITYENGINE_OLD
#endif
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Point.Collections.Buffer.LowLevel;
using Point.Collections.Threading;
using System;
using System.Collections;
using System.Collections.Generic;

#if !UNITYENGINE_OLD
// https://issuetracker.unity3d.com/issues/ecs-compiler-wrongly-detect-unmanaged-structs-as-containing-nullabe-fields

namespace Point.Collections.Buffer
{
#if UNITYENGINE
    [NativeContainer]
#endif
    public struct NativeLinearHashMap<TKey, TValue> :
        IEquatable<NativeLinearHashMap<TKey, TValue>>, IDisposable,
        IEnumerable<KeyValue<TKey, TValue>>

        where TKey : unmanaged, IEquatable<TKey>
        where TValue : unmanaged, IEquatable<TValue>
    {
        private UnsafeLinearHashMap<TKey, TValue> m_HashMap;
        private readonly ThreadInfo m_Owner;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle m_SafetyHandle;
        [NativeSetClassTypeToNullOnSchedule]
        private DisposeSentinel m_DisposeSentinel;
#endif

        public ref TValue this[TKey key]
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
                AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
                return ref m_HashMap[key];
            }
        }
        public bool Created => m_HashMap.IsCreated;
        public int Capacity => m_HashMap.Capacity;
        public int Count => m_HashMap.Count;

        public NativeLinearHashMap(int initialCount
#if UNITYENGINE
            , Allocator allocator
#endif
            )
        {
            m_HashMap = new UnsafeLinearHashMap<TKey, TValue>(initialCount
#if UNITYENGINE
                , allocator
#endif
                );
            m_Owner = ThreadInfo.CurrentThread;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Create(out m_SafetyHandle, out m_DisposeSentinel, 1, allocator);
#endif
        }

        public void Add(TKey key, TValue value)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            PointHelper.AssertThreadAffinity(in m_Owner);

            AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
            AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
            m_HashMap.Add(key, value);
        }
        public bool Remove(TKey key)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            PointHelper.AssertThreadAffinity(in m_Owner);

            AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
            AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
            return m_HashMap.Remove(key);
        }

        public void Dispose()
        {
            m_HashMap.Dispose();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Dispose(ref m_SafetyHandle, ref m_DisposeSentinel);
#endif
        }

        public bool Equals(NativeLinearHashMap<TKey, TValue> other) => m_HashMap.Equals(other.m_HashMap);

#if UNITYENGINE
        [NativeContainerIsReadOnly]
#endif
        public struct Enumerator : IEnumerator<KeyValue<TKey, TValue>>
        {
            private readonly ThreadInfo m_Owner;

            private UnsafeAllocator<KeyValue<TKey, TValue>>.ReadOnly m_Buffer;
            private int m_Index;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            private AtomicSafetyHandle m_SafetyHandle;
#endif

            public KeyValue<TKey, TValue> Current => m_Buffer[m_Index];
#if UNITYENGINE && UNITY_COLLECTIONS
            [NotBurstCompatible]
#endif
            object IEnumerator.Current => m_Buffer[m_Index];

            internal Enumerator(NativeLinearHashMap<TKey, TValue> hashMap)
            {
                m_Owner = hashMap.m_Owner;

                m_Buffer = hashMap.m_HashMap.Buffer;
                m_Index = 0;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
                m_SafetyHandle = hashMap.m_SafetyHandle;
                AtomicSafetyHandle.UseSecondaryVersion(ref m_SafetyHandle);
#endif
            }

            public bool MoveNext()
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                PointHelper.AssertThreadAffinity(in m_Owner);

                AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
                AtomicSafetyHandle.CheckReadAndThrow(m_SafetyHandle);
#endif
                while (m_Index < m_Buffer.Length &&
                        Current.Equals(default(TValue)))
                {
                    m_Index++;
                }

                if (Current.Equals(default(TValue))) return false;
                return true;
            }

            public void Reset()
            {
                m_Index = 0;
            }
            public void Dispose()
            {
                m_Index = -1;
            }
        }

        public IEnumerator<KeyValue<TKey, TValue>> GetEnumerator() => new Enumerator(this);
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator(this);
    }
}

#endif