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


using System.Collections.Generic;
using Unity.Mathematics;

namespace Point.Collections
{
    public interface IFormation
    {
        string DisplayName { get; }

#pragma warning disable IDE1006 // Naming Styles
        IFormation parent { get; set; }
        IEnumerator<IFormation> childs { get; }

        float4x4 localToWorld { get; }
        float4x4 worldToLocal { get; }
        float3 position { get; set; }
        float3 localPosition { get; set; }
        quaternion rotation { get; set; }
        quaternion localRotation { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }

    public abstract class Formation : IFormation
    {
        private float3 m_LocalPosition;
        private quaternion m_LocalRotation;

        public string DisplayName => throw new System.NotImplementedException();

        public IFormation parent { get; set; }
        public IEnumerator<IFormation> childs => throw new System.NotImplementedException();

        public float4x4 localToWorld => new float4x4(new float3x3(m_LocalRotation), m_LocalPosition);
        public float4x4 worldToLocal => math.fastinverse(worldToLocal);

        public float3 position
        {
            get
            {
                if (parent == null) return m_LocalPosition;

                return math.mul(parent.localToWorld, new float4(m_LocalPosition, 1)).xyz;
            }
            set
            {
                if (parent == null)
                {
                    m_LocalPosition = value;
                    return;
                }

                m_LocalPosition = math.mul(parent.worldToLocal, new float4(value, 1)).xyz;
            }
        }
        public float3 localPosition { get => m_LocalPosition; set => m_LocalPosition = value; }
        public quaternion rotation
        {
            get
            {
                if (parent == null) return m_LocalRotation;

                return math.mul(m_LocalRotation, parent.localRotation);
            }
            set
            {
                if (parent == null)
                {
                    m_LocalRotation = value;
                    return;
                }

                m_LocalRotation = math.mul(parent.localRotation, value);
            }
        }
        public quaternion localRotation { get => m_LocalRotation; set => m_LocalRotation = value; }
    }
}
