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
using UnityEngine;
using UnityEngine.Rendering;

namespace Point.Collections.Graphs
{
    [System.Serializable, NodeMenuItem("Logic/Conditional/Comparison")]
	public class Comparison : BaseNode
	{
		[Input(name = "In A")]
		public float inA;

		[Input(name = "In B")]
		public float inB;

		[Output(name = "Out")]
		public bool compared;

		public CompareFunction compareFunction = CompareFunction.LessEqual;

		public override string name => "Comparison";

		protected override void Process()
		{
			switch (compareFunction)
			{
				default:
				case CompareFunction.Disabled:
				case CompareFunction.Never: compared = false; break;
				case CompareFunction.Always: compared = true; break;
				case CompareFunction.Equal: compared = inA == inB; break;
				case CompareFunction.Greater: compared = inA > inB; break;
				case CompareFunction.GreaterEqual: compared = inA >= inB; break;
				case CompareFunction.Less: compared = inA < inB; break;
				case CompareFunction.LessEqual: compared = inA <= inB; break;
				case CompareFunction.NotEqual: compared = inA != inB; break;
			}
		}
	}
}

#endif
