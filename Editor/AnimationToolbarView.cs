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

using System.CodeDom;
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
        private FloatField m_FrameField;

        const string c_BaseClassName = "animation-button-image";

        public float frame
        {
            get => m_FrameField.value;
            set => m_FrameField.value = value;
        }

        public event System.Action OnFirstKeyButton
        {
            add => m_FirstKeyButton.clicked += value;
            remove => m_FirstKeyButton.clicked -= value;
        }
        public event System.Action OnPrevKeyButton
        {
            add => m_PrevKeyButton.clicked += value;
            remove => m_PrevKeyButton.clicked -= value;
        }
        public event System.Action OnPlayKeyButton
        {
            add => m_PlayKeyButton.clicked += value;
            remove => m_PlayKeyButton.clicked -= value;
        }
        public event System.Action OnNextKeyButton
        {
            add => m_NextKeyButton.clicked += value;
            remove => m_NextKeyButton.clicked -= value;
        }
        public event System.Action OnLastKeyButton
        {
            add => m_LastKeyButton.clicked += value;
            remove => m_LastKeyButton.clicked -= value;
        }

        public event EventCallback<ChangeEvent<float>> OnFrameChanged
        {
            add => m_FrameField.RegisterValueChangedCallback(value);
            remove => m_FrameField.UnregisterValueChangedCallback(value);
        }

        public AnimationToolbarView()
        {
            styleSheets.Add(CoreGUI.VisualElement.IconStyleSheet);

            m_Toolbar = new Toolbar();
            m_FirstKeyButton = new ToolbarButton();
            m_PrevKeyButton = new ToolbarButton();
            m_PlayKeyButton = new ToolbarButton();
            m_NextKeyButton = new ToolbarButton();
            m_LastKeyButton = new ToolbarButton();
            m_Toolbar.Add(m_FirstKeyButton);
            m_Toolbar.Add(m_PrevKeyButton);
            m_Toolbar.Add(m_PlayKeyButton);
            m_Toolbar.Add(m_NextKeyButton);
            m_Toolbar.Add(m_LastKeyButton);

            VisualElement
                firstKeyImg = new VisualElement(),
                prevKeyImg = new VisualElement(),
                playKeyImg = new VisualElement(),
                nextKeyImg = new VisualElement(),
                lastKeyImg = new VisualElement();
            m_FirstKeyButton.Add(firstKeyImg);
            m_PrevKeyButton.Add(prevKeyImg);
            m_PlayKeyButton.Add(playKeyImg);
            m_NextKeyButton.Add(nextKeyImg);
            m_LastKeyButton.Add(lastKeyImg);

            firstKeyImg.AddToClassList(c_BaseClassName);
            prevKeyImg.AddToClassList(c_BaseClassName);
            playKeyImg.AddToClassList(c_BaseClassName);
            nextKeyImg.AddToClassList(c_BaseClassName);
            lastKeyImg.AddToClassList(c_BaseClassName);

            firstKeyImg.AddToClassList("animation-firstkey");
            prevKeyImg.AddToClassList("animation-prevkey");
            playKeyImg.AddToClassList("animation-playkey");
            nextKeyImg.AddToClassList("animation-nextkey");
            lastKeyImg.AddToClassList("animation-lastkey");

            m_FrameField = new FloatField();
            m_FrameField.style.minWidth = new StyleLength(new Length(25, LengthUnit.Pixel));
            m_FrameField.style.maxWidth = new StyleLength(new Length(50, LengthUnit.Pixel));
            m_Toolbar.Add(m_FrameField);

            Add(m_Toolbar);
        }

        public void SetFrameWithoutNotify(float frame)
        {
            m_FrameField.SetValueWithoutNotify(frame);
        }
    }
}

#endif