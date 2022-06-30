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

using Unity.Mathematics;
using UnityEngine;

namespace Point.Collections.SceneManagement.LowLevel
{
    public class RandomPositionTransform : NativeTransform
    {
        [SerializeField] private bool m_Lerp;
        [SerializeField] private Vector3 m_TargetPosition;

        public override bool EnableOnAwake => true;

        private void Start()
        {
            SetRandomTarget();
        }
        private void Update()
        {
            localPosition = Vector3.Lerp(localPosition, m_TargetPosition, Time.deltaTime);

            float3 temp = ((float3)m_TargetPosition - localPosition);
            if (math.mul(temp, temp) < 100) SetRandomTarget();
        }

        private void SetRandomTarget()
        {
            m_TargetPosition = transform.position + GetRandomVector(-100, 100);
        }
        private static Vector3 GetRandomVector(float st, float en)
        {
            return new Vector3(
                GetRandom(st, en),
                GetRandom(st, en),
                GetRandom(st, en));
        }
        private static float GetRandom(float st, float en)
        {
            return UnityEngine.Random.Range(st, en);
        }
    }
}
