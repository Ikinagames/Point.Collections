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

using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public class AssetCreateView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<AssetCreateView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
            {
                name = "text",
                defaultValue = "NAME"
            };

            UxmlBoolAttributeDescription m_ShowTemplete = new UxmlBoolAttributeDescription
            {
                name = "show-Templete",
                defaultValue = true
            };
            UxmlStringAttributeDescription m_TempleteFieldName = new UxmlStringAttributeDescription
            {
                name = "templete-Field-Name",
                defaultValue = "Templete"
            };
            UxmlTypeAttributeDescription<UnityEngine.Object> m_TempleteFieldType = new UxmlTypeAttributeDescription<UnityEngine.Object>
            {
                name = "templete-Field-Type",
                defaultValue = TypeHelper.TypeOf<UnityEngine.Object>.Type
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                AssetCreateView ate = ve as AssetCreateView;

                ate.text = m_Text.GetValueFromBag(bag, cc);
                ate.templeteFieldName = m_TempleteFieldName.GetValueFromBag(bag, cc);
                ate.templeteFieldType = m_TempleteFieldType.GetValueFromBag(bag, cc);
            }
        }

        private Label m_HeaderLabel;
        private VisualElement m_ContentContainer;
        private VisualElement m_TempleteRow;

        private bool m_ShowTemplete = true;

        public string text { get => m_HeaderLabel.text; set => m_HeaderLabel.text = value; }
        public bool showTemplete
        {
            get => m_ShowTemplete;
            set
            {
                if (m_ShowTemplete != value)
                {
                    m_TempleteRow.style.Hide(!value);
                }
                m_ShowTemplete = value;
            }
        }
        public string templeteFieldName
        {
            get => templeteField.label;
            set => templeteField.label = value;
        }
        public Type templeteFieldType
        {
            get => templeteField.objectType;
            set => templeteField.objectType = value;
        }

        public ObjectField templeteField { get; set; }
        public Button templeteLocateButton { get; set; }

        public VisualElement beforeContentContainer { get; private set; }
        public override VisualElement contentContainer => m_ContentContainer;

        public AssetCreateView()
        {
            styleSheets.Add(CoreGUI.VisualElement.DefaultStyleSheet);
            AddToClassList("content-container");

            m_HeaderLabel = new Label(name);
            m_HeaderLabel.AddToClassList("header-label");
            hierarchy.Add(m_HeaderLabel);

            var rootContainer = new VisualElement();
            rootContainer.name = "RootContainer";
            hierarchy.Add(rootContainer);

            beforeContentContainer = new VisualElement();
            beforeContentContainer.name = "BeforeContentContainer";
            rootContainer.Add(beforeContentContainer);

            // Templete
            {
                m_TempleteRow = new VisualElement();
                m_TempleteRow.name = "TempleteRow";
                m_TempleteRow.style.flexDirection = FlexDirection.Row;
                beforeContentContainer.Add(m_TempleteRow);

                templeteField = new ObjectField();
                templeteField.style.flexGrow = 1;
                m_TempleteRow.Add(templeteField);

                templeteLocateButton = new Button();
                templeteLocateButton.text = "Locate";
                m_TempleteRow.Add(templeteLocateButton);
            }

            m_ContentContainer = new VisualElement();
            m_ContentContainer.name = "ContentContainer";
            rootContainer.Add(m_ContentContainer);

            contentContainer.AddToClassList("content-container");
            contentContainer.AddToClassList("inner-container");
        }
    }
}

#endif