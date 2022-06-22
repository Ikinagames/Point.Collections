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
#if UNITY_MATHEMATICS
#endif

using System;
using UnityEditor;
using UnityEngine;
using System.Reflection;

namespace Point.Collections.Editor
{
    // https://github.com/Unity-Technologies/UnityCsReference/blob/master/Editor/Mono/GameView/GameView.cs

    internal static class GameView
    {
        private static Type s_Type;
        public static Type Type
        {
            get
            {
                if (s_Type == null)
                {
                    var assembly = typeof(EditorWindow).Assembly;
                    s_Type = assembly.GetType("UnityEditor.GameView");
                }
                return s_Type;
            }
        }
        private static EditorWindow s_Window;
        public static EditorWindow Window
        {
            get
            {
                if (s_Window == null)
                {
                    s_Window = EditorWindow.GetWindow(Type);
                }
                return s_Window;
            }
        }

        private static FieldInfo s_RenderTextureFieldInfo = Type.GetField("m_RenderTexture", BindingFlags.Instance | BindingFlags.NonPublic);
        public static RenderTexture RenderTexture
        {
            get
            {
                return s_RenderTextureFieldInfo.GetValue(Window) as RenderTexture;
            }
        }

        public static void Repaint()
        {
            Window.Repaint();
        }
    }
}

#endif