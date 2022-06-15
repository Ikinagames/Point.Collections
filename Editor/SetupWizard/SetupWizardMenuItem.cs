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
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public abstract class SetupWizardMenuItem : IComparable<SetupWizardMenuItem>
    {
        public enum GUIType
        {
            IMGUI,
            VisualElement
        }

        public abstract string Name { get; }
        public abstract int Order { get; }
        public virtual GUIType Type => GUIType.IMGUI;

        public VisualElement Root { get; private set; }

        internal void Initialize()
        {
            Root = CreateVisualElement();
        }

        public virtual void OnVisible() { }
        public virtual void OnFocus() { }
        public virtual void OnLostFocus() { }

        public abstract bool Predicate();

        protected virtual VisualElement CreateVisualElement() => null;
        public virtual void OnGUI() { }

        int IComparable<SetupWizardMenuItem>.CompareTo(SetupWizardMenuItem other)
        {
            if (Order < other.Order) return -1;
            else if (Order > other.Order) return 1;
            return 0;
        }
    }
}
