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

using GraphProcessor;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Point.Collections.Graphs
{
    [System.Serializable]
	/// <summary>
	/// This is the base class for every node that is executed by the conditional processor, it takes an executed bool as input to 
	/// </summary>
	public abstract class ConditionalNode : BaseNode, IConditionalNode
	{
		// These booleans will controls wether or not the execution of the folowing nodes will be done or discarded.
		[Input(name = "Executed", allowMultiple = true)]
		public ConditionalLink executed;

		public abstract IEnumerable<ConditionalNode> GetExecutedNodes();

		// Assure that the executed field is always at the top of the node port section
		public override FieldInfo[] GetNodeFields()
		{
			var fields = base.GetNodeFields();
			Array.Sort(fields, (f1, f2) => f1.Name == nameof(executed) ? -1 : 1);
			return fields;
		}
	}
}

#endif
