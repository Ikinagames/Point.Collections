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

using UnityEngine;
using Point.Collections.Buffer.LowLevel;
using Unity.Mathematics;

namespace Point.Collections.SceneManagement.LowLevel
{
    [DisallowMultipleComponent]
    public class NativeTransform : PointMonobehaviour
    {
        [SerializeField] private bool m_EnableOnAwake = true;
        [SerializeField] private Vector3 m_Position;
        [SerializeField] private Vector3 m_Rotation;
        [SerializeField] private Vector3 m_Scale = Vector3.one;

        private UnsafeReference<UnsafeTransform> m_Ptr;

        public bool Enabled => m_Ptr.IsCreated;
        public virtual bool EnableOnAwake => m_EnableOnAwake;

        public float3 localPosition
        {
            get => m_Position;
            set
            {
                m_Position = value;
                if (Enabled)
                {
                    m_Ptr.Value.transformation.localPosition = m_Position;
                }
                else transform.localPosition = value;
            }
        } 
        public quaternion localRotation
        {
            get => quaternion.EulerZXY(m_Rotation * Math.Deg2Rad);
            set
            {
                m_Rotation = ((Quaternion)value).eulerAngles;

                if (Enabled)
                {
                    m_Ptr.Value.transformation.localRotation = value;
                }
                else transform.localRotation = value;
            }
        }
        public float3 localScale
        {
            get => m_Scale;
            set
            {
                m_Scale = value;

                if (Enabled)
                {
                    m_Ptr.Value.transformation.localScale = m_Scale;
                }
                else transform.localScale = value;
            }
        }
        public float3 eulerAngles
        {
            get => m_Rotation;
            set
            {
                m_Rotation = value;
                if (Enabled)
                {
                    m_Ptr.Value.transformation.localRotation = quaternion.EulerZXY(m_Rotation * Math.Deg2Rad);
                }
                else transform.eulerAngles = value;
            }
        }

        protected virtual void OnEnable()
        {
            if (EnableOnAwake) Enable();
        }
        protected virtual void OnDisable()
        {
            Disable();
        }

        public void Enable()
        {
            if (m_Ptr.IsCreated) return;

            m_Ptr = TransformSceneManager.Add(transform);
            m_Ptr.Value.transformation.localPosition = m_Position;
            m_Ptr.Value.transformation.localRotation = quaternion.EulerZXY(m_Rotation * Math.Deg2Rad);
            m_Ptr.Value.transformation.localScale = m_Scale;

            OnInitialized();
        }
        public void Disable()
        {
            if (!m_Ptr.IsCreated) return;

            TransformSceneManager.Remove(m_Ptr);

            m_Ptr = default;
        }

        protected virtual void OnInitialized() { }
    }
}
