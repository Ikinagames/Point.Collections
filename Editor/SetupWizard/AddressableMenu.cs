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

using Point.Collections.ResourceControl;
using Point.Collections.ResourceControl.Editor;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
#if UNITY_ADDRESSABLES
    internal sealed class AddressableMenu : SetupWizardMenuItem
    {
        public override string Name => "Addressables";
        public override int Order => 1;

        private List<ResourceList> m_RequireRebuildList = new List<ResourceList>();

        public override bool Predicate()
        {
            if (!ValidateResourceLists(m_RequireRebuildList))
            {
                return false;
            }

            return true;
        }
        static bool ValidateResourceLists(List<ResourceList> requireRebuildList)
        {
            requireRebuildList.Clear();
            bool result = true;
            foreach (var item in ResourceHashMap.Instance.ResourceLists)
            {
                if (!ResourceListEditor.Validate(item))
                {
                    requireRebuildList.Add(item);
                    result = false;
                }
            }

            return result;
        }

        public override void OnGUI()
        {
            if (CoreGUI.BoxButton("Locate Resource Hash Map", Color.grey))
            {
                ResourceHashMap hashMap = ResourceHashMap.Instance;

                Selection.activeObject = hashMap;
                EditorGUIUtility.PingObject(hashMap);
            }

            EditorGUILayout.Space();
            if (m_RequireRebuildList.Count > 0)
            {
                for (int i = 0; i < m_RequireRebuildList.Count; i++)
                {
                    EditorGUILayout.LabelField($"Require Rebuild {m_RequireRebuildList[i].name}");
                    if (GUILayout.Button("Require Rebuild"))
                    {
                        ResourceListEditor.Rebuild(m_RequireRebuildList[i]);
                    }
                }
            }
        }
    }
#endif
}

#endif