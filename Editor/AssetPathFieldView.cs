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
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public class AssetPathFieldView : BindableElement, INotifyValueChanged<string>
    {
        public new class UxmlFactory : UxmlFactory<AssetPathFieldView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
            {
                name = "text",
                defaultValue = "NAME"
            };
            UxmlStringAttributeDescription m_Value = new UxmlStringAttributeDescription
            {
                name = "value",
                defaultValue = ""
            };
            UxmlTypeAttributeDescription<UnityEngine.Object> m_ObjectType = new UxmlTypeAttributeDescription<UnityEngine.Object>
            {
                name = "objectType",
                defaultValue = TypeHelper.TypeOf<UnityEngine.Object>.Type
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                AssetPathFieldView ate = ve as AssetPathFieldView;

                ate.text = m_Text.GetValueFromBag(bag, cc);
                ate.value = m_Value.GetValueFromBag(bag, cc);
                ate.objectType = m_ObjectType.GetValueFromBag(bag, cc);
            }
        }

        private string m_Value;
        private ObjectField m_ObjectField;
        private TextField m_PathField;
        private Button m_Button;

        public string value
        {
            get => m_Value;
            set
            {
                using (ChangeEvent<string> ev = ChangeEvent<string>.GetPooled(m_Value, value))
                {
                    ev.target = this;
                    SetValueWithoutNotify(value);
                    SendEvent(ev);
                }
            }
        }
        public string text
        {
            get => m_ObjectField.label;
            set => m_ObjectField.label = value;
        }
        public Type objectType
        {
            get => m_ObjectField.objectType;
            set => m_ObjectField.objectType = value;
        }
        public override VisualElement contentContainer => null;

        public AssetPathFieldView()
        {
            styleSheets.Add(CoreGUI.VisualElement.DefaultStyleSheet);
            style.flexGrow = 1;
            style.flexDirection = FlexDirection.Row;

            m_ObjectField = new ObjectField();
            m_ObjectField.style.flexGrow = 1;
            m_ObjectField.RegisterValueChangedCallback(ObjectChanged);
            hierarchy.Add(m_ObjectField);

            m_PathField = new TextField();
            m_PathField.style.flexGrow = 1;
            m_PathField.style.overflow = Overflow.Hidden;
#if UNITY_2020_1_OR_NEWER
            m_PathField.style.textOverflow = TextOverflow.Ellipsis;
#endif
            m_PathField.style.maxWidth = new StyleLength(new Length(81, LengthUnit.Percent));
            m_PathField.style.Hide(true);
            hierarchy.Add(m_PathField);

            m_Button = new Button();
            m_Button.text = "Raw";
            m_Button.style.width = new StyleLength(new Length(45, LengthUnit.Pixel));
            hierarchy.Add(m_Button);
        }
        public AssetPathFieldView(SerializedProperty property) : this()
        {
            var strProp = property.FindPropertyRelative("p_AssetPath");
            if (strProp == null)
            {
                "?".ToLog();
                strProp = property;
            }

            this.BindProperty(strProp);
        }

        public void SetValueWithoutNotify(string newValue)
        {
            $"in {newValue}".ToLog();
            m_Value = newValue;
        }

        private void ObjectChanged(ChangeEvent<UnityEngine.Object> ev)
        {
            UnityEngine.Object target = ev.newValue;
            if (target == null)
            {
                m_PathField.SetValueWithoutNotify(string.Empty);
            }
            else if (PrefabUtility.IsPartOfPrefabInstance(target))
            {
                m_PathField.SetValueWithoutNotify(
                    PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target)
                    );
            }
            else
            {
                m_PathField.SetValueWithoutNotify(AssetDatabase.GetAssetPath(target));
            }

            value = (m_PathField.value);

            //guidProperty.stringValue = AssetDatabase.AssetPathToGUID(m_PathField.value);
            //pathProperty.serializedObject.ApplyModifiedProperties();
            //textField.SetValueWithoutNotify(pathProperty.stringValue);
        }
    }
}

#endif