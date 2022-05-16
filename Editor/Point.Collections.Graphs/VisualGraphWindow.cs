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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2020_1_OR_NEWER
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if ENABLE_NODEGRAPH && UNITYENGINE

using GraphProcessor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public class VisualGraphWindow : BaseGraphWindow
    {
        protected override void OnDestroy()
        {
            graphView?.Dispose();
        }
        protected override void InitializeWindow(BaseGraph graph)
        {
            // Set the window title
            titleContent = new GUIContent("Default Graph");

            // Here you can use the default BaseGraphView or a custom one (see section below)
            var graphView = new VisualGraphView(this);

            GridBackground background = new GridBackground();
            graphView.Insert(0, background);
            background.StretchToParentSize();

            graphView.Add(new MiniMapView(graphView));

            rootView.Add(graphView);
        }
    }
}

#endif