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

#if !UNITYENGINE

namespace Point.Collections
{
    public struct Ray : IFormattable
    {
        private float3 m_Origin;

        private float3 m_Direction;

        //
        // Summary:
        //     The origin point of the ray.
        public float3 origin
        {
            get
            {
                return m_Origin;
            }
            set
            {
                m_Origin = value;
            }
        }

        //
        // Summary:
        //     The direction of the ray.
        public float3 direction
        {
            get
            {
                return m_Direction;
            }
            set
            {
                m_Direction = Math.normalize(value);
            }
        }

        //
        // Summary:
        //     Creates a ray starting at origin along direction.
        //
        // Parameters:
        //   origin:
        //
        //   direction:
        public Ray(float3 origin, float3 direction)
        {
            m_Origin = origin;
            m_Direction = Math.normalize(direction);
        }

        //
        // Summary:
        //     Returns a point at distance units along the ray.
        //
        // Parameters:
        //   distance:
        public float3 GetPoint(float distance)
        {
            return m_Origin + m_Direction * distance;
        }

        //
        // Summary:
        //     Returns a formatted string for this ray.
        //
        // Parameters:
        //   format:
        //     A numeric format string.
        //
        //   formatProvider:
        //     An object that specifies culture-specific formatting.
        public override string ToString()
        {
            return ToString(null, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        }

        //
        // Summary:
        //     Returns a formatted string for this ray.
        //
        // Parameters:
        //   format:
        //     A numeric format string.
        //
        //   formatProvider:
        //     An object that specifies culture-specific formatting.
        public string ToString(string format)
        {
            return ToString(format, System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
        }

        //
        // Summary:
        //     Returns a formatted string for this ray.
        //
        // Parameters:
        //   format:
        //     A numeric format string.
        //
        //   formatProvider:
        //     An object that specifies culture-specific formatting.
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrEmpty(format))
            {
                format = "F1";
            }

            return String.Format("Origin: {0}, Dir: {1}", m_Origin.ToString(format, formatProvider), m_Direction.ToString(format, formatProvider));
        }
    }
}

#endif