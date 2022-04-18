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
#if UNITY_MATHEMATICS
#endif

namespace Point.Collections.Graphs
{
    [System.Serializable, NodeMenuItem("Primitives/Float")]
    public class FloatNode : BaseNode
    {
        [Output("Out")]
        public float output;

        [SerializeField, Input("In")]
        public float input;

        public override string name => "Float";

        protected override void Process() => output = input;

        //[CustomPortBehavior(nameof(input))]
        //IEnumerable<PortData> InputPortBehaviour(List<SerializableEdge> edges)
        //{
        //    yield return new PortData
        //    {
        //        displayName = string.Empty,
        //        displayType = 
        //    };
        //}
    }
    [System.Serializable, NodeMenuItem("Primitives/Color")]
    public class ColorNode : BaseNode
    {
        [Output(name = "Color"), SerializeField]
        new public Color color;

        public override string name => "Color";
    }
    [System.Serializable, NodeMenuItem("Primitives/String")]
    public class StringNode : BaseNode
    {
        [Output(name = "Out"), SerializeField]
        public string output;

        public override string name => "String";
    }
    public class IntNode : BaseNode
    {
        [Output("Out")]
        public int output;

        [SerializeField, Input("In")]
        public int input;

        public override string name => "Int";

        protected override void Process()
        {
            output = input;
        }
    }
    [System.Serializable, NodeMenuItem("Primitives/Vector2")]
    public class Vector2Node : BaseNode
    {
        [Output(name = "Out")]
        public Vector2 output;

        [Input(name = "In"), SerializeField]
        public Vector2 input;

        public override string name => "Vector2";

        protected override void Process()
        {
            output = input;
        }
    }
    [System.Serializable, NodeMenuItem("Primitives/Vector3")]
    public class Vector3Node : BaseNode
    {
        [Output(name = "Out")]
        public Vector3 output;

        [Input(name = "In"), SerializeField]
        public Vector3 input;

        public override string name => "Vector3";

        protected override void Process()
        {
            output = input;
        }
    }
    [System.Serializable, NodeMenuItem("Primitives/Vector4")]
    public class Vector4Node : BaseNode
    {
        [Output(name = "Out")]
        public Vector4 output;

        [Input(name = "In"), SerializeField]
        public Vector4 input;

        public override string name => "Vector4";

        protected override void Process()
        {
            output = input;
        }
    }
}

#endif
