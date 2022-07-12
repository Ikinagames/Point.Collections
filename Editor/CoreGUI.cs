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
#endif
using UnityEngine.UIElements;

using VE = UnityEngine.UIElements.VisualElement;
using System.Reflection;

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
        public static GUIContent EmptyContent => new GUIContent("None", EmptyIcon);

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

        #region Unity Internal

        internal static class ScrollWaitDefinitions
        {
            public const int firstWait = 250;

            public const int regularWait = 30;
        }
        internal static class GUIext
        {
            private const float s_ScrollStepSize = 10f;
            private static int s_ScrollControlId;

            private static int s_HotTextField = -1;

            private static readonly int s_BoxHash = "Box".GetHashCode();
            private static readonly int s_ButonHash = "Button".GetHashCode();
            private static readonly int s_RepeatButtonHash = "repeatButton".GetHashCode();
            private static readonly int s_ToggleHash = "Toggle".GetHashCode();
            private static readonly int s_ButtonGridHash = "ButtonGrid".GetHashCode();
            private static readonly int s_SliderHash = "Slider".GetHashCode();
            private static readonly int s_BeginGroupHash = "BeginGroup".GetHashCode();
            private static readonly int s_ScrollviewHash = "scrollView".GetHashCode();

            internal static DateTime nextScrollStepTime { get; set; }

            static GUIext()
            {
                nextScrollStepTime = DateTime.Now; // whatever but null
            }

            private static void InternalRepaintEditorWindow()
            {
                MethodInfo method = TypeHelper.TypeOf<UnityEngine.GUI>
                    .Type.GetMethod("InternalRepaintEditorWindow", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                method.Invoke(null, null);
            }
            private static void CheckOnGUI()
            {
                MethodInfo method = TypeHelper.TypeOf<GUIUtility>
                    .Type.GetMethod("CheckOnGUI", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                method.Invoke(null, null);
            }
            private static bool DoRepeatButton(Rect position, GUIContent content, GUIStyle style, FocusType focusType)
            {
                CheckOnGUI();
                int id = GUIUtility.GetControlID(s_RepeatButtonHash, focusType, position);
                switch (Event.current.GetTypeForControl(id))
                {
                    case EventType.MouseDown:
                        // If the mouse is inside the button, we say that we're the hot control
                        if (position.Contains(Event.current.mousePosition))
                        {
                            GUIUtility.hotControl = id;
                            Event.current.Use();
                        }
                        return false;
                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == id)
                        {
                            GUIUtility.hotControl = 0;

                            // If we got the mousedown, the mouseup is ours as well
                            // (no matter if the click was in the button or not)
                            Event.current.Use();

                            // But we only return true if the button was actually clicked
                            return position.Contains(Event.current.mousePosition);
                        }
                        return false;
                    case EventType.Repaint:
                        style.Draw(position, content, id, false, position.Contains(Event.current.mousePosition));
                        return id == GUIUtility.hotControl && position.Contains(Event.current.mousePosition);
                }
                return false;
            }
            internal static bool ScrollerRepeatButton(int scrollerID, Rect rect, GUIStyle style)
            {
                bool hasChanged = false;
                if (DoRepeatButton(rect, GUIContent.none, style, FocusType.Passive))
                {
                    bool firstClick = s_ScrollControlId != scrollerID;
                    s_ScrollControlId = scrollerID;

                    if (firstClick)
                    {
                        hasChanged = true;
                        nextScrollStepTime = DateTime.Now.AddMilliseconds(ScrollWaitDefinitions.firstWait);
                    }
                    else
                    {
                        if (DateTime.Now >= nextScrollStepTime)
                        {
                            hasChanged = true;
                            nextScrollStepTime = DateTime.Now.AddMilliseconds(ScrollWaitDefinitions.regularWait);
                        }
                    }

                    if (Event.current.type == EventType.Repaint)
                        InternalRepaintEditorWindow();
                }

                return hasChanged;
            }
            internal static float Scroller(Rect position, float value, float size, float leftValue, float rightValue, GUIStyle slider, GUIStyle thumb, GUIStyle leftButton, GUIStyle rightButton, bool horiz)
            {
                CheckOnGUI();
                int id = GUIUtility.GetControlID(s_SliderHash, FocusType.Passive, position);

                Rect sliderRect, minRect, maxRect;

                if (horiz)
                {
                    sliderRect = new Rect(
                        position.x + leftButton.fixedWidth, position.y,
                        position.width - leftButton.fixedWidth - rightButton.fixedWidth, position.height
                    );
                    minRect = new Rect(position.x, position.y, leftButton.fixedWidth, position.height);
                    maxRect = new Rect(position.xMax - rightButton.fixedWidth, position.y, rightButton.fixedWidth, position.height);
                }
                else
                {
                    sliderRect = new Rect(
                        position.x, position.y + leftButton.fixedHeight,
                        position.width, position.height - leftButton.fixedHeight - rightButton.fixedHeight
                    );
                    minRect = new Rect(position.x, position.y, position.width, leftButton.fixedHeight);
                    maxRect = new Rect(position.x, position.yMax - rightButton.fixedHeight, position.width, rightButton.fixedHeight);
                }

                value = UnityEngine.GUI.Slider(sliderRect, value, size, leftValue, rightValue, slider, thumb, horiz, id);

                bool wasMouseUpEvent = Event.current.type == EventType.MouseUp;

                if (ScrollerRepeatButton(id, minRect, leftButton))
                    value -= s_ScrollStepSize * (leftValue < rightValue ? 1f : -1f);

                if (ScrollerRepeatButton(id, maxRect, rightButton))
                    value += s_ScrollStepSize * (leftValue < rightValue ? 1f : -1f);

                if (wasMouseUpEvent && Event.current.type == EventType.Used) // repeat buttons ate mouse up event - release scrolling
                    s_ScrollControlId = 0;

                if (leftValue < rightValue)
                    value = Mathf.Clamp(value, leftValue, rightValue - size);
                else
                    value = Mathf.Clamp(value, rightValue, leftValue - size);
                return value;
            }
        }
        internal enum HighLevelEvent
        {
            None,
            Click,
            DoubleClick,
            ContextClick,
            BeginDrag,
            Drag,
            EndDrag,
            Delete,
            SelectionChanged,
            Copy,
            Paste
        }
        internal static class EditorGUIExt
        {
            private class Styles
            {
                public GUIStyle selectionRect = "SelectionRect";
            }

            private class MinMaxSliderState
            {
                public float dragStartPos = 0f;

                public float dragStartValue = 0f;

                public float dragStartSize = 0f;

                public float dragStartValuesPerPixel = 0f;

                public float dragStartLimit = 0f;

                public float dragEndLimit = 0f;

                public int whereWeDrag = -1;
            }

            private enum DragSelectionState
            {
                None,
                DragSelecting,
                Dragging
            }

            private static FieldInfo s_kFloatFieldFormatStringFieldInfo;
            public static string kFloatFieldFormatString
            {
                get
                {
                    if (s_kFloatFieldFormatStringFieldInfo == null)
                    {
                        s_kFloatFieldFormatStringFieldInfo =
                            TypeHelper.TypeOf<EditorGUI>.Type
                            .GetField("kFloatFieldFormatString", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    }
                    return s_kFloatFieldFormatStringFieldInfo.GetValue(null) as string;
                }
            }
            private static FieldInfo s_RecycledEditorFieldInfo;
            public static dynamic s_RecycledEditor
            {
                get
                {
                    if (s_RecycledEditorFieldInfo == null)
                    {
                        s_RecycledEditorFieldInfo =
                            TypeHelper.TypeOf<EditorGUI>.Type
                            .GetField("s_RecycledEditor", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
                    }
                    return s_RecycledEditorFieldInfo.GetValue(null);
                }
            }

            private static Styles ms_Styles = new Styles();

            private static int repeatButtonHash = "repeatButton".GetHashCode();

            private static float nextScrollStepTime = 0f;

            private static int firstScrollWait = 250;

            private static int scrollWait = 30;

            private static int scrollControlID;

            private static MinMaxSliderState s_MinMaxSliderState;

            private static int kFirstScrollWait = 250;

            private static int kScrollWait = 30;

            private static DateTime s_NextScrollStepTime = DateTime.Now;

            private static Vector2 s_MouseDownPos = Vector2.zero;

            private static DragSelectionState s_MultiSelectDragSelection = DragSelectionState.None;

            private static Vector2 s_StartSelectPos = Vector2.zero;

            private static List<bool> s_SelectionBackup = null;

            private static List<bool> s_LastFrameSelections = null;

            internal static int s_MinMaxSliderHash = "MinMaxSlider".GetHashCode();

            private static bool adding = false;

            private static bool[] initSelections;

            private static int initIndex = 0;

            private static bool DoRepeatButton(Rect position, GUIContent content, GUIStyle style, FocusType focusType)
            {
                int controlID = GUIUtility.GetControlID(repeatButtonHash, focusType, position);
                switch (Event.current.GetTypeForControl(controlID))
                {
                    case EventType.MouseDown:
                        if (position.Contains(Event.current.mousePosition))
                        {
                            GUIUtility.hotControl = controlID;
                            Event.current.Use();
                        }

                        return false;
                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == controlID)
                        {
                            GUIUtility.hotControl = 0;
                            Event.current.Use();
                            return position.Contains(Event.current.mousePosition);
                        }

                        return false;
                    case EventType.Repaint:
                        style.Draw(position, content, controlID, on: false, position.Contains(Event.current.mousePosition));
                        return controlID == GUIUtility.hotControl && position.Contains(Event.current.mousePosition);
                    default:
                        return false;
                }
            }

            private static bool ScrollerRepeatButton(int scrollerID, Rect rect, GUIStyle style)
            {
                bool result = false;
                if (DoRepeatButton(rect, GUIContent.none, style, FocusType.Passive))
                {
                    bool flag = scrollControlID != scrollerID;
                    scrollControlID = scrollerID;
                    if (flag)
                    {
                        result = true;
                        nextScrollStepTime = Time.realtimeSinceStartup + 0.001f * (float)firstScrollWait;
                    }
                    else if (Time.realtimeSinceStartup >= nextScrollStepTime)
                    {
                        result = true;
                        nextScrollStepTime = Time.realtimeSinceStartup + 0.001f * (float)scrollWait;
                    }

                    if (Event.current.type == EventType.Repaint)
                    {
                        HandleUtility.Repaint();
                    }
                }

                return result;
            }

            public static void MinMaxScroller(Rect position, int id, ref float value, ref float size, float visualStart, float visualEnd, float startLimit, float endLimit, GUIStyle slider, GUIStyle thumb, GUIStyle leftButton, GUIStyle rightButton, bool horiz)
            {
                float num = (!horiz) ? (size * 10f / position.height) : (size * 10f / position.width);
                Rect position2;
                Rect rect;
                Rect rect2;
                if (!horiz)
                {
                    position2 = new Rect(position.x, position.y + leftButton.fixedHeight, position.width, position.height - leftButton.fixedHeight - rightButton.fixedHeight);
                    rect = new Rect(position.x, position.y, position.width, leftButton.fixedHeight);
                    rect2 = new Rect(position.x, position.yMax - rightButton.fixedHeight, position.width, rightButton.fixedHeight);
                }
                else
                {
                    position2 = new Rect(position.x + leftButton.fixedWidth, position.y, position.width - leftButton.fixedWidth - rightButton.fixedWidth, position.height);
                    rect = new Rect(position.x, position.y, leftButton.fixedWidth, position.height);
                    rect2 = new Rect(position.xMax - rightButton.fixedWidth, position.y, rightButton.fixedWidth, position.height);
                }

                float num2 = Mathf.Min(visualStart, value);
                float num3 = Mathf.Max(visualEnd, value + size);
                MinMaxSlider(position2, ref value, ref size, num2, num3, num2, num3, slider, thumb, horiz);
                if (ScrollerRepeatButton(id, rect, leftButton))
                {
                    value -= num * ((visualStart < visualEnd) ? 1f : (-1f));
                }

                if (ScrollerRepeatButton(id, rect2, rightButton))
                {
                    value += num * ((visualStart < visualEnd) ? 1f : (-1f));
                }

                if (Event.current.type == EventType.MouseUp && Event.current.type == EventType.Used)
                {
                    scrollControlID = 0;
                }

                if (startLimit < endLimit)
                {
                    value = Mathf.Clamp(value, startLimit, endLimit - size);
                }
                else
                {
                    value = Mathf.Clamp(value, endLimit, startLimit - size);
                }
            }

            public static void MinMaxSlider(Rect position, ref float value, ref float size, float visualStart, float visualEnd, float startLimit, float endLimit, GUIStyle slider, GUIStyle thumb, bool horiz)
            {
                DoMinMaxSlider(position, GUIUtility.GetControlID(s_MinMaxSliderHash, FocusType.Passive), ref value, ref size, visualStart, visualEnd, startLimit, endLimit, slider, thumb, horiz);
            }

            private static float ThumbSize(bool horiz, GUIStyle thumb)
            {
                if (horiz)
                {
                    return (thumb.fixedWidth != 0f) ? thumb.fixedWidth : ((float)thumb.padding.horizontal);
                }

                return (thumb.fixedHeight != 0f) ? thumb.fixedHeight : ((float)thumb.padding.vertical);
            }

            internal static void DoMinMaxSlider(Rect position, int id, ref float value, ref float size, float visualStart, float visualEnd, float startLimit, float endLimit, GUIStyle slider, GUIStyle thumb, bool horiz)
            {
                Event current = Event.current;
                bool flag = size == 0f;
                float num = Mathf.Min(visualStart, visualEnd);
                float num2 = Mathf.Max(visualStart, visualEnd);
                float num3 = Mathf.Min(startLimit, endLimit);
                float num4 = Mathf.Max(startLimit, endLimit);
                MinMaxSliderState minMaxSliderState = s_MinMaxSliderState;
                if (GUIUtility.hotControl == id && minMaxSliderState != null)
                {
                    num = minMaxSliderState.dragStartLimit;
                    num3 = minMaxSliderState.dragStartLimit;
                    num2 = minMaxSliderState.dragEndLimit;
                    num4 = minMaxSliderState.dragEndLimit;
                }

                float num5 = 0f;
                float num6 = Mathf.Clamp(value, num, num2);
                float num7 = Mathf.Clamp(value + size, num, num2) - num6;
                float num8 = (!(visualStart > visualEnd)) ? 1 : (-1);
                if (slider == null || thumb == null)
                {
                    return;
                }

                Rect rect = thumb.margin.Remove(slider.padding.Remove(position));
                float num9 = ThumbSize(horiz, thumb);
                float num10;
                Rect position2;
                Rect position3;
                Rect position4;
                float num11;
                if (horiz)
                {
                    float height = (thumb.fixedHeight != 0f) ? thumb.fixedHeight : rect.height;
                    num10 = (position.width - (float)slider.padding.horizontal - num9) / (num2 - num);
                    position2 = new Rect((num6 - num) * num10 + rect.x, rect.y, num7 * num10 + num9, height);
                    position3 = new Rect(position2.x, position2.y, thumb.padding.left, position2.height);
                    position4 = new Rect(position2.xMax - (float)thumb.padding.right, position2.y, thumb.padding.right, position2.height);
                    num11 = current.mousePosition.x - position.x;
                }
                else
                {
                    float width = (thumb.fixedWidth != 0f) ? thumb.fixedWidth : rect.width;
                    num10 = (position.height - (float)slider.padding.vertical - num9) / (num2 - num);
                    position2 = new Rect(rect.x, (num6 - num) * num10 + rect.y, width, num7 * num10 + num9);
                    position3 = new Rect(position2.x, position2.y, position2.width, thumb.padding.top);
                    position4 = new Rect(position2.x, position2.yMax - (float)thumb.padding.bottom, position2.width, thumb.padding.bottom);
                    num11 = current.mousePosition.y - position.y;
                }

                switch (current.GetTypeForControl(id))
                {
                    case EventType.MouseDown:
                        if (current.button != 0 || !position.Contains(current.mousePosition) || num - num2 == 0f)
                        {
                            break;
                        }

                        if (minMaxSliderState == null)
                        {
                            minMaxSliderState = (s_MinMaxSliderState = new MinMaxSliderState());
                        }

                        minMaxSliderState.dragStartLimit = startLimit;
                        minMaxSliderState.dragEndLimit = endLimit;
                        if (position2.Contains(current.mousePosition))
                        {
                            minMaxSliderState.dragStartPos = num11;
                            minMaxSliderState.dragStartValue = value;
                            minMaxSliderState.dragStartSize = size;
                            minMaxSliderState.dragStartValuesPerPixel = num10;
                            if (position3.Contains(current.mousePosition))
                            {
                                minMaxSliderState.whereWeDrag = 1;
                            }
                            else if (position4.Contains(current.mousePosition))
                            {
                                minMaxSliderState.whereWeDrag = 2;
                            }
                            else
                            {
                                minMaxSliderState.whereWeDrag = 0;
                            }

                            GUIUtility.hotControl = id;
                            current.Use();
                        }
                        else
                        {
                            if (slider == GUIStyle.none)
                            {
                                break;
                            }

                            if (size != 0f && flag)
                            {
                                if (horiz)
                                {
                                    if (num11 > position2.xMax - position.x)
                                    {
                                        value += size * num8 * 0.9f;
                                    }
                                    else
                                    {
                                        value -= size * num8 * 0.9f;
                                    }
                                }
                                else if (num11 > position2.yMax - position.y)
                                {
                                    value += size * num8 * 0.9f;
                                }
                                else
                                {
                                    value -= size * num8 * 0.9f;
                                }

                                minMaxSliderState.whereWeDrag = 0;
                                GUI.changed = true;
                                s_NextScrollStepTime = DateTime.Now.AddMilliseconds(kFirstScrollWait);
                                float num12 = horiz ? current.mousePosition.x : current.mousePosition.y;
                                float num13 = horiz ? position2.x : position2.y;
                                minMaxSliderState.whereWeDrag = ((num12 > num13) ? 4 : 3);
                            }
                            else
                            {
                                if (horiz)
                                {
                                    value = (num11 - position2.width * 0.5f) / num10 + num - size * 0.5f;
                                }
                                else
                                {
                                    value = (num11 - position2.height * 0.5f) / num10 + num - size * 0.5f;
                                }

                                minMaxSliderState.dragStartPos = num11;
                                minMaxSliderState.dragStartValue = value;
                                minMaxSliderState.dragStartSize = size;
                                minMaxSliderState.dragStartValuesPerPixel = num10;
                                minMaxSliderState.whereWeDrag = 0;
                                GUI.changed = true;
                            }

                            GUIUtility.hotControl = id;
                            value = Mathf.Clamp(value, num3, num4 - size);
                            current.Use();
                        }

                        break;
                    case EventType.MouseDrag:
                        {
                            if (GUIUtility.hotControl != id)
                            {
                                break;
                            }

                            float num15 = (num11 - minMaxSliderState.dragStartPos) / minMaxSliderState.dragStartValuesPerPixel;
                            switch (minMaxSliderState.whereWeDrag)
                            {
                                case 0:
                                    value = Mathf.Clamp(minMaxSliderState.dragStartValue + num15, num3, num4 - size);
                                    break;
                                case 1:
                                    value = minMaxSliderState.dragStartValue + num15;
                                    size = minMaxSliderState.dragStartSize - num15;
                                    if (value < num3)
                                    {
                                        size -= num3 - value;
                                        value = num3;
                                    }

                                    if (size < num5)
                                    {
                                        value -= num5 - size;
                                        size = num5;
                                    }

                                    break;
                                case 2:
                                    size = minMaxSliderState.dragStartSize + num15;
                                    if (value + size > num4)
                                    {
                                        size = num4 - value;
                                    }

                                    if (size < num5)
                                    {
                                        size = num5;
                                    }

                                    break;
                            }

                            GUI.changed = true;
                            current.Use();
                            break;
                        }
                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == id)
                        {
                            current.Use();
                            GUIUtility.hotControl = 0;
                        }

                        break;
                    case EventType.Repaint:
                        slider.Draw(position, GUIContent.none, id);
                        thumb.Draw(position2, GUIContent.none, id);
                        EditorGUIUtility.AddCursorRect(position3, horiz ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical, (minMaxSliderState != null && minMaxSliderState.whereWeDrag == 1) ? id : (-1));
                        EditorGUIUtility.AddCursorRect(position4, horiz ? MouseCursor.ResizeHorizontal : MouseCursor.ResizeVertical, (minMaxSliderState != null && minMaxSliderState.whereWeDrag == 2) ? id : (-1));
                        if (GUIUtility.hotControl != id || !position.Contains(current.mousePosition) || num - num2 == 0f)
                        {
                            break;
                        }

                        if (position2.Contains(current.mousePosition))
                        {
                            if (minMaxSliderState != null && (minMaxSliderState.whereWeDrag == 3 || minMaxSliderState.whereWeDrag == 4))
                            {
                                GUIUtility.hotControl = 0;
                            }
                        }
                        else
                        {
                            if (DateTime.Now < s_NextScrollStepTime)
                            {
                                break;
                            }

                            float num12 = horiz ? current.mousePosition.x : current.mousePosition.y;
                            float num13 = horiz ? position2.x : position2.y;
                            int num14 = (num12 > num13) ? 4 : 3;
                            if (minMaxSliderState != null && num14 != minMaxSliderState.whereWeDrag)
                            {
                                break;
                            }

                            if (size != 0f && flag)
                            {
                                if (horiz)
                                {
                                    if (num11 > position2.xMax - position.x)
                                    {
                                        value += size * num8 * 0.9f;
                                    }
                                    else
                                    {
                                        value -= size * num8 * 0.9f;
                                    }
                                }
                                else if (num11 > position2.yMax - position.y)
                                {
                                    value += size * num8 * 0.9f;
                                }
                                else
                                {
                                    value -= size * num8 * 0.9f;
                                }

                                if (minMaxSliderState != null)
                                {
                                    minMaxSliderState.whereWeDrag = -1;
                                }

                                GUI.changed = true;
                            }

                            value = Mathf.Clamp(value, num3, num4 - size);
                            s_NextScrollStepTime = DateTime.Now.AddMilliseconds(kScrollWait);
                        }

                        break;
                }
            }

            public static bool DragSelection(Rect[] positions, ref bool[] selections, GUIStyle style)
            {
                int controlID = GUIUtility.GetControlID(34553287, FocusType.Keyboard);
                Event current = Event.current;
                int num = -1;
                for (int num2 = positions.Length - 1; num2 >= 0; num2--)
                {
                    if (positions[num2].Contains(current.mousePosition))
                    {
                        num = num2;
                        break;
                    }
                }

                switch (current.GetTypeForControl(controlID))
                {
                    case EventType.Repaint:
                        {
                            for (int l = 0; l < positions.Length; l++)
                            {
                                style.Draw(positions[l], GUIContent.none, controlID, selections[l]);
                            }

                            break;
                        }
                    case EventType.MouseDown:
                        {
                            if (current.button != 0 || num < 0)
                            {
                                break;
                            }

                            GUIUtility.keyboardControl = 0;
                            bool flag = false;
                            if (selections[num])
                            {
                                int num3 = 0;
                                bool[] array = selections;
                                for (int i = 0; i < array.Length; i++)
                                {
                                    if (array[i])
                                    {
                                        num3++;
                                        if (num3 > 1)
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (num3 == 1)
                                {
                                    flag = true;
                                }
                            }

                            if (!current.shift && !EditorGUI.actionKey)
                            {
                                for (int j = 0; j < positions.Length; j++)
                                {
                                    selections[j] = false;
                                }
                            }

                            initIndex = num;
                            initSelections = (bool[])selections.Clone();
                            adding = true;
                            if ((current.shift || EditorGUI.actionKey) && selections[num])
                            {
                                adding = false;
                            }

                            selections[num] = (!flag && adding);
                            GUIUtility.hotControl = controlID;
                            current.Use();
                            return true;
                        }
                    case EventType.MouseDrag:
                        {
                            if (GUIUtility.hotControl != controlID || current.button != 0)
                            {
                                break;
                            }

                            if (num < 0)
                            {
                                Rect rect = new Rect(positions[0].x, positions[0].y - 200f, positions[0].width, 200f);
                                if (rect.Contains(current.mousePosition))
                                {
                                    num = 0;
                                }

                                rect.y = positions[^1].yMax;
                                if (rect.Contains(current.mousePosition))
                                {
                                    num = selections.Length - 1;
                                }
                            }

                            if (num < 0)
                            {
                                return false;
                            }

                            int num4 = Mathf.Min(initIndex, num);
                            int num5 = Mathf.Max(initIndex, num);
                            for (int k = 0; k < selections.Length; k++)
                            {
                                if (k >= num4 && k <= num5)
                                {
                                    selections[k] = adding;
                                }
                                else
                                {
                                    selections[k] = initSelections[k];
                                }
                            }

                            current.Use();
                            return true;
                        }
                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == controlID)
                        {
                            GUIUtility.hotControl = 0;
                        }

                        break;
                }

                return false;
            }

            private static bool Any(bool[] selections)
            {
                for (int i = 0; i < selections.Length; i++)
                {
                    if (selections[i])
                    {
                        return true;
                    }
                }

                return false;
            }

            public static HighLevelEvent MultiSelection(Rect rect, Rect[] positions, GUIContent content, Rect[] hitPositions, ref bool[] selections, bool[] readOnly, out int clickedIndex, out Vector2 offset, out float startSelect, out float endSelect, GUIStyle style)
            {
                int controlID = GUIUtility.GetControlID(41623453, FocusType.Keyboard);
                Event current = Event.current;
                offset = Vector2.zero;
                clickedIndex = -1;
                startSelect = (endSelect = 0f);
                if (current.type == EventType.Used)
                {
                    return HighLevelEvent.None;
                }

                bool flag = false;
                if (Event.current.type != EventType.Layout && GUIUtility.keyboardControl == controlID)
                {
                    flag = true;
                }

                switch (current.GetTypeForControl(controlID))
                {
                    case EventType.Repaint:
                        {
                            if (GUIUtility.hotControl == controlID && s_MultiSelectDragSelection == DragSelectionState.DragSelecting)
                            {
                                float num = Mathf.Min(s_StartSelectPos.x, current.mousePosition.x);
                                float num2 = Mathf.Max(s_StartSelectPos.x, current.mousePosition.x);
                                Rect position = new Rect(0f, 0f, rect.width, rect.height);
                                position.x = num;
                                position.width = num2 - num;
                                if (position.width > 1f)
                                {
                                    GUI.Box(position, "", ms_Styles.selectionRect);
                                }
                            }

                            Color color = GUI.color;
                            for (int i = 0; i < positions.Length; i++)
                            {
                                if (readOnly != null && readOnly[i])
                                {
                                    GUI.color = color * new Color(0.9f, 0.9f, 0.9f, 0.5f);
                                }
                                else if (selections[i])
                                {
                                    GUI.color = color * new Color(0.3f, 0.55f, 0.95f, 1f);
                                }
                                else
                                {
                                    GUI.color = color * new Color(0.9f, 0.9f, 0.9f, 1f);
                                }

                                style.Draw(positions[i], content, controlID, selections[i]);
                            }

                            GUI.color = color;
                            break;
                        }
                    case EventType.MouseDown:
                        {
                            if (current.button != 0)
                            {
                                break;
                            }

                            GUIUtility.hotControl = controlID;
                            GUIUtility.keyboardControl = controlID;
                            s_StartSelectPos = current.mousePosition;
                            int indexUnderMouse = GetIndexUnderMouse(hitPositions, readOnly);
                            if (Event.current.clickCount == 2 && indexUnderMouse >= 0)
                            {
                                for (int l = 0; l < selections.Length; l++)
                                {
                                    selections[l] = false;
                                }

                                selections[indexUnderMouse] = true;
                                current.Use();
                                clickedIndex = indexUnderMouse;
                                return HighLevelEvent.DoubleClick;
                            }

                            if (indexUnderMouse >= 0)
                            {
                                if (!current.shift && !EditorGUI.actionKey && !selections[indexUnderMouse])
                                {
                                    for (int m = 0; m < hitPositions.Length; m++)
                                    {
                                        selections[m] = false;
                                    }
                                }

                                if (current.shift || EditorGUI.actionKey)
                                {
                                    selections[indexUnderMouse] = !selections[indexUnderMouse];
                                }
                                else
                                {
                                    selections[indexUnderMouse] = true;
                                }

                                s_MouseDownPos = current.mousePosition;
                                s_MultiSelectDragSelection = DragSelectionState.None;
                                current.Use();
                                clickedIndex = indexUnderMouse;
                                return HighLevelEvent.SelectionChanged;
                            }

                            bool flag4 = false;
                            if (!current.shift && !EditorGUI.actionKey)
                            {
                                for (int n = 0; n < hitPositions.Length; n++)
                                {
                                    selections[n] = false;
                                }

                                flag4 = true;
                            }
                            else
                            {
                                flag4 = false;
                            }

                            s_SelectionBackup = new List<bool>(selections);
                            s_LastFrameSelections = new List<bool>(selections);
                            s_MultiSelectDragSelection = DragSelectionState.DragSelecting;
                            current.Use();
                            return flag4 ? HighLevelEvent.SelectionChanged : HighLevelEvent.None;
                        }
                    case EventType.MouseDrag:
                        if (GUIUtility.hotControl != controlID)
                        {
                            break;
                        }

                        if (s_MultiSelectDragSelection == DragSelectionState.DragSelecting)
                        {
                            float num3 = Mathf.Min(s_StartSelectPos.x, current.mousePosition.x);
                            float num4 = Mathf.Max(s_StartSelectPos.x, current.mousePosition.x);
                            s_SelectionBackup.CopyTo(selections);
                            for (int j = 0; j < hitPositions.Length; j++)
                            {
                                if (!selections[j])
                                {
                                    float num5 = hitPositions[j].x + hitPositions[j].width * 0.5f;
                                    if (num5 >= num3 && num5 <= num4)
                                    {
                                        selections[j] = true;
                                    }
                                }
                            }

                            current.Use();
                            startSelect = num3;
                            endSelect = num4;
                            bool flag3 = false;
                            for (int k = 0; k < selections.Length; k++)
                            {
                                if (selections[k] != s_LastFrameSelections[k])
                                {
                                    flag3 = true;
                                    s_LastFrameSelections[k] = selections[k];
                                }
                            }

                            return flag3 ? HighLevelEvent.SelectionChanged : HighLevelEvent.None;
                        }

                        offset = current.mousePosition - s_MouseDownPos;
                        current.Use();
                        if (s_MultiSelectDragSelection == DragSelectionState.None)
                        {
                            s_MultiSelectDragSelection = DragSelectionState.Dragging;
                            return HighLevelEvent.BeginDrag;
                        }

                        return HighLevelEvent.Drag;
                    case EventType.MouseUp:
                        if (GUIUtility.hotControl == controlID)
                        {
                            GUIUtility.hotControl = 0;
                            if (s_StartSelectPos != current.mousePosition)
                            {
                                current.Use();
                            }

                            if (s_MultiSelectDragSelection != 0)
                            {
                                s_MultiSelectDragSelection = DragSelectionState.None;
                                s_SelectionBackup = null;
                                s_LastFrameSelections = null;
                                return HighLevelEvent.EndDrag;
                            }

                            clickedIndex = GetIndexUnderMouse(hitPositions, readOnly);
                            if (current.clickCount == 1)
                            {
                                return HighLevelEvent.Click;
                            }
                        }

                        break;
                    case EventType.ValidateCommand:
                    case EventType.ExecuteCommand:
                        {
                            if (!flag)
                            {
                                break;
                            }

                            bool flag2 = current.type == EventType.ExecuteCommand;
                            switch (current.commandName)
                            {
                                case "Delete":
                                    current.Use();
                                    if (flag2)
                                    {
                                        return HighLevelEvent.Delete;
                                    }

                                    break;
                                case "Copy":
                                    current.Use();
                                    if (flag2)
                                    {
                                        return HighLevelEvent.Copy;
                                    }

                                    break;
                                case "Paste":
                                    current.Use();
                                    if (flag2)
                                    {
                                        return HighLevelEvent.Paste;
                                    }

                                    break;
                            }

                            break;
                        }
                    case EventType.KeyDown:
                        if (flag && (current.keyCode == KeyCode.Backspace || current.keyCode == KeyCode.Delete))
                        {
                            current.Use();
                            return HighLevelEvent.Delete;
                        }

                        break;
                    case EventType.ContextClick:
                        {
                            int indexUnderMouse = GetIndexUnderMouse(hitPositions, readOnly);
                            if (indexUnderMouse >= 0)
                            {
                                clickedIndex = indexUnderMouse;
                                GUIUtility.keyboardControl = controlID;
                                current.Use();
                                return HighLevelEvent.ContextClick;
                            }

                            break;
                        }
                }

                return HighLevelEvent.None;
            }

            private static int GetIndexUnderMouse(Rect[] hitPositions, bool[] readOnly)
            {
                Vector2 mousePosition = Event.current.mousePosition;
                for (int num = hitPositions.Length - 1; num >= 0; num--)
                {
                    if ((readOnly == null || !readOnly[num]) && hitPositions[num].Contains(mousePosition))
                    {
                        return num;
                    }
                }

                return -1;
            }

            internal static Rect FromToRect(Vector2 start, Vector2 end)
            {
                Rect result = new Rect(start.x, start.y, end.x - start.x, end.y - start.y);
                if (result.width < 0f)
                {
                    result.x += result.width;
                    result.width = 0f - result.width;
                }

                if (result.height < 0f)
                {
                    result.y += result.height;
                    result.height = 0f - result.height;
                }

                return result;
            }
        }
        internal static class HandleUtilityExt
        {
            internal static void ApplyWireMaterial()
            {
                MethodInfo method = TypeHelper.TypeOf<HandleUtility>.Type
                    .GetMethod("ApplyWireMaterial", 
                    BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public,
                    null,
                    Array.Empty<Type>(),
                    null);
                method.Invoke(null, null);
            }
        }
        internal static class MathUtilsExt
        {
            internal static int GetNumberOfDecimalsForMinimumDifference(float minDifference)
            {
                return Mathf.Clamp(-Mathf.FloorToInt(Mathf.Log10(Mathf.Abs(minDifference))), 0, int.MaxValue);
            }
            internal static int GetNumberOfDecimalsForMinimumDifference(double minDifference)
            {
                return (int)System.Math.Max(0.0, -System.Math.Floor(System.Math.Log10(System.Math.Abs(minDifference))));
            }

            internal static float RoundBasedOnMinimumDifference(float valueToRound, float minDifference)
            {
                if (minDifference == 0)
                    return (float)DiscardLeastSignificantDecimal((double)valueToRound);
                return (float)System.Math.Round(valueToRound, GetNumberOfDecimalsForMinimumDifference(minDifference),
                    System.MidpointRounding.AwayFromZero);
            }
            internal static double RoundBasedOnMinimumDifference(double valueToRound, double minDifference)
            {
                if (minDifference == 0)
                    return DiscardLeastSignificantDecimal(valueToRound);
                return System.Math.Round(valueToRound, GetNumberOfDecimalsForMinimumDifference(minDifference),
                    System.MidpointRounding.AwayFromZero);
            }
            internal static double DiscardLeastSignificantDecimal(double v)
            {
                int decimals = System.Math.Max(0, (int)(5 - System.Math.Log10(System.Math.Abs(v))));
                try
                {
                    return System.Math.Round(v, decimals);
                }
                catch (System.ArgumentOutOfRangeException)
                {
                    // This can happen for very small numbers.
                    return 0;
                }
            }
        }

        #endregion

        #region Visual Element

        public struct VisualElement
        {
            private static StyleSheet s_DefaultStyleSheet, s_IconStyleSheet;
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
            public static StyleSheet IconStyleSheet
            {
                get
                {
                    if (s_IconStyleSheet == null)
                    {
                        s_IconStyleSheet =
                            AssetHelper.LoadAsset<StyleSheet>("icons-uss", "PointEditor");
                    }
                    return s_IconStyleSheet;
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

            public static PropertyField PropertyField(string label, SerializedProperty property)
            {
                var field = PropertyField(property);
                field.label = label;
                return field;
            }
            public static PropertyField PropertyField(SerializedProperty property)
            {
                PropertyField field = new PropertyField(property);
                if (property != null)
                {
                    field.BindProperty(property);
                }
                else
                {
                    "?".ToLogError();
                }

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
            public static T OpenWindowAtCenterSafe<T>(string title, bool utility)
                where T : UnityEditor.EditorWindow
            {
                var window = (T)UnityEditor.EditorWindow.GetWindow(TypeHelper.TypeOf<T>.Type, utility, title);

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

                    return OpenWindowAtCenterSafe<T>(title, utility);
                }

                return window;
            }
            public static T OpenWindowAtCenterSafe<T>(string title, bool utility, Vector2 size)
                where T : UnityEditor.EditorWindow
            {
                var window = (T)UnityEditor.EditorWindow.GetWindow(TypeHelper.TypeOf<T>.Type, utility, title);

                window.minSize = size;

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
            public static T OpenWindowAtCenterSafe<T>(string title, bool utility, Vector2 size, Vector2 maxSize)
                where T : UnityEditor.EditorWindow
            {
                var window = (T)UnityEditor.EditorWindow.GetWindow(TypeHelper.TypeOf<T>.Type, utility, title);

                window.minSize = size;
                window.maxSize = maxSize;

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

                    return OpenWindowAtCenterSafe<T>(title, utility, size, maxSize);
                }

                return window;
            }
        }
    }
}

#endif