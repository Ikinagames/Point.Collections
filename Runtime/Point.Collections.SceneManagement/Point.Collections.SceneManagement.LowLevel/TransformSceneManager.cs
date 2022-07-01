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
using Unity.Jobs;
using UnityEngine;

namespace Point.Collections.SceneManagement.LowLevel
{
    internal sealed class TransformSceneManager : StaticMonobehaviour<TransformSceneManager>
    {
        protected override bool EnableLog => base.EnableLog;
        protected override bool HideInInspector => base.HideInInspector;

        private bool m_ModifiedInThisFrame = false;
        private UnsafeTransformScene m_Scene;
        
        protected override void OnInitialize()
        {
            m_Scene = new UnsafeTransformScene(Unity.Collections.Allocator.Persistent);

            PointApplication.OnLateUpdate += UpdateScene;
        }
        protected override void OnShutdown()
        {
            PointApplication.OnLateUpdate -= UpdateScene;

            m_Scene.Dispose();
        }

        public static UnsafeReference<UnsafeTransform> Add(Transform tr)
        {
            if (PointApplication.IsShutdown) return default;

            if (Instance.m_Scene.RequireResize())
            {
                Instance.m_Scene.Resize();
                "resized".ToLog();
            }

            UnsafeReference<UnsafeTransform> ptr = Instance.m_Scene.AddTransform(new Transformation(tr));
            if (!ptr.IsCreated)
            {
                "?? error".ToLogError();
            }

            Renderer[] renderers = tr.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < renderers.Length; i++)
            {
                Renderer ren = renderers[i];

                Mesh mesh;
                if (ren is MeshRenderer meshRen)
                {
                    mesh = meshRen.GetComponent<MeshFilter>().sharedMesh;
                }
                else
                {
                    $"not support {ren.GetType().Name}".ToLog();
                    continue;
                }

                Instance.BuildModel(ptr, mesh, ren.sharedMaterials);
            }

            Instance.m_ModifiedInThisFrame = true;
            return ptr;
        }
        public static void Remove(UnsafeReference<UnsafeTransform> ptr)
        {
            if (PointApplication.IsShutdown) return;

            int index = Instance.m_Scene.RemoveTransform(ptr);

            Instance.m_ModifiedInThisFrame = true;
        }

        #region BatchedData

        private struct BatchedData
        {
            public Material material;
            public List<UnsafeGraphicsModel> graphics;
        }
        private Dictionary<Material, int> m_BatchedDataHashMap = new Dictionary<Material, int>();
        private List<BatchedData> m_BatchedData = new List<BatchedData>();

        private static BatchedData GetBatchedData(Material material)
        {
            if (!Instance.m_BatchedDataHashMap.TryGetValue(material, out int index))
            {
                index = Instance.m_BatchedData.Count;
                Instance.m_BatchedData.Add(new BatchedData
                {
                    material = material,
                    graphics = new List<UnsafeGraphicsModel>()
                });
                Instance.m_BatchedDataHashMap.Add(material, index);
            }

            return Instance.m_BatchedData[index];
        }
        private static void SetBatchedData(Material material, BatchedData data)
        {
            if (!Instance.m_BatchedDataHashMap.TryGetValue(material, out int index))
            {
                index = Instance.m_BatchedData.Count;
                Instance.m_BatchedData.Add(new BatchedData
                {
                    material = material,
                    graphics = new List<UnsafeGraphicsModel>(),
                });
                Instance.m_BatchedDataHashMap.Add(material, index);
            }

            Instance.m_BatchedData[index] = data;
        }

        #endregion

        private void BuildModel(UnsafeReference<UnsafeTransform> transform, 
            Mesh mesh, Material[] materials)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                BatchedData data = GetBatchedData(materials[i]);

                UnsafeGraphicsModel model 
                    = new UnsafeGraphicsModel(transform, materials[i], mesh, i, false);
                data.graphics.Add(model);

                SetBatchedData(materials[i], data);
            }
        }

        private void UpdateScene()
        {
            if (m_ModifiedInThisFrame)
            {
                m_ModifiedInThisFrame = false;
                return;
            }

            m_Scene.Synchronize();
            JobHandle.ScheduleBatchedJobs();
        }
    }
}
