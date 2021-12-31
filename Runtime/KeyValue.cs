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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

using Point.Collections.Buffer.LowLevel;
using System;

namespace Point.Collections
{
    public struct KeyValue<TKey, TValue> : IEmpty, IEquatable<KeyValue<TKey, TValue>>
        where TKey : unmanaged, IEquatable<TKey>
        where TValue : unmanaged
    {
        public readonly TKey key;
        public TValue value;

        public KeyValue(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }

        public bool IsEmpty()
        {
            if (key is IEmpty emptyAble)
            {
                return emptyAble.IsEmpty();
            }

            return this.key.Equals(default(TKey));
        }
        public bool IsKeyEmptyOrEquals(in TKey key)
        {
            return IsEmpty() || this.key.Equals(key);
        }

        public bool Equals(KeyValue<TKey, TValue> other)
        {
            if (!key.Equals(other.key)) return false;

            return UnsafeBufferUtility.BinaryComparer(ref value, ref other.value);
        }
    }
}