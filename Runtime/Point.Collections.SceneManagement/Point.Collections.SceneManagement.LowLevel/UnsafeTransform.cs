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
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
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
            public readonly Transformation parent;
            public readonly Transformation transformation;

            public ReadOnly(UnsafeTransform tr)
            {
                hashCode = tr.hashCode;
                if (tr.parent.IsCreated)
                {
                    parent = tr.parent.Value.transformation;
                }
                else parent = Transformation.identity;

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

    public struct UnsafeTransformScene : IDisposable
    {
        public const int INIT_COUNT = 512;

        private struct Data : IDisposable
        {
            public UnsafeAllocator<UnsafeTransform> buffer;
            public UnsafeFixedListWrapper<UnsafeTransform> transforms;
            public JobHandle jobHandle;

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
        [BurstCompile(CompileSynchronously = true)]
        private struct SynchronizeJob : IJobParallelForTransform
        {
            [DeallocateOnJobCompletion]
            private NativeArray<UnsafeTransform.ReadOnly> buffer;

            public SynchronizeJob(NativeArray<UnsafeTransform.ReadOnly> buffer)
            {
                this.buffer = buffer;
            }

            public void Execute(int i, TransformAccess transform)
            {
                // TODO : 
                transform.position = buffer[0].transformation.localPosition;
            }
        }

        private UnsafeAllocator<Data> data;
        private TransformAccessArray transformAccessArray;

        public UnsafeTransformScene(Allocator allocator, int initCount = INIT_COUNT)
        {
            data = new UnsafeAllocator<Data>(1, allocator);

            data[0].buffer = new UnsafeAllocator<UnsafeTransform>(
                initCount,
                allocator);
            data[0].transforms = new UnsafeFixedListWrapper<UnsafeTransform>(data[0].buffer, 0);
            transformAccessArray = new TransformAccessArray(initCount);
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

        public UnsafeReference<UnsafeTransform> AddTransform(UnityEngine.Transform transform, 
            Transformation transformation = default(Transformation))
        {
            if (transformation.Equals(default(Transformation)))
            {
                transformation = Transformation.identity;
            }

            Assert.IsFalse(RequireResize(), "This Scene require resize but you didn\'t.");

            int count = data[0].transforms.Length;
            data[0].transforms.AddNoResize(new UnsafeTransform(transformation));
            transformAccessArray.Add(transform);

            UnsafeReference<UnsafeTransform> ptr = data[0].buffer.ElementAt(count);
            return ptr;
        }
        public void RemoveTransform(UnsafeReference<UnsafeTransform> ptr)
        {
            if (!data[0].buffer.Contains(ptr))
            {
                $"?? not in the buffer {data[0].buffer.Ptr} ? {ptr}".ToLog();
                return;
            }

            int index = data[0].buffer.IndexOf(ptr);
            if (index < 0)
            {
                $"?? not in the buffer {data[0].buffer.Ptr} ? {ptr}".ToLog();
                return;
            }

            UnsafeTransform tr = data[0].buffer[index];
            index = data[0].transforms.IndexOf(tr);

            data[0].transforms.RemoveAtSwapback(index);
            transformAccessArray.RemoveAtSwapBack(index);
            "success".ToLog();
        }

        public JobHandle Synchronize(JobHandle dependsOn = default)
        {
            int dataCount = data[0].transforms.Length;

            NativeArray<UnsafeTransform.ReadOnly> readonlyData
                = new NativeArray<UnsafeTransform.ReadOnly>(dataCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
            CollectTransformationJob collectJob = new CollectTransformationJob
            {
                buffer = data[0].buffer,
                results = readonlyData
            };
            JobHandle jobHandle = collectJob.Schedule(dataCount, 64, JobHandle.CombineDependencies(data[0].jobHandle, dependsOn));

            SynchronizeJob job = new SynchronizeJob(readonlyData);
            JobHandle handle = job.Schedule(
                transformAccessArray,
                jobHandle);

            data[0].jobHandle = JobHandle.CombineDependencies(data[0].jobHandle, handle);

            return handle;
        }

        public void Dispose()
        {
            data[0].Dispose();
            data.Dispose();
            transformAccessArray.Dispose();
        }
    }
}
