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
using System.Collections.Generic;
using System.Reflection;

namespace Point.Collections.Graphs
{
    interface IConditionalNode
	{
		/// <summary>
		/// 이 노드에서 다음 노드로 갈 수 있는 노드를 지정합니다.
		/// </summary>
		/// <returns></returns>
		IEnumerable<BaseNode> GetExecutableNodes();

		FieldInfo[] GetNodeFields(); // Provide a custom order for fields (so conditional links are always at the top of the node)
	}
}

#endif
