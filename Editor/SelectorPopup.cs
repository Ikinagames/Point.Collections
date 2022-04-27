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
using UnityEngine;
#if UNITY_MATHEMATICS
using UnityEditor.IMGUI.Controls;
using System.Threading.Tasks;
#endif

namespace Point.Collections.Editor
{
    public sealed class SelectorPopup<T, TA> : PopupWindowContent
    {
        private static SelectorPopup<T, TA> m_Window;

        SelectorTreeView m_Tree;
        TreeViewState m_TreeState;
        bool m_ShouldClose;
        string m_CurrentName = string.Empty;
        SearchField m_SearchField;

        private bool m_Closed = false;

        IList<TA> m_List;
        Func<TA, T> m_Getter;
        Action<T> m_Setter;
        Func<TA, string> m_GetName;
        T m_NoneValue;

        public bool Closed => m_Closed;

        void ForceClose()
        {
            m_ShouldClose = true;
        }
        public async Task WaitForClose()
        {
            while (!m_Closed)
            {
                await Task.Delay(100);
            }
        }
        public override void OnClose()
        {
            m_Closed = true;
        }

        private SelectorPopup(Action<T> setter, Func<TA, T> getter, IList<TA> list, T noneValue, Func<TA, string> getName)
        {
            m_SearchField = new SearchField();
            m_Setter = setter;
            m_Getter = getter;

            m_NoneValue = noneValue;
            m_GetName = getName;

            m_List = list;
        }
        public static SelectorPopup<T, TA> GetWindow(IList<TA> list, Action<T> setter, Func<TA, T> getter, T noneValue, Func<TA, string> getName = null)
        {
            return new SelectorPopup<T, TA>(setter, getter, list, noneValue, getName);
        }
        public override void OnOpen()
        {
            m_SearchField.SetFocus();
            base.OnOpen();
        }
        public override Vector2 GetWindowSize()
        {
            Vector2 result = base.GetWindowSize();
            result.x += 80;
            return result;
        }
        public override void OnGUI(Rect rect)
        {
            int border = 4;
            int topPadding = 12;
            int searchHeight = 20;
            var searchRect = new Rect(border, topPadding, rect.width - border * 2, searchHeight);
            var remainTop = topPadding + searchHeight + border;
            var remainingRect = new Rect(border, topPadding + searchHeight + border, rect.width - border * 2, rect.height - remainTop - border);
            m_CurrentName = m_SearchField.OnGUI(searchRect, m_CurrentName);

            if (m_Tree == null)
            {
                if (m_TreeState == null)
                    m_TreeState = new TreeViewState();
                m_Tree = new SelectorTreeView(m_TreeState, this);
                m_Tree.Reload();
            }

            m_Tree.searchString = m_CurrentName;
            m_Tree.OnGUI(remainingRect);

            if (m_ShouldClose)
            {
                GUIUtility.hotControl = 0;

                GUI.changed = true;
                editorWindow.Close();
            }
        }

        private class AttributeTreeViewItem : TreeViewItem
        {
            public T Value;

            public AttributeTreeViewItem(int id, int depth, string displayName, T value)
                : base(id, depth, displayName)
            {
                Value = value;
            }
        }
        private class SelectorTreeView : TreeView
        {
            SelectorPopup<T, TA> m_Popup;
            Texture2D m_WarningIcon;

            public SelectorTreeView(TreeViewState state, SelectorPopup<T, TA> popup)
                : base(state)
            {
                m_Popup = popup;
                showBorder = true;
                showAlternatingRowBackgrounds = true;
                m_WarningIcon = EditorGUIUtility.FindTexture("console.warnicon");
            }

            protected override bool CanMultiSelect(TreeViewItem item) => false;
            protected override void SelectionChanged(IList<int> selectedIds)
            {
                if (selectedIds != null && selectedIds.Count == 1)
                {
                    var assetRefItem = FindItem(selectedIds[0], rootItem) as AttributeTreeViewItem;
                    m_Popup.m_Setter.Invoke(assetRefItem.Value);

                    m_Popup.ForceClose();
                }
            }

            protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
            {
                if (string.IsNullOrEmpty(searchString))
                {
                    return base.BuildRows(root);
                }

                List<TreeViewItem> rows = new List<TreeViewItem>();

                foreach (var child in rootItem.children)
                {
                    if (child.displayName.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0)
                        rows.Add(child);
                }

                return rows;
            }

            const string noAssetString = "Null";
            protected override TreeViewItem BuildRoot()
            {
                var root = new TreeViewItem(-1, -1);

                root.AddChild(new AttributeTreeViewItem(0, 0, "None", m_Popup.m_NoneValue));

                IList<TA> list = m_Popup.m_List;
                for (int i = 0; i < list.Count; i++)
                {
                    string displayName;
                    if (m_Popup.m_GetName != null) displayName = m_Popup.m_GetName.Invoke(list[i]);
                    else displayName = list[i].ToString();

                    root.AddChild(new AttributeTreeViewItem(i + 1, 0, displayName, m_Popup.m_Getter.Invoke(list[i])));
                }
                return root;
            }
        }
    }
}

#endif