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

using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public class PinPoint<TValue> : VisualElement
    {
        public const string PinClassName = "pin";

        public Vector3 position
        {
#if UNITY_2021_1_OR_NEWER
            get => new Vector3(style.translate.value.x.value + (size.x * .5f), style.translate.value.y.value + (size.y * .5f));
            set
            {
                style.translate = new StyleTranslate(new Translate(
                    value.x - 7.5f, value.y - 7.5f, 0));
            }
#else
            get => transform.position;
            set => transform.position = value;
#endif
        }
        private Vector2 size
        {
#if UNITY_2021_1_OR_NEWER
            get => resolvedStyle.scale.value;
#else
            get => transform.scale;
#endif
        }

        public TValue value { get; set; }
        public DragManipulator manipulator { get; private set; }

        public event Action<PinPoint<TValue>, Vector3> OnDrag;
        public event Action<PinPoint<TValue>, Vector3> OnDragEnded;

        public PinPoint(VisualElement root)
        {
            styleSheets.Add(CoreGUI.VisualElement.IconStyleSheet);
            this.AddToClassList(PinClassName);
            this.AddToClassList("dot-image");
            this.AddToClassList("dot-fill");

            style.position = Position.Absolute;

            manipulator = new DragManipulator();
            manipulator.root = root;
            manipulator.OnDragEnded += Manipulator_OnDragEnded;
            manipulator.OnDrag += Manipulator_OnDrag;
            this.AddManipulator(manipulator);
        }

        private void Manipulator_OnDrag(Vector3 obj)
        {
            OnDrag?.Invoke(this, obj);
        }
        private void Manipulator_OnDragEnded(Vector3 obj)
        {
            OnDragEnded?.Invoke(this, obj);
        }
    }
}

#endif