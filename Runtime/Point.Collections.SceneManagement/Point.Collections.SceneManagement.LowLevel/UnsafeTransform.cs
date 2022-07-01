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
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Jobs;
#else
#define POINT_COLLECTIONS_NATIVE
#endif

namespace Point.Collections.SceneManagement.LowLevel
{
#if UNITYENGINE
    [BurstCompatible]
#endif
    public struct UnsafeTransform : IEquatable<UnsafeTransform>
    {
        public struct ReadOnly
        {
            public readonly int hashCode;
            public readonly float4x4 localToWorld, worldToLocal;
            public readonly quaternion parentRotation;
            public readonly float3 parentScale;
            public readonly Transformation transformation;

            public ReadOnly(UnsafeTransform tr)
            {
                hashCode = tr.hashCode;
                if (tr.parent.IsCreated)
                {
                    localToWorld = tr.parent.Value.transformation.localToWorld;
                    worldToLocal = tr.parent.Value.transformation.worldToLocal;
                    parentRotation = tr.parent.Value.transformation.localRotation;
                    parentScale = tr.parent.Value.transformation.localScale;
                }
                else
                {
                    localToWorld = float4x4.identity;
                    worldToLocal = float4x4.identity;
                    parentRotation = quaternion.identity;
                    parentScale = 1;
                }

                transformation = tr.transformation;
            }
        }

        public static UnsafeTransform Invalid => new UnsafeTransform(default(Transformation));
        public int hashCode;

        public UnsafeReference<UnsafeTransform> parent;
        public Transformation transformation;

        public UnsafeTransform(Transformation tr)
        {
            this = default(UnsafeTransform);
            hashCode = CollectionUtility.CreateHashCode();

            transformation = tr;
        }
        public UnsafeTransform(Transformation tr, UnsafeReference<UnsafeTransform> parent) : this(tr)
        {
            this.parent = parent;
        }
        public ReadOnly AsReadOnly() => new ReadOnly(this);

        public override int GetHashCode() => hashCode;

        public bool Equals(UnsafeTransform other) => hashCode == other.hashCode;
    }

    [BurstCompatible]
    public struct UnsafeTransformScene : IValidation, IDisposable
    {
        public const int INIT_COUNT = 512;

        [BurstCompatible]
        private struct Data : IDisposable
        {
            public UnsafeAllocator<UnsafeTransform> buffer;
            public UnsafeFixedListWrapper<UnsafeTransform> transforms;

            public void Dispose()
            {
                buffer.Dispose();
            }
        }
        [BurstCompile(CompileSynchronously = true)]
        private struct CollectTransformationJob : IJobParallelFor
        {
            [ReadOnly]
            public UnsafeAllocator<UnsafeTransform> buffer;
            [WriteOnly]
            public NativeArray<UnsafeTransform.ReadOnly> results;

            public void Execute(int i)
            {
                results[i] = buffer[i].AsReadOnly();
            }
        }
        //[BurstCompile(CompileSynchronously = true)]
        //private struct SynchronizeJob : IJobParallelForTransform
        //{
        //    [DeallocateOnJobCompletion, ReadOnly]
        //    private NativeArray<UnsafeTransform.ReadOnly> buffer;

        //    public SynchronizeJob(NativeArray<UnsafeTransform.ReadOnly> buffer)
        //    {
        //        this.buffer = buffer;
        //    }

        //    public void Execute(int i, TransformAccess tr)
        //    {
        //        UnsafeTransform.ReadOnly transform = buffer[i];

        //        // TODO : 
        //        tr.position 
        //            = math.mul(transform.localToWorld, new float4(transform.transformation.localPosition, 1)).xyz;
        //        tr.rotation = math.mul(transform.transformation.localRotation, transform.parentRotation);
        //        tr.localScale = transform.parentScale / transform.transformation.localScale;
        //    }
        //}

        private UnsafeAllocator<Data> data;
        //private TransformAccessArray transformAccessArray;
        private JobHandle jobHandle;

        public UnsafeTransformScene(Allocator allocator, int initCount = INIT_COUNT)
        {
            data = new UnsafeAllocator<Data>(1, allocator);

            data[0].buffer = new UnsafeAllocator<UnsafeTransform>(
                initCount,
                allocator);
            data[0].transforms = new UnsafeFixedListWrapper<UnsafeTransform>(data[0].buffer, 0);
            //transformAccessArray = new TransformAccessArray(initCount);

            jobHandle = default(JobHandle);
        }
        public void Resize(int length)
        {
            Complete();

            data[0].buffer.Resize(length);
            data[0].transforms = new UnsafeFixedListWrapper<UnsafeTransform>(
                data[0].buffer, data[0].transforms.Length);

            jobHandle = default(JobHandle);
        }
        public void Resize()
        {
            Resize(data[0].buffer.Length * 2);
        }

        public bool IsValid()
        {
            return data.IsCreated;
        }
        public bool RequireResize()
        {
            if (data[0].transforms.Length >= data[0].buffer.Length ||
                data[0].transforms.Length + 1 >= data[0].buffer.Length)
            {
                return true;
            }
            return false;
        }

        public UnsafeReference<UnsafeTransform> AddTransform(Transformation transformation = default(Transformation))
        {
            if (RequireResize())
            {
                UnityEngine.Debug.LogError("require Resize");
                Debug.Break();
                return default;
            }
            if (transformation.Equals(default(Transformation)))
            {
                transformation = Transformation.identity;
            }

            Assert.IsFalse(RequireResize(), "This Scene require resize but you didn\'t.");

            UnsafeReference<UnsafeTransform> ptr 
                = data[0].transforms.AddNoResize(new UnsafeTransform(transformation));

            return ptr;
        }
        public int RemoveTransform(UnsafeReference<UnsafeTransform> ptr)
        {
            if (!data[0].buffer.Contains(ptr))
            {
                UnityEngine.Debug.LogError($"?? not in the buffer {data[0].buffer.Ptr} ? {ptr}");
                return -1;
            }

            int index = data[0].buffer.IndexOf(ptr);
            if (index < 0)
            {
                UnityEngine.Debug.LogError($"?? not in the buffer {data[0].buffer.Ptr} ? {ptr}");
                return -1;
            }

            UnsafeTransform tr = data[0].buffer[index];
            index = data[0].transforms.IndexOf(tr);

            data[0].transforms.RemoveAtSwapback(index);
            
            return index;
        }

        public void Complete()
        {
            jobHandle.Complete();
        }

        public JobHandle Synchronize(JobHandle dependsOn = default)
        {
            Complete();

            int dataCount = data[0].transforms.Length;

            NativeArray<UnsafeTransform.ReadOnly> readonlyData
                = new NativeArray<UnsafeTransform.ReadOnly>(dataCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            CollectTransformationJob collectJob = new CollectTransformationJob
            {
                buffer = data[0].buffer,
                results = readonlyData
            };
            JobHandle jobHandle = collectJob.Schedule(
                dataCount, 64, JobHandle.CombineDependencies(this.jobHandle, dependsOn));

            //SynchronizeJob job = new SynchronizeJob(readonlyData);
            //JobHandle handle = job.Schedule(
            //    transformAccessArray,
            //    jobHandle);

            //jobHandle = handle;

            return jobHandle;
        }

        public void Dispose()
        {
            Complete();

            data[0].Dispose();
            data.Dispose();
            //transformAccessArray.Dispose();
        }
    }

    public struct UnsafeGraphicsModel
    {
        public Material material;
        public Mesh mesh;
        public int subMeshIndex;
        public UnsafeReference<UnsafeTransform> transform;

        public UnsafeGraphicsModel(
            UnsafeReference<UnsafeTransform> transform, Material material, 
            Mesh mesh, int subMeshIndex, bool hasCollider)
        {
            if (hasCollider)
            {
                // https://docs.unity3d.com/ScriptReference/Physics.BakeMesh.html
                Physics.BakeMesh(mesh.GetInstanceID(), false);
            }

            this.material = material;
            this.mesh = mesh;
            this.subMeshIndex = subMeshIndex;
            this.transform = transform;
        }
    }
}
