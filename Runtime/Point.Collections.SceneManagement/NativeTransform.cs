// Copyright 2021 Ikina Games
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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#if UNITY_2019 || !UNITY_2020_OR_NEWER
#define UNITYENGINE_OLD
#endif
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
#if UNITY_MATHEMATICS
using Unity.Mathematics;
#else
using math = Point.Collections.Math;
#endif
#else
#define POINT_COLLECTIONS_NATIVE
using math = Point.Collections.Math;
#endif

#if !UNITYENGINE_OLD
using Point.Collections.Buffer.LowLevel;
using Point.Collections.SceneManagement.LowLevel;
using System;

// https://issuetracker.unity3d.com/issues/ecs-compiler-wrongly-detect-unmanaged-structs-as-containing-nullabe-fields

namespace Point.Collections.SceneManagement
{
#if UNITYENGINE && UNITY_COLLECTIONS
    [BurstCompatible]
#endif
    public struct NativeTransform : IEquatable<NativeTransform>, IValidation
    {
        private readonly UnsafeAllocator<KeyValue<SceneID, UnsafeTransform>> m_Buffer;
        private readonly int m_Index;
        private readonly int m_HashCode;
#if ENABLE_UNITY_COLLECTIONS_CHECKS 
        private AtomicSafetyHandle m_SafetyHandle;
#endif

        public SceneID ID => m_Buffer[m_Index].Key;

        public float3 position
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
                AtomicSafetyHandle.CheckReadAndThrow(m_SafetyHandle);
#endif
                float3 pos;
                ref UnsafeTransform tr = ref m_Buffer[m_Index].Value;
                if (tr.parentIndex < 0) pos = tr.localPosition;
                else
                {
                    UnsafeTransform parent = m_Buffer[tr.parentIndex].Value;

                    float4x4 local2World = float4x4.TRS(parent.localPosition, parent.localRotation, parent.localScale);
                    pos = math.mul(local2World, new float4(tr.localPosition, 1)).xyz;
                }

                return pos;
            }
#if UNITYENGINE
            [WriteAccessRequired]
#endif
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
                AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
                float3 target;
                ref UnsafeTransform tr = ref m_Buffer[m_Index].Value;
                if (tr.parentIndex < 0) target = value;
                else
                {
                    UnsafeTransform parent = m_Buffer[tr.parentIndex].Value;
                    float4x4 world2Local = math.inverse(float4x4.TRS(parent.localPosition, parent.localRotation, parent.localScale));

                    target = math.mul(world2Local, new float4(value, 1)).xyz;
                }

                tr.localPosition = target;
            }
        }
        public float3 localPosition
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
                AtomicSafetyHandle.CheckReadAndThrow(m_SafetyHandle);
#endif
                return m_Buffer[m_Index].Value.localPosition;
            }
#if UNITYENGINE
            [WriteAccessRequired]
#endif
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
                AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
                m_Buffer[m_Index].Value.localPosition = value;
            }
        }
        public quaternion rotation
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
                AtomicSafetyHandle.CheckReadAndThrow(m_SafetyHandle);
#endif
                quaternion rot;
                ref UnsafeTransform tr = ref m_Buffer[m_Index].Value;
                if (tr.parentIndex < 0) rot = tr.localRotation;
                else
                {
                    UnsafeTransform parent = m_Buffer[tr.parentIndex].Value;

                    rot = math.mul(tr.localRotation, parent.localRotation);
                }

                return rot;
            }
#if UNITYENGINE
            [WriteAccessRequired]
#endif
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
                AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
                quaternion target;
                ref UnsafeTransform tr = ref m_Buffer[m_Index].Value;
                if (tr.parentIndex < 0) target = value;
                else
                {
                    UnsafeTransform parent = m_Buffer[tr.parentIndex].Value;

                    target = math.mul(parent.localRotation, value);
                }

                tr.localRotation = target;
            }
        }
        public quaternion localRotation
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
                AtomicSafetyHandle.CheckReadAndThrow(m_SafetyHandle);
#endif
                return m_Buffer[m_Index].Value.localRotation;
            }
#if UNITYENGINE
            [WriteAccessRequired]
#endif
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
                AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
                m_Buffer[m_Index].Value.localRotation = value;
            }
        }
        public float3 eulerAngles
        {
            get => rotation.Euler() * Math.Rad2Deg;
#if UNITYENGINE
            [WriteAccessRequired]
#endif
            set
            {
                float3 temp = value * Math.Deg2Rad;
                temp = math.round(temp * 1000) * .001f;
                
                rotation = quaternion.EulerZXY(temp);
            }
        }
        public float3 localEulerAngles
        {
            get => localRotation.Euler() * Math.Rad2Deg;
#if UNITYENGINE
            [WriteAccessRequired]
#endif
            set
            {
                float3 temp = value * Math.Deg2Rad;
                temp = math.round(temp * 1000) * .001f;

                localRotation = quaternion.EulerZXY(temp);
            }
        }
        public float3 localScale
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
                AtomicSafetyHandle.CheckReadAndThrow(m_SafetyHandle);
#endif
                return m_Buffer[m_Index].Value.localScale;
            }
#if UNITYENGINE
            [WriteAccessRequired]
#endif
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
                AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
                m_Buffer[m_Index].Value.localScale = value;
            }
        }

        public float3 right
        {
            get => math.mul(rotation, math.right());
        }
        public float3 up
        {
            get => math.mul(rotation, math.up());
        }
        public float3 forward
        {
            get => math.mul(rotation, math.forward());
        }

        public float4x4 localToWorldMatrix
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
                AtomicSafetyHandle.CheckReadAndThrow(m_SafetyHandle);
#endif
                UnsafeTransform tr = m_Buffer[m_Index].Value;
                return float4x4.TRS(tr.localPosition, tr.localRotation, tr.localScale);
            }
        }
        public float4x4 worldToLocalMatrix => math.inverse(localToWorldMatrix);

        internal NativeTransform(UnsafeLinearHashMap<SceneID, UnsafeTransform> hashMap, 
            SceneID id
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            , AtomicSafetyHandle safetyHandle
#endif
            )
        {
            m_Buffer = hashMap.GetUnsafeAllocator();
            hashMap.TryGetIndex(id, out m_Index);
            m_HashCode = m_Buffer[m_Index].Value.hashCode;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_SafetyHandle = safetyHandle;
            AtomicSafetyHandle.UseSecondaryVersion(ref m_SafetyHandle);
#endif
        }

#if UNITYENGINE
        [WriteAccessRequired]
#endif
        public void SetParent(in NativeTransform parent)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
            AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
            m_Buffer[m_Index].Value.parentIndex = parent.m_Index;
        }
#if UNITYENGINE
        [WriteAccessRequired]
#endif
        public void RemoveParent()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
            AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
            m_Buffer[m_Index].Value.parentIndex = -1;
        }

        public bool Equals(NativeTransform other) 
            => m_Index.Equals(other.m_Index) && m_HashCode.Equals(other.m_HashCode) && m_Buffer.Equals(other.m_Buffer);

        public bool IsValid()
        {
            if (m_Index < 0 || m_Buffer[m_Index].Value.GetHashCode() != m_HashCode) return false;

            return true;
        }
    }
}

#endif
