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

namespace Point.Collections.Formations
{
    // https://www.hindawi.com/journals/mpe/2020/4383915/
    // https://www.oreilly.com/library/view/ai-for-game/0596005555/ch04.html

    public interface IFormationGroupProvider
    {
        /// <summary>
        /// 감속을 시작할 거리
        /// </summary>
        float StopDistance { get; set; }
        /// <summary>
        /// 평상 속도
        /// </summary>
        float Speed { get; set; }
        float Acceleration { get; set; }

#pragma warning disable IDE1006 // Naming Styles
        IReadOnlyList<IFormation> children { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        float3 CalculateOffset(int index, IFormation child);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="index"></param>
        /// <param name="child"></param>
        /// <param name="targetLocalPosition"></param>
        /// <returns>실제 적용할 로컬 좌표입니다.</returns>
        float3 UpdatePosition(int index, IFormation child, float3 targetLocalPosition);
    }
}
