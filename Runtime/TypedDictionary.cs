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
using System.Collections.Generic;

namespace Point.Collections
{
    public sealed class TypedDictionary<TValue>
    {
        private static readonly TypedDictionary<TValue> s_Shared;
        public static TypedDictionary<TValue> Shared => s_Shared;

        private readonly Dictionary<Type, TValue> m_Dictionary;

        public TypedDictionary()
        {

        }
        ~TypedDictionary()
        {
            m_Dictionary.Clear();
        }

        public void Add<TKey>(TValue value)
        {
            m_Dictionary.Add(TypeHelper.TypeOf<TKey>.Type, value);
        }
        public void Add(Type key, TValue value)
        {
            m_Dictionary.Add(key, value);
        }
        public void Remove<TKey>()
        {
            m_Dictionary.Remove(TypeHelper.TypeOf<TKey>.Type);
        }
        public void Remove(Type key)
        {
            m_Dictionary.Remove(key);
        }

        public TValue Get<TKey>()
        {
            return m_Dictionary[TypeHelper.TypeOf<TKey>.Type];
        }
        public TValue Get(Type key)
        {
            return m_Dictionary[key];
        }
        public bool TryGetValue<TKey>(out TValue value)
        {
            return m_Dictionary.TryGetValue(TypeHelper.TypeOf<TKey>.Type, out value);
        }
        public bool TryGetValue(Type key, out TValue value)
        {
            return m_Dictionary.TryGetValue(key, out value);
        }
    }
}
