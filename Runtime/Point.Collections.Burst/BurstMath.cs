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

using System;
using Unity.Burst;
using Unity.Mathematics;

namespace Point.Collections.Burst
{
    [BurstCompile]
    public static unsafe class BurstMath
    {
#if !POINT_COLLECTIONS_NATIVE
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
#endif

        #region Grid

        [BurstCompile]
        private static void grid_positionToLocation(in AABB aabb, in float cellSize, in float3 pos, int2* result)
        {
            float
                half = cellSize * .5f,
                firstCenterX = aabb.min.x + half/* + (cellSize * 1)*/,
                firstCenterZ = aabb.max.z - half /*- (cellSize * 1)*/;

            int
                x = math.abs(Convert.ToInt32((pos.x - firstCenterX) / cellSize)),
                y = math.abs(Convert.ToInt32((pos.z - firstCenterZ) / cellSize));

            result->x = x;
            result->y = y;
        }
        [BurstCompile]
        private static void grid_locationToindex(in AABB aabb, in float cellSize, in int x, in int y, int* output)
        {
            int zSize = (int)math.floor(aabb.size.z / cellSize);

            *output = zSize * y + x;
        }
        [BurstCompile]
        private static void grid_locationToPosition(in AABB aabb, in float cellSize, in int x, in int y, float3* output)
        {
            float
                half = cellSize * .5f,
                targetX = aabb.min.x + half + (cellSize * x),
                targetY = aabb.center.y,
                targetZ = aabb.max.z - half - (cellSize * y);

            *output = new float3(targetX, targetY, targetZ);
        }
        [BurstCompile]
        private static void grid_indexToLocation(in AABB aabb, in float cellSize, in int idx, int2* output)
        {
            if (idx == 0)
            {
                *output = int2.zero;
                return;
            }

            int zSize = (int)math.floor(aabb.size.z / cellSize);

            int y = idx / zSize;
            int x = idx - (zSize * y);

            *output = new int2(x, y);
        }

        public static int2 PositionToLocation(in AABB aabb, in float cellSize, in float3 pos)
        {
            int2 location;
            grid_positionToLocation(in aabb, in cellSize, in pos, &location);

            return location;
        }
        public static int PositionToIndex(in AABB aabb, in float cellSize, in float3 pos)
        {
            int2 location = PositionToLocation(in aabb, in cellSize, in pos);
            return LocationToIndex(in aabb, in cellSize, in location);
        }

        public static int LocationToIndex(in AABB aabb, in float cellSize, in int2 xy) => LocationToIndex(in aabb, in cellSize, in xy.x, in xy.y);
        public static int LocationToIndex(in AABB aabb, in float cellSize, in int x, in int y)
        {
            int result;
            grid_locationToindex(in aabb, in cellSize, in x, in y, &result);

            return result;
        }
        public static float3 LocationToPosition(in AABB aabb, in float cellSize, in int2 xy) => LocationToPosition(in aabb, in cellSize, in xy.x, in xy.y);
        public static float3 LocationToPosition(in AABB aabb, in float cellSize, in int x, in int y)
        {
            float3 result;
            grid_locationToPosition(in aabb, in cellSize, in x, in y, &result);

            return result;
        }

        public static int2 IndexToLocation(in AABB aabb, in float cellSize, in int idx)
        {
            int2 result;
            grid_indexToLocation(in aabb, in cellSize, in idx, &result);

            return result;
        }
        public static float3 IndexToPosition(in AABB aabb, in float cellSize, in int idx)
        {
            int2 location = IndexToLocation(in aabb, in cellSize, in idx);
            return LocationToPosition(in aabb, in cellSize, in location);
        }

        public static AABB PositionToAABB(in AABB aabb, in float cellSize, in float3 pos)
        {
            float3 center = LocationToPosition(in aabb, in cellSize, PositionToLocation(in aabb, in cellSize, in pos));
            return new AABB(center, new float3(cellSize, aabb.size.y, cellSize));
        }
        public static AABB IndexToAABB(in AABB aabb, in float cellSize, in int cellIdx)
        {
            float3 center = IndexToPosition(in aabb, in cellSize, in cellIdx);
            return new AABB(center, new float3(cellSize, aabb.size.y, cellSize));
        }
        public static int AABBToIndex(in AABB aabb, in AABB cellAabb) => PositionToIndex(in aabb, cellAabb.size.x, cellAabb.center);
        public static int2 AABBToLocation(in AABB aabb, in AABB cellAabb) => PositionToLocation(in aabb, cellAabb.size.x, cellAabb.center);

        #endregion
    }
}
