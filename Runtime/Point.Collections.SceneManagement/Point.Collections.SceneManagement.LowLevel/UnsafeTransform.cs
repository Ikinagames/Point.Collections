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
            //public UnsafeFixedListWrapper<Transformation> transforms;

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
                //transforms = new UnsafeFixedListWrapper<Transformation>(transformations, 0);
            }

            public int GetEmptyIndex()
            {
                if (indexList.Length == 0)
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
                indexList.AddNoResize(index);
            }

            private void Resize(int length)
            {
                int prevLength = transformations.Length;

                transformations.Resize(length);
                indices.Resize(length);

                indexList = new UnsafeFixedListWrapper<int>(
                    indices, indexList.Length);
                //transforms = new UnsafeFixedListWrapper<Transformation>(
                //    transformations, prevLength);
            }
            public void Dispose()
            {
                transformations.Dispose();
                indices.Dispose();
            }
        }

        private UnsafeAllocator<Data> m_Data;
        private JobHandle jobHandle;

        //public int length => m_Data[0].transforms.Capacity;
        //public int count => m_Data[0].transforms.Length;

        public UnsafeAllocator<Data> Buffer => m_Data;
        public UnsafeAllocator<Transformation> transformations => Buffer[0].transformations;
        
        public unsafe UnsafeTransformScene(Allocator allocator, int initCount = INIT_COUNT)
        {
            m_Data = new UnsafeAllocator<Data>(1, allocator);
            m_Data[0] = new Data(initCount, allocator);

            jobHandle = default(JobHandle);
        }

        public bool IsValid()
        {
            return m_Data.IsCreated;
        }
        
        public int AddTransform(Transformation transformation = default(Transformation))
        {
            //if (RequireResize())
            //{
            //    UnityEngine.Debug.LogError("require Resize");
            //    Debug.Break();
            //    return default;
            //}
            if (transformation.Equals(default(Transformation)))
            {
                transformation = Transformation.identity;
            }

            //Assert.IsFalse(RequireResize(), "This Scene require resize but you didn\'t.");

            int index = m_Data[0].GetEmptyIndex();
            m_Data[0].transformations[index] = transformation;

            //int count = m_Data[0].transforms.Length;
            //m_Data[0].transforms.AddNoResize(transformation);

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

    public struct UnsafeGraphicsModel
    {
        private int transform;

        public int index => transform;
        //public float4x4 localToWorld => transform.Value.transformation.localToWorld;
        //public Bounds bounds
        //{
        //    get
        //    {
        //        float3 pos = transform.Value.transformation.localPosition;
        //        return new Bounds(pos, Vector3.one);
        //    }
        //}

        public UnsafeGraphicsModel(
            int transform, bool hasCollider)
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
