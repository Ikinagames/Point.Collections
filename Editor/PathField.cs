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

using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public sealed class PathField : BindableElement, INotifyValueChanged<string>
    {
        public new class UxmlFactory : UxmlFactory<PathField, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_Label = new UxmlStringAttributeDescription
            {
                name = "label",
                defaultValue = "LABEL"
            };
            UxmlStringAttributeDescription m_Value = new UxmlStringAttributeDescription
            {
                name = "value",
                defaultValue = "PATH"
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                PathField ate = ve as PathField;

                ate.label = m_Label.GetValueFromBag(bag, cc);
                ate.value = m_Value.GetValueFromBag(bag, cc);
            }
        }

        private TextField m_Field;
        private Button m_Button;

        public override VisualElement contentContainer => null;

        public string label
        {
            get => m_Field.label;
            set => m_Field.label = value;
        }
        public string value
        {
            get => m_Field.value;
            set
            {
                using (ChangeEvent<string> ev = ChangeEvent<string>.GetPooled(m_Field.value, value))
                {
                    ev.target = this;
                    SetValueWithoutNotify(value);
                    SendEvent(ev);
                }
            }
        }

        public PathField()
        {
            this.styleSheets.Add(CoreGUI.VisualElement.DefaultStyleSheet);

            style.flexGrow = 1;
            style.flexDirection = FlexDirection.Row;
            style.justifyContent = Justify.SpaceBetween;

            m_Field = new TextField();
            m_Field.style.flexShrink = 1;
            m_Field.style.flexGrow = 1;
            hierarchy.Add(m_Field);

            m_Button = new Button();
            m_Button.style.flexShrink = 1;
            m_Button.style.width = new StyleLength(new Length(80, LengthUnit.Pixel));
            m_Button.text = "Open";
            m_Button.clicked += M_Button_clicked;
            hierarchy.Add(m_Button);
        }

        private void M_Button_clicked()
        {
            string appPath = Application.dataPath.Replace("Assets", string.Empty);

            string openPath = Path.Combine(appPath, value);
            string result = EditorUtility.OpenFolderPanel("Select Folder", openPath, string.Empty);
            if (!result.IsNullOrEmpty())
            {
                result = result.Replace(appPath, string.Empty);
                value = result;
            }
        }

        public void SetValueWithoutNotify(string newValue)
        {
            m_Field.SetValueWithoutNotify(newValue);
        }
    }
}

#endif