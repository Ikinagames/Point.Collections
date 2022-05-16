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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2020_1_OR_NEWER
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if ENABLE_NODEGRAPH && UNITYENGINE

using GraphProcessor;
using Point.Collections.Graphs;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    [NodeCustomEditor(typeof(Comparison))]
    public class ComparisonView : BaseNodeView
    {
        public override void Enable()
        {
            Comparison comparisonNode = nodeTarget as Comparison;
            DrawDefaultInspector();

            var inputA = new FloatField("In A") { value = comparisonNode.inA };
            var inputB = new FloatField("In B") { value = comparisonNode.inB };
            inputA.RegisterValueChangedCallback(v => {
                owner.RegisterCompleteObjectUndo("Change InA value");
                comparisonNode.inA = v.newValue;
            });
            inputB.RegisterValueChangedCallback(v => {
                owner.RegisterCompleteObjectUndo("Change InB value");
                comparisonNode.inB = v.newValue;
            });

            nodeTarget.onAfterEdgeConnected += UpdateVisibleFields;
            nodeTarget.onAfterEdgeDisconnected += UpdateVisibleFields;

            UpdateVisibleFields(null);

            void UpdateVisibleFields(SerializableEdge _)
            {
                var inA = nodeTarget.GetPort(nameof(comparisonNode.inA), null);
                var inB = nodeTarget.GetPort(nameof(comparisonNode.inB), null);

                controlsContainer.Add(inputA);
                controlsContainer.Add(inputB);

                if (inA.GetEdges().Count > 0)
                    controlsContainer.Remove(inputA);
                if (inB.GetEdges().Count > 0)
                    controlsContainer.Remove(inputB);
            }
        }
    }
}

#endif