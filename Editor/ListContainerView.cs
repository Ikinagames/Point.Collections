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
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public class ListContainerView : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<ListContainerView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
            {
                name = "text",
                defaultValue = "NAME"
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                ListContainerView ate = ve as ListContainerView;

                ate.text = m_Text.GetValueFromBag(bag, cc);
            }
        }

        private Label m_HeaderLabel;
        private VisualElement m_ContentContainer;
        private Button m_AddButton, m_RemoveButton;
        private bool m_IsExpanded = false;
        private SerializedProperty m_BindedArrayProperty;

        public override VisualElement contentContainer => m_ContentContainer;
        public string text
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
            }
        }

        public event Action<bool> onExpand;
        public event Action onAddButtonClicked
        {
            add => m_AddButton.clicked += value;
            remove => m_AddButton.clicked -= value;
        }
        public event Action onRemoveButtonClicked
        {
            add => m_RemoveButton.clicked += value;
            remove => m_RemoveButton.clicked -= value;
        }

        public ListContainerView()
        {
            styleSheets.Add(CoreGUI.VisualElement.DefaultStyleSheet);
            AddToClassList("content-border");

            VisualElement headerContainer = new VisualElement();
            headerContainer.AddToClassList("header-list-2");
            headerContainer.style.SetMargin(1.5f);
            {
                m_HeaderLabel = new Label(name);
                m_HeaderLabel.name = "H3-Label";
                m_HeaderLabel.RegisterCallback<MouseDownEvent>(OnExpand);
                headerContainer.Add(m_HeaderLabel);

                m_AddButton = new Button();
                m_RemoveButton = new Button();
                headerContainer.Add(m_AddButton);
                headerContainer.Add(m_RemoveButton);

                m_AddButton.name = "AddButton";
                m_AddButton.text = "+";
                m_AddButton.AddToClassList("header-button-1");
                m_AddButton.clicked += OnAddButtonClicked;

                m_RemoveButton.name = "RemoveButton";
                m_RemoveButton.text = "-";
                m_RemoveButton.AddToClassList("header-button-1");
                m_RemoveButton.SetEnabled(false);
                m_RemoveButton.clicked += OnRemoveButtonClicked;
            }
            hierarchy.Add(headerContainer);

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
        public ListContainerView(string text, string tooltip, SerializedProperty array) : this()
        {
            if (!text.IsNullOrEmpty())
            {
                m_HeaderLabel.text = text;
                if (!tooltip.IsNullOrEmpty())
                {
                    m_HeaderLabel.tooltip = tooltip;
                    m_HeaderLabel.displayTooltipWhenElided = true;
                }
            }

            if (array != null && array.isArray)
            {
                m_BindedArrayProperty = array;
                m_HeaderLabel.RegisterCallback<MouseDownEvent>(OnExpandProperty);

                for (int i = 0; i < m_BindedArrayProperty.arraySize; i++)
                {
                    var prop = m_BindedArrayProperty.GetArrayElementAtIndex(i);
                    PropertyField field = new PropertyField(prop);

                    Add(field);
                }

                isExpanded = m_BindedArrayProperty.isExpanded;
            }
        }
        public ListContainerView(string text, SerializedProperty array) : this(text, string.Empty, array)
        {
        }
        public ListContainerView(SerializedProperty array)
            : this(array.displayName, array.tooltip, array)
        {
        }

        private void OnExpandProperty(MouseDownEvent t)
        {
            m_BindedArrayProperty.isExpanded = !m_BindedArrayProperty.isExpanded;
            m_BindedArrayProperty.serializedObject.ApplyModifiedProperties();
        }
        private void OnExpand(MouseDownEvent t)
        {
            isExpanded = !isExpanded;
        }

        protected virtual void OnAddButtonClicked()
        {
            if (m_BindedArrayProperty == null) return;

            int index = m_BindedArrayProperty.arraySize;
            m_BindedArrayProperty.InsertArrayElementAtIndex(index);
            var prop = m_BindedArrayProperty.GetArrayElementAtIndex(index);
            prop.SetDefaultValue();

            PropertyField field = new PropertyField(prop);
            field.BindProperty(prop);

            Add(field);
            m_BindedArrayProperty.serializedObject.ApplyModifiedProperties();
        }
        protected virtual void OnRemoveButtonClicked()
        {
            if (m_BindedArrayProperty == null) return;

            int index = m_BindedArrayProperty.arraySize - 1;

            m_BindedArrayProperty.DeleteArrayElementAtIndex(index);
            RemoveAt(index);

            m_BindedArrayProperty.serializedObject.ApplyModifiedProperties();
        }

        private List<VisualElement> m_Childs = new List<VisualElement>();

        /// <inheritdoc cref="VisualElement.Add(VisualElement)"/>
        public new void Add(VisualElement item)
        {
            if (contentContainer.childCount == 0)
            {
                m_ContentContainer.style.Hide(false);
                m_RemoveButton.SetEnabled(true);
            }

            VisualElement element = new VisualElement();
            element.AddToClassList("list-content");
            //element.style.SetBorderColor(Color.white);
            //element.style.SetBorderWidth(1);

            Button button = new Button();
            button.style.width = 30;
            button.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            button.style.flexShrink = 1;
            button.text = "-";
            button.clicked += delegate
            {
                Remove(item);
            };

            item.AddToClassList("list-content-item");

            element.Add(button);
            element.Add(item);

            base.Add(element);
            m_Childs.Add(item);
        }

        public new void Remove(VisualElement item)
        {
            item.RemoveFromClassList("list-content-item");

            int index = m_Childs.IndexOf(item);
            item = this[index];
            base.Remove(item);
            m_Childs.RemoveAt(index);

            if (contentContainer.childCount == 0)
            {
                m_ContentContainer.style.Hide(true);
                m_RemoveButton.SetEnabled(false);
            }

            //item.RemoveFromClassList("list-content-margin");
        }
        public new void RemoveAt(int index)
        {
            VisualElement item = this[index];
            item.ElementAt(1).RemoveFromClassList("list-content-item");

            base.RemoveAt(index);
            m_Childs.RemoveAt(index);

            if (contentContainer.childCount == 0)
            {
                m_ContentContainer.style.Hide(true);
                m_RemoveButton.SetEnabled(false);
            }

            //item.RemoveFromClassList("list-content-margin");
        }
    }
}

#endif