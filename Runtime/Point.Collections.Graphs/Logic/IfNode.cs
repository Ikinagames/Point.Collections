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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace Point.Collections.Graphs
{
    [System.Serializable, NodeMenuItem("Logic/Conditional/If"), NodeMenuItem("Logic/Conditional/Branch")]
	public class IfNode : ConditionalNode
	{
		[Input(name = "Condition")]
		public bool condition;

		[Output(name = "True")]
		public ConditionalLink @true;
		[Output(name = "False")]
		public ConditionalLink @false;

		[Setting("Compare Function")]
		public CompareFunction compareOperator;

		public override string name => "If";

		public override IEnumerable<BaseNode> GetExecutableNodes()
		{
			string fieldName = condition ? nameof(@true) : nameof(@false);

			// Return all the nodes connected to either the true or false node
			return outputPorts.FirstOrDefault(n => n.fieldName == fieldName)
				.GetEdges().Select(e => e.inputNode as BaseNode);
		}
	}
}

#endif
