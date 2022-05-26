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

#if UNITY_2019_1_OR_NEWER
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using Point.Collections.ResourceControl;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
#if UNITY_ADDRESSABLES
    internal sealed class AddressableMenu : SetupWizardMenuItem
    {
        public override string Name => "Addressables";
        public override int Order => 1;

        public override bool Predicate()
        {
            return true;
        }
        public override void OnGUI()
        {
            if (CoreGUI.BoxButton("Locate Resource Hash Map", Color.grey))
            {
                ResourceHashMap hashMap = ResourceHashMap.Instance;

                Selection.activeObject = hashMap;
                EditorGUIUtility.PingObject(hashMap);
            }
        }
    }
#endif
}

#endif