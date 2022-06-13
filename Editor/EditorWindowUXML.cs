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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2019_1_OR_NEWER
#define UNITYENGINE
#if UNITY_2019 || !UNITY_2020_OR_NEWER
#define UNITYENGINE_OLD
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using UnityEditor;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public abstract class EditorWindowUXML : EditorWindow
    {
        public VisualElement UserVisualElement { get; private set; }

        private void CreateGUI()
        {
            UserVisualElement = CreateVisualElement();
            SetupVisualElement(UserVisualElement);

            rootVisualElement.Add(UserVisualElement);
        }

        protected abstract VisualElement CreateVisualElement();
        protected virtual void SetupVisualElement(VisualElement root) { }
    }
}

#endif