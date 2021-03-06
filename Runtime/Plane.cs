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
using Unity.Collections;
#if UNITY_MATHEMATICS
using Unity.Mathematics;
#else
using math = Point.Collections.Math;
using float3 = UnityEngine.Vector3;
#endif
#else
#define POINT_COLLECTIONS_NATIVE
using math = Point.Collections.Math;
#endif

using System;
using System.Globalization;

namespace Point.Collections
{
#if UNITYENGINE && UNITY_COLLECTIONS
    [BurstCompatible]
#endif
    public struct Plane
    {
        // sizeof(Plane) is not const in C# and so cannot be used in fixed arrays, so we define it here
        internal const int size = 16;

        float3 m_Normal;
        float m_Distance;

        // Normal vector of the plane.
        public float3 normal
        {
            get => m_Normal;
            set => m_Normal = value;
        }
        // Distance from the origin to the plane.
        public float distance
        {
            get => m_Distance;
            set => m_Distance = value;
        }

        // Creates a plane.
        public Plane(float3 inNormal, float3 inPoint)
        {
            m_Normal = math.normalize(inNormal);
            m_Distance = -math.dot(m_Normal, inPoint);
        }

        // Creates a plane.
        public Plane(float3 inNormal, float d)
        {
            m_Normal = math.normalize(inNormal);
            m_Distance = d;
        }

        // Creates a plane.
        public Plane(float3 a, float3 b, float3 c)
        {
            m_Normal = math.normalize(math.cross(b - a, c - a));
            m_Distance = -math.dot(m_Normal, a);
        }

        // Sets a plane using a point that lies within it plus a normal to orient it (note that the normal must be a normalized vector).
        public void SetNormalAndPosition(float3 inNormal, float3 inPoint)
        {
            m_Normal = math.normalize(inNormal);
            m_Distance = -math.dot(inNormal, inPoint);
        }

        // Sets a plane using three points that lie within it.  The points go around clockwise as you look down on the top surface of the plane.
        public void Set3Points(float3 a, float3 b, float3 c)
        {
            m_Normal = math.normalize(math.cross(b - a, c - a));
            m_Distance = -math.dot(m_Normal, a);
        }

        // Make the plane face the opposite direction
        public void Flip() { m_Normal = -m_Normal; m_Distance = -m_Distance; }

        // Return a version of the plane that faces the opposite direction
        public Plane flipped { get { return new Plane(-m_Normal, -m_Distance); } }

        // Translates the plane into a given direction
        public void Translate(float3 translation) { m_Distance += math.dot(m_Normal, translation); }

        // Creates a plane that's translated into a given direction
        public static Plane Translate(Plane plane, float3 translation) { return new Plane(plane.m_Normal, plane.m_Distance += math.dot(plane.m_Normal, translation)); }

        // Calculates the closest point on the plane.
        public float3 ClosestPointOnPlane(float3 point)
        {
            var pointToPlaneDistance = math.dot(m_Normal, point) + m_Distance;
            return point - (m_Normal * pointToPlaneDistance);
        }

        // Returns a signed distance from plane to point.
        public float GetDistanceToPoint(float3 point) { return math.dot(m_Normal, point) + m_Distance; }

        // Is a point on the positive side of the plane?
        public bool GetSide(float3 point) { return math.dot(m_Normal, point) + m_Distance > 0.0F; }

        // Are two points on the same side of the plane?
        public bool SameSide(float3 inPt0, float3 inPt1)
        {
            float d0 = GetDistanceToPoint(inPt0);
            float d1 = GetDistanceToPoint(inPt1);
            return (d0 > 0.0f && d1 > 0.0f) ||
                (d0 <= 0.0f && d1 <= 0.0f);
        }

        // Intersects a ray with the plane.
        public bool Raycast(float3 origin, float3 direction, out float enter)
        {
            float vdot = math.dot(direction, m_Normal);
            float ndot = -math.dot(origin, m_Normal) - m_Distance;


            if (Approximately(vdot, 0.0f))
            {
                enter = 0.0F;
                return false;
            }

            enter = ndot / vdot;

            return enter > 0.0F;
        }

        public override string ToString() => ToString(null, CultureInfo.InvariantCulture.NumberFormat);
        public string ToString(string format) => ToString(format, CultureInfo.InvariantCulture.NumberFormat);
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
                format = "F1";
            return string.Format("(normal:{0}, distance:{1})", m_Normal.ToString(format, formatProvider), m_Distance.ToString(format, formatProvider));
        }

        // Compares two floating point values if they are similar.
        private static bool Approximately(float a, float b)
        {
            // If a or b is zero, compare that the other is less or equal to epsilon.
            // If neither a or b are 0, then find an epsilon that is good for
            // comparing numbers at the maximum magnitude of a and b.
            // Floating points have about 7 significant digits, so
            // 1.000001f can be represented while 1.0000001f is rounded to zero,
            // thus we could use an epsilon of 0.000001f for comparing values close to 1.
            // We multiply this epsilon by the biggest magnitude of a and b.
            return math.abs(b - a) < math.max(0.000001f * math.max(math.abs(a), math.abs(b)), math.EPSILON * 8);
        }
    }
}
