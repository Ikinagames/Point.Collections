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

using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public abstract class InspectorEditor<T> : InspectorEditorBase<T>
        where T : UnityEngine.Object
    {
        public override sealed void OnInspectorGUI()
        {
            EditorUtilities.StringHeader(GetHeaderName());
            EditorUtilities.Line();

            OnInspectorGUIContents();
        }
        /// <summary>
        /// <inheritdoc cref="OnInspectorGUI"/>
        /// </summary>
        /// <see cref="UnityEditor.Editor.OnInspectorGUI"/> 와 같습니다.
        protected virtual void OnInspectorGUIContents() { base.OnInspectorGUI(); }
    }
    public abstract class InspectorEditorUXML<T> : InspectorEditorBase<T>
        where T : UnityEngine.Object
    {
        // Animation Reference
        // https://gamedev-resources.com/use-style-transitions-to-animate-a-menu/
        // https://forum.unity.com/threads/announcing-uss-transition.1203832/
        // https://forum.unity.com/threads/quick-transition-tutorial.1203841/

        public VisualElement RootVisualElement { get; private set; }
        protected virtual bool ShowEditorScript => false;

        public override sealed VisualElement CreateInspectorGUI()
        {
            if (RootVisualElement == null)
            {
                RootVisualElement = CreateVisualElement();
                SetupVisualElement(RootVisualElement);

                RootVisualElement.Bind(serializedObject);
            }

            return RootVisualElement;
        }

        protected virtual VisualElement CreateVisualElement()
        {
            VisualElement root = new VisualElement();
            var prop = serializedObject.GetIterator();
            prop.NextVisible(true);

            var label = CoreGUI.VisualElement.Label(GetHeaderName(), 20);
            label.style.unityFontStyleAndWeight = FontStyle.Bold;
            root.Add(label);

            //var scriptProp = prop.Copy();
            //PropertyField field = new PropertyField(scriptProp);
            //field.SetEnabled(false);
            //root.Add(field);

            if (ShowEditorScript)
            {
                ObjectField objectField = new ObjectField("Editor Script");
                objectField.objectType = TypeHelper.TypeOf<MonoScript>.Type;

                objectField.value = ScriptUtilities.FindEditorScriptFromClassName<T>();
                root.Add(objectField);
            }

            root.Add(CoreGUI.VisualElement.Space(10, LengthUnit.Pixel));

            while (prop.NextVisible(false))
            {
                var element = prop.Copy();
                if (element.GetFieldInfo().GetCustomAttribute<SpaceAttribute>() != null)
                {
                    root.Add(CoreGUI.VisualElement.Space());
                }

                var field = new PropertyField(element);

                string[] sp = element.propertyPath.Split('.');
                string fieldName = sp.Last();
                if (fieldName.Contains("data["))
                {
                    fieldName = sp[sp.Length - 3];
                }

                field.name = fieldName;

                root.Add(field);
            }

            return root;
        }
        protected virtual void SetupVisualElement(VisualElement root) { }

        public TElement Q<TElement>(string name = null, string className = null)
            where TElement : VisualElement
        {
            return RootVisualElement.Q<TElement>(name, className);
        }
        public VisualElement Q(string name = null, string className = null)
        {
            return RootVisualElement.Q(name, className);
        }
        public void RepaintVisualElement()
        {
            RootVisualElement.MarkDirtyRepaint();
        }

        #region Utils

        protected PropertyField PropertyField(string bindingPath)
        {
            PropertyField propertyField = new PropertyField(serializedObject.FindProperty(bindingPath));

            return propertyField;
        }

        #endregion
    }
}

#endif