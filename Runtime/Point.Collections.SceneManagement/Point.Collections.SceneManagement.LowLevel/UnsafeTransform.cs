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
        public static UnsafeTransform Invalid => new UnsafeTransform(-1, default);
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
                    var parentTr = tr.parent.Value.m_Transformation.Value;
                    localToWorld = parentTr.localToWorld;
                    worldToLocal = parentTr.worldToLocal;
                    parentRotation = parentTr.localRotation;
                    parentScale = parentTr.localScale;
                }
                else
                {
                    localToWorld = float4x4.identity;
                    worldToLocal = float4x4.identity;
                    parentRotation = quaternion.identity;
                    parentScale = 1;
                }

                transformation = tr.m_Transformation.Value;
            }
        }

        public readonly int index, hashCode;

        private UnsafeReference<UnsafeTransform> parent;
        private UnsafeReference<Transformation> m_Transformation;

        private AtomicOperator op;

        public Transformation transformation
        {
            get
            {
                op.Enter();
                var boxed = m_Transformation.Value;
                op.Exit();

                return boxed;
            }
            set
            {
                op.Enter();
                m_Transformation.Value = value;
                op.Exit();
            }
        }

        public UnsafeTransform(int index, UnsafeReference<Transformation> tr)
        {
            this = default(UnsafeTransform);
            this.index = index;
            hashCode = CollectionUtility.CreateHashCode();

            m_Transformation = tr;
        }
        public ReadOnly AsReadOnly()
        {
            op.Enter();
            var temp = new ReadOnly(this);
            op.Exit();

            return temp;
        }

        public void SetParent(UnsafeReference<UnsafeTransform> parent)
        {
            op.Enter();
            this.parent = parent;
            op.Exit();
        }

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
            /// <summary>
            /// <see cref="GraphicsBuffer"/>
            /// </summary>
            public UnsafeAllocator<Transformation> transformations;
            public UnsafeFixedListWrapper<UnsafeTransform> transforms;

            public void Dispose()
            {
                buffer.Dispose();
                transformations.Dispose();
            }
        }

        private UnsafeAllocator<Data> data;
        private JobHandle jobHandle;

        public int length => data[0].buffer.Length;
        public int count => data[0].transforms.Length;

        public UnsafeAllocator<Transformation> transformations => data[0].transformations;

        public unsafe UnsafeTransformScene(Allocator allocator, int initCount = INIT_COUNT)
        {
            data = new UnsafeAllocator<Data>(1, allocator);

            data[0].buffer = new UnsafeAllocator<UnsafeTransform>(
                initCount,
                allocator);
            data[0].transformations = new UnsafeAllocator<Transformation>(
                initCount,
                allocator);
            data[0].transforms = new UnsafeFixedListWrapper<UnsafeTransform>(data[0].buffer, 0);
            //transformAccessArray = new TransformAccessArray(initCount);

            jobHandle = default(JobHandle);
        }
        public unsafe void Resize(int length)
        {
            Complete();

            data[0].buffer.Resize(length);
            data[0].transformations.Resize(length);
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

            int count = data[0].transforms.Length;
            UnsafeReference<UnsafeTransform> ptr 
                = data[0].transforms.AddNoResize(new UnsafeTransform(count, data[0].transformations.ElementAt(count)));

            $"tr added at {count}".ToLog();

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
        private UnsafeReference<UnsafeTransform> transform;

        public int index => transform.Value.index;
        public float4x4 localToWorld => transform.Value.transformation.localToWorld;
        public Bounds bounds
        {
            get
            {
                float3 pos = transform.Value.transformation.localPosition;
                return new Bounds(pos, Vector3.one);
            }
        }

        public UnsafeGraphicsModel(
            UnsafeReference<UnsafeTransform> transform, bool hasCollider)
        {
            //if (hasCollider)
            //{
            //    // https://docs.unity3d.com/ScriptReference/Physics.BakeMesh.html
            //    Physics.BakeMesh(mesh.GetInstanceID(), false);
            //}

            this.transform = transform;
        }
    }
}
