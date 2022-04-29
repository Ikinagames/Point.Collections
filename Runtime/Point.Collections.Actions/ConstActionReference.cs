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
using System.Collections.Generic;
using System.Linq;

namespace Point.Collections.Actions
{
    /// <inheritdoc cref="IConstActionReference"/>
    [Serializable]
    public sealed class ConstActionReference : ConstActionReferenceBase
    {
        public ConstActionReference()
        {
            m_Guid = Guid.Empty.ToString();
            m_Arguments = Array.Empty<object>();
        }
        public ConstActionReference(Guid guid, IEnumerable<object> args)
        {
            m_Guid = guid.ToString();
            if (args == null || !args.Any())
            {
                m_Arguments = Array.Empty<object>();
            }
            else m_Arguments = args.ToArray();
        }
    }
    /// <inheritdoc cref="IConstActionReference"/>
    /// <typeparam name="TResult">이 액션이 수행했을 때 반환하는 값의 타입입니다.</typeparam>
    [Serializable]
    public sealed class ConstActionReference<TResult> : ConstActionReferenceBase
    {
        public ConstActionReference()
        {
            m_Guid = Guid.Empty.ToString();
            m_Arguments = Array.Empty<object>();
        }
        public ConstActionReference(Guid guid, IEnumerable<object> args)
        {
            m_Guid = guid.ToString();
            if (args == null || !args.Any())
            {
                m_Arguments = Array.Empty<object>();
            }
            else m_Arguments = args.ToArray();
        }
    }

    public abstract class ConstActionReferenceBase : IConstActionReference
    {
#if UNITYENGINE
        [UnityEngine.SerializeField]
#endif
        [JsonProperty(Order = 0, PropertyName = "Guid")]
        protected string m_Guid = String.Empty;
#if UNITYENGINE
        [UnityEngine.SerializeReference]
#endif
        [JsonProperty(Order = 1, PropertyName = "Arguments")]
        protected object[] m_Arguments = Array.Empty<object>();

        public Guid Guid
        {
            get
            {
                if (!Guid.TryParse(m_Guid, out var result)) return Guid.Empty;
                return result;
            }
        }
        public object[] Arguments => m_Arguments;

        public bool IsEmpty() => m_Guid.Equals(Guid.Empty);
        public void SetArguments(params object[] args) => m_Arguments = args;
    }
}
