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

#if ENABLE_NODEGRAPH && UNITY_2020_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

using GraphProcessor;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Point.Collections.Graphs
{
    [Serializable]
    public sealed class ObjectExposedParameter : VisualExposedParameter
    {
        [SerializeField, Output("Object")]
        private UnityEngine.Object m_Object;

        public override Type GetValueType() => TypeHelper.TypeOf<UnityEngine.Object>.Type;
        public override object value { get => m_Object; set => m_Object = (UnityEngine.Object)value; }

        [CustomPortOutput(nameof(m_Object), typeof(UnityEngine.Object))]
        public void PushOutputs(List<SerializableEdge> connectedEdges)
        {
            "push objs".ToLog();
            for (int i = 0; i < connectedEdges.Count; i++)
            {
                connectedEdges[i].passThroughBuffer = m_Object;
            }
        }
    }
}

#endif
