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
using UnityEditor;

namespace Point.Collections.Editor
{
    public abstract class SceneViewEditor : IDisposable
    {
        protected virtual bool HideTools => true;

        public bool IsOpened { get; private set; } = false;

        ~SceneViewEditor()
        {
            Dispose();
        }

        public void Open()
        {
            if (IsOpened) return;

            if (HideTools) Tools.hidden = true;
            SceneView.duringSceneGui += OnSceneView;
            SceneView.RepaintAll();
            IsOpened = true;

            OnOpen();
        }
        public void Close()
        {
            if (!IsOpened) return;

            Tools.hidden = false;
            SceneView.duringSceneGui -= OnSceneView;
            SceneView.RepaintAll();
            IsOpened = false;

            OnClose();
        }
        public void Dispose()
        {
            Close();
        }

        private void OnSceneView(SceneView sceneView)
        {
            Handles.BeginGUI();
            OnSceneGUI(sceneView);
            Handles.EndGUI();

            using (new Handles.DrawingScope())
            {
                OnSceneHandles(sceneView);
            }
        }
        protected virtual void OnSceneGUI(SceneView sceneView) { }
        protected virtual void OnSceneHandles(SceneView sceneView) { }

        protected virtual void OnOpen() { }
        protected virtual void OnClose() { }
    }
}

#endif