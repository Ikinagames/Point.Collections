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
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    [NodeCustomEditor(typeof(GetComponentNode))]
    public class GetComponentNodeView : BaseNodeView
    {
        private static string[] m_Types;
        protected static string[] Types
        {
            get
            {
                if (m_Types == null)
                {
                    m_Types = TypeCache.GetTypesDerivedFrom<Component>()
                        .Where(t => !t.IsAbstract)
                        .Select(t => t.AssemblyQualifiedName)
                        .ToArray();
                }
                return m_Types;
            }
        }
        private int m_CurrentIndex;

        public override void Enable()
        {
            IMGUIContainer field = new IMGUIContainer(OnGUI);
            
            contentContainer.Add(field);
        }
        private void OnGUI()
        {
            var getComponentNode = nodeTarget as GetComponentNode;

            EditorGUI.BeginChangeCheck();
            m_CurrentIndex = EditorGUILayout.Popup("Type", m_CurrentIndex, Types);

            if (EditorGUI.EndChangeCheck())
            {
                getComponentNode.type = Types[m_CurrentIndex];
            }
        }
    }
}

#endif