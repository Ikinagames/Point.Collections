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

#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif

using Point.Collections;
using Point.Collections.Buffer.LowLevel;
using Point.Collections.LowLevel;
using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Point.Collections
{
    [BurstCompatible]
    public struct NativeTransform : IEquatable<NativeTransform>, IValidation
    {
        private readonly UnsafeAllocator<KeyValue<SceneID, UnsafeTransform>> m_Buffer;
        private readonly int m_Index;
        private readonly int m_HashCode;
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle m_SafetyHandle;
#endif

        public SceneID ID => m_Buffer[m_Index].key;

        public float3 position
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(in m_SafetyHandle);
                AtomicSafetyHandle.CheckReadAndThrow(m_SafetyHandle);
#endif
                float3 pos;
                ref UnsafeTransform tr = ref m_Buffer[m_Index].value;
                if (tr.parentIndex < 0) pos = tr.localPosition;
                else
                {
                    UnsafeTransform parent = m_Buffer[tr.parentIndex].value;

                    float4x4 local2World = float4x4.TRS(parent.localPosition, parent.localRotation, parent.localScale);
                    pos = math.mul(local2World, new float4(tr.localPosition, 1)).xyz;
                }

                return pos;
            }
            [WriteAccessRequired]
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(in m_SafetyHandle);
                AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
                float3 target;
                ref UnsafeTransform tr = ref m_Buffer[m_Index].value;
                if (tr.parentIndex < 0) target = value;
                else
                {
                    UnsafeTransform parent = m_Buffer[tr.parentIndex].value;
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
                AtomicSafetyHandle.CheckExistsAndThrow(in m_SafetyHandle);
                AtomicSafetyHandle.CheckReadAndThrow(m_SafetyHandle);
#endif
                return m_Buffer[m_Index].value.localPosition;
            }
            [WriteAccessRequired]
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(in m_SafetyHandle);
                AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
                m_Buffer[m_Index].value.localPosition = value;
            }
        }
        public quaternion rotation
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(in m_SafetyHandle);
                AtomicSafetyHandle.CheckReadAndThrow(m_SafetyHandle);
#endif
                quaternion rot;
                ref UnsafeTransform tr = ref m_Buffer[m_Index].value;
                if (tr.parentIndex < 0) rot = tr.localRotation;
                else
                {
                    UnsafeTransform parent = m_Buffer[tr.parentIndex].value;

                    rot = math.mul(tr.localRotation, parent.localRotation);
                }

                return rot;
            }
            [WriteAccessRequired]
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(in m_SafetyHandle);
                AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
                quaternion target;
                ref UnsafeTransform tr = ref m_Buffer[m_Index].value;
                if (tr.parentIndex < 0) target = value;
                else
                {
                    UnsafeTransform parent = m_Buffer[tr.parentIndex].value;

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
                AtomicSafetyHandle.CheckExistsAndThrow(in m_SafetyHandle);
                AtomicSafetyHandle.CheckReadAndThrow(m_SafetyHandle);
#endif
                return m_Buffer[m_Index].value.localRotation;
            }
            [WriteAccessRequired]
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(in m_SafetyHandle);
                AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
                m_Buffer[m_Index].value.localRotation = value;
            }
        }
        public float3 eulerAngles
        {
            get => rotation.Euler() * UnityEngine.Mathf.Rad2Deg;
            [WriteAccessRequired]
            set
            {
                float3 temp = value * UnityEngine.Mathf.Deg2Rad;
                temp = math.round(temp * 1000) * .001f;
                
                rotation = quaternion.EulerZXY(temp);
            }
        }
        public float3 localEulerAngles
        {
            get => localRotation.Euler() * UnityEngine.Mathf.Rad2Deg;
            [WriteAccessRequired]
            set
            {
                float3 temp = value * UnityEngine.Mathf.Deg2Rad;
                temp = math.round(temp * 1000) * .001f;

                localRotation = quaternion.EulerZXY(temp);
            }
        }
        public float3 localScale
        {
            get
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(in m_SafetyHandle);
                AtomicSafetyHandle.CheckReadAndThrow(m_SafetyHandle);
#endif
                return m_Buffer[m_Index].value.localScale;
            }
            [WriteAccessRequired]
            set
            {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                AtomicSafetyHandle.CheckExistsAndThrow(in m_SafetyHandle);
                AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
                m_Buffer[m_Index].value.localScale = value;
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
                AtomicSafetyHandle.CheckExistsAndThrow(in m_SafetyHandle);
                AtomicSafetyHandle.CheckReadAndThrow(m_SafetyHandle);
#endif
                UnsafeTransform tr = m_Buffer[m_Index].value;
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
            m_Buffer = hashMap.Ptr;
            hashMap.TryGetIndex(id, out m_Index);
            m_HashCode = m_Buffer[m_Index].value.hashCode;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            m_SafetyHandle = safetyHandle;
            AtomicSafetyHandle.UseSecondaryVersion(ref m_SafetyHandle);
#endif
        }

        [WriteAccessRequired]
        public void SetParent(in NativeTransform parent)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckExistsAndThrow(in m_SafetyHandle);
            AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
            m_Buffer[m_Index].value.parentIndex = parent.m_Index;
        }
        [WriteAccessRequired]
        public void RemoveParent()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckExistsAndThrow(in m_SafetyHandle);
            AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
            m_Buffer[m_Index].value.parentIndex = -1;
        }

        public bool Equals(NativeTransform other) => m_Index.Equals(other.m_Index);

        public bool IsValid()
        {
            if (m_Index < 0 || m_Buffer[m_Index].value.GetHashCode() != m_HashCode) return false;

            return true;
        }
    }
}
