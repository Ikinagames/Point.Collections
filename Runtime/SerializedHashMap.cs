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

using System;
using System.Linq;
#if UNITY_2019_1_OR_NEWER
using UnityEngine;
using System.Collections.Generic;
#if UNITY_MATHEMATICS
#endif

namespace Point.Collections
{
    [Serializable]
    public class SerializedHashMap<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private TKey[] m_Keys = Array.Empty<TKey>();
        [SerializeField] private TValue[] m_Values = Array.Empty<TValue>();

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            for (int i = 0; i < m_Keys.Length; i++)
            {
                Add(m_Keys[i], m_Values[i]);
            }

            m_Keys = Array.Empty<TKey>();
            m_Values = Array.Empty<TValue>();
        }
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            m_Keys = Keys.ToArray();
            m_Values = new TValue[m_Keys.Length];

            for (int i = 0; i < m_Keys.Length; i++)
            {
                m_Values[i] = this[m_Keys[i]];
            }
        }
    }
}

#endif
