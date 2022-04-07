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

#if UNITY_2020_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Point.Collections.Editor
{
    public abstract class InspectorEditor<T> : UnityEditor.Editor
        where T : UnityEngine.Object
    {
        /// <summary>
        /// <inheritdoc cref="UnityEditor.Editor.target"/>
        /// </summary>
        public new T target => (T)base.target;
        /// <summary>
        /// <inheritdoc cref="UnityEditor.Editor.targets"/>
        /// </summary>
        public new T[] targets => base.targets.Select(t => (T)t).ToArray();

        protected override sealed void OnHeaderGUI()
        {
            EditorUtilities.StringRich("Copyright 2022 Ikinagames. All rights reserved.", 11, true);
            EditorUtilities.StringRich("Point Framework®", 11, true);

            base.OnHeaderGUI();
        }

        protected SerializedProperty GetSerializedProperty(string name)
        {
            return serializedObject.FindProperty(name);
        }
        protected SerializedProperty GetSerializedProperty(SerializedProperty property, string name)
        {
            return property.FindPropertyRelative(name);
        }

        public virtual string GetHeaderName() => TypeHelper.ToString(target.GetType());
        public override sealed void OnInspectorGUI()
        {
            EditorUtilities.StringHeader(GetHeaderName());
            EditorUtilities.Line();

            OnInspectorGUIContents();
        }
        /// <summary>
        /// <see cref="UnityEditor.Editor.OnInspectorGUI"/> 와 같습니다.
        /// </summary>
        protected virtual void OnInspectorGUIContents() { base.OnInspectorGUI(); }

        #region Reflections

        private Dictionary<string, MemberInfo> m_ParsedMemberInfos = new Dictionary<string, MemberInfo>();

        protected FieldInfo GetFieldInfo(string fieldName, BindingFlags bindingFlags)
        {
            if (!m_ParsedMemberInfos.TryGetValue(fieldName, out MemberInfo fieldInfo))
            {
                fieldInfo = TypeHelper.TypeOf<T>.GetFieldInfo(fieldName, bindingFlags);
                m_ParsedMemberInfos.Add(fieldName, fieldInfo);
            }

            return (FieldInfo)fieldInfo;
        }
        protected FieldInfo GetFieldInfo(string fieldName) => GetFieldInfo(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        protected PropertyInfo GetPropertyInfo(string propertyName, BindingFlags bindingFlags)
        {
            if (!m_ParsedMemberInfos.TryGetValue(propertyName, out MemberInfo propertyInfo))
            {
                propertyInfo = TypeHelper.TypeOf<T>.GetPropertyInfo(propertyName, bindingFlags);
                m_ParsedMemberInfos.Add(propertyName, propertyInfo);
            }

            return (PropertyInfo)propertyInfo;
        }
        protected PropertyInfo GetPropertyInfo(string propertyName) => GetPropertyInfo(propertyName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);

        #endregion
    }
}

#endif