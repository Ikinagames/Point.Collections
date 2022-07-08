// Copyright 2021 Ikina Games
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
#if UNITY_2019 && !UNITY_2020_OR_NEWER
#define UNITYENGINE_OLD
#if !UNITY_MATHEMATICS
using math = Point.Collections.Math;
#endif
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Point.Collections
{
    public static class GeometryMath
    {
        public static Bounds GetOcculusionBounds(GameObject obj)
        {
            Bounds worldBounds = new Bounds(obj.transform.position, Vector3.zero);
            foreach (var item in obj.GetComponentsInChildren<Renderer>())
            {
                float4x4 itemMat = item.transform.localToWorldMatrix;
                Bounds targetBounds =
#if UNITY_2021_1_OR_NEWER
                    item.localBounds
#else
                    item.bounds
#endif
                    ;
                float3 
                    min = targetBounds.min,
                    max = targetBounds.max;
                targetBounds = new Bounds(
                    mul(itemMat, float4(targetBounds.center, 1)).xyz,
                    Vector3.zero
                    );
                targetBounds.Encapsulate(
                    mul(itemMat, float4(min, 1)).xyz);
                targetBounds.Encapsulate(
                    mul(itemMat, float4(max, 1)).xyz);

                worldBounds.Encapsulate(targetBounds);
            }

            return worldBounds;
        }
    }
}
