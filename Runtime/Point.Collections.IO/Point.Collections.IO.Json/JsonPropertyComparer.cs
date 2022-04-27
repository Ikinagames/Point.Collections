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
#if UNITY_MATHEMATICS
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Point.Collections.IO.Json
{
    public struct JsonPropertyComparer :
        IComparer<JsonPropertyAttribute>,
        IComparer<Type>,
        IComparer<FieldInfo>
    {
        public int Compare(JsonPropertyAttribute x, JsonPropertyAttribute y)
        {
            if (x.Order < y.Order) return -1;
            else if (x.Order > y.Order) return 1;
            return 0;
        }
        public int Compare(Type x, Type y)
        {
            JsonPropertyAttribute
                a = x.GetCustomAttribute<JsonPropertyAttribute>(),
                b = y.GetCustomAttribute<JsonPropertyAttribute>();

            if (a == null || b == null) return 0;
            return Compare(a, b);
        }
        public int Compare(FieldInfo x, FieldInfo y)
        {
            JsonPropertyAttribute
                a = x.FieldType.GetCustomAttribute<JsonPropertyAttribute>(),
                b = y.FieldType.GetCustomAttribute<JsonPropertyAttribute>();

            if (a == null || b == null) return 0;
            return Compare(a, b);
        }
    }
}
