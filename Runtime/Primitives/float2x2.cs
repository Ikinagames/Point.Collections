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
using System.Runtime.CompilerServices;

namespace Point.Collections
{
    [Serializable]
    public struct float2x2 : IEquatable<float2x2>, IFormattable
    {
        public float2 c0;

        public float2 c1;

        public static readonly float2x2 identity = new float2x2(1f, 0f, 0f, 1f);

        public static readonly float2x2 zero;

        public unsafe ref float2 this[int index]
        {
            get
            {
                if ((uint)index >= 2u)
                {
                    throw new ArgumentException("index must be between[0...1]");
                }

                fixed (float2x2* ptr = &this)
                {
                    return ref *(float2*)((byte*)ptr + (nint)index * (nint)sizeof(float2));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x2(float2 c0, float2 c1)
        {
            this.c0 = c0;
            this.c1 = c1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x2(float m00, float m01, float m10, float m11)
        {
            c0 = new float2(m00, m10);
            c1 = new float2(m01, m11);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x2(float v)
        {
            c0 = v;
            c1 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x2(bool v)
        {
            c0 = Math.select(new float2(0f), new float2(1f), v);
            c1 = Math.select(new float2(0f), new float2(1f), v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x2(bool2x2 v)
        {
            c0 = Math.select(new float2(0f), new float2(1f), v.c0);
            c1 = Math.select(new float2(0f), new float2(1f), v.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x2(int v)
        {
            c0 = v;
            c1 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x2(int2x2 v)
        {
            c0 = v.c0;
            c1 = v.c1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x2(uint v)
        {
            c0 = v;
            c1 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x2(uint2x2 v)
        {
            c0 = v.c0;
            c1 = v.c1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x2(double v)
        {
            c0 = (float2)v;
            c1 = (float2)v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x2(double2x2 v)
        {
            c0 = (float2)v.c0;
            c1 = (float2)v.c1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float2x2(float v)
        {
            return new float2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float2x2(bool v)
        {
            return new float2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float2x2(bool2x2 v)
        {
            return new float2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float2x2(int v)
        {
            return new float2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float2x2(int2x2 v)
        {
            return new float2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float2x2(uint v)
        {
            return new float2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float2x2(uint2x2 v)
        {
            return new float2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float2x2(double v)
        {
            return new float2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float2x2(double2x2 v)
        {
            return new float2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator *(float2x2 lhs, float2x2 rhs)
        {
            return new float2x2(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator *(float2x2 lhs, float rhs)
        {
            return new float2x2(lhs.c0 * rhs, lhs.c1 * rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator *(float lhs, float2x2 rhs)
        {
            return new float2x2(lhs * rhs.c0, lhs * rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator +(float2x2 lhs, float2x2 rhs)
        {
            return new float2x2(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator +(float2x2 lhs, float rhs)
        {
            return new float2x2(lhs.c0 + rhs, lhs.c1 + rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator +(float lhs, float2x2 rhs)
        {
            return new float2x2(lhs + rhs.c0, lhs + rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator -(float2x2 lhs, float2x2 rhs)
        {
            return new float2x2(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator -(float2x2 lhs, float rhs)
        {
            return new float2x2(lhs.c0 - rhs, lhs.c1 - rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator -(float lhs, float2x2 rhs)
        {
            return new float2x2(lhs - rhs.c0, lhs - rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator /(float2x2 lhs, float2x2 rhs)
        {
            return new float2x2(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator /(float2x2 lhs, float rhs)
        {
            return new float2x2(lhs.c0 / rhs, lhs.c1 / rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator /(float lhs, float2x2 rhs)
        {
            return new float2x2(lhs / rhs.c0, lhs / rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator %(float2x2 lhs, float2x2 rhs)
        {
            return new float2x2(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator %(float2x2 lhs, float rhs)
        {
            return new float2x2(lhs.c0 % rhs, lhs.c1 % rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator %(float lhs, float2x2 rhs)
        {
            return new float2x2(lhs % rhs.c0, lhs % rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator ++(float2x2 val)
        {
            return new float2x2(++val.c0, ++val.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator --(float2x2 val)
        {
            return new float2x2(--val.c0, --val.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator <(float2x2 lhs, float2x2 rhs)
        {
            return new bool2x2(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator <(float2x2 lhs, float rhs)
        {
            return new bool2x2(lhs.c0 < rhs, lhs.c1 < rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator <(float lhs, float2x2 rhs)
        {
            return new bool2x2(lhs < rhs.c0, lhs < rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator <=(float2x2 lhs, float2x2 rhs)
        {
            return new bool2x2(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator <=(float2x2 lhs, float rhs)
        {
            return new bool2x2(lhs.c0 <= rhs, lhs.c1 <= rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator <=(float lhs, float2x2 rhs)
        {
            return new bool2x2(lhs <= rhs.c0, lhs <= rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator >(float2x2 lhs, float2x2 rhs)
        {
            return new bool2x2(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator >(float2x2 lhs, float rhs)
        {
            return new bool2x2(lhs.c0 > rhs, lhs.c1 > rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator >(float lhs, float2x2 rhs)
        {
            return new bool2x2(lhs > rhs.c0, lhs > rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator >=(float2x2 lhs, float2x2 rhs)
        {
            return new bool2x2(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator >=(float2x2 lhs, float rhs)
        {
            return new bool2x2(lhs.c0 >= rhs, lhs.c1 >= rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator >=(float lhs, float2x2 rhs)
        {
            return new bool2x2(lhs >= rhs.c0, lhs >= rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator -(float2x2 val)
        {
            return new float2x2(-val.c0, -val.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 operator +(float2x2 val)
        {
            return new float2x2(+val.c0, +val.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator ==(float2x2 lhs, float2x2 rhs)
        {
            return new bool2x2(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator ==(float2x2 lhs, float rhs)
        {
            return new bool2x2(lhs.c0 == rhs, lhs.c1 == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator ==(float lhs, float2x2 rhs)
        {
            return new bool2x2(lhs == rhs.c0, lhs == rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator !=(float2x2 lhs, float2x2 rhs)
        {
            return new bool2x2(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator !=(float2x2 lhs, float rhs)
        {
            return new bool2x2(lhs.c0 != rhs, lhs.c1 != rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator !=(float lhs, float2x2 rhs)
        {
            return new bool2x2(lhs != rhs.c0, lhs != rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(float2x2 rhs)
        {
            if (c0.Equals(rhs.c0))
            {
                return c1.Equals(rhs.c1);
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (o is float2x2)
            {
                float2x2 rhs = (float2x2)o;
                return Equals(rhs);
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return (int)Math.hash(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return $"float2x2({c0.x}f, {c1.x}f,  {c0.y}f, {c1.y}f)";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"float2x2({c0.x.ToString(format, formatProvider)}f, {c1.x.ToString(format, formatProvider)}f,  {c0.y.ToString(format, formatProvider)}f, {c1.y.ToString(format, formatProvider)}f)";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 Rotate(float angle)
        {
            Math.sincos(angle, out float s, out float c);
            return new float2x2(c, 0f - s, s, c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 Scale(float s)
        {
            return new float2x2(s, 0f, 0f, s);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 Scale(float x, float y)
        {
            return new float2x2(x, 0f, 0f, y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 Scale(float2 v)
        {
            return Scale(v.x, v.y);
        }
    }
}

#endif