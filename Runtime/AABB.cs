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

#if UNITY_2020
#define UNITYENGINE
#endif

using Newtonsoft.Json;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if UNITYENGINE
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
#endif

namespace Point.Collections
{
#if UNITYENGINE
    [BurstCompile(CompileSynchronously = true, DisableSafetyChecks = true)]
#endif
    [StructLayout(LayoutKind.Sequential)]
    [JsonConverter(typeof(IO.Json.AABBJsonConverter))]
    [Guid("a4c54f61-12c0-4069-92ca-9d1881952f2d")]
    public struct AABB : IEquatable<AABB>
    {
        public static readonly AABB Zero = new AABB(float3.zero, float3.zero);

        private float3 m_Center;
        private float3 m_Extents;

        public AABB(float3 center, float3 size)
        {
            m_Center = center;
            m_Extents = size * .5f;
        }
        public AABB(int3 center, int3 size)
        {
            m_Center = new float3(center);
            m_Extents = new float3(size) * .5f;
        }

#pragma warning disable IDE1006 // Naming Styles
        [JsonIgnore] public float3 center { get => m_Center; set { m_Center = value; } }
        [JsonIgnore] public float3 size { get => m_Extents * 2; set { m_Extents = value * 0.5F; } }
        [JsonIgnore] public float3 extents { get => m_Extents; set { m_Extents = value; } }
        [JsonIgnore] public float3 min { get => center - extents; set { SetMinMax(value, max); } }
        [JsonIgnore] public float3 max { get => center + extents; set { SetMinMax(min, value); } }

        //[JsonIgnore] public float3[] vertices => GetVertices(in this);
#pragma warning restore IDE1006 // Naming Styles

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetMinMax(float3 min, float3 max)
        {
            extents = (max - min) * .5f;
            center = min + extents;
        }

        public bool Contains(float3 position)
        {
            float3
                min = this.min,
                max = this.max;

            return position.x >= min.x
                && position.y >= min.y
                && position.z >= min.z
                && position.x < max.x
                && position.y < max.y
                && position.z < max.z;
        }

        public bool Intersect(Ray ray)
        {
            float3x4[] squares = GetSquares(in this);

            for (int i = 0; i < squares.Length; i++)
            {
                if (IntersectQuad(squares[i].c0, squares[i].c1, squares[i].c2, squares[i].c3, ray, out _))
                {
                    return true;
                }
            }
            return false;
        }
        public bool Intersect(Ray ray, out float distance)
        {
            distance = float.MaxValue;
            float3x4[] squares = GetSquares(in this);

            bool intersect = false;
            for (int i = 0; i < squares.Length; i++)
            {
                if (IntersectQuad(squares[i].c0, squares[i].c1, squares[i].c2, squares[i].c3, ray, out float tempDistance))
                {
                    if (tempDistance < distance)
                    {
                        distance = tempDistance;
                    }
                    intersect = true;
                }
            }
            return intersect;
        }
        public bool Intersect(Ray ray, out float distance, out float3 point)
        {
            point = new float3(float.MaxValue, float.MaxValue, float.MaxValue);
            bool intersect = Intersect(ray, out distance);
            if (intersect)
            {
                point = ray.origin + (ray.direction * distance);
            }

            return intersect;
        }
        public bool Intersect(AABB aabb)
        {
            return (min.x <= aabb.max.x) && (max.x >= aabb.min.x) &&
                (min.y <= aabb.max.y) && (max.y >= aabb.min.y) &&
                (min.z <= aabb.max.z) && (max.z >= aabb.min.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Encapsulate(float3 point) =>
#if !UNITYENGINE
            SetMinMax(Math.min(min, point), Math.max(max, point));
#else
            SetMinMax(math.min(min, point), math.max(max, point));
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Encapsulate(AABB aabb)
        {
            Encapsulate(aabb.center - aabb.extents);
            Encapsulate(aabb.center + aabb.extents);
        }

#if UNITYENGINE
        public AABB Rotation(in quaternion rot, in float3 scale)
        {
            AABB result;
            unsafe
            {
                Burst.BurstMath.aabb_calculateRotationWithVertices(in this, in rot, in scale, &result);
            }
            
            return result;
        }
#endif
#if UNITYENGINE
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public NativeArray<float3> GetVertices(Allocator allocator)
        {
            NativeArray<float3> temp = new NativeArray<float3>(8, allocator);
            temp[0] = min;
            temp[1] = new float3(min.x, max.y, min.z);
            temp[2] = new float3(max.x, max.y, min.z);
            temp[3] = new float3(max.x, min.y, min.z);

            temp[4] = new float3(max.x, min.y, max.z);
            temp[5] = max;
            temp[6] = new float3(min.x, max.y, max.z);
            temp[7] = new float3(min.x, min.y, max.z);
            return temp;
        }
#endif
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float3[] GetVertices(in AABB aabb)
        {
            float3 min = aabb.min;
            float3 max = aabb.max;

            return new float3[]
            {
                min,
                new float3(min.x, max.y, min.z),
                new float3(max.x, max.y, min.z),
                new float3(max.x, min.y, min.z),

                new float3(max.x, min.y, max.z),
                max,
                new float3(min.x, max.y, max.z),
                new float3(min.x, min.y, max.z),
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static float3x4[] GetSquares(in AABB aabb)
        {
            float3
                minPos = aabb.min,
                maxPos = aabb.max;

            return new float3x4[] {
                new float3x4(
                    minPos,
                    new float3(minPos.x, minPos.y, maxPos.z),
                    new float3(maxPos.x, minPos.y, maxPos.z),
                    new float3(maxPos.x, minPos.y, minPos.z)
                    ),
                new float3x4(
                    minPos,
                    new float3(minPos.x, maxPos.y, minPos.z),
                    new float3(maxPos.x, maxPos.y, minPos.z),
                    new float3(maxPos.x, minPos.y, maxPos.z)
                    ),
                new float3x4(
                    new float3(maxPos.x, minPos.y, minPos.z),
                    new float3(maxPos.x, maxPos.y, minPos.z),
                    new float3(maxPos.x, maxPos.y, maxPos.z),
                    new float3(maxPos.x, minPos.y, maxPos.z)
                    ),
                new float3x4(
                    new float3(maxPos.x, minPos.y, maxPos.z),
                    new float3(maxPos.x, maxPos.y, maxPos.z),
                    new float3(minPos.x, maxPos.y, maxPos.z),
                    new float3(minPos.x, minPos.y, maxPos.z)
                    ),
                new float3x4(
                    new float3(minPos.x, minPos.y, maxPos.z),
                    new float3(minPos.x, maxPos.y, maxPos.z),
                    new float3(minPos.x, maxPos.y, minPos.z),
                    minPos
                    ),
                new float3x4(
                    new float3(minPos.x, maxPos.y, minPos.z),
                    new float3(minPos.x, maxPos.y, maxPos.z),
                    new float3(maxPos.x, maxPos.y, maxPos.z),
                    new float3(maxPos.x, maxPos.y, minPos.z)
                    )
            };
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static bool IntersectQuad(float3 p1, float3 p2, float3 p3, float3 p4, Ray ray, out float distance)
        {
            if (IntersectTriangle(p1, p2, p4, ray, out distance)) return true;
            else if (IntersectTriangle(p3, p4, p2, ray, out distance)) return true;
            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        /// <summary>
        /// Checks if the specified ray hits the triagnlge descibed by p1, p2 and p3.
        /// Möller–Trumbore ray-triangle intersection algorithm implementation.
        /// </summary>
        /// <param name="p1">Vertex 1 of the triangle.</param>
        /// <param name="p2">Vertex 2 of the triangle.</param>
        /// <param name="p3">Vertex 3 of the triangle.</param>
        /// <param name="ray">The ray to test hit for.</param>
        /// <returns><c>true</c> when the ray hits the triangle, otherwise <c>false</c></returns>
        private static bool IntersectTriangle(in float3 p1, in float3 p2, in float3 p3, Ray ray, out float distance)
        {
#if !UNITYENGINE
            distance = 0;
            // Vectors from p1 to p2/p3 (edges)
            float3 e1, e2;

            float3 p, q, t;
            float det, invDet, u, v;

            //Find vectors for two edges sharing vertex/point p1
            e1 = p2 - p1;
            e2 = p3 - p1;

            // calculating determinant 
            p = Math.cross(ray.direction, e2);

            //Calculate determinat
            det = Math.dot(e1, p);

            //if determinant is near zero, ray lies in plane of triangle otherwise not
            if (det > -Math.EPSILON && det < Math.EPSILON) { return false; }
            invDet = 1.0f / det;

            //calculate distance from p1 to ray origin
            t = ((float3)ray.origin) - p1;

            //Calculate u parameter
            u = Math.dot(t, p) * invDet;

            //Check for ray hit
            if (u < 0 || u > 1) { return false; }

            //Prepare to test v parameter
            q = Math.cross(t, e1);

            //Calculate v parameter
            v = Math.dot(ray.direction, q) * invDet;

            //Check for ray hit
            if (v < 0 || u + v > 1) { return false; }

            distance = (Math.dot(e2, q) * invDet);
            if (distance > Math.EPSILON)
            {
                //ray does intersect
                return true;
            }

            // No hit at all
            return false;
#else
            distance = 0;
            // Vectors from p1 to p2/p3 (edges)
            float3 e1, e2;

            float3 p, q, t;
            float det, invDet, u, v;

            //Find vectors for two edges sharing vertex/point p1
            e1 = p2 - p1;
            e2 = p3 - p1;

            // calculating determinant 
            p = math.cross(ray.direction, e2);

            //Calculate determinat
            det = math.dot(e1, p);

            //if determinant is near zero, ray lies in plane of triangle otherwise not
            if (det > -math.EPSILON && det < math.EPSILON) { return false; }
            invDet = 1.0f / det;

            //calculate distance from p1 to ray origin
            t = ((float3)ray.origin) - p1;

            //Calculate u parameter
            u = math.dot(t, p) * invDet;

            //Check for ray hit
            if (u < 0 || u > 1) { return false; }

            //Prepare to test v parameter
            q = math.cross(t, e1);

            //Calculate v parameter
            v = math.dot(ray.direction, q) * invDet;

            //Check for ray hit
            if (v < 0 || u + v > 1) { return false; }

            distance = (math.dot(e2, q) * invDet);
            if (distance > math.EPSILON)
            {
                //ray does intersect
                return true;
            }

            // No hit at all
            return false;
#endif
        }

        public bool Equals(AABB other)
        {
            return m_Center.Equals(other.m_Center) && m_Extents.Equals(other.m_Extents);
        }

#if UNITYENGINE
        public static implicit operator AABB(Bounds a) => new AABB(a.center, a.size);
        public static implicit operator Bounds(AABB a) => new Bounds(a.center, a.size);
#endif
    }
}
