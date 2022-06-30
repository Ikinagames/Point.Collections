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
        [SerializeField] private Vector3 m_Position;
        [SerializeField] private Vector3 m_Rotation;
        [SerializeField] private Vector3 m_Scale = Vector3.one;

        private UnsafeReference<UnsafeTransform> m_Ptr;

        public float3 localPosition
        {
            get => m_Position;
            set
            {
                m_Position = value;
                m_Ptr.Value.transformation.localPosition = m_Position;
            }
        } 
        public quaternion localRotation
        {
            get => quaternion.EulerZXY(m_Rotation * Math.Deg2Rad);
            set
            {
                m_Rotation = ((Quaternion)value).eulerAngles;
                m_Ptr.Value.transformation.localRotation = value;
            }
        }
        public float3 localScale
        {
            get => m_Scale;
            set
            {
                m_Scale = value;
                m_Ptr.Value.transformation.localScale = m_Scale;
            }
        }
        public float3 eulerAngles
        {
            get => m_Rotation;
            set
            {
                m_Rotation = value;
                m_Ptr.Value.transformation.localRotation = quaternion.EulerZXY(m_Rotation * Math.Deg2Rad);
            }
        }

        protected virtual void OnEnable()
        {
            m_Ptr = TransformSceneManager.Add(transform);
            m_Ptr.Value.transformation.localPosition = m_Position;
            m_Ptr.Value.transformation.localRotation = quaternion.EulerZXY(m_Rotation * Math.Deg2Rad);
            m_Ptr.Value.transformation.localScale = m_Scale;

            OnInitialized();
        }
        protected virtual void OnDisable()
        {
            TransformSceneManager.Remove(m_Ptr);
        }
//#if UNITY_EDITOR
//        protected virtual void OnValidate()
//        {
//            if (!Application.isPlaying && !m_Ptr.IsCreated) return;

//            m_Ptr.Value.transformation.localPosition = m_Position;
//            m_Ptr.Value.transformation.localRotation = quaternion.EulerZXY(m_Rotation * Math.Deg2Rad);
//            m_Ptr.Value.transformation.localScale = m_Scale;
//        }
//#endif

        protected virtual void OnInitialized() { }
    }
}
