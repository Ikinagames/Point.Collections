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

#if UNITY_2020
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

using UnityEngine;

namespace Point.Collections
{
    public sealed class Console : StaticMonobehaviour<Console>, IStaticInitializer
    {
        protected override bool EnableLog => false;

        [SerializeField] private bool m_EnableConsole = false;

        public Vector2 pos = new Vector2(0, 17.45f);
        public GUIStyle m_ConsoleStyle = "textfield";

        public string m_ConsoleText = string.Empty;

        public bool EnableConsole { get => m_EnableConsole; set => m_EnableConsole = value; }

        private void OnGUI()
        {
            if (!m_EnableConsole) return;

            Rect scrRect = new Rect(pos.x, Screen.height - pos.y, Screen.width, 100);
            
            using (new GUILayout.AreaScope(scrRect))
            {
                var rect = GUILayoutUtility.GetRect(GUIContent.none, m_ConsoleStyle);
                int keyboardID = GUIUtility.GetControlID(FocusType.Keyboard) + 1;
                GUI.SetNextControlName("CmdTextField");
                m_ConsoleText = GUI.TextField(rect, m_ConsoleText, m_ConsoleStyle);

                if (GUIUtility.keyboardControl == keyboardID)
                {
#if ENABLE_INPUT_SYSTEM
                    var currentKeyboard = UnityEngine.InputSystem.Keyboard.current;

                    var enterKey = currentKeyboard.enterKey;
                    if (enterKey.wasReleasedThisFrame)
                    {
                        ProcessCommand(m_ConsoleText);
                        m_ConsoleText = string.Empty;
                    }
                    var tabKey = currentKeyboard.tabKey;
                    if (tabKey.wasReleasedThisFrame)
                    {
                        ProcessTab(ref m_ConsoleText);
                        SetTextCursorToLast("CmdTextField");
                        GUI.FocusControl("CmdTextField");
                    }
#endif
                }
            }
        }

        private void ProcessTab(ref string cmd)
        {

        }
        private void ProcessCommand(in string cmd)
        {
            "in".ToLog();
        }

        private static TextEditor SetTextCursorToLast(string controlName)
        {
            GUI.FocusControl(controlName);
            TextEditor t = (TextEditor)GUIUtility.GetStateObject(TypeHelper.TypeOf<TextEditor>.Type, GUIUtility.keyboardControl);

            t.cursorIndex = t.text.Length;
            t.selectIndex = t.text.Length;

            return t;
        }
    }
}

#endif