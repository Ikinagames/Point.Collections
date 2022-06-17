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
using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEditor.UIElements;
#if UNITY_MATHEMATICS
using Unity.Mathematics;
using UnityEngine.UIElements;
#endif

using VE = UnityEngine.UIElements.VisualElement;

namespace Point.Collections.Editor
{
    public static class CoreGUI
    {
        private static Texture2D s_EmptyIcon;
        public static Texture2D EmptyIcon
        {
            get
            {
                if (s_EmptyIcon == null)
                {
                    Texture2D temp = new Texture2D(1, 1);
                    temp.SetPixel(0, 0, new Color(0, 0, 0, 0));

                    s_EmptyIcon = temp;
                }
                return s_EmptyIcon;
            }
        }

        // https://rito15.github.io/posts/unity-editor-built-in-icons/
        public static Texture2D GetEditorDefaultIcon(string name)
        {
            return EditorGUIUtility.FindTexture(name);
        }
        public static GUIContent GetEditorDefaultIconContent(string name)
        {
            return EditorGUIUtility.IconContent(name);
        }

        #region GUI Styles

        private static GUIStyle s_BoxButtonStyle = null;
        private static readonly Dictionary<TextAnchor, GUIStyle> s_CachedLabelStyles = new Dictionary<TextAnchor, GUIStyle>();

        public static GUIStyle BoxButtonStyle
        {
            get
            {
                if (s_BoxButtonStyle == null)
                {
                    s_BoxButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
                }
                return s_BoxButtonStyle;
            }
        }

        #endregion

        #region Line

        public static void SectorLine(int lines = 1)
        {
            Color old = GUI.backgroundColor;
            GUI.backgroundColor = EditorGUIUtility.isProSkin ? Color.white : Color.grey;

            GUILayout.Space(8);
            GUILayout.Box("", EditorStyleUtilities.SplitStyle, GUILayout.MaxHeight(1.5f));
            GUILayout.Space(2);

            for (int i = 1; i < lines; i++)
            {
                GUILayout.Space(2);
                GUILayout.Box("", EditorStyleUtilities.SplitStyle, GUILayout.MaxHeight(1.5f));
            }

            GUI.backgroundColor = old;
        }
        public static void Line()
        {
            Rect rect = EditorGUILayout.GetControlRect(false, 1f);
            rect.height = 1f;
            rect = EditorGUI.IndentedRect(rect);
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }
        public static void Line(Rect rect)
        {
            rect.height = 1f;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, 1));
        }
        public static void Line(Rect rect, AnimFloat alpha)
        {
            rect.height = 1f;
            EditorGUI.DrawRect(rect, new Color(0.5f, 0.5f, 0.5f, alpha.value));
        }
        public static void SectorLine(float width, int lines = 1)
        {
            Color old = GUI.backgroundColor;
            GUI.backgroundColor = EditorGUIUtility.isProSkin ? Color.white : Color.grey;

            GUILayout.Space(8);
            GUILayout.Box(string.Empty, EditorStyleUtilities.SplitStyle, GUILayout.Width(width), GUILayout.MaxHeight(1.5f));
            GUILayout.Space(2);

            for (int i = 1; i < lines; i++)
            {
                GUILayout.Space(2);
                GUILayout.Box("", EditorStyleUtilities.SplitStyle, GUILayout.MaxHeight(1.5f));
            }

            GUI.backgroundColor = old;
        }

        #endregion

        #region Label

        public static void Label(GUIContent text, StringColor color)
        {
            GUIStyle style = GetLabelStyle(TextAnchor.UpperLeft);

            GUIContent content = new GUIContent(text);
            content.text = HTMLString.String(content.text, color);

            EditorGUILayout.LabelField(content, style);
        }
        public static void Label(GUIContent text, int size)
        {
            GUIStyle style = GetLabelStyle(TextAnchor.UpperLeft);

            GUIContent content = new GUIContent(text);
            content.text = HTMLString.String(content.text, size);

            EditorGUILayout.LabelField(content, style);
        }
        public static void Label(string text, int size)
        {
            GUIStyle style = GetLabelStyle(TextAnchor.UpperLeft);

            text = HTMLString.String(text, size);
            EditorGUILayout.LabelField(text, style);
        }
        public static void Label(GUIContent text, StringColor color, int size)
        {
            GUIStyle style = GetLabelStyle(TextAnchor.UpperLeft);

            GUIContent content = new GUIContent(text);
            content.text = HTMLString.String(content.text, color, size);

            EditorGUILayout.LabelField(content, style);
        }
        public static void Label(GUIContent text, TextAnchor textAnchor = TextAnchor.MiddleLeft, params GUILayoutOption[] options)
        {
            GUIStyle style = GetLabelStyle(textAnchor);

            Rect rect = GUILayoutUtility.GetRect(text, style, options);
            EditorGUI.LabelField(rect, text, style);
        }
        public static void Label(GUIContent text, int size, TextAnchor textAnchor = TextAnchor.MiddleLeft, params GUILayoutOption[] options)
        {
            GUIStyle style = GetLabelStyle(textAnchor);
            GUIContent content = new GUIContent(text);
            content.text = HTMLString.String(text.text, size);

            Rect rect = GUILayoutUtility.GetRect(content, style, options);
            EditorGUI.LabelField(rect, content, style);
        }
        public static void Label(string text, int size, TextAnchor textAnchor = TextAnchor.MiddleLeft, params GUILayoutOption[] options)
        {
            GUIStyle style = GetLabelStyle(textAnchor);
            text = HTMLString.String(text, size);

            EditorGUILayout.LabelField(text, style, options);
        }
        public static void Label(GUIContent text, int size, StringColor color, TextAnchor textAnchor = TextAnchor.MiddleLeft, params GUILayoutOption[] options)
        {
            GUIStyle style = GetLabelStyle(textAnchor);
            GUIContent content = new GUIContent(text);
            content.text = HTMLString.String(text.text, color, size);

            Rect rect = GUILayoutUtility.GetRect(content, style, options);
            EditorGUI.LabelField(rect, content, style);
        }
        public static void Label(GUIContent text1, GUIContent text2, TextAnchor textAnchor = TextAnchor.MiddleLeft, params GUILayoutOption[] options)
        {
            GUIStyle style = GetLabelStyle(textAnchor);

            Rect rect = GUILayoutUtility.GetRect(text2, style, options);

            EditorGUI.LabelField(rect, text1, text2, style);
        }

        public static void Label(Rect rect, GUIContent text, StringColor color)
        {
            GUIStyle style = GetLabelStyle(TextAnchor.UpperLeft);

            GUIContent content = new GUIContent(text);
            content.text = HTMLString.String(content.text, color);

            EditorGUI.LabelField(rect, content, style);
        }
        public static void Label(Rect rect, GUIContent text, int size)
        {
            GUIStyle style = GetLabelStyle(TextAnchor.UpperLeft);

            GUIContent content = new GUIContent(text);
            content.text = HTMLString.String(content.text, size);

            EditorGUI.LabelField(rect, content, style);
        }
        public static void Label(Rect rect, GUIContent text, StringColor color, int size)
        {
            GUIStyle style = GetLabelStyle(TextAnchor.UpperLeft);

            GUIContent content = new GUIContent(text);
            content.text = HTMLString.String(content.text, color, size);

            EditorGUI.LabelField(rect, content, style);
        }
        public static void Label(Rect rect, string text) => Label(rect, new GUIContent(text), TextAnchor.MiddleLeft);
        public static void Label(Rect rect, GUIContent text) => Label(rect, text, TextAnchor.MiddleLeft);
        public static void Label(Rect rect, GUIContent text, TextAnchor textAnchor) => EditorGUI.LabelField(rect, text, GetLabelStyle(textAnchor));
        public static void Label(Rect rect, string text, TextAnchor textAnchor) => EditorGUI.LabelField(rect, text, GetLabelStyle(textAnchor));
        public static void Label(Rect rect, GUIContent text, AnimFloat alpha, TextAnchor textAnchor)
        {
            GUIStyle style = GetLabelStyle(textAnchor);
            Color temp = style.normal.textColor;
            temp.a = alpha.target;
            style.normal.textColor = temp;

            EditorGUI.LabelField(rect, text, style);
        }
        public static void Label(Rect rect, GUIContent text, int size, TextAnchor textAnchor)
        {
            GUIContent temp = new GUIContent(text);
            temp.text = EditorUtilities.String(text.text, size);

            EditorGUI.LabelField(rect, temp, GetLabelStyle(textAnchor));
        }
        public static void Label(Rect rect, string text, int size, TextAnchor textAnchor)
        {
            EditorGUI.LabelField(rect, EditorUtilities.String(text, size), GetLabelStyle(textAnchor));
        }
        public static void Label(Rect rect, GUIContent text1, GUIContent text2, TextAnchor textAnchor)
        {
            GUIStyle style = GetLabelStyle(textAnchor);

            EditorGUI.LabelField(rect, text1, text2, style);
        }

        public static GUIStyle GetLabelStyle(TextAnchor textAnchor)
        {
            if (!s_CachedLabelStyles.TryGetValue(textAnchor, out var style))
            {
                style = new GUIStyle(EditorStyles.label)
                {
                    alignment = textAnchor,
                    richText = true
                };
                s_CachedLabelStyles.Add(textAnchor, style);
            }

            return style;
        }

        #endregion

        #region Button

        public static bool LabelButton(string text, TextAnchor textAnchor = TextAnchor.UpperLeft)
        {
            return GUILayout.Button(text, GetLabelStyle(textAnchor));
        }
        public static bool LabelButton(GUIContent content, TextAnchor textAnchor = TextAnchor.UpperLeft)
        {
            return GUILayout.Button(content, GetLabelStyle(textAnchor));
        }
        public static bool LabelButton(GUIContent text, int size, TextAnchor textAnchor)
        {
            GUIContent temp = new GUIContent(text);
            temp.text = EditorUtilities.String(text.text, size);

            return GUILayout.Button(temp, GetLabelStyle(textAnchor));
        }
        public static bool LabelButton(Rect rect, GUIContent text, int size, TextAnchor textAnchor)
        {
            GUIContent temp = new GUIContent(text);
            temp.text = EditorUtilities.String(text.text, size);

            return GUI.Button(rect, temp, GetLabelStyle(textAnchor));
        }

        public static bool BoxButton(string content, Color color, params GUILayoutOption[] options) => BoxButton(content, color, null, options);
        public static bool BoxButton(string content, Color color, Action onContextClick, params GUILayoutOption[] options)
        {
            GUIContent label = new GUIContent(content);
            Rect rect = GUILayoutUtility.GetRect(label, BoxButtonStyle, options);

            return BoxButton(rect, label, color, onContextClick);
        }
        public static bool BoxButton(Rect rect, string content, Color color) => BoxButton(rect, new GUIContent(content), color, null);
        public static bool BoxButton(Rect rect, string content, Color color, Action onContextClick) => BoxButton(rect, new GUIContent(content), color, onContextClick);
        public static bool BoxButton(Rect rect, GUIContent content, Color color, Action onContextClick)
        {
            int enableCullID = GUIUtility.GetControlID(FocusType.Passive, rect);

            bool clicked = false;
            switch (Event.current.GetTypeForControl(enableCullID))
            {
                case EventType.Repaint:
                    bool isHover = rect.Contains(Event.current.mousePosition);

                    Color origin = GUI.color;
                    GUI.color = Color.Lerp(color, Color.white, isHover && GUI.enabled ? .7f : 0);
                    BoxButtonStyle.Draw(rect,
                        isHover, isActive: true, on: true, false);
                    GUI.color = origin;

                    GetLabelStyle(TextAnchor.MiddleCenter).Draw(rect, content, enableCullID);
                    break;
                case EventType.ContextClick:
                    if (!GUI.enabled || !rect.Contains(Event.current.mousePosition)) break;

                    onContextClick?.Invoke();
                    Event.current.Use();

                    break;
                case EventType.MouseDown:
                    if (!GUI.enabled || !rect.Contains(Event.current.mousePosition)) break;

                    if (Event.current.button == 0)
                    {
                        GUIUtility.hotControl = enableCullID;
                        clicked = true;
                        GUI.changed = true;
                        Event.current.Use();
                    }

                    break;
                case EventType.MouseUp:
                    if (!GUI.enabled || !rect.Contains(Event.current.mousePosition)) break;

                    var drag = DragAndDrop.GetGenericData("GenericDragColumnDragging");
                    if (drag != null)
                    {
                        Debug.Log($"in {drag.GetType().Name}");
                    }

                    if (GUIUtility.hotControl == enableCullID)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    break;
                default:
                    break;
            }

            return clicked;
        }

        #endregion

        #region Toggle

        static class ToggleHelper
        {
            public static GUIContent
                FoldoutOpenedContent = new GUIContent(EditorStyleUtilities.FoldoutOpendString),
                FoldoutClosedContent = new GUIContent(EditorStyleUtilities.FoldoutClosedString);
        }

        public static bool LabelToggle(Rect rect, bool value, string text)
        {
            GUIContent temp = new GUIContent(text);

            return GUI.Toggle(rect, value, temp, GetLabelStyle(TextAnchor.MiddleLeft));
        }
        public static bool LabelToggle(bool value, string text, int size, TextAnchor textAnchor)
        {
            text = HTMLString.String(text, size);
            //return GUILayout.Toggle(value, 
            //    (value ? EditorStyleUtilities.FoldoutOpendString : EditorStyleUtilities.FoldoutClosedString) +
            //    text, 
            return GUILayout.Toggle(value, text, 
                
                GetLabelStyle(textAnchor));
        }
        public static bool LabelToggle(bool value, GUIContent text, int size, TextAnchor textAnchor)
        {
            GUIContent temp = new GUIContent(text);
            temp.text = EditorUtilities.String(text.text, size);

            return GUILayout.Toggle(value, temp, GetLabelStyle(textAnchor));
        }
        public static bool LabelToggle(Rect rect, bool value, GUIContent text, int size, TextAnchor textAnchor)
        {
            GUIContent temp = new GUIContent(text);
            temp.text = EditorUtilities.String(text.text, size);

            return GUI.Toggle(rect, value, temp, GetLabelStyle(textAnchor));
        }

        public static bool BoxToggleButton(
            bool value, string content, Color enableColor, Color disableColor, params GUILayoutOption[] options)
        {
            GUIContent label = new GUIContent(content);
            Rect rect = GUILayoutUtility.GetRect(label, BoxButtonStyle, options);

            return BoxToggleButton(rect, value, label, enableColor, disableColor);
        }
        public static bool BoxToggleButton(
            bool value, Color enableColor, Color disableColor, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(
                value ? ToggleHelper.FoldoutOpenedContent : ToggleHelper.FoldoutClosedContent,
                BoxButtonStyle, options);

            return BoxToggleButton(rect, value, enableColor, disableColor);
        }
        public static bool BoxToggleButton(
            bool value, GUIContent content, Color enableColor, Color disableColor, params GUILayoutOption[] options)
        {
            Rect rect = GUILayoutUtility.GetRect(content, BoxButtonStyle, options);
            return BoxToggleButton(rect, value, content, enableColor, disableColor);
        }
        public static bool BoxToggleButton(
            Rect rect, bool value, Color enableColor, Color disableColor)
            => BoxToggleButton(rect, value, value ? ToggleHelper.FoldoutOpenedContent : ToggleHelper.FoldoutClosedContent, enableColor, disableColor);
        public static bool BoxToggleButton(
            Rect rect, bool value, GUIContent content, Color enableColor, Color disableColor)
        {
            int enableCullID = GUIUtility.GetControlID(FocusType.Passive, rect);

            switch (Event.current.GetTypeForControl(enableCullID))
            {
                case EventType.Repaint:
                    bool isHover = rect.Contains(Event.current.mousePosition);

                    Color origin = GUI.backgroundColor;
                    GUI.backgroundColor = value ? enableColor : disableColor;
                    GUI.backgroundColor = Color.Lerp(GUI.backgroundColor, Color.white, isHover && GUI.enabled ? .7f : 0);
                    BoxButtonStyle.Draw(rect,
                        isHover, isActive: true, on: true, false);
                    GUI.backgroundColor = origin;

                    var temp = new GUIStyle(EditorStyles.label);
                    temp.alignment = TextAnchor.MiddleCenter;
                    GetLabelStyle(TextAnchor.MiddleCenter).Draw(rect, content, enableCullID);
                    break;
                case EventType.MouseDown:
                    if (!GUI.enabled) break;
                    else if (!rect.Contains(Event.current.mousePosition)) break;

                    if (Event.current.button == 0)
                    {
                        GUIUtility.hotControl = enableCullID;
                        value = !value;
                        GUI.changed = true;
                        Event.current.Use();
                    }

                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == enableCullID)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    break;
                default:
                    break;
            }

            return value;
        }

        #endregion

        #region Min-Max Slider

        public static float2 MinMaxSlider(Rect position, string label, ref float minValue, ref float maxValue, float minLimit, float maxLimit)
        {
            const float c_Width = 80;

            position.width -= c_Width;
            EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, minLimit, maxLimit);

            var tempRect = position;
            tempRect.x += position.width - 10f;
            tempRect.width = (c_Width * .5f) + 5f;

            minValue = EditorGUI.DelayedFloatField(tempRect, GUIContent.none, minValue, EditorStyles.textField);
            tempRect.x += (c_Width * .5f) - 2.5f;
            maxValue = EditorGUI.DelayedFloatField(tempRect, GUIContent.none, maxValue, EditorStyles.textField);

            return new float2(minValue, maxValue);
        }
        public static float2 MinMaxSlider(Rect position, GUIContent label, ref float minValue, ref float maxValue, float minLimit, float maxLimit)
        {
            const float c_Width = 80;

            position.width -= c_Width;
            EditorGUI.MinMaxSlider(position, label, ref minValue, ref maxValue, minLimit, maxLimit);

            var tempRect = position;
            tempRect.x += position.width - 10f;
            tempRect.width = (c_Width * .5f) + 5f;

            minValue = EditorGUI.DelayedFloatField(tempRect, GUIContent.none, minValue, EditorStyles.textField);
            tempRect.x += (c_Width * .5f) - 2.5f;
            maxValue = EditorGUI.DelayedFloatField(tempRect, GUIContent.none, maxValue, EditorStyles.textField);

            return new float2(minValue, maxValue);
        }

        public static void MinMaxSlider(Rect position, string label, SerializedProperty minValue, SerializedProperty maxValue, float minLimit, float maxLimit)
        {
            position.width -= 50;
            float tempMin = minValue.floatValue, tempMax = maxValue.floatValue;
            EditorGUI.MinMaxSlider(position, label, ref tempMin, ref tempMax, minLimit, maxLimit);

            var tempRect = position;
            tempRect.x += position.width + .75f;
            tempRect.width = 25 - 1.5f;

            minValue.floatValue = tempMin;
            maxValue.floatValue = tempMax;

            EditorGUI.PropertyField(tempRect, minValue, GUIContent.none, true);
            tempRect.x += 1.5f + 25;
            EditorGUI.PropertyField(tempRect, maxValue, GUIContent.none, true);
        }
        public static void MinMaxSlider(Rect position, GUIContent label, SerializedProperty minValue, SerializedProperty maxValue, float minLimit, float maxLimit)
        {
            position.width -= 50;
            float tempMin = minValue.floatValue, tempMax = maxValue.floatValue;
            EditorGUI.MinMaxSlider(position, label, ref tempMin, ref tempMax, minLimit, maxLimit);

            var tempRect = position;
            tempRect.x += position.width + .75f;
            tempRect.width = 25 - 1.5f;

            minValue.floatValue = tempMin;
            maxValue.floatValue = tempMax;

            EditorGUI.PropertyField(tempRect, minValue, GUIContent.none, true);
            tempRect.x += 1.5f + 25;
            EditorGUI.PropertyField(tempRect, maxValue, GUIContent.none, true);
        }

        #endregion

        #region Slider

        public static float Slider(Rect position, string label, float value, float minLimit, float maxLimit)
        {
            value = EditorGUI.Slider(position, label, value, minLimit, maxLimit);

            return value;
        }
        public static float Slider(Rect position, GUIContent label, float value, float minLimit, float maxLimit)
        {
            value = EditorGUI.Slider(position, label, value, minLimit, maxLimit);

            return value;
        }

        #endregion

        #region Draw

        public sealed class BoxBlock : IDisposable
        {
            Color m_PrevColor;
            int m_PrevIndent;

            GUILayout.HorizontalScope m_HorizontalScope;
            GUILayout.VerticalScope m_VerticalScope;

            public BoxBlock(Color color, params GUILayoutOption[] options)
            {
                m_PrevColor = GUI.backgroundColor;
                m_PrevIndent = EditorGUI.indentLevel;

                EditorGUI.indentLevel = 0;

                m_HorizontalScope = new GUILayout.HorizontalScope();
                GUILayout.Space(m_PrevIndent * 15);
                GUI.backgroundColor = color;

                m_VerticalScope = new GUILayout.VerticalScope(EditorStyleUtilities.Box, options);
                GUI.backgroundColor = m_PrevColor;
            }
            public void Dispose()
            {
                m_VerticalScope.Dispose();
                m_HorizontalScope.Dispose();

                m_VerticalScope = null;
                m_HorizontalScope = null;

                EditorGUI.indentLevel = m_PrevIndent;
                GUI.backgroundColor = m_PrevColor;
            }
        }

        public static void DrawBlock(Rect rect, Color color)
        {
            color.a = .25f;

            GUI.Box(rect, GUIContent.none, EditorStyles.helpBox);
            EditorGUI.DrawRect(rect, color);
        }
        public static void DrawRect(Rect rect, Color color)
        {
            color.a = .25f;

            EditorGUI.DrawRect(rect, color);
        }

        #endregion

        #region Utils

        public static float GetLineHeight(int lineCount)
        {
            float height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
            return height * lineCount;
        }

        public static object AutoField(Type type, string label, object value, params GUILayoutOption[] options)
        {
            if (type == TypeHelper.TypeOf<int>.Type)
            {
                return EditorGUILayout.IntField(label, Convert.ToInt32(value), options);
            }
            else if (type == TypeHelper.TypeOf<float>.Type)
            {
                return EditorGUILayout.FloatField(label, Convert.ToSingle(value), options);
            }
            else if (type == TypeHelper.TypeOf<bool>.Type)
            {
                return EditorGUILayout.ToggleLeft(label, Convert.ToBoolean(value), options);
            }
            else if (type == TypeHelper.TypeOf<string>.Type)
            {
                return EditorGUILayout.TextField(label, Convert.ToString(value), options);
            }
            //else if (fieldInfo.FieldType == TypeHelper.TypeOf<float3>.Type)
            //{
            //    return EditorGUILayout.Vector3Field(label, (float3)(value), options);
            //}
            else if (type == TypeHelper.TypeOf<Vector3>.Type)
            {
                return EditorGUILayout.Vector3Field(label, (Vector3)(value), options);
            }

            throw new NotImplementedException();
        }
        public static object AutoField(Rect rect, Type type, string label, object value)
        {
            if (type == TypeHelper.TypeOf<int>.Type)
            {
                return EditorGUI.IntField(rect, label, Convert.ToInt32(value));
            }
            else if (type == TypeHelper.TypeOf<float>.Type)
            {
                return EditorGUI.FloatField(rect, label, Convert.ToSingle(value));
            }
            else if (type == TypeHelper.TypeOf<bool>.Type)
            {
                return EditorGUI.ToggleLeft(rect, label, Convert.ToBoolean(value));
            }
            else if (type == TypeHelper.TypeOf<string>.Type)
            {
                return EditorGUI.TextField(rect, label, Convert.ToString(value));
            }
            //else if (fieldInfo.FieldType == TypeHelper.TypeOf<float3>.Type)
            //{
            //    return EditorGUILayout.Vector3Field(label, (float3)(value), options);
            //}
            else if (type == TypeHelper.TypeOf<Vector3>.Type)
            {
                return EditorGUI.Vector3Field(rect, label, (Vector3)(value));
            }

            throw new NotImplementedException();
        }

        #endregion

        #region Visual Element

        public struct VisualElement
        {
            private static StyleSheet s_DefaultStyleSheet;
            public static StyleSheet DefaultStyleSheet
            {
                get
                {
                    if (s_DefaultStyleSheet == null)
                    {
                        s_DefaultStyleSheet = AssetHelper.LoadAsset<StyleSheet>("default-uss", "PointEditor");
                    }
                    return s_DefaultStyleSheet;
                }
            }

            public static VE Space(float value, LengthUnit lengthUnit)
            {
                VE v = new VE();
                v.styleSheets.Add(DefaultStyleSheet);

                v.style.height = new StyleLength(new Length(value, lengthUnit));

                return v;
            }
            public static VE Space() => Space(14, LengthUnit.Pixel);

            public static PropertyField PropertyField(SerializedProperty property)
            {
                PropertyField field = new PropertyField(property);
                field.BindProperty(property);

                return field;
            }

            public static Label Label(string text, float size)
            {
                Label v = new Label(text);
                v.styleSheets.Add(DefaultStyleSheet);

                v.style.fontSize = size;

                return v;
            }
            public static Label Label(string text)
            {
                Label v = new Label(text);
                v.styleSheets.Add(DefaultStyleSheet);

                return v;
            }
        }

        public static void Hide(this IStyle t, bool hide)
        {
            if (hide)
            {
                t.display = DisplayStyle.None;
                t.visibility = Visibility.Hidden;
                return;
            }

            t.display = DisplayStyle.Flex;
            t.visibility = Visibility.Visible;
        }

        public static void SetMargin(this IStyle t, StyleLength value)
        {
            t.marginLeft = value;
            t.marginRight = value;
            t.marginTop = value;
            t.marginBottom = value;
        }
        public static void SetPadding(this IStyle t, StyleLength value)
        {
            t.paddingLeft = value;
            t.paddingRight = value;
            t.paddingTop = value;
            t.paddingBottom = value;
        }
        public static void SetBorderWidth(this IStyle t, StyleFloat value)
        {
            t.borderRightWidth = value;
            t.borderLeftWidth = value;
            t.borderTopWidth = value;
            t.borderBottomWidth = value;
        }
        public static void SetBorderColor(this IStyle t, StyleColor color)
        {
            t.borderRightColor = color;
            t.borderLeftColor = color;
            t.borderTopColor = color;
            t.borderBottomColor = color;
        }
        public static void SetBorderRadius(this IStyle t, StyleLength value)
        {
            t.borderTopLeftRadius = value;
            t.borderTopRightRadius = value;
            t.borderBottomLeftRadius = value;
            t.borderBottomRightRadius = value;
        }

        #endregion

        public struct EditorWindow
        {
            public enum WindowType
            {
                InspectorWindow,
                ConsoleWindow,
                SceneViewWindow,
            }

            private static Type GetTypeFrom(WindowType type)
            {
                Type t = null;
                if (type == WindowType.InspectorWindow)
                {
                    t = Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
                }
                else if (type == WindowType.ConsoleWindow)
                {
                    t = Type.GetType("UnityEditor.ConsoleWindow,UnityEditor.dll");
                }
                else if (type == WindowType.SceneViewWindow)
                {
                    t = TypeHelper.TypeOf<SceneView>.Type;
                }

                return t;
            }
            public static T OpenWindowSafe<T>(string title, WindowType desireDocking)
                where T : UnityEditor.EditorWindow
            {
                Type type = GetTypeFrom(desireDocking);
                var window = UnityEditor.EditorWindow.GetWindow<T>(title, type);

                try
                {
                    var pos = window.position;
                    window.position = pos;
                }
                catch (Exception)
                {
                    UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(TypeHelper.TypeOf<PointSetupWizard>.Type);
                    foreach (var item in array)
                    {
                        UnityEngine.Object.DestroyImmediate(item);
                    }

                    return OpenWindowSafe<T>(title, desireDocking);
                }

                return window;
            }
            public static T OpenWindowSafe<T>(string title, bool utility)
                where T : UnityEditor.EditorWindow
            {
                var window = (T)UnityEditor.EditorWindow.GetWindow(TypeHelper.TypeOf<T>.Type, utility, title);

                try
                {
                    var pos = window.position;
                    window.position = pos;
                }
                catch (Exception)
                {
                    UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(TypeHelper.TypeOf<PointSetupWizard>.Type);
                    foreach (var item in array)
                    {
                        UnityEngine.Object.DestroyImmediate(item);
                    }

                    return OpenWindowSafe<T>(title, utility);
                }

                return window;
            }
            public static T OpenWindowAtCenterSafe<T>(string title, bool utility, Vector2 size)
                where T : UnityEditor.EditorWindow
            {
                var window = (T)UnityEditor.EditorWindow.GetWindow(TypeHelper.TypeOf<T>.Type, utility, title);

                window.minSize = window.maxSize = size;
                var position = new Rect(Vector2.zero, window.minSize);
                Vector2 screenCenter = new Vector2(Screen.currentResolution.width, Screen.currentResolution.height) / 2;
                position.center = screenCenter / EditorGUIUtility.pixelsPerPoint;

                try
                {
                    window.position = position;
                }
                catch (Exception)
                {
                    UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(TypeHelper.TypeOf<PointSetupWizard>.Type);
                    foreach (var item in array)
                    {
                        UnityEngine.Object.DestroyImmediate(item);
                    }

                    return OpenWindowAtCenterSafe<T>(title, utility, size);
                }

                return window;
            }
        }
    }
}

#endif