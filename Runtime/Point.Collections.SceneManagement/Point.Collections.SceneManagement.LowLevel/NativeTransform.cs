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

#if UNITY_2020_1_OR_NEWER && UNITY_COLLECTIONS
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

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

        private TransformSceneManager.TransformInterface m_TransformInterface;

        public bool Enabled => m_TransformInterface.IsValid();
        public virtual bool EnableOnAwake => m_EnableOnAwake;

        public float3 localPosition
        {
            get => m_Position;
            set
            {
                m_Position = value;
                if (Enabled && !PointApplication.IsShutdown)
                {
                    m_TransformInterface.SetPosition(value);
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

                if (Enabled && !PointApplication.IsShutdown)
                {
                    m_TransformInterface.SetRotation(value);
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

                if (Enabled && !PointApplication.IsShutdown)
                {
                    m_TransformInterface.SetScale(value);
                }
                else transform.localScale = value;
            }
        }
        public float3 localEulerAngles
        {
            get => m_Rotation;
            set
            {
                m_Rotation = value;
                if (Enabled && !PointApplication.IsShutdown)
                {
                    m_TransformInterface.SetRotation(quaternion.EulerZXY(m_Rotation * Math.Deg2Rad));
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
#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            localPosition = m_Position;
            localEulerAngles = m_Rotation;
            localScale = m_Scale;
        }
#endif

        public void Enable()
        {
            if (Enabled || PointApplication.IsShutdown) return;

            m_TransformInterface = TransformSceneManager.Add(transform);
            if (!m_TransformInterface.IsValid()) return;

            localPosition = m_Position;
            localRotation = quaternion.EulerZXY(m_Rotation * Math.Deg2Rad);
            localScale = m_Scale;

            OnInitialized();
        }
        public void Disable()
        {
            if (!Enabled || PointApplication.IsShutdown) return;

            TransformSceneManager.Remove(m_TransformInterface);

            m_TransformInterface = default;
        }

        protected virtual void OnInitialized() { }
    }
}

#endif