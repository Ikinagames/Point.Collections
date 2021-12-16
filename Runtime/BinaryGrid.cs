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

using Point.Collections.Burst;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;

namespace Point.Collections
{
    [StructLayout(LayoutKind.Sequential)]
    public struct BinaryGrid : IValidation, IEquatable<BinaryGrid>
    {
        private readonly AABB m_AABB;

        private readonly int m_Length;
        private readonly float m_CellSize;

        public int length => m_Length;
        public int2 gridSize => new int2(
            (int)math.floor(m_AABB.size.x / m_CellSize),
            (int)math.floor(m_AABB.size.z / m_CellSize));

        public float cellSize => m_CellSize;
        public float3 center => m_AABB.center;
        public float3 size => m_AABB.size;
        public AABB bounds => m_AABB;

        public BinaryGrid(int3 center, int3 size, float cellSize)
        {
            m_AABB = new AABB(center, size);

            m_CellSize = cellSize;

            int
                xSize = (int)math.floor(size.x / cellSize),
                zSize = (int)math.floor(size.z / cellSize);
            m_Length = xSize * zSize;
        }

        #region Get Set

        public bool HasCell(int idx) => idx < m_Length;
        public bool HasCell(int x, int y)
        {
            if (x < 0 || y < 0 ||
                    x > m_AABB.size.x || y > m_AABB.size.z) return false;

            return HasCell(BurstMath.LocationToIndex(m_AABB, x, y));
        }
        public bool HasCell(in float3 position) => m_AABB.Contains(position) && HasCell(BurstMath.PositionToIndex(in m_AABB, in m_CellSize, in position));

        public int PositionToIndex(float3 position) => BurstMath.PositionToIndex(in m_AABB, in m_CellSize, in position);
        public int2 PositionToLocation(float3 position) => BurstMath.PositionToLocation(in m_AABB, in m_CellSize, in position);

        public float3 IndexToPosition(in int idx) => BurstMath.IndexToPosition(in m_AABB, in m_CellSize, in idx);
        public int2 IndexToLocation(in int idx) => BurstMath.IndexToLocation(in m_AABB, in m_CellSize, in idx);

        public float3 LocationToPosition(int2 location) => BurstMath.LocationToPosition(in m_AABB, in m_CellSize, in location);
        public int LocationToIndex(in int2 location) => BurstMath.LocationToIndex(in m_AABB, in m_CellSize, in location);

        public float3 PositionToPosition(float3 position)
        {
            int2 idx = BurstMath.PositionToLocation(in m_AABB, in m_CellSize, in position);
            return BurstMath.LocationToPosition(in m_AABB, in m_CellSize, in idx);
        }

        #endregion

        #region Get Ranges

        public FixedList32Bytes<int> GetRange8(in int idx, in int range)
        {
            int2 gridSize = this.gridSize;
            FixedList32Bytes<int> targets = new FixedList32Bytes<int>();

            int count = 0;

            int startIdx = idx - range + (gridSize.y * range);
            int height = ((range * 2) + 1);
            for (int yGrid = 0; yGrid < height; yGrid++)
            {
                for (int xGrid = 0; xGrid < height; xGrid++)
                {
                    int temp = startIdx - (yGrid * gridSize.y) + xGrid;

                    if (HasCell(temp))
                    {
                        targets.Add(temp);
                        count += 1;
                    }
                    //if (temp >= temp - (temp % gridSize.x) + gridSize.x - 1) break;
                }
            }
            return targets;
        }
        public FixedList64Bytes<int> GetRange16(in int idx, in int range)
        {
            int2 gridSize = this.gridSize;
            FixedList64Bytes<int> targets = new FixedList64Bytes<int>();

            int count = 0;

            int startIdx = idx - range + (gridSize.y * range);
            int height = ((range * 2) + 1);
            for (int yGrid = 0; yGrid < height; yGrid++)
            {
                for (int xGrid = 0; xGrid < height; xGrid++)
                {
                    int temp = startIdx - (yGrid * gridSize.y) + xGrid;

                    if (HasCell(temp))
                    {
                        targets.Add(temp);
                        count += 1;
                    }
                    //if (temp >= temp - (temp % gridSize.x) + gridSize.x - 1) break;
                }
            }
            return targets;
        }
        public FixedList128Bytes<int> GetRange32(in int idx, in int range)
        {
            int2 gridSize = this.gridSize;
            FixedList128Bytes<int> targets = new FixedList128Bytes<int>();

            int count = 0;

            int startIdx = idx - range + (gridSize.y * range);
            int height = ((range * 2) + 1);
            for (int yGrid = 0; yGrid < height; yGrid++)
            {
                for (int xGrid = 0; xGrid < height; xGrid++)
                {
                    int temp = startIdx - (yGrid * gridSize.y) + xGrid;

                    if (HasCell(temp))
                    {
                        targets.Add(temp);
                        count += 1;
                    }
                    //if (temp >= temp - (temp % gridSize.x) + gridSize.x - 1) break;
                }
            }
            return targets;
        }
        public FixedList4096Bytes<int> GetRange1024(in int idx, in int range)
        {
            int2 gridSize = this.gridSize;
            FixedList4096Bytes<int> targets = new FixedList4096Bytes<int>();

            int count = 0;

            int startIdx = idx - range + (gridSize.y * range);
            int height = ((range * 2) + 1);
            for (int yGrid = 0; yGrid < height; yGrid++)
            {
                for (int xGrid = 0; xGrid < height; xGrid++)
                {
                    int temp = startIdx - (yGrid * gridSize.y) + xGrid;

                    if (HasCell(temp))
                    {
                        targets.Add(temp);
                        count += 1;
                    }
                    //if (temp >= temp - (temp % gridSize.x) + gridSize.x - 1) break;
                }
            }
            return targets;
        }

        public void GetRange(ref NativeList<int> targets, in int idx, in int range)
        {
            targets.Clear();
            int2 gridSize = this.gridSize;

            int count = 0;

            int startIdx = idx - range + (gridSize.y * range);
            int height = ((range * 2) + 1);
            for (int yGrid = 0; yGrid < height; yGrid++)
            {
                for (int xGrid = 0; xGrid < height; xGrid++)
                {
                    int temp = startIdx - (yGrid * gridSize.y) + xGrid;

                    if (HasCell(temp))
                    {
                        //$"add {temp}".ToLog();
                        targets.Add(temp);
                        count += 1;
                    }
                    //if (temp >= temp - (temp % gridSize.x) + gridSize.x - 1) break;
                }
            }
        }
        unsafe public void GetRange(in int* buffer, in int bufferLength, in int idx, in int range, out int count)
        {
            //targets.Clear();
            int2 gridSize = this.gridSize;
            //Debug.Log($"from{IndexToLocation(idx)}: {bufferLength}");

            count = 0;

            int startIdx = idx - range + (gridSize.y * range);
            int height = ((range * 2) + 1);
            for (int yGrid = 0; yGrid < height; yGrid++)
            {
                for (int xGrid = 0; xGrid < height; xGrid++)
                {
                    int temp = startIdx - (yGrid * gridSize.y) + xGrid;

                    if (HasCell(temp))
                    {
                        //$"add {temp}".ToLog();
                        //Debug.Log($"add {IndexToLocation(temp)}");
                        buffer[count] = (temp);
                        count += 1;
                    }
                    //if (temp >= temp - (temp % gridSize.x) + gridSize.x - 1) break;

                    if (count >= bufferLength) return;
                }
            }
        }

        [Obsolete]
        public int[] GetRange(in int idx, in int range)
        {
            int2 gridSize = this.gridSize;
            List<int> targets = new List<int>();

            int startIdx = idx - range + (gridSize.y * range);
            int height = ((range * 2) + 1);
            for (int yGrid = 0; yGrid < height; yGrid++)
            {
                for (int xGrid = 0; xGrid < height; xGrid++)
                {
                    int temp = startIdx - (yGrid * gridSize.y) + xGrid;

                    if (HasCell(temp))
                    {
                        //$"add {temp}".ToLog();
                        targets.Add(temp);
                    }
                    //if (temp >= temp - (temp % gridSize.x) + gridSize.x - 1) break;
                }
            }

            return targets.ToArray();
        }
        [Obsolete]
        public int[] GetRange(in int2 location, in int range) => GetRange(BurstMath.LocationToIndex(in m_AABB, in m_CellSize, in location), in range);

        #endregion

        public int2 GetDirection(in int from, in Direction direction)
            => GetDirection(IndexToLocation(in from), in direction);
        public int2 GetDirection(in int2 from, in Direction direction)
        {
            int2 location = from;
            if ((direction & Direction.Up) == Direction.Up)
            {
                location.y += 1;
            }
            if ((direction & Direction.Down) == Direction.Down)
            {
                location.y -= 1;
            }
            if ((direction & Direction.Left) == Direction.Left)
            {
                location.x -= 1;
            }
            if ((direction & Direction.Right) == Direction.Right)
            {
                location.x += 1;
            }
            return location;
        }

        public bool IsValid() => m_Length != 0;
        public bool Equals(BinaryGrid other)
        {
            return m_Length == other.length && m_CellSize == other.cellSize && m_AABB.Equals(other.m_AABB);
        }
    }
}
