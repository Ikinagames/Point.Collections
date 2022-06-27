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

namespace Point.Collections.Formations
{
    public abstract class FormationGroup : IFormationGroupProvider
    {
        public bool EnableLerp { get; set; } = false;
        public float StopDistance { get; set; } = .2f;
        public float UpdateMultipiler { get; set; } = 1;

        public IReadOnlyList<IFormation> children { get; set; }

        public abstract float3 CalculateOffset(int index, IFormation child);

        float3 IFormationGroupProvider.UpdatePosition(int index, IFormation child, float3 targetLocalPosition)
        {
            return UpdatePosition(index, child, targetLocalPosition);
        }
        protected virtual float3 UpdatePosition(int index, IFormation child, float3 targetLocalPosition)
        {
            if (!EnableLerp) return targetLocalPosition;

            return math.lerp(child.localPosition, targetLocalPosition, Time.deltaTime * UpdateMultipiler);
        }
    }
}
