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
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace Point.Collections.Editor
{
    /// <summary>
    /// <see cref="SearchWindow.Open{T}(SearchWindowContext, T)"/>
    /// </summary>
    public abstract class SearchProviderBase : ScriptableObject, ISearchWindowProvider
    {
        public event Action<SearchTreeEntry, SearchWindowContext> OnSelected;

        protected abstract string DisplayName { get; }

        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> list = new List<SearchTreeEntry>();
            list.Add(new SearchTreeGroupEntry(new GUIContent(DisplayName)));

            BuildSearchTree(context, list);

            return list;
        }
        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            if (OnSelectEntry(context, SearchTreeEntry))
            {
                OnSelected?.Invoke(SearchTreeEntry, context);
                return true;
            }
            return false;
        }

        protected virtual void BuildSearchTree(in SearchWindowContext ctx, in List<SearchTreeEntry> list)
        {
        }
        protected virtual bool OnSelectEntry(in SearchWindowContext ctx, in SearchTreeEntry entry)
        {
            return true;
        }
    }
    /// <summary>
    /// <inheritdoc cref="SearchProviderBase"/>
    /// </summary>
    /// <typeparam name="TTarget"></typeparam>
    public abstract class SearchProviderBase<TTarget> : ScriptableObject, ISearchWindowProvider
    {
        public event Action<Entry, SearchWindowContext> OnSelected;

        protected abstract string DisplayName { get; }

        public sealed class Entry : SearchTreeEntry
        {
            public static Entry Empty => new Entry(CoreGUI.EmptyContent) { level = 1 };

            public new TTarget userData
            {
                get => (TTarget)base.userData;
                set => base.userData = value;
            }

            public Entry(GUIContent content) : base(content)
            {
            }
        }

        List<SearchTreeEntry> ISearchWindowProvider.CreateSearchTree(SearchWindowContext context)
        {
            List<SearchTreeEntry> list = new List<SearchTreeEntry>();
            list.Add(new SearchTreeGroupEntry(new GUIContent(DisplayName)));

            List<Entry> entries = new List<Entry>();
            BuildSearchTree(context, entries);
            list.AddRange(entries);

            return list;
        }
        bool ISearchWindowProvider.OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
        {
            if (OnSelectEntry(context, SearchTreeEntry as Entry))
            {
                OnSelect(SearchTreeEntry as Entry, context);
                OnSelected?.Invoke(SearchTreeEntry as Entry, context);
                return true;
            }
            return false;
        }

        protected abstract void BuildSearchTree(in SearchWindowContext ctx, in List<Entry> list);
        protected virtual bool OnSelectEntry(in SearchWindowContext ctx, in Entry entry)
        {
            return true;
        }
        protected virtual void OnSelect(Entry entry, SearchWindowContext ctx) { }

        protected static Entry CreateEntry(string displayName, TTarget data, int level = 1)
        {
            return new Entry(new GUIContent(displayName, CoreGUI.EmptyIcon))
            {
                level = level,
                userData = data
            };
        }
    }
}

#endif