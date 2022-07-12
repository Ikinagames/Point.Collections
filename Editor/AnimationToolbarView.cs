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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#if !UNITY_2020_1_OR_NEWER
#define UNITYENGINE_OLD
#endif

using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public class AnimationToolbarView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<AnimationToolbarView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                AnimationToolbarView ate = ve as AnimationToolbarView;
            }
        }

        private Toolbar m_Toolbar;
        private ToolbarButton
            m_FirstKeyButton,
            m_PrevKeyButton,
            m_PlayKeyButton,
            m_NextKeyButton,
            m_LastKeyButton;

        const string c_BaseClassName = "animation-button-image";

        public AnimationToolbarView()
        {
            styleSheets.Add(CoreGUI.VisualElement.IconStyleSheet);

            m_Toolbar = new Toolbar();
            m_FirstKeyButton = new ToolbarButton();
            m_PrevKeyButton = new ToolbarButton();
            m_PlayKeyButton = new ToolbarButton();
            m_NextKeyButton = new ToolbarButton();
            m_LastKeyButton = new ToolbarButton();

            m_FirstKeyButton.AddToClassList(c_BaseClassName);
            m_PrevKeyButton.AddToClassList(c_BaseClassName);
            m_PlayKeyButton.AddToClassList(c_BaseClassName);
            m_NextKeyButton.AddToClassList(c_BaseClassName);
            m_LastKeyButton.AddToClassList(c_BaseClassName);

            m_FirstKeyButton.AddToClassList("animation-firstkey");
            m_PrevKeyButton.AddToClassList("animation-prevkey");
            m_PlayKeyButton.AddToClassList("animation-playkey");
            m_NextKeyButton.AddToClassList("animation-nextkey");
            m_LastKeyButton.AddToClassList("animation-lastkey");
        }
    }
}

#endif