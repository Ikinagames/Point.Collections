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

#if UNITY_2020_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

using UnityEngine;
using GraphProcessor;
using System.Collections.Generic;
using System.Linq;
#if UNITY_MATHEMATICS
#endif

namespace Point.Collections.Graphs
{
    [System.Serializable, NodeMenuItem("Unity/Get Component")]
    public abstract class GetComponentNode<TComponent> : BaseNode
        where TComponent : Component
    {
        [Input(name = "Target")]
        protected object target;

        [Output(name = "Output")]
        protected TComponent output;

        protected override sealed void Process()
        {
            if (target == null)
            {
                //PointHelper.LogError(Channel.Core,
                //    $"Input is null.");
                return;
            }
            else if (!TypeHelper.InheritsFrom<UnityEngine.Object>(target.GetType()))
            {
                PointHelper.LogError(Channel.Core,
                    $"Input({TypeHelper.ToString(target.GetType())}) is not inherits from {nameof(UnityEngine.Object)}. ");
                return;
            }

            if (target is GameObject gameObject)
            {
                output = gameObject.GetComponentInChildren<TComponent>();
            }
            else if (target is Component component)
            {
                output = component.GetComponent<TComponent>();
            }
        }

        [CustomPortInput(nameof(target), typeof(UnityEngine.Object))]
        public void PullInputs(List<SerializableEdge> edges)
        {
            "pull input executed".ToLog();

            if (edges.Count != 1)
            {
                target = null;
                return;
            }

            target = (UnityEngine.Object)edges[0].passThroughBuffer;

            ParameterNode node = (ParameterNode)edges[0].outputNode;
            $"{node.parameterGUID} :: {node.parameter?.ToString()} :: {node.parameter ?.value} :: {edges[0].passThroughBuffer}".ToLog();
        }
    }
}

#endif
