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
using Point.Collections.Buffer.LowLevel;
using Point.Collections.Threading;
using System;
using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Jobs;
using UnityEngine.Rendering;
#else
#define POINT_COLLECTIONS_NATIVE
#endif

namespace Point.Collections.SceneManagement.LowLevel
{
//#if UNITYENGINE
//    [BurstCompatible]
//#endif
//    public struct UnsafeTransform : IEquatable<UnsafeTransform>
//    {
//        public static UnsafeTransform Invalid => new UnsafeTransform(-1, default);
//        public struct ReadOnly
//        {
//            public readonly int hashCode;
//            public readonly float4x4 localToWorld, worldToLocal;
//            public readonly quaternion parentRotation;
//            public readonly float3 parentScale;
//            public readonly Transformation transformation;

//            public ReadOnly(UnsafeTransform tr)
//            {
//                hashCode = tr.hashCode;
//                if (tr.parent.IsCreated)
//                {
//                    var parentTr = tr.parent.Value.m_Transformation.Value;
//                    localToWorld = parentTr.localToWorld;
//                    worldToLocal = parentTr.worldToLocal;
//                    parentRotation = parentTr.localRotation;
//                    parentScale = parentTr.localScale;
//                }
//                else
//                {
//                    localToWorld = float4x4.identity;
//                    worldToLocal = float4x4.identity;
//                    parentRotation = quaternion.identity;
//                    parentScale = 1;
//                }

//                transformation = tr.m_Transformation.Value;
//            }
//        }

//        public readonly int index, hashCode;

//        private UnsafeReference<UnsafeTransform> parent;

//        private AtomicOperator op;

//        public Transformation transformation
//        {
//            get
//            {
//                op.Enter();
//                var boxed = m_Transformation.Value;
//                op.Exit();

//                return boxed;
//            }
//            set
//            {
//                op.Enter();
//                m_Transformation.Value = value;
//                op.Exit();
//            }
//        }

//        public UnsafeTransform(int index, UnsafeReference<Transformation> tr)
//        {
//            this = default(UnsafeTransform);
//            this.index = index;
//            hashCode = CollectionUtility.CreateHashCode();

//            m_Transformation = tr;
//        }
//        public ReadOnly AsReadOnly()
//        {
//            op.Enter();
//            var temp = new ReadOnly(this);
//            op.Exit();

//            return temp;
//        }

//        public void SetParent(UnsafeReference<UnsafeTransform> parent)
//        {
//            op.Enter();
//            this.parent = parent;
//            op.Exit();
//        }

//        public override int GetHashCode() => hashCode;

//        public bool Equals(UnsafeTransform other) => hashCode == other.hashCode;
//    }

    [BurstCompatible]
    public struct UnsafeTransformScene : IValidation, IDisposable
    {
        public const int INIT_COUNT = 512;

        [BurstCompatible]
        public struct Data : IDisposable
        {
            public UnsafeAllocator<Transformation> transformations;
            private UnsafeAllocator<int> indices;

            private UnsafeFixedListWrapper<int> indexList;

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

            public int GetEmptyIndex()
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
                int count = index;
                for (int i = 0; i < count; i++)
                {
                    bool found = indexList.FindFor(i, 0, i + 1);
                    if (found)
                    {
                        index--;
                        //$"corrected {count} -> {index} :: {i} not found".ToLog();
                    }
                }

                return transformations.ElementAt(index);
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
        public void Resize() => m_Data[0].Resize();
        public int AddTransform(Transformation transformation = default(Transformation))
        {
            if (transformation.Equals(default(Transformation)))
            {
                transformation = Transformation.identity;
            }

            int index = m_Data[0].GetEmptyIndex();
            m_Data[0].transformations[index] = transformation;

            return index;
        }
        public void RemoveTransform(int index)
        {
            m_Data[0].ReserveIndex(index);
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
        private readonly ComputeShader m_GetMatricesCS;

        public Bounds bounds;

        private readonly MeshMaterial meshMaterial;
        private readonly int subMeshIndex;
        public readonly UnsafeTransformScene scene;
        private readonly ComputeBuffer transformationBuffer, matricesBuffer, argumentBuffer;

        public UnsafeBatchedScene(
            ComputeShader getMatricesCS,
            Mesh mesh, Material material, int subMeshIndex)
        {
            m_GetMatricesCS = getMatricesCS;

            meshMaterial = MeshMaterial.GetMeshMaterial(mesh, material);
            this.subMeshIndex = subMeshIndex;
            scene = new UnsafeTransformScene(Allocator.Persistent);

            transformationBuffer = new ComputeBuffer(
                UnsafeTransformScene.INIT_COUNT, UnsafeUtility.SizeOf<Transformation>(), ComputeBufferType.Structured);
            matricesBuffer = new ComputeBuffer(
                UnsafeTransformScene.INIT_COUNT, UnsafeUtility.SizeOf<float4x4>(), ComputeBufferType.Structured);
            argumentBuffer = new ComputeBuffer(
                5, sizeof(uint) * 5, ComputeBufferType.IndirectArguments);
        }

        public int Add(Transformation tr)
        {
            int index = scene.AddTransform(tr);

            return index;
        }
        public void Remove(int index)
        {
            scene.RemoveTransform(index);
        }

        private void SetupArgumentBuffer(Mesh mesh)
        {
            NativeArray<uint> arr = argumentBuffer.BeginWrite<uint>(0, 5);
            arr[0] = mesh.GetIndexCount(subMeshIndex);
            arr[1] = (uint)scene.count;
            arr[2] = mesh.GetIndexStart(subMeshIndex);
            arr[3] = (uint)mesh.GetBaseVertex(subMeshIndex);
            arr[4] = 0;
            argumentBuffer.EndWrite<uint>(5);
        }
        private void UpdateMatrices(Material material)
        {
            material.SetBuffer("_Matrices", matricesBuffer);
        }
        private void Setup(Mesh mesh, Material material)
        {
            SetupArgumentBuffer(mesh);
            UpdateMatrices(material);
        }

        private void UpdateBuffer()
        {
            scene.transformations.CopyToBuffer(transformationBuffer);
            m_GetMatricesCS.SetBuffer(0, "_Transforms", transformationBuffer);
            m_GetMatricesCS.SetBuffer(0, "_Result", matricesBuffer);
            m_GetMatricesCS.Dispatch(0, 16, 1, 1);
        }

        public void Draw()
        {
            Mesh mesh = meshMaterial.Mesh;
            Material material = meshMaterial.Material;

            UpdateBuffer();

            int count = scene.count;
            float4x4[] rawMats = ArrayPool<float4x4>.Shared.Rent(count);
            matricesBuffer.GetData(rawMats, 0, 0, count);

            Matrix4x4[] mats = ArrayPool<Matrix4x4>.Shared.Rent(count);
            for (int i = 0; i < count; i++)
            {
                mats[i] = rawMats[i];
            }
            ArrayPool<float4x4>.Shared.Return(rawMats);

            Graphics.DrawMeshInstanced(
                mesh, subMeshIndex, material, mats
                );
            ArrayPool<Matrix4x4>.Shared.Return(mats);
        }
        public void DrawIndirect()
        {
            Mesh mesh = meshMaterial.Mesh;
            Material material = meshMaterial.Material;

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
