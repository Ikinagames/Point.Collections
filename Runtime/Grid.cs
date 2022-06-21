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
#if UNITY_2019 && !UNITY_2020_OR_NEWER
#define UNITYENGINE_OLD
#if !UNITY_MATHEMATICS
using math = Point.Collections.Math;
#endif
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Point.Collections.Burst;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_COLLECTIONS
using Unity.Collections;
#endif
#if UNITY_MATHEMATICS
using Unity.Mathematics;
#endif

#if UNITY_COLLECTIONS && UNITY_MATHEMATICS

namespace Point.Collections
{
    [BurstCompatible]
    internal unsafe struct Grid
    {
        public readonly short m_CheckSum;

        private AABB m_AABB;
        private float m_CellSize;

        public int length
        {
            get
            {
                float3 size = m_AABB.size;
                int
                    xSize = Convert.ToInt32(math.floor(size.x / m_CellSize)),
                    zSize = Convert.ToInt32(math.floor(size.z / m_CellSize));
                return xSize * zSize;
            }
        }
        public AABB aabb { get => m_AABB; set => m_AABB = value; }
        public float cellSize { get => m_CellSize; set => m_CellSize = value; }
        public int3 gridSize
        {
            get
            {
                float3 size = m_AABB.size;
                return new int3(
                    Convert.ToInt32(math.floor(size.x / m_CellSize)),
                    Convert.ToInt32(math.floor(size.y / m_CellSize)),
                    Convert.ToInt32(math.floor(size.z / m_CellSize)));
            }
        }

        internal Grid(AABB aabb, float cellSize)
        {
            this = default(Grid);

            m_CheckSum = CollectionUtility.CreateHashCodeInt16();

            m_AABB = aabb;
            m_CellSize = cellSize;
        }

        #region Index

        public int3 PositionToLocation(in float3 position)
        {
            int3 location;
            unsafe
            {
                BurstGridMathematics.positionToLocation(in m_AABB, in m_CellSize, in position, &location);
            }
            return location;
        }
        public ulong PositionToIndex(in float3 position)
        {
            ulong index;
            unsafe
            {
                BurstGridMathematics.positionToIndex(in m_AABB, in m_CellSize, in position, &index);
            }
            return index;
        }
        public ulong LocationToIndex(in int3 location)
        {
            ulong index;
            unsafe
            {
                BurstGridMathematics.locationToIndex(in location, &index);
            }
            return index;
        }
        public float3 LocationToPosition(in int3 location)
        {
            float3 position;
            unsafe
            {
                BurstGridMathematics.locationToPosition(in m_AABB, in m_CellSize, in location, &position);
            }
            return position;
        }
        public int3 IndexToLocation(in ulong index)
        {
            int3 location;
            unsafe
            {
                BurstGridMathematics.indexToLocation(in index, &location);
            }
            return location;
        }
        public float3 IndexToPosition(in ulong index)
        {
            float3 position;
            unsafe
            {
                BurstGridMathematics.indexToPosition(in m_AABB, in m_CellSize, in index, &position);
            }
            return position;
        }

        public FixedList4096Bytes<GridIndex> AABBToIndices(in AABB aabb)
        {
            FixedList4096Bytes<ulong> temp;
            unsafe
            {
                BurstGridMathematics.aabbToIndices(in m_AABB, in m_CellSize, aabb, &temp);
            }
            FixedList4096Bytes<GridIndex> list = new FixedList4096Bytes<GridIndex>();

            for (int i = 0; i < temp.Length; i++)
            {
                list.Add(new GridIndex(this, temp[i]));
            }

            return list;
        }

        #endregion

        public void GetMinMaxLocation(out int3 min, out int3 max)
        {
            int3 tempMin, tempMax;
            BurstGridMathematics.minMaxLocation(aabb, cellSize, &tempMin, &tempMax);

            min = tempMin;
            max = tempMax;
        }

        public bool Contains(in ulong index)
        {
            bool result;
            BurstGridMathematics.containIndex(in m_AABB, in m_CellSize, in index, &result);
            return result;
        }
        public bool Contains(in int3 location)
        {
            bool result;
            BurstGridMathematics.containLocation(in m_AABB, in m_CellSize, in location, &result);
            return result;
        }
        public bool Contains(in float3 position)
        {
            return m_AABB.Contains(position);
        }
        public bool Contains(in AABB aabb)
        {
            return m_AABB.Contains(aabb.min) && m_AABB.Contains(aabb.max);
        }

        public struct RangeEnumerator : IEnumerable<GridIndex>
        {
            private Grid m_Grid;
            private GridIndex m_From;
            private int3 m_Range;

            internal RangeEnumerator(
                in Grid grid,
                in GridIndex from,
                in int3 range)
            {
                m_Grid = grid;
                m_From = from;
                m_Range = range;
            }

            public IEnumerator<GridIndex> GetEnumerator()
            {
                int maxCount = ((m_Range.x * 2) + 1) * ((m_Range.z * 2) + 1) * ((m_Range.y * 2) + 1);
                if (maxCount > 255)
                {
                    PointHelper.LogError(Channel.Collections,
                        $"You\'re trying to get range of grid that exceeding length 255. " +
                        $"Buffer is fixed to 255 length, overloading indices({maxCount - 255}) will be dropped.");
                }

                int3
                    location = m_From.Location,
                    minRange, maxRange;
                m_Grid.GetMinMaxLocation(out minRange, out maxRange);

                int
                    minX = location.x - m_Range.x < 0 ? 0 : math.min(location.x - m_Range.x, maxRange.x),
                    maxX = location.x + m_Range.x < 0 ? 0 : math.min(location.x + m_Range.x, maxRange.x),

                    minY = location.y - m_Range.y < 0 ?
                        math.max(location.y - m_Range.y, minRange.y) : math.min(location.y - m_Range.y, maxRange.y),
                    maxY = location.y + m_Range.y < 0 ?
                        math.max(location.y + m_Range.y, minRange.y) : math.min(location.y + m_Range.y, maxRange.y),

                    minZ = location.z - m_Range.z < 0 ? 0 : math.min(location.z - m_Range.z, maxRange.z),
                    maxZ = location.z + m_Range.z < 0 ? 0 : math.min(location.z + m_Range.z, maxRange.z);

                int3
                    start = new int3(minX, minY, minZ),
                    end = new int3(maxX, maxY, maxZ);

                int count = 0;
                for (int y = start.y; y < end.y + 1 && count < maxCount; y++)
                {
                    for (int x = start.x; x < end.x + 1 && count < maxCount; x++)
                    {
                        for (int z = start.z;
                            z < end.z + 1 && count < maxCount;
                            z++, count++)
                        {
                            yield return new GridIndex(m_Grid.m_CheckSum, new int3(x, y, z));
                        }
                    }
                }
            }
            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }

        public RangeEnumerator GetRange(in GridIndex from,
            in int3 range)
        {
            return new RangeEnumerator(in this, in from, in range);
        }
    }
    [BurstCompatible]
    public readonly struct GridIndex : IEmpty, IEquatable<GridIndex>, IEquatable<int3>
    {
        internal readonly short m_CheckSum;
        private readonly ulong m_Index;

        public ulong Index => m_Index;
        public int3 Location
        {
            get
            {
                int3 location;
                unsafe
                {
                    BurstGridMathematics.indexToLocation(in m_Index, &location);
                }
                return location;
            }
        }

        internal GridIndex(Grid grid, ulong index)
        {
            m_CheckSum = grid.m_CheckSum;
            m_Index = index;
        }
        internal GridIndex(short checkSum, ulong index)
        {
            m_CheckSum = checkSum;
            m_Index = index;
        }
        internal GridIndex(short checkSum, int3 location)
        {
            m_CheckSum = checkSum;
            ulong temp;
            unsafe
            {
                BurstGridMathematics.locationToIndex(location, &temp);
            }
            m_Index = temp;
        }

        //public GridIndex GetDirection(in Direction direction)
        //{
        //    int3 location = Location;
        //    if ((direction & Direction.Up) == Direction.Up)
        //    {
        //        location.y += 1;
        //    }
        //    if ((direction & Direction.Down) == Direction.Down)
        //    {
        //        location.y -= 1;
        //    }
        //    if ((direction & Direction.Left) == Direction.Left)
        //    {
        //        location.x -= 1;
        //    }
        //    if ((direction & Direction.Right) == Direction.Right)
        //    {
        //        location.x += 1;
        //    }
        //    if ((direction & Direction.Forward) == Direction.Forward)
        //    {
        //        location.z += 1;
        //    }
        //    if ((direction & Direction.Forward) == Direction.Backward)
        //    {
        //        location.z -= 1;
        //    }
        //    return new GridIndex(m_CheckSum, location);
        //}

        public bool Equals(GridIndex other) => m_Index.Equals(other.m_Index) && m_CheckSum.Equals(m_CheckSum);
        public bool Equals(int3 other) => Location.Equals(other);

        public bool IsEmpty() => m_CheckSum == 0 && m_Index == 0;

        [NotBurstCompatible]
        public override string ToString() => $"{m_Index}({Location})";
    }
    public struct GridBlock
    {
        public int3 GridIndex;

        // 0 Left
        // 1 Right
        // 2 Down (Forward)
        // 3 Up (Backward)
        public bool4 HasBlocks;
        public float3x2 Vertices0, Vertices1, Vertices2, Vertices3;
    }
}

#endif