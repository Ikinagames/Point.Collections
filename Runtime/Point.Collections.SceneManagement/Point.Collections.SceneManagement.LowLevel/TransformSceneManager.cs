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
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Point.Collections.Buffer.LowLevel;
using System.Collections.Generic;
using UnityEngine;

namespace Point.Collections.SceneManagement.LowLevel
{
    internal sealed class TransformSceneManager : StaticMonobehaviour<TransformSceneManager>
    {
        protected override bool EnableLog => base.EnableLog;
        protected override bool HideInInspector => base.HideInInspector;

        private bool m_ModifiedInThisFrame = false;
        private UnsafeTransformScene m_Scene;
        private List<Transform> m_TransformList = new List<Transform>();

        protected override void OnInitialize()
        {
            m_Scene = new UnsafeTransformScene(Unity.Collections.Allocator.Persistent);

            PointApplication.OnFrameUpdate += UpdateScene;
        }
        protected override void OnShutdown()
        {
            PointApplication.OnFrameUpdate -= UpdateScene;

            m_Scene.Dispose();
        }

        public static UnsafeReference<UnsafeTransform> Add(Transform tr)
        {
            if (PointApplication.IsShutdown) return default;

            if (Instance.m_Scene.RequireResize())
            {
                Instance.m_Scene.Resize(Instance.m_TransformList);
                "resized".ToLog();
            }

            var ptr = Instance.m_Scene.AddTransform(tr, new Transformation(tr));
            if (ptr.IsCreated)
            {
                Instance.m_TransformList.Add(tr);
            }

            Instance.m_ModifiedInThisFrame = true;
            return ptr;
        }
        public static void Remove(UnsafeReference<UnsafeTransform> ptr)
        {
            if (PointApplication.IsShutdown) return;

            int index = Instance.m_Scene.RemoveTransform(ptr);
            Instance.m_TransformList.RemoveAt(index);

            Instance.m_ModifiedInThisFrame = true;
        }

        private void UpdateScene()
        {
            if (m_ModifiedInThisFrame)
            {
                m_ModifiedInThisFrame = false;
                return;
            }

            m_Scene.Synchronize();
        }
    }
}
