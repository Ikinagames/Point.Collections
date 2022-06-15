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
#endif

#if UNITY_2019_1_OR_NEWER
#define UNITYENGINE
#if UNITY_2019 || !UNITY_2020_OR_NEWER
#define UNITYENGINE_OLD
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    /// <summary>
    /// <inheritdoc cref="UnityEditor.EditorWindow"/>
    /// </summary>
    /// <remarks>
    /// Uxml 전용 에디터 윈도우, <see cref="GetVisualTreeAsset"/> 가 null 을 반환하면,
    /// 코드로 생성하는 <see cref="CreateVisualElement"/> 를 사용합니다.
    /// </remarks>
    public abstract class EditorWindowUXML : EditorWindow
    {
        public VisualElement UserVisualElement { get; private set; }

        protected void CreateGUI()
        {
            var asset = GetVisualTreeAsset();
            if (asset == null)
            {
                try
                {
                    UserVisualElement = CreateVisualElement();
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }
                if (UserVisualElement == null)
                {
                    UserVisualElement = new VisualElement();
                    UserVisualElement.Add(new Label($"No VisualElement Implement"));
                }
                
                rootVisualElement.Add(UserVisualElement);
            }
            else
            {
                asset.CloneTree(rootVisualElement);
                UserVisualElement = rootVisualElement;
            }

            try
            {
                SetupVisualElement(UserVisualElement);
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }

        /// <summary>
        /// 사용자가 직접 VisualElement 를 구성합니다.
        /// </summary>
        /// <returns></returns>
        protected virtual VisualElement CreateVisualElement() => new VisualElement();
        /// <summary>
        /// 미리 만들어진 <see cref="VisualTreeAsset"/> 을 통해 UI 를 구성합니다.
        /// </summary>
        /// <returns></returns>
        protected virtual VisualTreeAsset GetVisualTreeAsset() => null;

        protected virtual void SetupVisualElement(VisualElement root) { }
    }
}

#endif