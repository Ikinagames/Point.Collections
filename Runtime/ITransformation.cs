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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#if UNITY_COLLECTIONS
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif


using Unity.Mathematics;
using UnityEngine.Assertions;

namespace Point.Collections
{
    public interface ITransformation
    {
#pragma warning disable IDE1006 // Naming Styles
        ITransformation parent { get; set; }

        float4x4 localToWorld { get; }
        float4x4 worldToLocal { get; }
        float3 position { get; set; }
        float3 localPosition { get; set; }
        quaternion rotation { get; set; }
        quaternion localRotation { get; set; }
        float3 eulerAngles { get; set; }
        float3 localEulerAngles { get; set; }
        float3 lossyScale { get; set; }
        float3 localScale { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        void SetPosition(float3 position);
    }
    public class UnityTransformProvider : ITransformation
    {
        private ITransformation m_Parent;
        private UnityEngine.Transform m_Transform;
        private UnityEngine.AI.NavMeshAgent m_Agent;

        public ITransformation parent
        {
            get => m_Parent;
            set
            {
                Assert.IsNotNull(m_Transform);
                if (value == null && m_Parent is UnityTransformProvider)
                {
                    m_Transform.SetParent(null);
                }
                m_Parent = value;
                if (value != null && value is UnityTransformProvider unityTransform)
                {
                    m_Transform.SetParent(unityTransform.m_Transform);
                }
            }
        }

        public float4x4 localToWorld => m_Transform.localToWorldMatrix;
        public float4x4 worldToLocal => m_Transform.worldToLocalMatrix;

        public float3 position { get => m_Transform.position; set => m_Transform.position = value; }
        public float3 localPosition { get => m_Transform.localPosition; set => m_Transform.localPosition = value; }
        public quaternion rotation { get => m_Transform.rotation; set => m_Transform.rotation = value; }
        public quaternion localRotation { get => m_Transform.localRotation; set => m_Transform.localRotation = value; }
        public float3 eulerAngles { get => m_Transform.eulerAngles; set => m_Transform.eulerAngles = value; }
        public float3 localEulerAngles { get => m_Transform.localEulerAngles; set => m_Transform.localEulerAngles = value; }
        public float3 lossyScale { get => m_Transform.lossyScale; set => throw new System.NotImplementedException(); }
        public float3 localScale { get => m_Transform.localScale; set => m_Transform.localScale = value; }

        public UnityTransformProvider(UnityEngine.Transform transform)
        {
            Assert.IsNotNull(transform);

            m_Transform = transform;
            if (transform.parent != null)
            {
                parent = new UnityTransformProvider(transform.parent);
            }
        }
        public UnityTransformProvider(UnityEngine.Transform transform, UnityEngine.AI.NavMeshAgent navAgent) : this(transform)
        {
            Assert.IsNotNull(navAgent);

            m_Agent = navAgent;
        }

        public void SetPosition(float3 position)
        {
            if (m_Agent == null)
            {
                this.position = position;
                return;
            }

            m_Agent.SetDestination(position);
        }
    }
}
