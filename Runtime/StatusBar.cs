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
#if UNITY_COLLECTIONS
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections
{
    public sealed class StatusBar : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<StatusBar, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlFloatAttributeDescription m_Value = new UxmlFloatAttributeDescription
            {
                name = "value",
                defaultValue = 50
            };

            UxmlFloatAttributeDescription m_MinimumFill = new UxmlFloatAttributeDescription
            {
                name = "minimum-fill",
                defaultValue = 0
            };
            UxmlFloatAttributeDescription m_MaximumFill = new UxmlFloatAttributeDescription
            {
                name = "maximum-fill",
                defaultValue = 100
            };
            UxmlColorAttributeDescription m_BackgroundColor = new UxmlColorAttributeDescription
            {
                name = "background-color",
                defaultValue = Color.clear
            };
            UxmlColorAttributeDescription m_FillColor = new UxmlColorAttributeDescription
            {
                name = "fill-color",
                defaultValue = Color.black
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                StatusBar ate = ve as StatusBar;

                ate.value = m_Value.GetValueFromBag(bag, cc);

                ate.minimumFill = m_MinimumFill.GetValueFromBag(bag, cc);
                ate.maximumFill = m_MaximumFill.GetValueFromBag(bag, cc);

                ate.backgroundColor = m_BackgroundColor.GetValueFromBag(bag, cc);
                ate.fillColor = m_FillColor.GetValueFromBag(bag, cc);
            }
        }

        private FilableBox m_FilableBox;

        public float minimumFill { get => m_FilableBox.minimumFill; set => m_FilableBox.minimumFill = value; }
        public float maximumFill { get => m_FilableBox.maximumFill; set => m_FilableBox.maximumFill = value; }

        public float value { get => m_FilableBox.value; set => m_FilableBox.value = value; }

        public Color backgroundColor { get => m_FilableBox.backgroundColor; set => m_FilableBox.backgroundColor = value; }
        public Color fillColor { get => m_FilableBox.fillColor; set => m_FilableBox.fillColor = value; }

        public event Action<float> valueChanged
        {
            add => m_FilableBox.valueChanged += value;
            remove => m_FilableBox.valueChanged -= value;
        }

        public StatusBar() : this(0, 100, null) { }
        public StatusBar(float minFill, float maxFill, Action<float> valueChanged)
        {
            m_FilableBox = new FilableBox(minFill, maxFill, valueChanged);
        }
    }
}
