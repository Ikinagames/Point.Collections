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

using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    public sealed class EditorStyleUtilities
    {
        public const string FoldoutOpendString = "▼";
        public const string FoldoutClosedString = "▶";

        public static GUIStyle TextField => EditorStyles.textField;
        public static GUIStyle MiniButton => EditorStyles.miniButton;

        static GUIStyle m_BoxStyle = null;
        public static GUIStyle Box
        {
            get
            {
                const string Box = "Box";

                if (m_BoxStyle == null)
                {
                    m_BoxStyle = new GUIStyle(Box);
                }
                return m_BoxStyle;
            }
        }

        static GUIStyle m_SelectorStyle = null;
        public static GUIStyle SelectorStyle
        {
            get
            {

                if (m_SelectorStyle == null)
                {
                    GUIStyle st = new GUIStyle(TextField);
                    st.clipping = TextClipping.Clip;
                    st.stretchWidth = true;
                    st.alignment = TextAnchor.MiddleCenter;
                    st.wordWrap = true;

                    m_SelectorStyle = st;
                }

                return m_SelectorStyle;
            }
        }

        static GUIStyle _headerStyle;
        public static GUIStyle HeaderStyle
        {
            get
            {
                if (_headerStyle == null)
                {
                    _headerStyle = new GUIStyle
                    {
                        richText = true
                    };
                }
                return _headerStyle;
            }
        }
        static GUIStyle _centerStyle;
        public static GUIStyle CenterStyle
        {
            get
            {
                if (_centerStyle == null)
                {
                    _centerStyle = new GUIStyle
                    {
                        richText = true,
                        alignment = TextAnchor.MiddleCenter
                    };
                }
                return _centerStyle;
            }
        }
        static GUIStyle _bttStyle;
        public static GUIStyle BttStyle
        {
            get
            {
                if (_bttStyle == null)
                {
                    _bttStyle = "Button";
                    _bttStyle.richText = true;
                }
                return _bttStyle;
            }
        }
        static GUIStyle _splitStyle;
        public static GUIStyle SplitStyle
        {
            get
            {
                if (_splitStyle == null)
                {
                    _splitStyle = new GUIStyle();
                    _splitStyle.normal.background = UnityEditor.EditorGUIUtility.whiteTexture;
                    _splitStyle.margin = new RectOffset(6, 6, 0, 0);
                }
                return _splitStyle;
            }
        }
    }
}

#endif