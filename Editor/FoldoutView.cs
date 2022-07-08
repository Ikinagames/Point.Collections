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
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public class FoldoutView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<FoldoutView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
            {
                name = "label",
                defaultValue = "NAME"
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                FoldoutView ate = ve as FoldoutView;

                ate.label = m_Label.GetValueFromBag(bag, cc);
            }
        }

        private VisualElement m_HeaderContainer, m_ContentContainer;
        private Label m_HeaderLabel;
        private bool m_IsExpanded = false;

        public override VisualElement contentContainer => m_ContentContainer;
        public string label
        {
            get => m_HeaderLabel.text;
            set => m_HeaderLabel.text = value;
        }
        public bool isExpanded
        {
            get => m_IsExpanded;
            set
            {
                m_IsExpanded = value;

                if (contentContainer.childCount == 0)
                {
                    contentContainer.style.Hide(true);
                }
                else
                {
                    if (m_IsExpanded)
                    {
                        if (contentContainer.childCount >= 0)
                        {
                            m_ContentContainer.style.Hide(false);
                        }
                    }
                    else
                    {
                        m_ContentContainer.style.Hide(true);
                    }
                }

                onExpand?.Invoke(m_IsExpanded);
                OnExpanded(m_IsExpanded);
            }
        }

        public event Action<bool> onExpand;

        public FoldoutView()
        {
            styleSheets.Add(CoreGUI.VisualElement.DefaultStyleSheet);
            AddToClassList("content-border");

            m_HeaderContainer = new VisualElement();
            m_HeaderContainer.AddToClassList("header-list-2");
            m_HeaderContainer.style.SetMargin(1.5f);
            {
                m_HeaderLabel = new Label(name);
                m_HeaderLabel.name = "H3-Label";
                m_HeaderLabel.RegisterCallback<MouseDownEvent>(OnExpand);
            }
            m_HeaderContainer.Add(m_HeaderLabel);
            hierarchy.Add(m_HeaderContainer);

            AfterCreateHeaderContainer(m_HeaderContainer);

            m_ContentContainer = new VisualElement();
            m_ContentContainer.AddToClassList("content-container");
            m_ContentContainer.AddToClassList("inner-container");
            m_ContentContainer.style.SetMargin(4);
            m_ContentContainer.style.paddingLeft = 17;
            m_ContentContainer.style.flexGrow = 1;
            {
                m_ContentContainer.style.Hide(true);
            }
            hierarchy.Add(m_ContentContainer);
        }
        public FoldoutView(string label) : this()
        {
            this.label = label;
        }

        private void OnExpand(MouseDownEvent t)
        {
            isExpanded = !isExpanded;
        }

        protected virtual void OnExpanded(bool expand) { }
        protected virtual void AfterCreateHeaderContainer(VisualElement headerContainer) { }
    }
}

#endif