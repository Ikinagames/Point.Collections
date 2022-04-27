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
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Newtonsoft.Json;
using System;

namespace Point.Collections.Actions
{
    /// <summary>
    /// <see cref="IConstAction"/> 을 참조할 수 있는 레퍼런스 입니다.
    /// </summary>
    [JsonConverter(typeof(IO.Json.ConstActionReferenceJsonConverter))]
    public interface IConstActionReference : IEmpty
    {
        [JsonIgnore]
        Guid Guid { get; }
        [JsonIgnore]
        object[] Arguments { get; }

        void SetArguments(params object[] args);
    }
}
