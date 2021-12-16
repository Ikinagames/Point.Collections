// Copyright 2021 Ikina Games
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

using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    public sealed class EditorUtilities
    {
        #region Strings

        public static string String(string text) => HTMLString.String(text, EditorGUIUtility.isProSkin ? StringColor.white : StringColor.black);
        public static string String(string text, int size) => $"<size={size}>{HTMLString.String(text, EditorGUIUtility.isProSkin ? StringColor.white : StringColor.black)}</size>";
        public static string String(string text, StringColor color, int size) => String(HTMLString.String(text, color), size);

        public static void StringHeader(string text, StringColor color, bool center)
        {
            EditorGUILayout.LabelField(String(text, color, 20), center ? EditorStyleUtilities.CenterStyle : EditorStyleUtilities.HeaderStyle);
        }
        public static void StringHeader(string text, int size = 20)
        {
            EditorGUILayout.LabelField(String(text, StringColor.grey, size), EditorStyleUtilities.HeaderStyle);
        }
        public static void StringHeader(string text, int size, bool center)
        {
            EditorGUILayout.LabelField(String(text, StringColor.grey, size), center ? EditorStyleUtilities.CenterStyle : EditorStyleUtilities.HeaderStyle);
        }
        public static void StringHeader(string text, int size, params GUILayoutOption[] options)
        {
            EditorGUILayout.LabelField(String(text, StringColor.grey, size), EditorStyleUtilities.HeaderStyle, options);
        }
        public static void StringHeader(string text, StringColor color, int size = 20)
        {
            EditorGUILayout.LabelField(String(text, color, size), EditorStyleUtilities.HeaderStyle);
        }
        public static void StringRich(string text, bool center = false)
        {
            EditorGUILayout.LabelField(HTMLString.String(text, EditorGUIUtility.isProSkin ? StringColor.white : StringColor.black), center ? EditorStyleUtilities.CenterStyle : EditorStyleUtilities.HeaderStyle);
        }
        public static void StringRich(string text, GUIStyle style, bool center = false)
        {
            if (style == null) style = new GUIStyle("Label");

            style.richText = true;
            if (center) style.alignment = TextAnchor.MiddleCenter;
            EditorGUILayout.LabelField(HTMLString.String(text, EditorGUIUtility.isProSkin ? StringColor.white : StringColor.black), style);
        }
        public static void StringRich(string text, StringColor color, bool center, GUIStyle style, params GUILayoutOption[] options)
        {
            if (style == null) style = new GUIStyle("Label");
            style.richText = true;
            if (center) style.alignment = TextAnchor.MiddleCenter;

            GUILayout.Label(HTMLString.String(text, color), style, options);
        }
        public static void StringRich(string text, bool center, GUIStyle style, params GUILayoutOption[] options)
        {
            if (style == null) style = new GUIStyle("Label");
            style.richText = true;
            if (center) style.alignment = TextAnchor.MiddleCenter;

            GUILayout.Label(HTMLString.String(text, EditorGUIUtility.isProSkin ? StringColor.white : StringColor.black), style, options);
        }
        public static void StringRich(string text, StringColor color, bool center = false)
        {
            EditorGUILayout.LabelField(HTMLString.String(text, color), center ? EditorStyleUtilities.CenterStyle : EditorStyleUtilities.HeaderStyle);
        }
        public static void StringRich(string text, int size, bool center = false)
        {
            EditorGUILayout.LabelField(String(text, EditorGUIUtility.isProSkin ? StringColor.white : StringColor.black, size), center ? EditorStyleUtilities.CenterStyle : EditorStyleUtilities.HeaderStyle);
        }
        public static void StringRich(string text, int size, StringColor color, bool center = false)
        {
            EditorGUILayout.LabelField(String(text, color, size), center ? EditorStyleUtilities.CenterStyle : EditorStyleUtilities.HeaderStyle);
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

        public static bool Foldout(bool foldout, string name, int size = -1)
        {
            string firstKey = foldout ? EditorStyleUtilities.FoldoutOpendString : EditorStyleUtilities.FoldoutClosedString;
            if (size < 0)
            {
                return EditorGUILayout.Foldout(foldout, HTMLString.String($"{firstKey} {name}", StringColor.grey), true, EditorStyleUtilities.HeaderStyle);
            }
            else
            {
                return EditorGUILayout.Foldout(foldout, HTMLString.String($"<size={size}>{firstKey} {name}</size>", StringColor.grey), true, EditorStyleUtilities.HeaderStyle);
            }
        }

        public static void DeleteFilesRecursively(string path)
        {
            foreach (string newPath in Directory.GetFiles(path, "*", SearchOption.AllDirectories))
            {
                File.Delete(newPath);
            }

            foreach (string dirPath in Directory.GetDirectories(path, "*", SearchOption.AllDirectories))
            {
                Directory.Delete(dirPath);
            }
        }
        public static void CopyFilesRecursively(string sourcePath, string targetPath)
        {
            //Now Create all of the directories
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            //Copy all the files & Replaces any files with the same name
            foreach (string newPath in Directory.GetFiles(sourcePath, "*", SearchOption.AllDirectories))
            {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
    }
}
