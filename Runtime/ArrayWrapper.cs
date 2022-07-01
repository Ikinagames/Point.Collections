﻿// Copyright 2022 Ikina Games
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

using Newtonsoft.Json;
using Point.Collections.Buffer.LowLevel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Point.Collections
{
    [Serializable, JsonArray]
    public class ArrayWrapper<T> : ICloneable, IList<T>, IReadOnlyList<T>, ISerializationCallbackReceiver
    {
        public static ArrayWrapper<T> Empty => Array.Empty<T>();

        [SerializeField, JsonProperty]
        private T[] m_Array = Array.Empty<T>();
        private int m_Count = 0;

        public T this[int index]
        {
            get => m_Array[index];
            set => m_Array[index] = value;
        }

        public int Length => m_Array.Length;
        public bool IsFixedSize => false;
        public bool IsReadOnly => false;

        /// <summary>
        /// 데이터가 남긴 자식의 갯수입니다.
        /// </summary>
        /// <remarks>
        /// 배열의 총 길이는 <seealso cref="Length"/>를 참조하세요.
        /// </remarks>
        public int Count => m_Count;
        public bool IsSynchronized => true;
        public object SyncRoot => throw new NotImplementedException();

        T IList<T>.this[int index]
        {
            get => m_Array[index];
            set => m_Array[index] = value;
        }

        public ArrayWrapper()
        {
            m_Array = Array.Empty<T>();
            m_Count = 0;
        }
        public ArrayWrapper(int initLength)
        {
            m_Array = new T[initLength];
            m_Count = 0;
        }
        [JsonConstructor]
        public ArrayWrapper(IEnumerable<T> attributes)
        {
            m_Array = attributes.ToArray();
            m_Count = m_Array.Length;
        }

        public object Clone()
        {
            ArrayWrapper<T> obj = (ArrayWrapper<T>)MemberwiseClone();

            obj.m_Array = (T[])m_Array.Clone();

            return obj;
        }

        public int IndexOf(T item)
        {
            return Array.IndexOf<T>(m_Array, item);
        }

        public void Insert(int index, T item)
        {
            throw new NotImplementedException();
        }

        public void Add(T item)
        {
            if (m_Array.Length <= m_Count)
            {
                Array.Resize(ref m_Array, m_Array.Length + 1);
            }
            int index = m_Count;
            m_Array[index] = item;
            m_Count++;
        }
        public void AddRange(IList<T> items)
        {
            if (m_Array.Length <= m_Count)
            {
                Array.Resize(ref m_Array, m_Array.Length + items.Count);
            }

            for (int i = m_Count, j = 0; j < items.Count; i++, j++)
            {
                m_Array[i] = items[j];
                m_Count++;
            }
        }

        public void Clear()
        {
            m_Array = Array.Empty<T>();
        }

        public bool Contains(T item) => m_Array.Contains(item);
        public void CopyTo(T[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public void RemoveAt(int index)
        {
            if (!m_Array.RemoveAtSwapBack(index))
            {
                return;
            }

            Array.Resize(ref m_Array, m_Array.Length - 1);
            m_Count--;
        }
        public bool Remove(T item)
        {
            if (!RemoveSwapback(item, default(T)))
            {
                return false;
            }

            Array.Resize(ref m_Array, m_Array.Length - 1);
            return true;
        }
        public bool RemoveAtSwapback(int index, T defaultValue = default(T))
        {
            m_Array[index] = defaultValue;
            UnsafeBufferUtility.RemoveAtSwapBack(m_Array, index);
            m_Count--;

            return true;
        }
        public bool RemoveSwapback(T item, T defaultValue = default(T))
        {
            int index = IndexOf(item);

            if (index < 0) return false;

            m_Array[index] = defaultValue;
            UnsafeBufferUtility.RemoveAtSwapBack(m_Array, index);
            m_Count--;

            return true;
        }
        public bool RemoveEmptyElements()
        {
            if (m_Count == m_Array.Length) return false;

            var list = m_Array.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
            {
#if UNITYENGINE
                if (list[i] is UnityEngine.Object obj && obj == null)
                {
                    list.RemoveAt(i);
                    continue;
                }
#endif
                if (EqualityComparer<T>.Default.Equals(list[i], default(T)))
                {
                    list.RemoveAt(i);
                    continue;
                }
            }
            m_Array = list.ToArray();
            return true;
        }

        public void Resize(int length)
        {
            int prev = m_Array.Length;
            Array.Resize(ref m_Array, length);

            if (length < Count)
            {
                m_Count = length;
            }
        }

        public IEnumerator<T> GetEnumerator() => ((IList<T>)m_Array).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_Array.GetEnumerator();

        #region ISerializationCallbackReceiver Implements

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
#if UNITYENGINE
            if (TypeHelper.InheritsFrom<UnityEngine.Object>(TypeHelper.TypeOf<T>.Type))
            {
                m_Count = 0;
                for (int i = 0; i < m_Array.Length; i++)
                {
                    UnityEngine.Object obj = m_Array[i] as UnityEngine.Object;
                    if (obj == null) continue;

                    m_Count++;
                }
                return;
            }
#endif
            m_Count = m_Array.Length;
        }

        #endregion

        public static implicit operator T[](ArrayWrapper<T> t) => t.m_Array;
        public static implicit operator ArrayWrapper<T>(T[] t) => new ArrayWrapper<T>(t);
    }
}

#endif