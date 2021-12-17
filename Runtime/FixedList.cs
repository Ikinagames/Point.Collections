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

using System;
using UnityEngine;

namespace Point.Collections
{
    [Serializable]
    public sealed class FixedList<T> where T : class
    {
        [SerializeField] private T[] m_Value;
        private uint m_Length;

        public T this[int index]
        {
            get => m_Value[index];
            set => m_Value[index] = value;
        }
        public T[] Value => m_Value;
        public long LongLength => (long)m_Length;
        public int Length => (int)m_Length;

        public FixedList(int length)
        {
            m_Value = new T[length];
            m_Length = 0;
        }

        public void Add(T t)
        {
            if (m_Length++ > m_Value.Length)
            {
                Array.Resize(ref m_Value, Length);
            }

            m_Value[m_Length] = t;
        }
        public void Remove(T t)
        {
            for (int i = 0; i < m_Length; i++)
            {
                if (m_Value[i] == t)
                {
                    RemoveAt(i);
                    break;
                }
            }
        }
        public void RemoveAt(int index)
        {
            if (index + 1 < m_Length)
            {
                m_Length--;
                for (int i = index; i < m_Length; i++)
                {
                    m_Value[i] = m_Value[i + 1];
                }
                return;
            }

            m_Length--;
            m_Value[index] = null;
        }
    }
}
