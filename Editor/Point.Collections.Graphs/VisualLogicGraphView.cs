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

using Point.Collections.Graphs;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace Point.Collections.Editor
{
    public class VisualLogicGraphView : VisualGraphView
    {
        protected override bool canDeleteSelection
        {
            get
            {
                if (selection.Any(t => t is EntryNode) && graph.nodes.Count == 1)
                {
                    return false;
                }

                return base.canDeleteSelection;
            }
        }
        public VisualLogicGraphView(EditorWindow window) : base(window)
        {
        }
    }
}

#endif