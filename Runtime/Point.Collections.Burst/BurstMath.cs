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

#if UNITY_2019_1_OR_NEWER && UNITY_BURST && UNITY_MATHEMATICS
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

using Unity.Burst;
using Unity.Mathematics;

namespace Point.Collections.Burst
{
    /// <summary>
    /// Unity.Burst 와 Unity.Mathematics 가 필요합니다.
    /// </summary>
    [BurstCompile(CompileSynchronously = true)]
    public static unsafe class BurstMath
    {
        [BurstCompile]
        public static void unity_todB(double* linear, double* output)
        {
            const double kMindB = -80;

            if (*linear == 0) *output = kMindB;
            else
            {
                *output = 20 * math.log10(*linear);
            }
        }
        [BurstCompile]
        public static void unity_fromdB(double* dB, double* output)
        {
            *output = math.pow(10, *dB / 20);
        }

        #region AABB

        [BurstCompile]
        public static void aabb_calculateRotation(in AABB aabb, in quaternion quaternion, AABB* result)
        {
            float3
                originCenter = aabb.center,
                originExtents = aabb.extents,
                originMin = (-originExtents + originCenter),
                originMax = (originExtents + originCenter);
            float4x4 trMatrix = Unity.Mathematics.float4x4.TRS(originCenter, quaternion, originExtents);

            float3
                size = originExtents * 2,
                minPos = math.mul(trMatrix, new float4(-size, 1)).xyz,
                maxPos = math.mul(trMatrix, new float4(size, 1)).xyz;

            AABB temp = new AABB(originCenter, Unity.Mathematics.float3.zero);

            // TODO : 최소 width, height 값이 설정되지않아 무한대로 축소함. 수정할 것.
            temp.SetMinMax(
                originMin + (minPos - originMin),
                originMax + (maxPos - originMax));

            *result = temp;
        }
        [BurstCompile]
        public static void aabb_calculateRotationWithVertices(in AABB aabb, in quaternion quaternion, in float3 scale, AABB* result)
        {
            float3 originCenter = aabb.center;
            float4x4 trMatrix = Unity.Mathematics.float4x4.TRS(originCenter, quaternion, scale * .5f);

            AABB temp = new AABB(originCenter, Unity.Mathematics.float3.zero);

            float3
                min = aabb.min,
                max = aabb.max,

                a1 = new float3(min.x, max.y, min.z),
                a2 = new float3(max.x, max.y, min.z),
                a3 = new float3(max.x, min.y, min.z),

                b1 = new float3(max.x, min.y, max.z),
                b3 = new float3(min.x, max.y, max.z),
                b4 = new float3(min.x, min.y, max.z);

            temp.Encapsulate(math.mul(trMatrix, new float4((min - originCenter) * 2, 1)).xyz);
            temp.Encapsulate(math.mul(trMatrix, new float4((a1 - originCenter) * 2, 1)).xyz);
            temp.Encapsulate(math.mul(trMatrix, new float4((a2 - originCenter) * 2, 1)).xyz);
            temp.Encapsulate(math.mul(trMatrix, new float4((a3 - originCenter) * 2, 1)).xyz);
            temp.Encapsulate(math.mul(trMatrix, new float4((b1 - originCenter) * 2, 1)).xyz);
            temp.Encapsulate(math.mul(trMatrix, new float4((max - originCenter) * 2, 1)).xyz);
            temp.Encapsulate(math.mul(trMatrix, new float4((b3 - originCenter) * 2, 1)).xyz);
            temp.Encapsulate(math.mul(trMatrix, new float4((b4 - originCenter) * 2, 1)).xyz);

            *result = temp;
        }
        [BurstCompile]
        public static void aabb_getVertices(in AABB aabb, float3* buffer)
        {
            float3
                min = aabb.min,
                max = aabb.max;

            buffer[0] = aabb.min;
            buffer[1] = new float3(min.x, max.y, min.z);
            buffer[2] = new float3(max.x, max.y, min.z);
            buffer[3] = new float3(max.x, min.y, min.z);

            buffer[4] = new float3(max.x, min.y, max.z);
            buffer[5] = aabb.max;
            buffer[6] = new float3(min.x, max.y, max.z);
            buffer[7] = new float3(min.x, min.y, max.z);
        }

        #endregion
    }
}

#endif