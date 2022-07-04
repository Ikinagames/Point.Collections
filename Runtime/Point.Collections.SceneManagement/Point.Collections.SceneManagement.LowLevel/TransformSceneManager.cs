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
        private UnsafeTransformScene m_Scene;

        //private CommandBuffer m_CommandBuffer;
        private UnsafeAllocator<float4x4> m_Matrices;
        private GraphicsBuffer m_TransformationGBuffer, m_MatricesGBuffer;


        protected override void OnInitialize()
        {
//            m_CommandBuffer = new CommandBuffer();
//#if UNITY_EDITOR
//            m_CommandBuffer.name = "Point.TransformSceneManager";
//#endif
            //m_CommandBuffer.SetExecutionFlags(CommandBufferExecutionFlags.AsyncCompute);

            m_Matrices = new UnsafeAllocator<float4x4>(UnsafeTransformScene.INIT_COUNT, Allocator.Persistent);

            m_TransformationGBuffer = new GraphicsBuffer(
                GraphicsBuffer.Target.Structured, UnsafeTransformScene.INIT_COUNT, UnsafeUtility.SizeOf<Transformation>());
            m_MatricesGBuffer = new GraphicsBuffer(
                GraphicsBuffer.Target.Structured, UnsafeTransformScene.INIT_COUNT, UnsafeUtility.SizeOf<float4x4>());
            m_Scene = new UnsafeTransformScene(Allocator.Persistent);

            "set cbuffer".ToLog();

            // https://forum.unity.com/threads/compute-shader-buffer-rwbuffer-vs-structuredbuffer-rwstructuredbuffer.755672/

            PointApplication.OnLateUpdate += UpdateScene;
        }
        protected override void OnShutdown()
        {
            PointApplication.OnLateUpdate -= UpdateScene;

            m_Scene.Dispose();

            m_Matrices.Dispose();
            m_TransformationGBuffer.Release();
            m_MatricesGBuffer.Release();
        }
        private void UpdateBuffer()
        {
            m_Scene.transformations.CopyToBuffer(m_TransformationGBuffer);

            int csMain = m_GetMatricesCS.FindKernel("CSMain");
            {
                m_GetMatricesCS.SetBuffer(csMain, "_Transforms", m_TransformationGBuffer);
                m_GetMatricesCS.SetBuffer(csMain, "_Result", m_MatricesGBuffer);
                m_GetMatricesCS.Dispatch(csMain, 16, 1, 1);
            }
        }
        private void UpdateCommand()
        {
            // TODO : temp code
            m_Matrices.ReadFromBuffer(m_MatricesGBuffer);

            for (int i = 0; i < m_BatchedData.Count; i++)
            {
                Mesh mesh = m_BatchedData[i].key.meshMaterial.Mesh;
                Material material = m_BatchedData[i].key.meshMaterial.Material;
                int subMeshIndex = m_BatchedData[i].key.subMeshIndex;

                Matrix4x4[] mats = ArrayPool<Matrix4x4>.Shared.Rent(m_BatchedData[i].graphics.Count);
                for (int j = 0; j < m_BatchedData[i].graphics.Count; j++)
                {
                    int matrixIndex = m_BatchedData[i].graphics[j].index;
                    mats[j] = m_Matrices[matrixIndex];
                }

                Graphics.DrawMeshInstanced(
                    mesh, subMeshIndex, material, mats, m_BatchedData[i].graphics.Count);

                ArrayPool<Matrix4x4>.Shared.Return(mats);
            }
        }
        //private void ExecuteCommand()
        //{
        //    Graphics.ExecuteCommandBuffer(m_CommandBuffer);
        //}

        private unsafe void Resize()
        {
            int targetLength = m_Scene.length * 2;

            m_Matrices.Resize(targetLength);

            GraphicsBuffer newTrGBuffer = new GraphicsBuffer(
                GraphicsBuffer.Target.Structured, targetLength, UnsafeUtility.SizeOf<Transformation>());
            m_TransformationGBuffer.Release();
            m_TransformationGBuffer = newTrGBuffer;

            var newMatricsGBuffer = new GraphicsBuffer(
                GraphicsBuffer.Target.Structured, targetLength, UnsafeUtility.SizeOf<float4x4>());
            m_MatricesGBuffer.Release();
            m_MatricesGBuffer = newMatricsGBuffer;

            m_Scene.Resize(targetLength);

            m_GetMatricesCS.SetConstantBuffer("_Transforms", m_TransformationGBuffer, 0,
                m_TransformationGBuffer.count * m_TransformationGBuffer.stride);
            m_GetMatricesCS.SetConstantBuffer("Result", m_MatricesGBuffer, 0,
                m_MatricesGBuffer.count * m_MatricesGBuffer.stride);

            "resized".ToLog();
        }

        public static UnsafeReference<UnsafeTransform> Add(Transform tr)
        {
            if (PointApplication.IsShutdown) return default;

            if (Instance.m_Scene.RequireResize())
            {
                Instance.Resize();
            }

            UnsafeReference<UnsafeTransform> ptr = Instance.m_Scene.AddTransform(new Transformation(tr));
            if (!ptr.IsCreated)
            {
                "?? error".ToLogError();
                Debug.Break();
                return ptr;
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

                ren.enabled = false;
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

        private struct MeshMaterial : IEquatable<MeshMaterial>
        {
            private static Dictionary<int, Material> s_MaterialMap = new Dictionary<int, Material>();
            private static Dictionary<int, Mesh> s_MeshMap = new Dictionary<int, Mesh>();

            private readonly int mesh;
            private readonly int material;

            public Mesh Mesh => s_MeshMap[mesh];
            public Material Material => s_MaterialMap[material];

            public MeshMaterial(Mesh mesh, Material material)
            {
                this.mesh = mesh.GetInstanceID();
                this.material = material.GetInstanceID();
                Mesh.GetNativeIndexBufferPtr();
            }

            public bool Equals(MeshMaterial other) 
                => mesh == other.mesh && material == other.material;

            public static MeshMaterial GetMeshMaterialKey(Mesh mesh, Material material)
            {
                s_MaterialMap[material.GetInstanceID()] = material;
                s_MeshMap[mesh.GetInstanceID()] = mesh;

                return new MeshMaterial(mesh, material);
            }
        }

        private struct BatchedDataKey : IEquatable<BatchedDataKey>
        {
            public MeshMaterial meshMaterial;
            public int subMeshIndex;

            public BatchedDataKey(MeshMaterial key, int meshIndex)
            {
                this.meshMaterial = key;
                subMeshIndex = meshIndex;
            }

            public bool Equals(BatchedDataKey other)
            {
                return meshMaterial.Equals(other.meshMaterial) && subMeshIndex == other.subMeshIndex;
            }
        }
        private struct BatchedData
        {
            public BatchedDataKey key;

            public List<UnsafeGraphicsModel> graphics;
        }

        private Dictionary<BatchedDataKey, int> m_BatchedDataHashMap = new Dictionary<BatchedDataKey, int>();

        private List<BatchedData> m_BatchedData = new List<BatchedData>();

        private static BatchedData GetBatchedData(Mesh mesh, Material material, int subMeshIndex)
        {
            BatchedDataKey key = new BatchedDataKey(MeshMaterial.GetMeshMaterialKey(mesh, material), subMeshIndex);

            if (!Instance.m_BatchedDataHashMap.TryGetValue(key, out int index))
            {
                index = Instance.m_BatchedData.Count;
                Instance.m_BatchedData.Add(new BatchedData
                {
                    key = key,
                    graphics = new List<UnsafeGraphicsModel>()
                });
                Instance.m_BatchedDataHashMap.Add(key, index);
            }

            return Instance.m_BatchedData[index];
        }
        private static void SetBatchedData(Mesh mesh, Material material, BatchedData data)
        {
            if (!Instance.m_BatchedDataHashMap.TryGetValue(data.key, out int index))
            {
                Assert.IsTrue(false);
            }

            Instance.m_BatchedData[index] = data;
        }

        // https://toqoz.fyi/thousands-of-meshes.html
        private static void SetupBuffers()
        {
            GraphicsBuffer argGBuffer = new GraphicsBuffer(
                GraphicsBuffer.Target.IndirectArguments, 1, 5 * sizeof(uint));

            Graphics.DrawMeshInstancedIndirect(null, 0, null, default, argGBuffer);
        }
        private static void SetupBatchedData(BatchedData data, ComputeBuffer argBuffer)
        {
            Mesh mesh = data.key.meshMaterial.Mesh;
            Material material = data.key.meshMaterial.Material;

            // 0 == number of triangle indices, 1 == population, others are only relevant if drawing submeshes.
            uint[] args = new uint[5]
            {
                (uint)mesh.GetIndexCount(data.key.subMeshIndex),
                (uint)data.graphics.Count,
                (uint)mesh.GetIndexStart(data.key.subMeshIndex),
                (uint)mesh.GetBaseVertex(data.key.subMeshIndex),
                0
            };

            //material.set
        }
        private void SendDrawCalls()
        {
            for (int i = 0; i < m_BatchedData.Count; i++)
            {
                BatchedData data = m_BatchedData[i];
                //Material material = data.key.Material;

                GraphicsBuffer gbuffer = new GraphicsBuffer(GraphicsBuffer.Target.Vertex, 1, sizeof(int));
                //gbuffer.SetData<int>(,);
                //Graphics.drawmesh
                //float4x4.TRS
            }
        }

        #endregion

        private void BuildModel(UnsafeReference<UnsafeTransform> transform, 
            Mesh mesh, Material[] materials)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                if (!materials[i].enableInstancing)
                {
                    materials[i] = new Material(materials[i]);
                    materials[i].enableInstancing = true;
                }

                BatchedData data = GetBatchedData(mesh, materials[i], i);

                UnsafeGraphicsModel model 
                    = new UnsafeGraphicsModel(transform, false);
                data.graphics.Add(model);

                SetBatchedData(mesh, materials[i], data);
            }
        }
        
        private void UpdateScene()
        {
            if (m_ModifiedInThisFrame)
            {
                m_ModifiedInThisFrame = false;
                return;
            }

            UpdateBuffer();
            UpdateCommand();
            //ExecuteCommand();

            //for (int i = 0; i < m_Scene.count; i++)
            //{
            //    $"{m_Matrices[i]}".ToLog();
            //}

            ////m_Scene.Synchronize();
            //JobHandle.ScheduleBatchedJobs();
        }
    }
}
