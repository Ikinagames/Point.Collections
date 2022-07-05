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

using Point.Collections.Buffer;
using Point.Collections.Buffer.LowLevel;
using System;
using System.Buffers;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

namespace Point.Collections.SceneManagement.LowLevel
{
    internal sealed class TransformSceneManager : StaticMonobehaviour<TransformSceneManager>
    {
        [SerializeField] private ComputeShader m_GetMatricesCS;

        protected override bool EnableLog => base.EnableLog;
        protected override bool HideInInspector => base.HideInInspector;

        private bool m_ModifiedInThisFrame = false;
        private List<UnsafeBatchedScene> m_BatchedData = new List<UnsafeBatchedScene>();
        private Dictionary<BatchedKey, UnsafeBatchedScene> m_BatchedDataMap = new Dictionary<BatchedKey, UnsafeBatchedScene>();
        
        public struct BatchedKey : IEquatable<BatchedKey>
        {
            private readonly MeshMaterial meshMaterial;
            private readonly int subMeshIndex;

            public BatchedKey(Mesh mesh, Material material, int subMeshIndex)
            {
                meshMaterial = MeshMaterial.GetMeshMaterial(mesh, material);
                this.subMeshIndex = subMeshIndex;
            }
            public bool Equals(BatchedKey other) => meshMaterial.Equals(other.meshMaterial) && subMeshIndex == other.subMeshIndex;
        }
        public struct TransformInterface : IValidation
        {
            private SceneKey[][] m_Indices;

            public TransformInterface(SceneKey[][] indices)
            {
                m_Indices = indices;
            }

            public bool IsValid()
            {
                return m_Indices != null && m_Indices.Length > 0;
            }

            public void SetPosition(float3 pos)
            {
                for (int i = 0; i < m_Indices.Length; i++)
                {
                    SetPosition(i, pos);
                }
            }
            public void SetPosition(int index, float3 pos)
            {
                for (int i = 0; i < m_Indices[index].Length; i++)
                {
                    BatchedKey key = m_Indices[index][i].batchedKey;
                    int sceneIndex = m_Indices[index][i].sceneIndex;

                    UnsafeBatchedScene data = Instance.m_BatchedDataMap[key];
                    data.scene.GetTransform(sceneIndex).Value.localPosition = pos;
                }
            }
            public void SetRotation(quaternion rot)
            {
                for (int i = 0; i < m_Indices.Length; i++)
                {
                    SetRotation(i, rot);
                }
            }
            public void SetRotation(int index, quaternion rot)
            {
                for (int i = 0; i < m_Indices[index].Length; i++)
                {
                    BatchedKey key = m_Indices[index][i].batchedKey;
                    int sceneIndex = m_Indices[index][i].sceneIndex;

                    UnsafeBatchedScene data = Instance.m_BatchedDataMap[key];
                    data.scene.GetTransform(sceneIndex).Value.localRotation = rot;
                }
            }
            public void SetScale(float3 scale)
            {
                for (int i = 0; i < m_Indices.Length; i++)
                {
                    SetScale(i, scale);
                }
            }
            public void SetScale(int index, float3 scale)
            {
                for (int i = 0; i < m_Indices[index].Length; i++)
                {
                    BatchedKey key = m_Indices[index][i].batchedKey;
                    int sceneIndex = m_Indices[index][i].sceneIndex;

                    UnsafeBatchedScene data = Instance.m_BatchedDataMap[key];
                    data.scene.GetTransform(sceneIndex).Value.localScale = scale;
                }
            }

            public void Release()
            {
                for (int i = 0; i < m_Indices.Length; i++)
                {
                    for (int j = 0; j < m_Indices[i].Length; j++)
                    {
                        BatchedKey key = m_Indices[i][j].batchedKey;
                        int sceneIndex = m_Indices[i][j].sceneIndex;

                        UnsafeBatchedScene data = Instance.m_BatchedDataMap[key];

                        data.Remove(sceneIndex);
                    }
                }
            }
        }

        protected override void OnInitialize()
        {
            PointApplication.OnLateUpdate += UpdateScene;
        }
        protected override void OnShutdown()
        {
            PointApplication.OnLateUpdate -= UpdateScene;

            for (int i = 0; i < m_BatchedData.Count; i++)
            {
                m_BatchedData[i].Dispose();
            }
        }

        public static TransformInterface Add(Transform tr)
        {
            if (PointApplication.IsShutdown)
            {
                return default;
            }

            Renderer[] renderers = tr.GetComponentsInChildren<Renderer>();
            SceneKey[][] vs = new SceneKey[renderers.Length][];
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

                ren.enabled = false;
                vs[i] = Instance.BuildModel(mesh, ren.sharedMaterials, new Transformation(tr));
            }

            Instance.m_ModifiedInThisFrame = true;
            return new TransformInterface(vs);
        }
        public static void Remove(TransformInterface ptr)
        {
            if (PointApplication.IsShutdown) return;

            ptr.Release();

            Instance.m_ModifiedInThisFrame = true;
        }

        public struct SceneKey
        {
            public BatchedKey batchedKey;
            public int sceneIndex;
        }
        private SceneKey[] BuildModel(Mesh mesh, Material[] materials, Transformation tr)
        {
            SceneKey[] indices = new SceneKey[materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                BatchedKey key = new BatchedKey(mesh, materials[i], i);
                if (!m_BatchedDataMap.TryGetValue(key, out UnsafeBatchedScene data))
                {
                    data = new UnsafeBatchedScene(m_GetMatricesCS, mesh, materials[i], i);
                    m_BatchedData.Add(data);
                    m_BatchedDataMap.Add(key, data);
                }

                indices[i] = new SceneKey
                {
                    batchedKey = key,
                    sceneIndex = data.Add(tr)
                };
            }

            return indices;

            //for (int i = 0; i < materials.Length; i++)
            //{
            //    if (!materials[i].enableInstancing)
            //    {
            //        materials[i] = new Material(materials[i]);
            //        materials[i].enableInstancing = true;
            //    }

            //    BatchedData data = GetBatchedData(mesh, materials[i], i);

            //    UnsafeGraphicsModel model
            //        = new UnsafeGraphicsModel(transform, false);
            //    data.graphics.Add(model);
            //}
        }

        private void UpdateScene()
        {
            if (m_ModifiedInThisFrame)
            {
                m_ModifiedInThisFrame = false;
                return;
            }

        }
    }
}
