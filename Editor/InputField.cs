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
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public class InputField : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<InputField, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
            {
                name = "text",
                defaultValue = "LABEL"
            };
            UxmlStringAttributeDescription m_ItemText = new UxmlStringAttributeDescription
            {
                name = "itemText",
                defaultValue = "ITEMTEXT"
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                InputField ate = ve as InputField;

                ate.text = m_Text.GetValueFromBag(bag, cc);
                ate.itemText = m_ItemText.GetValueFromBag(bag, cc);
            }
        }

        private sealed class InputFieldDisplay : VisualElement
        {
            private const string ussClassName = "unity-object-field-display";
            private const string iconUssClassName = ussClassName + "__icon";
            private const string labelUssClassName = ussClassName + "__label";

            private InputField m_Field;
            private Label m_Label;
            private Image m_Icon;

            public string itemText
            {
                get => m_Label.text;
                set => m_Label.text = value;
            }
            public Texture itemIcon
            {
                get => m_Icon.image;
                set => m_Icon.image = value;
            }

            public InputFieldDisplay(InputField field)
            {
                AddToClassList(ussClassName);

                m_Field = field;

                m_Icon = new Image
                {
                    scaleMode = ScaleMode.ScaleAndCrop,
                    pickingMode = PickingMode.Ignore
                };
                m_Icon.AddToClassList(iconUssClassName);
                m_Label = new Label()
                {
                    pickingMode = PickingMode.Ignore
                };
                m_Label.AddToClassList(labelUssClassName);

                Add(m_Icon);
                Add(m_Label);
            }
        }
        private sealed class FieldSelector : VisualElement
        {
            private readonly InputField m_Field;
            public FieldSelector(InputField field)
            {
                m_Field = field;
            }
            protected override void ExecuteDefaultAction(EventBase evt)
            {
                base.ExecuteDefaultAction(evt);
                MouseDownEvent obj = evt as MouseDownEvent;
                if (obj != null && obj.button == 0)
                {
                    m_Field.ShowSelector();
                }
            }
        }

        public const string ussBaseClassName = "unity-base-field";
        public const string ussClassName = "unity-object-field";
        public const string labelUssBaseClassName = ussBaseClassName + "__label";
        public const string labelUssClassName = ussClassName + "__label";
        public const string inputUssBaseClassName = ussBaseClassName + "__input";
        public const string inputUssClassName = ussClassName + "__input";
        public const string objectUssClassName = ussClassName + "__object";
        public const string selectorUssClassName = ussClassName + "__selector";

        private Label m_Label;
        private InputFieldDisplay m_Display;
        private FieldSelector m_Selector;

        /// <summary>
        /// <see cref="SearchProviderBase"/>
        /// </summary>
        public event Action OnSelectorOpen;
        private readonly Action m_AsyncOnProjectOrHierarchyChangedCallback;
        private readonly Action m_OnProjectOrHierarchyChangedCallback;

        public string text
        {
            get => m_Label.text;
            set => m_Label.text = value;
        }
        public Texture itemIcon
        {
            get => m_Display.itemIcon;
            set => m_Display.itemIcon = value;
        }
        public string itemText
        {
            get => m_Display.itemText;
            set => m_Display.itemText = value;
        }

        public override VisualElement contentContainer => null;

        public event Action OnFieldMouseClicked;

        public InputField() : this("LABEL", CoreGUI.EmptyIcon, "ITEMTEXT") { }
        public InputField(string text, Texture itemIcon, string itemText)
        {
            styleSheets.Add(CoreGUI.VisualElement.DefaultStyleSheet);
            AddToClassList(ussBaseClassName);
            AddToClassList(ussClassName);

            m_Label = new Label(text);
            m_Label.AddToClassList(labelUssBaseClassName);
            m_Label.AddToClassList(labelUssClassName);

            m_Display = new InputFieldDisplay(this);
            m_Display.AddToClassList(inputUssBaseClassName);
            m_Display.AddToClassList(inputUssClassName);
            m_Display.AddToClassList(objectUssClassName);
            m_Display.RegisterCallback<MouseDownEvent>(OnFieldMouseClickEvent);
            {
                m_Selector = new FieldSelector(this);
                m_Selector.AddToClassList(selectorUssClassName);

            }
            m_Display.Add(m_Selector);

            hierarchy.Add(m_Label);
            hierarchy.Add(m_Display);

            //

            m_AsyncOnProjectOrHierarchyChangedCallback = delegate
            {
                base.schedule.Execute(m_OnProjectOrHierarchyChangedCallback);
            };
            m_OnProjectOrHierarchyChangedCallback = delegate
            {
                //m_Display.Update();
            };
            RegisterCallback<AttachToPanelEvent>(delegate
            {
                EditorApplication.projectChanged += m_AsyncOnProjectOrHierarchyChangedCallback;
                EditorApplication.hierarchyChanged += m_AsyncOnProjectOrHierarchyChangedCallback;
            });
            RegisterCallback<DetachFromPanelEvent>(delegate
            {
                EditorApplication.projectChanged -= m_AsyncOnProjectOrHierarchyChangedCallback;
                EditorApplication.hierarchyChanged -= m_AsyncOnProjectOrHierarchyChangedCallback;
            });

            this.itemIcon = itemIcon;
            this.itemText = itemText;
        }

        private void OnFieldMouseClickEvent(MouseDownEvent ev)
        {
            OnFieldMouseClicked?.Invoke();
        }

        public void ShowSelector()
        {
            OnShowSelector();

            OnSelectorOpen?.Invoke();
        }
        public void SetItem<TType>(TType obj) where TType : UnityEngine.Object
        {
            GUIContent gUIContent = EditorGUIUtility.ObjectContent(
                obj, TypeHelper.TypeOf<TType>.Type);
            itemIcon = gUIContent.image;
            itemText = gUIContent.text;
        }
        public void SetItem<TType>(string text, TType obj) where TType : UnityEngine.Object
        {
            GUIContent gUIContent = EditorGUIUtility.ObjectContent(
                obj, TypeHelper.TypeOf<TType>.Type);
            itemIcon = gUIContent.image;
            itemText = text;
        }
        public void SetItem(Texture icon, string text)
        {
            itemIcon = icon;
            itemText = text;
        }

        protected virtual void OnShowSelector() { }
    }
}

#endif