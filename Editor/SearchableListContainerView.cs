﻿// Copyright 2022 Ikina Games
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
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public class SearchableListContainerView : ListContainerView
    {
        public new class UxmlFactory : UxmlFactory<SearchableListContainerView, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription
            {
                name = "text",
                defaultValue = "NAME"
            };

            public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
            {
                get { yield break; }
            }
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                SearchableListContainerView ate = ve as SearchableListContainerView;

                ate.label = m_Text.GetValueFromBag(bag, cc);
            }
        }

        private ToolbarSearchField m_SearchField;
        private string m_SearchString;

        public event Action<string> OnSearchStringChanged;

        public SearchableListContainerView() : base() { }
        public SearchableListContainerView(string text) : base(text)
        {
        }
        //public SearchableListContainerView(SerializedProperty property) : base(property) { }

        protected override void AfterCreateHeaderContainer(VisualElement headerContainer)
        {
            VisualElement root = new VisualElement();
            hierarchy.Add(root);

            m_SearchField = new ToolbarSearchField();
            m_SearchField.style.width = new StyleLength(new Length(98, LengthUnit.Percent));
            m_SearchField.style.Hide(!isExpanded);
            root.Add(m_SearchField);

            m_SearchField.RegisterValueChangedCallback(SearchFieldChanged);
        }
        protected override void OnExpanded(bool expand)
        {
            m_SearchField.style.Hide(!expand);
        }

        private void SearchFieldChanged(ChangeEvent<string> e)
        {
            m_SearchString = e.newValue;

            OnSearchStringChanged?.Invoke(e.newValue);
        }
    }
}

#endif