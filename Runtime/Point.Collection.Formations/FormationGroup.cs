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
using UnityEngine;
using UnityEngine.AI;

namespace Point.Collections.Formations
{
    [System.Serializable]
    public abstract class FormationGroup : IFormationGroupProvider
    {
        [SerializeField] private float m_StopDistance;
        [SerializeField] private float m_Speed = 2;
        [SerializeField] private float m_Acceleration = 1;

        public float StopDistance { get => m_StopDistance; set => m_StopDistance = value; }
        public float Speed { get => m_Speed; set => m_Speed = value; }
        public float Acceleration { get => m_Acceleration; set => m_Acceleration = value; }

        public IReadOnlyList<IFormation> children { get; set; }

        float3 IFormationGroupProvider.CalculateOffset(int index, IFormation child)
        {
            return CalculateOffset(index, child);
        }
        float3 IFormationGroupProvider.UpdatePosition(int index, IFormation child, float3 targetLocalPosition)
        {
            return UpdatePosition(index, child, targetLocalPosition);
        }

        protected abstract float3 CalculateOffset(int index, IFormation child);
        protected virtual float3 UpdatePosition(int index, IFormation child, float3 targetLocalPosition)
        {
            float3 dir = targetLocalPosition - child.localPosition;

            if (child.targetSpeed == 0)
            {
                var temp = child.localPosition + (child.currentVelocity * Time.deltaTime * child.currentSpeed);

                return temp;
            }

            float3 norm = math.normalize(dir);
            float3 targetPos = child.localPosition + (norm * (Time.deltaTime * child.currentSpeed));

            return targetPos;
        }
    }
}
