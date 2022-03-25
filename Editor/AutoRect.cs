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

using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    public struct AutoRect
    {
        private readonly Rect m_OriginalRect;
        private Rect m_Rect;

        public AutoRect(Rect rect)
        {
            m_OriginalRect = rect;
            m_Rect = m_OriginalRect;
        }

        public void Reset()
        {
            m_Rect = m_OriginalRect;
        }

        public Rect Pop()
        {
            if (m_Rect.height <= 0)
            {
                throw new System.Exception("no space");
            }

            Rect temp = m_Rect;
            temp.height = PropertyDrawerHelper.GetPropertyHeight(1);

            m_Rect.y += temp.height;
            m_Rect.height -= temp.height;

            temp = EditorGUI.IndentedRect(temp);
            return temp;
        }
        public void Indent(float pixel)
        {
            m_Rect.x += pixel;
            m_Rect.width -= pixel;
        }
    }
}

#endif