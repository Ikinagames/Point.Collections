﻿// Copyright 2022 Ikina Games
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

using GraphProcessor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Point.Collections.Graphs
{
    [Serializable, NodeMenuItem("Logic/Conditional/IsNullOrEmpty")]
	public sealed class IsNullOrEmptyNode : ConditionalNode
    {
		[Input(name = "Object")]
		public object obj;

		[Output(name = "True")]
		public ConditionalLink @true;
		[Output(name = "False")]
		public ConditionalLink @false;

		public override string name => "Is Null or Empty";

        public override IEnumerable<BaseNode> GetExecutableNodes()
        {
			bool condition;
			if (obj == null ||
				(obj is IEmpty empty && empty.IsEmpty()))
			{
				condition = true;
			}
			else condition = false;

			string fieldName = condition ? nameof(@true) : nameof(@false);

			return outputPorts.FirstOrDefault(n => n.fieldName.Equals(fieldName, StringComparison.InvariantCultureIgnoreCase))
				.GetEdges().Select(e => e.inputNode as BaseNode);
		}
    }
}

#endif
