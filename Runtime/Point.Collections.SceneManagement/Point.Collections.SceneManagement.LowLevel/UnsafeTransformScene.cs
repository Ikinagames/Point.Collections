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

#if UNITY_2020_1_OR_NEWER && UNITY_COLLECTIONS
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
using Point.Collections.Buffer.LowLevel;
using Point.Collections.Threading;
using System;
#if SYSTEM_BUFFER
using System.Buffers;
#endif
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Jobs;
using UnityEngine.Rendering;

namespace Point.Collections.SceneManagement.LowLevel
{
    [BurstCompatible]
    public struct UnsafeTransformScene : IValidation, IDisposable
    {
        public const int INIT_COUNT = 4096;

        [BurstCompatible]
        public struct Data : IDisposable
        {
            private static ProfilerMarker GetTransformMarker = new ProfilerMarker(
                ProfilerCategory.Scripts, "UnsafeTransformScene.Data.GetTransform");

            public UnsafeAllocator<Transformation> transformations;
            private UnsafeAllocator<int> indices;

            private UnsafeFixedListWrapper<int> indexList;

            public int capacity => transformations.Length;
            public int count => transformations.Length - indexList.Length;

            public Data(int count, Allocator allocator)
            {
                transformations = new UnsafeAllocator<Transformation>(
                    count,
                    allocator);
                indices = new UnsafeAllocator<int>(
                    count,
                    allocator);

                indexList = new UnsafeFixedListWrapper<int>(indices, 0);
                for (int i = 0; i < count; i++)
                {
                    indexList.AddNoResize(i);
                }
            }

            public int Add()
            {
                if (RequireResize())
                {
                    "resizeing".ToLog();
                    Resize(indexList.Capacity * 2);
                }

                int index = indexList[0];
                indexList.RemoveAtSwapback(0);

                return index;
            }
            public void ReserveIndex(int index)
            {
                transformations.RemoveAtSwapBack(index);

                indexList.InsertNoResize(0, index);
                indexList.Sort(new Comparer());
            }
            private struct Comparer : IComparer<int>
            {
                public int Compare(int x, int y)
                {
                    if (x < y) return -1;
                    else if (x > y) return 1;
                    return 0;
                }
            }

            public UnsafeReference<Transformation> GetTransform(int index)
            {
                using (GetTransformMarker.Auto())
                {
                    //$"{index} :: {count}".ToLog();
                    if (index >= count)
                    {
                        $"corrected {index} -> {index - (index - count + 1)}".ToLog();
                        index -= (index - count + 1);
                    }
                    //for (int i = 0; i < count; i++)
                    //{
                    //    bool found = indexList.Contains(i);
                    //    if (found)
                    //    {
                    //        index--;
                    //        
                    //    }
                    //}
                    
                    return transformations.ElementAt(index);
                }
            }

            public bool RequireResize() => indexList.Length == 0;
            public int Resize() => Resize(indexList.Capacity * 2);
            public int Resize(int length)
            {
                transformations.Resize(length);
                indices.Resize(length);

                indexList = new UnsafeFixedListWrapper<int>(
                    indices, indexList.Length);

                return length;
            }
            public void Dispose()
            {
                transformations.Dispose();
                indices.Dispose();
            }
        }

        private UnsafeAllocator<Data> m_Data;
        private JobHandle jobHandle;

        public UnsafeAllocator<Data> buffer => m_Data;
        public UnsafeAllocator<Transformation> transformations => buffer[0].transformations;
        public int capacity => buffer[0].capacity;
        public int count => buffer[0].count;
        
        public unsafe UnsafeTransformScene(Allocator allocator, int initCount = INIT_COUNT)
        {
            m_Data = new UnsafeAllocator<Data>(1, allocator);
            m_Data[0] = new Data(initCount, allocator);

            jobHandle = default(JobHandle);
        }

        public UnsafeReference<Transformation> GetTransform(int index) => buffer[0].GetTransform(index);

        public bool IsValid()
        {
            return m_Data.IsCreated;
        }

        public bool RequireResize() => m_Data[0].RequireResize();
        public int Resize() => m_Data[0].Resize();
        public int AddTransform(Transformation transformation = default(Transformation))
        {
            if (transformation.Equals(default(Transformation)))
            {
                transformation = Transformation.identity;
            }

            int index = m_Data[0].Add();
            m_Data[0].transformations[index] = transformation;

            $"added {index}".ToLog();
            return index;
        }
        public void RemoveTransform(int index)
        {
            m_Data[0].ReserveIndex(index);

            $"reserved {index}".ToLog();
        }

        public void Complete()
        {
            jobHandle.Complete();
        }
        public void Dispose()
        {
            Complete();

            m_Data[0].Dispose();
            m_Data.Dispose();
            //transformAccessArray.Dispose();
        }
    }

    internal sealed class UnsafeBatchedScene : IDisposable
    {
        private static readonly int 
            s_GetMatrices_TR = Shader.PropertyToID("_Transforms"),
            s_GetMatrices_Result = Shader.PropertyToID("_Result"),
            s_Material_Matrices = Shader.PropertyToID("_Matrices");
        private readonly ComputeShader m_GetMatricesCS;

        public Bounds bounds;

        private readonly Mesh mesh;
        private readonly Material material;
        private readonly int subMeshIndex;
        public readonly UnsafeTransformScene scene;
        private ComputeBuffer transformationBuffer, matricesBuffer, argumentBuffer;

        private bool m_CountChanged = true;

        public UnsafeBatchedScene(
            ComputeShader getMatricesCS,
            Mesh mesh, Material material, int subMeshIndex)
        {
            m_GetMatricesCS = getMatricesCS;

            this.mesh = mesh;
            this.material = material;
            this.subMeshIndex = subMeshIndex;
            scene = new UnsafeTransformScene(Allocator.Persistent);

            transformationBuffer = new ComputeBuffer(
                UnsafeTransformScene.INIT_COUNT, UnsafeUtility.SizeOf<Transformation>(), ComputeBufferType.Structured);
            matricesBuffer = new ComputeBuffer(
                UnsafeTransformScene.INIT_COUNT, UnsafeUtility.SizeOf<float4x4>(), ComputeBufferType.Structured);
            argumentBuffer = new ComputeBuffer(
                5, sizeof(uint) * 5, ComputeBufferType.IndirectArguments, ComputeBufferMode.SubUpdates);

            UpdateMaterial();
        }

        public int Add(Transformation tr)
        {
            if (scene.RequireResize())
            {
                int newLength = scene.Resize();

                transformationBuffer.Dispose();
                matricesBuffer.Dispose();

                transformationBuffer = new ComputeBuffer(
                    newLength, UnsafeUtility.SizeOf<Transformation>(), ComputeBufferType.Structured);
                matricesBuffer = new ComputeBuffer(
                    newLength, UnsafeUtility.SizeOf<float4x4>(), ComputeBufferType.Structured);

                UpdateMaterial();
            }

            int index = scene.AddTransform(tr);

            m_CountChanged = true;
            return index;
        }
        public void Remove(int index)
        {
            scene.RemoveTransform(index);

            m_CountChanged = true;
        }

        private void UpdateMaterial()
        {
            material.SetBuffer(s_Material_Matrices, matricesBuffer);
        }

        private void SetupArgumentBuffer(Mesh mesh)
        {
            if (!m_CountChanged) return;

            NativeArray<uint> arr = argumentBuffer.BeginWrite<uint>(0, 5);
            arr[0] = mesh.GetIndexCount(subMeshIndex);
            arr[1] = (uint)scene.count;
            arr[2] = mesh.GetIndexStart(subMeshIndex);
            arr[3] = (uint)mesh.GetBaseVertex(subMeshIndex);
            arr[4] = 0;
            argumentBuffer.EndWrite<uint>(5);

            m_CountChanged = false;
        }
        
        private void Setup(Mesh mesh, Material material)
        {
            SetupArgumentBuffer(mesh);
            //UpdateMatrices(material);
        }

        private void UpdateBuffer()
        {
            scene.transformations.CopyToBuffer(transformationBuffer);
            m_GetMatricesCS.SetBuffer(0, s_GetMatrices_TR, transformationBuffer);
            m_GetMatricesCS.SetBuffer(0, s_GetMatrices_Result, matricesBuffer);
            m_GetMatricesCS.Dispatch(0, 32, 1, 1);
        }

        public void Draw()
        {
            UpdateBuffer();

            int count = scene.count;
            float4x4[] rawMats
#if SYSTEM_BUFFER
                = ArrayPool<float4x4>.Shared.Rent(count);
#else
                = new float4x4[count];
#endif
            matricesBuffer.GetData(rawMats, 0, 0, count);

            Matrix4x4[] mats
#if SYSTEM_BUFFER
                = ArrayPool<Matrix4x4>.Shared.Rent(count);
#else
                = new Matrix4x4[count];
#endif
            for (int i = 0; i < count; i++)
            {
                mats[i] = rawMats[i];
                //$"{mats[i]}".ToLog();
            }
#if SYSTEM_BUFFER
            ArrayPool<float4x4>.Shared.Return(rawMats);
#endif

            Graphics.DrawMeshInstanced(
                mesh, subMeshIndex, material, mats, count
                );
            //$"{count} drawed".ToLog();
#if SYSTEM_BUFFER
            ArrayPool<Matrix4x4>.Shared.Return(mats);
#endif
        }
        public void DrawIndirect()
        {
            UpdateBuffer();
            Setup(mesh, material);

            Graphics.DrawMeshInstancedIndirect(
                mesh, subMeshIndex, material, bounds, argumentBuffer
                );
        }

        public void Dispose()
        {
            transformationBuffer.Dispose();
            matricesBuffer.Dispose();
            argumentBuffer.Dispose();

            scene.Dispose();
        }
    }

    struct MeshMaterial : IEquatable<MeshMaterial>
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

        public static MeshMaterial GetMeshMaterial(Mesh mesh, Material material)
        {
            s_MaterialMap[material.GetInstanceID()] = material;
            s_MeshMap[mesh.GetInstanceID()] = mesh;

            return new MeshMaterial(mesh, material);
        }
    }
}

#endif