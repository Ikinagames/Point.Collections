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
#if UNITY_COLLECTIONS
#endif


using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;

namespace Point.Collections.IO
{
    public struct SaveData : ISaveable
    {
        private Identifier m_Name;
        private KeyValuePair<Identifier, System.Type>[] m_Properties;
        private object[] m_Values;

        private struct Comparer : IComparer<KeyValuePair<Identifier, System.Type>>
        {
            public int Compare(KeyValuePair<Identifier, Type> x, KeyValuePair<Identifier, Type> y)
            {
                uint 
                    xx = x.Key,
                    yy = y.Key;

                if (xx < yy) return -1;
                else if (xx > yy) return 1;
                return 0;
            }
        }
        public object this[Identifier id]
        {
            get
            {
                int index = Array.BinarySearch(m_Properties, 
                    new KeyValuePair<Identifier, Type>(id, null), new Comparer());
                if (index < 0)
                {
                    throw new KeyNotFoundException();
                }

                return m_Values[index];
            }
            set
            {
                int index = Array.BinarySearch(m_Properties,
                    new KeyValuePair<Identifier, Type>(id, null), new Comparer());
                if (index < 0)
                {
                    throw new KeyNotFoundException();
                }

                m_Values[index] = value;
            }
        }

        Identifier ISaveable.Identifier => m_Name;

        public SaveData(Identifier name, IEnumerable<KeyValuePair<Identifier, System.Type>> values)
        {
            m_Name = name;
            m_Properties = values.OrderBy(t => (uint)t.Key).ToArray();
            m_Values = new object[m_Properties.Length];
        }
        void ISaveable.SaveValues(ref Bucket bucket)
        {
            for (int i = 0; i < m_Values.Length; i++)
            {
                bucket.Save(m_Values[i]);
            }
        }
        void ISaveable.LoadValues(ref Bucket bucket)
        {
            for (int i = 0; i < m_Values.Length; i++)
            {
                m_Values[i] = bucket.Load(m_Properties[i].Value);
            }
        }

        public void SetValue<T>(Identifier id, T value) where T : struct
        {
            this[id] = value;
        }
        public void SetValue(Identifier id, string value)
        {
            FixedString128Bytes str = value;
            this[id] = str;
        }
    }
}

#endif