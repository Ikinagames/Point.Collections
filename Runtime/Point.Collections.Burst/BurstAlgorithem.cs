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

using Point.Collections.Buffer.LowLevel;
using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Point.Collections.Burst
{
    [BurstCompile(CompileSynchronously = true)]
    public static class BurstAlgorithem
    {
        // https://www.geeksforgeeks.org/bresenhams-circle-drawing-algorithm/?ref=lbp
        public static void FastCircle(int xc, int yc, int r, List<float3> list)
        {
            int
                x = 0, y = r,
                d = 3 - 2 * r;
            list.Add(float3(xc + x, yc + y, 0));
            list.Add(float3(xc - x, yc + y, 0));
            list.Add(float3(xc + x, yc - y, 0));
            list.Add(float3(xc - x, yc - y, 0));

            list.Add(float3(xc + y, yc + x, 0));
            list.Add(float3(xc - y, yc + x, 0));
            list.Add(float3(xc + y, yc - x, 0));
            list.Add(float3(xc - y, yc - x, 0));

            while (y >= x)
            {
                x++;

                if (d > 0)
                {
                    y--;
                    d = d + 4 * (x - y) + 10;
                }
                else d = d + 4 * x + 6;

                list.Add(float3(xc + x, yc + y, 0));
                list.Add(float3(xc - x, yc + y, 0));
                list.Add(float3(xc + x, yc - y, 0));
                list.Add(float3(xc - x, yc - y, 0));

                list.Add(float3(xc + y, yc + x, 0));
                list.Add(float3(xc - y, yc + x, 0));
                list.Add(float3(xc + y, yc - x, 0));
                list.Add(float3(xc - y, yc - x, 0));
            }
        }
        /// <summary>
        /// 8 * x = points
        /// </summary>
        /// <param name="xc"></param>
        /// <param name="yc"></param>
        /// <param name="r"></param>
        /// <param name="points"></param>
        /// <param name="iteration"></param>
        [BurstCompile]
        public static void BurstFastCircle(ref UnsafeAllocator<float3> points, [NoAlias] int xc, [NoAlias] int yc, [NoAlias] int r, int iteration = 0)
        {
            int
                x = 0, y = r,
                d = 3 - 2 * r;
            points[0] = float3(xc + x, yc + y, 0);
            points[1] = float3(xc - x, yc + y, 0);
            points[2] = float3(xc + x, yc - y, 0);
            points[3] = float3(xc - x, yc - y, 0);

            points[4] = float3(xc + y, yc + x, 0);
            points[5] = float3(xc - y, yc + x, 0);
            points[6] = float3(xc + y, yc - x, 0);
            points[7] = float3(xc - y, yc - x, 0);

            UnsafeFixedListWrapper<float3> list = new UnsafeFixedListWrapper<float3>(points, 8);

            int currentIter = 0;
            while (y >= x && currentIter < iteration)
            {
                x++;
                currentIter++;

                if (d > 0)
                {
                    y--;
                    d = d + 4 * (x - y) + 10;
                }
                else d = d + 4 * x + 6;

                list.AddNoResize(float3(xc + x, yc + y, 0));
                list.AddNoResize(float3(xc - x, yc + y, 0));
                list.AddNoResize(float3(xc + x, yc - y, 0));
                list.AddNoResize(float3(xc - x, yc - y, 0));

                list.AddNoResize(float3(xc + y, yc + x, 0));
                list.AddNoResize(float3(xc - y, yc + x, 0));
                list.AddNoResize(float3(xc + y, yc - x, 0));
                list.AddNoResize(float3(xc - y, yc - x, 0));
            }
        }
    }
}

#endif