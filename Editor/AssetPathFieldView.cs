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
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public class AssetPathFieldView : BindableElement, INotifyValueChanged<string>
    {
        public new class UxmlFactory : UxmlFactory<AssetPathFieldView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
            {
                name = "label",
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

                ate.label = m_Label.GetValueFromBag(bag, cc);
                ate.value = m_Value.GetValueFromBag(bag, cc);
                ate.objectType = m_ObjectType.GetValueFromBag(bag, cc);
            }
        }

        private string m_Value;
        private bool m_DisplayPath = false;
        private Label m_Label;
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
        public UnityEngine.Object objectValue
        {
            get => AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(m_Value);
            set
            {
                string path;
                if (value != null && PrefabUtility.IsPartOfPrefabInstance(value))
                {
                    path = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(value);
                }
                else path = value == null ? string.Empty : AssetDatabase.GetAssetPath(value);

                using (ChangeEvent<string> ev = ChangeEvent<string>.GetPooled(m_Value, path))
                {
                    ev.target = this;
                    SetValueWithoutNotify(path);
                    SendEvent(ev);
                }
            }
        }
        public string label
        {
            get => m_Label.text;
            set
            {
                m_Label.text = value;
                m_Label.style.Hide(value.IsNullOrEmpty());
                m_Label.MarkDirtyRepaint();
            }
        }
        public Type objectType
        {
            get => m_ObjectField.objectType;
            set => m_ObjectField.objectType = value;
        }
        public override VisualElement contentContainer => null;

        public const string ussBaseClassName = "unity-base-field";
        public const string ussClassName = "unity-object-field";
        public const string labelUssBaseClassName = ussBaseClassName + "__label";
        public const string labelUssClassName = ussClassName + "__label";

        public AssetPathFieldView()
        {
            styleSheets.Add(CoreGUI.VisualElement.DefaultStyleSheet);
            AddToClassList(ussBaseClassName);
            AddToClassList(ussClassName);
            //style.flexGrow = 1;
            style.maxWidth = new StyleLength(new Length(100, LengthUnit.Percent));
            style.flexDirection = FlexDirection.Row;

            m_Label = new Label();
            m_Label.AddToClassList(labelUssBaseClassName);
            m_Label.AddToClassList(labelUssClassName);
            m_Label.style.flexGrow = 1;
            m_Label.style.Hide(true);
            hierarchy.Add(m_Label);

            m_ObjectField = new ObjectField();
            m_ObjectField.allowSceneObjects = true;
            //m_ObjectField.style.flexShrink = 1;
            //m_ObjectField.style.flexGrow = 1;
            m_ObjectField.style.width = new StyleLength(new Length(50, LengthUnit.Percent));
            m_ObjectField.RegisterValueChangedCallback(ObjectChanged);
            hierarchy.Add(m_ObjectField);

            m_PathField = new TextField();
            //m_PathField.style.flexShrink = 1;
            //m_PathField.style.flexGrow = 1;
            m_PathField.style.width = new StyleLength(new Length(50, LengthUnit.Percent));
            m_PathField.style.overflow = Overflow.Hidden;
#if UNITY_2020_1_OR_NEWER
            m_PathField.style.textOverflow = TextOverflow.Ellipsis;
#endif
            m_PathField.style.Hide(true);
            m_PathField.RegisterValueChangedCallback(PathChanged);
            hierarchy.Add(m_PathField);

            m_Button = new Button();
            m_Button.text = "Raw";
            m_Button.style.flexShrink = 1;
            m_Button.style.flexGrow = 0;
            m_Button.style.SetBorderRadius(.1f);
            m_Button.style.width = new StyleLength(new Length(45, LengthUnit.Pixel));
            m_Button.clicked += M_Button_clicked;
            hierarchy.Add(m_Button);
        }
        public AssetPathFieldView(SerializedProperty property) : this()
        {
            BindProperty(property);
        }
        /// <inheritdoc cref="BindingExtensions.BindProperty(IBindable, SerializedProperty)"/>
        public void BindProperty(SerializedProperty property)
        {
            Assert.IsTrue(property.type.Contains(nameof(AssetPathField)),
                $"Cannot bind this property({property.displayName}, {property.type}) " +
                $"because it\'s not {nameof(AssetPathField)}");

            var strProp = property.FindPropertyRelative("p_AssetPath");
            Assert.IsNotNull(strProp);

            var propertyType = property.GetFieldInfo().FieldType;
            Type targetType = TypeHelper.TypeOf<UnityEngine.Object>.Type;
            if (propertyType.GenericTypeArguments.Length == 1)
            {
                targetType = propertyType.GenericTypeArguments[0];
            }
            objectType = targetType;

            ((IBindable)this).BindProperty(strProp);
        }

        private void M_Button_clicked()
        {
            m_DisplayPath = !m_DisplayPath;
            if (m_DisplayPath)
            {
                m_PathField.style.Hide(false);
                m_ObjectField.style.Hide(true);
            }
            else
            {
                m_PathField.style.Hide(true);
                m_ObjectField.style.Hide(false);
            }
        }
        public void SetValueWithoutNotify(string newValue)
        {
            m_Value = newValue;

            m_ObjectField.SetValueWithoutNotify(AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(newValue));
            m_PathField.SetValueWithoutNotify(newValue);

            //$"0 {m_Value}".ToLog();
        }   

        private void PathChanged(ChangeEvent<string> ev)
        {
            value = ev.newValue;
            //string newValue = ev.newValue;
            //UnityEngine.Object target = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(ev.newValue);
            //if (target == null)
            //{
            //    m_ObjectField.SetValueWithoutNotify(null);
            //}
            //else
            //{
            //    m_ObjectField.SetValueWithoutNotify(target);
            //}

            //$"1 {m_Value}".ToLog();
            //m_Value = newValue;
        }
        private void ObjectChanged(ChangeEvent<UnityEngine.Object> ev)
        {
            objectValue = ev.newValue;
            //UnityEngine.Object target = ev.newValue;
            //if (target == null)
            //{
            //    m_PathField.SetValueWithoutNotify(string.Empty);
            //}
            //else if (PrefabUtility.IsPartOfPrefabInstance(target))
            //{
            //    m_PathField.SetValueWithoutNotify(
            //        PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(target)
            //        );
            //}
            //else
            //{
            //    m_PathField.SetValueWithoutNotify(AssetDatabase.GetAssetPath(target));
            //}

            //m_Value = (m_PathField.value);
            //$"2 {m_Value}".ToLog();

            //guidProperty.stringValue = AssetDatabase.AssetPathToGUID(m_PathField.value);
            //pathProperty.serializedObject.ApplyModifiedProperties();
            //textField.SetValueWithoutNotify(pathProperty.stringValue);
        }
    }
}

#endif