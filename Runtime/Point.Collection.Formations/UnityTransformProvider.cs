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

namespace Point.Collections.Formations
{
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
                //if (value == null && m_Parent is UnityTransformProvider)
                //{
                //    m_Transform.SetParent(null);
                //}
                m_Parent = value;
                //if (value != null && value is UnityTransformProvider unityTransform)
                //{
                //    m_Transform.SetParent(unityTransform.m_Transform);
                //}
            }
        }
        public UnityEngine.AI.NavMeshAgent NavMeshAgent => m_Agent;
        
        public float4x4 localToWorld => m_Transform.localToWorldMatrix;
        public float4x4 worldToLocal => m_Transform.worldToLocalMatrix;

        public float3 position
        {
            get
            {
                if (parent == null) return localPosition;

                return math.mul(parent.localToWorld, new float4(localPosition, 1)).xyz;
            }
            set
            {
                if (parent == null)
                {
                    localPosition = value;
                    return;
                }

                localPosition = math.mul(parent.worldToLocal, new float4(value, 1)).xyz;
            }
        }
        public float3 localPosition
        {
            get
            {
                if (parent == null) return m_Transform.position;

                float3 currentPos = m_Transform.position;
                return math.mul(parent.worldToLocal, new float4(currentPos, 1)).xyz;
            }
            set
            {
                if (parent == null)
                {
                    m_Transform.position = value;
                    return;
                }

                m_Transform.position = math.mul(parent.localToWorld, new float4(value, 1)).xyz;
            }
        }
        public quaternion rotation
        {
            get
            {
                if (parent == null) return localRotation;

                return math.mul(localRotation, parent.rotation);
            }
            set
            {
                if (parent == null)
                {
                    localRotation = value;
                    return;
                }

                localRotation = math.mul(parent.rotation, value);
            }
        }
        public quaternion localRotation
        {
            get => m_Transform.rotation;
            set => m_Transform.rotation = value;
        }
        public float3 eulerAngles
        {
            get => rotation.Euler() * Math.Rad2Deg;
            set
            {
                float3 temp = value * Math.Deg2Rad;
                // .001f
                temp = math.round(temp * 1000) * 0.001f;
                rotation = quaternion.EulerZXY(temp);
            }
        }
        public float3 localEulerAngles
        {
            get => localRotation.Euler() * Math.Rad2Deg;
            set
            {
                float3 temp = value * Math.Deg2Rad;
                // .001f
                temp = math.round(temp * 1000) * 0.001f;
                localRotation = quaternion.EulerZXY(temp);
            }
        }
        public float3 lossyScale 
        { 
            get
            {
                if (parent == null)
                {
                    return localScale;
                }
                return parent.lossyScale / localScale;
            }
            set => throw new System.NotImplementedException(); 
        }
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
            m_Agent = navAgent;
        }

        /// <summary>
        /// Perform data transformation setter.
        /// </summary>
        /// <param name="position"></param>
        public void SetPosition(float3 position)
        {
            //m_Destination = position;

            if (m_Agent == null)
            {
                this.position = position;
                return;
            }

            m_Agent.SetDestination(position);
        }
    }
}
