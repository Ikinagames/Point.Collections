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
using System.Linq;
using UnityEngine;

namespace Point.Collections.Graphs
{
    [System.Serializable]
	/// <summary>
	/// This class represent a waitable node which invokes another node after a time/frame
	/// </summary>
	public abstract class WaitableNode : LinearConditionalNode
	{
		[Output(name = "Execute After")]
		public ConditionalLink executeAfter;

		protected void ProcessFinished()
		{
			onProcessFinished.Invoke(this);
		}

		[HideInInspector]
		public Action<WaitableNode> onProcessFinished;

		public IEnumerable<ConditionalNode> GetExecuteAfterNodes()
		{
			return outputPorts.FirstOrDefault(n => n.fieldName == nameof(executeAfter))
							  .GetEdges().Select(e => e.inputNode as ConditionalNode);
		}
	}
}

#endif
