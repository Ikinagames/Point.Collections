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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    /// <inheritdoc cref="PropertyDrawer"/>
    /// <typeparam name="T">목표 타입</typeparam>
    public abstract class PropertyDrawer<T> : PropertyDrawer
    {
        protected virtual bool EnableHeightAnimation => false;
        protected virtual float HeightAnimationSpeed => 8;

        private bool m_Initialized = false;

        private float m_AutoHeight = 0;
        private AnimFloat m_Height;

        public override sealed VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            return base.CreatePropertyGUI(property);
        }
        public override sealed float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = PropertyHeight(property, label);

            height += m_AutoHeight;

            if (EnableHeightAnimation)
            {
                if (m_Height == null)
                {
                    m_Height = new AnimFloat(height);
                    m_Height.speed = HeightAnimationSpeed;
                }

                m_Height.target = height;

                return m_Height.value;
            }

            return height;
        }
        public override sealed void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!m_Initialized)
            {
                OnInitialize(property);
                OnInitialize(property, label);
                m_Initialized = true;
            }

            AutoRect rect = new AutoRect(position);
            if (Event.current.type == EventType.Layout)
            {
                m_AutoHeight = 0;
            }

            BeforePropertyGUI(ref rect, property, label);

            bool notEditable = false;
            foreach (var att in fieldInfo.GetCustomAttributes())
            {
                if (att is NotEditableAttribute) notEditable = true;
                //else if (att is SpaceAttribute space)
                //{
                //    rect.Pop(space.height == 0 ? EditorGUIUtility.standardVerticalSpacing : space.height);
                //}
            }

            using (new EditorGUI.DisabledGroupScope(notEditable))
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                OnPropertyGUI(ref rect, property, label);
            }
        }

        /// <summary>
        /// 이 Propery 의 높이를 결정합니다.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        protected virtual float PropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
        /// <summary>
        /// 이 <see cref="PropertyDrawer"/>(<typeparamref name="T"/>) 의 인스턴스 객체가 한번만 실행하는 함수입니다.
        /// </summary>
        /// <param name="property"></param>
        protected virtual void OnInitialize(SerializedProperty property) { }
        /// <summary><inheritdoc cref="OnInitialize(SerializedProperty)"/></summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected virtual void OnInitialize(SerializedProperty property, GUIContent label) { }

        /// <summary>
        /// <see cref="EditorGUI.PropertyScope"/> 진입 전 실행되는 함수입니다.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected virtual void BeforePropertyGUI(ref AutoRect rect, SerializedProperty property, GUIContent label) { }
        /// <summary>
        /// <see cref="EditorGUI.PropertyScope"/> 내에서 실행되는 함수입니다.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected virtual void OnPropertyGUI(ref AutoRect rect, SerializedProperty property, GUIContent label) { }

        #region GUI

        protected void AddAutoHeight(float height)
        {
            if (Event.current.type == EventType.Layout)
            {
                m_AutoHeight += height;
            }
        }

        #region Line

        protected void Line(ref AutoRect rect)
        {
            AddAutoHeight(6);

            rect.Pop(3);
            CoreGUI.Line(EditorGUI.IndentedRect(rect.Pop(3)));
        }
        protected void Line(ref AutoRect rect, AnimFloat alpha)
        {
            AddAutoHeight(6);

            rect.Pop(3);
            CoreGUI.Line(EditorGUI.IndentedRect(rect.Pop(3)), alpha);
        }

        #endregion

        protected void Space(ref AutoRect rect, float pixel)
        {
            AddAutoHeight(pixel);

            rect.Pop(pixel);
        }

        protected void Label(ref AutoRect rect, string content)
        {
            AddAutoHeight(CoreGUI.GetLineHeight(1));

            CoreGUI.Label(rect.Pop(CoreGUI.GetLineHeight(1)), content);
        }
        protected string TextField(ref AutoRect rect, string text)
        {
            return TextField(ref rect, GUIContent.none, text);
        }
        protected string TextField(ref AutoRect rect, string label, string text)
        {
            GUIContent content = new GUIContent(label);
            return TextField(ref rect, content, text);
        }
        protected string TextField(ref AutoRect rect, GUIContent content, string text)
        {
            float height = EditorGUI.GetPropertyHeight(SerializedPropertyType.String, content);
            AddAutoHeight(height);

            return EditorGUI.TextField(rect.Pop(height), content, text);
        }

        #region PropertyField

        protected void PropertyField(ref AutoRect rect, SerializedProperty property)
        {
            float height = EditorGUI.GetPropertyHeight(property);
            AddAutoHeight(height);

            EditorGUI.PropertyField(rect.Pop(height), property);
        }
        protected void PropertyField(ref AutoRect rect, SerializedProperty property, bool includeChildren)
        {
            float height = EditorGUI.GetPropertyHeight(property, includeChildren);
            AddAutoHeight(height);

            EditorGUI.PropertyField(rect.Pop(height), property, includeChildren);
        }
        protected void PropertyField(ref AutoRect rect, SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(property, label);
            AddAutoHeight(height);

            EditorGUI.PropertyField(rect.Pop(height), property, label);
        }
        protected void PropertyField(ref AutoRect rect, SerializedProperty property, GUIContent label, bool includeChildren)
        {
            float height = EditorGUI.GetPropertyHeight(property, label, includeChildren);
            AddAutoHeight(height);

            EditorGUI.PropertyField(rect.Pop(height), property, label, includeChildren);
        }

        #endregion

        #region Button

        protected bool Button(ref AutoRect rect, string text)
        {
            float height = CoreGUI.GetLineHeight(1);
            AddAutoHeight(height);

            Rect pos = rect.Pop(height);
            return GUI.Button(EditorGUI.IndentedRect(pos), text);
        }
        protected bool Button(ref AutoRect rect, string text, float height)
        {
            AddAutoHeight(height);

            Rect pos = rect.Pop(height);
            return GUI.Button(EditorGUI.IndentedRect(pos), text);
        }

        #endregion

        #region Toggle

        protected bool LabelToggle(ref AutoRect rect, bool value, GUIContent content, int size, TextAnchor textAnchor)
        {
            Rect pos = rect.Pop(GetLabelToggleHeight(in rect, content, size, textAnchor));
            AddAutoHeight(pos.height);

            return CoreGUI.LabelToggle(EditorGUI.IndentedRect(pos), value, content, size, textAnchor);
        }
        protected bool LabelToggle(ref AutoRect rect, bool value, GUIContent content, int size, TextAnchor textAnchor, out Rect pos)
        {
            pos = rect.Pop(GetLabelToggleHeight(in rect, content, size, textAnchor));
            AddAutoHeight(pos.height);

            return CoreGUI.LabelToggle(EditorGUI.IndentedRect(pos), value, content, size, textAnchor);
        }
        protected float GetLabelToggleHeight(in AutoRect rect, GUIContent content, int size, TextAnchor textAnchor)
        {
            GUIStyle style = CoreGUI.GetLabelStyle(textAnchor);
            float width = EditorGUI.IndentedRect(rect.Current).width;

            GUIContent temp = new GUIContent(content);
            temp.text = HTMLString.String(content.text, size);

            return style.CalcHeight(temp, width);
        }

        #endregion

        #endregion

        #region Utils

        [Obsolete]
        protected List<Rect> GetValueRect(Rect rawRect, GUIStyle style, params string[] names)
        {
            List<Rect> rects = EditorGUIUtility.GetFlowLayoutedRects(rawRect, style, EditorGUIUtility.standardVerticalSpacing, EditorGUIUtility.standardVerticalSpacing, names.ToList());

            return rects;
        }

        /// <summary>
        /// <paramref name="BaseObject"/> 가 그려지고 있는 <see cref="Editor"/> 를 다시 그리도록 요청합니다.
        /// </summary>
        /// <param name="BaseObject"></param>
        public static void RepaintInspector(SerializedObject BaseObject)
        {
            foreach (var item in ActiveEditorTracker.sharedTracker.activeEditors)
                if (item.serializedObject == BaseObject)
                { item.Repaint(); return; }
        }

        #endregion

        #region Base Sealed

        public override sealed bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override sealed int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override sealed string ToString()
        {
            return base.ToString();
        }

        #endregion
    }
    /// <summary>
    /// <inheritdoc cref="PropertyDrawer"/>
    /// </summary>
    /// <remarks>
    /// UXML 전용
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public abstract class PropertyDrawerUXML<T> : PropertyDrawer
    {
        #region Fallback IMGUI

        private bool m_Initialized = false;
        private float m_AutoHeight = 0;

        public override sealed float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float height = PropertyHeight(property, label);

            height += m_AutoHeight;

            return height;
        }
        public override sealed void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!m_Initialized)
            {
                OnInitialize(property);
                OnInitialize(property, label);
                m_Initialized = true;
            }

            AutoRect rect = new AutoRect(position);
            if (Event.current.type == EventType.Layout)
            {
                m_AutoHeight = 0;
            }

            BeforePropertyGUI(ref rect, property, label);

            bool notEditable = false;
            foreach (var att in fieldInfo.GetCustomAttributes())
            {
                if (att is NotEditableAttribute) notEditable = true;
                //else if (att is SpaceAttribute space)
                //{
                //    rect.Pop(space.height == 0 ? EditorGUIUtility.standardVerticalSpacing : space.height);
                //}
            }

            using (new EditorGUI.DisabledGroupScope(notEditable))
            using (new EditorGUI.PropertyScope(position, label, property))
            {
                OnPropertyGUI(ref rect, property, label);
            }
        }

        /// <summary>
        /// 이 Propery 의 높이를 결정합니다.
        /// </summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        protected virtual float PropertyHeight(SerializedProperty property, GUIContent label)
        {
            return base.GetPropertyHeight(property, label);
        }
        /// <summary>
        /// 이 <see cref="PropertyDrawer"/>(<typeparamref name="T"/>) 의 인스턴스 객체가 한번만 실행하는 함수입니다.
        /// </summary>
        /// <param name="property"></param>
        protected virtual void OnInitialize(SerializedProperty property) { }
        /// <summary><inheritdoc cref="OnInitialize(SerializedProperty)"/></summary>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected virtual void OnInitialize(SerializedProperty property, GUIContent label) { }

        /// <summary>
        /// <see cref="EditorGUI.PropertyScope"/> 진입 전 실행되는 함수입니다.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected virtual void BeforePropertyGUI(ref AutoRect rect, SerializedProperty property, GUIContent label) { }
        /// <summary>
        /// <see cref="EditorGUI.PropertyScope"/> 내에서 실행되는 함수입니다.
        /// </summary>
        /// <param name="rect"></param>
        /// <param name="property"></param>
        /// <param name="label"></param>
        protected virtual void OnPropertyGUI(ref AutoRect rect, SerializedProperty property, GUIContent label)
        {
            float height = EditorGUI.GetPropertyHeight(property, label, false);
            property.isExpanded = EditorGUI.Foldout(rect.Pop(height), property.isExpanded, label, true);
            if (Event.current.type == EventType.Layout)
            {
                m_AutoHeight += height;
            }

            if (!property.isExpanded) return;

            rect.Indent(10);
            float boxHeight = 0;
            foreach (var item in property.ForEachVisibleChild())
            {
                height = EditorGUI.GetPropertyHeight(item);

                if (Event.current.type == EventType.Layout)
                {
                    m_AutoHeight += height;
                    boxHeight += height;
                }

                EditorGUI.PropertyField(rect.Pop(height), item);
            }
            rect.Indent(-10);
        }

        #endregion

        public override sealed VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            var ve = CreateVisualElement(property);
            SetupVisualElement(property, ve);

            return ve;
        }

        protected virtual VisualElement CreateVisualElement(SerializedProperty property)
        {
            Foldout root = new Foldout();
            root.text = property.displayName;

            foreach (var item in property.ForEachVisibleChild())
            {
                PropertyField field = new PropertyField(item);
                root.contentContainer.Add(field);
            }

            return root;
        }
        protected virtual void SetupVisualElement(SerializedProperty property, VisualElement root) { }

        #region Base Sealed

        public override sealed bool Equals(object obj)
        {
            return base.Equals(obj);
        }
        public override sealed int GetHashCode()
        {
            return base.GetHashCode();
        }
        public override sealed string ToString()
        {
            return base.ToString();
        }

        #endregion
    }
}

#endif