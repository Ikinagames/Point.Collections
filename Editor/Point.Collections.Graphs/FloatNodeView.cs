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

#if UNITYENGINE

using GraphProcessor;
using Point.Collections.Graphs;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    [NodeCustomEditor(typeof(FloatNode))]
    public class FloatNodeView : BaseNodeView
    {
        public override void Enable()
        {
            var floatNode = nodeTarget as FloatNode;

            DoubleField floatField = new DoubleField
            {
                value = floatNode.input
            };

            floatNode.onProcessed += () => floatField.value = floatNode.input;

            floatField.RegisterValueChangedCallback((v) => {
                owner.RegisterCompleteObjectUndo("Updated floatNode input");
                floatNode.input = (float)v.newValue;
            });

            controlsContainer.Add(floatField);
        }
    }
    [NodeCustomEditor(typeof(ColorNode))]
    public class ColorNodeView : BaseNodeView
    {
        public override void Enable()
        {
            AddControlField(nameof(ColorNode.color));
            style.width = 200;
        }
    }
    [NodeCustomEditor(typeof(StringNode))]
    public class StringNodeView : BaseNodeView
    {
        public override void Enable()
        {
            var node = nodeTarget as StringNode;

            var textArea = new TextField(-1, true, false, '*') { value = node.output };
            textArea.Children().First().style.unityTextAlign = TextAnchor.UpperLeft;
            textArea.style.width = 200;
            textArea.style.height = 100;
            textArea.RegisterValueChangedCallback(v => {
                owner.RegisterCompleteObjectUndo("Edit string node");
                node.output = v.newValue;
            });
            controlsContainer.Add(textArea);
        }
    }
}

#endif