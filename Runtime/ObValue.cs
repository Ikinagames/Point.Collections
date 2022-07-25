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
#if !UNITY_COLLECTIONS
using FixedString512Bytes = Point.Collections.FixedChar512Bytes;
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

namespace Point.Collections
{
    [Serializable]
    public class ObValue<TValue>
    {
        [SerializeField] private TValue m_Value;

        public TValue Value
        {
            get => m_Value;
            set
            {
                TValue prev = m_Value;
                m_Value = value;

                if (!EqualityComparer<TValue>.Default.Equals(prev, value))
                {
                    OnValueChanged?.Invoke(prev, m_Value);
                }
            }
        }

        public event Action<TValue, TValue> OnValueChanged;

        public ObValue(TValue value)
        {
            m_Value = value;
        }

        public static implicit operator TValue(ObValue<TValue> t) => t.Value;
    }
}
