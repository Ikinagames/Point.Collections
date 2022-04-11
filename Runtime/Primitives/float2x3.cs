﻿// Copyright 2022 Ikina Games
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

#if UNITY_2019 || !UNITY_2020_OR_NEWER

using System;
using System.Runtime.CompilerServices;

namespace Point.Collections
{
    [Serializable]
    public struct float2x3 : IEquatable<float2x3>, IFormattable
    {
        public float2 c0;

        public float2 c1;

        public float2 c2;

        public static readonly float2x3 zero;

        public unsafe ref float2 this[int index]
        {
            get
            {
                if ((uint)index >= 3u)
                {
                    throw new ArgumentException("index must be between[0...2]");
                }

                fixed (float2x3* ptr = &this)
                {
                    return ref *(float2*)((byte*)ptr + index * sizeof(float2));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x3(float2 c0, float2 c1, float2 c2)
        {
            this.c0 = c0;
            this.c1 = c1;
            this.c2 = c2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x3(float m00, float m01, float m02, float m10, float m11, float m12)
        {
            c0 = new float2(m00, m10);
            c1 = new float2(m01, m11);
            c2 = new float2(m02, m12);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x3(float v)
        {
            c0 = v;
            c1 = v;
            c2 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x3(bool v)
        {
            c0 = Math.select(new float2(0f), new float2(1f), v);
            c1 = Math.select(new float2(0f), new float2(1f), v);
            c2 = Math.select(new float2(0f), new float2(1f), v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x3(bool2x3 v)
        {
            c0 = Math.select(new float2(0f), new float2(1f), v.c0);
            c1 = Math.select(new float2(0f), new float2(1f), v.c1);
            c2 = Math.select(new float2(0f), new float2(1f), v.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x3(int v)
        {
            c0 = v;
            c1 = v;
            c2 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x3(int2x3 v)
        {
            c0 = v.c0;
            c1 = v.c1;
            c2 = v.c2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x3(uint v)
        {
            c0 = v;
            c1 = v;
            c2 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x3(uint2x3 v)
        {
            c0 = v.c0;
            c1 = v.c1;
            c2 = v.c2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x3(double v)
        {
            c0 = (float2)v;
            c1 = (float2)v;
            c2 = (float2)v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float2x3(double2x3 v)
        {
            c0 = (float2)v.c0;
            c1 = (float2)v.c1;
            c2 = (float2)v.c2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float2x3(float v)
        {
            return new float2x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float2x3(bool v)
        {
            return new float2x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float2x3(bool2x3 v)
        {
            return new float2x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float2x3(int v)
        {
            return new float2x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float2x3(int2x3 v)
        {
            return new float2x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float2x3(uint v)
        {
            return new float2x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float2x3(uint2x3 v)
        {
            return new float2x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float2x3(double v)
        {
            return new float2x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float2x3(double2x3 v)
        {
            return new float2x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator *(float2x3 lhs, float2x3 rhs)
        {
            return new float2x3(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator *(float2x3 lhs, float rhs)
        {
            return new float2x3(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator *(float lhs, float2x3 rhs)
        {
            return new float2x3(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator +(float2x3 lhs, float2x3 rhs)
        {
            return new float2x3(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator +(float2x3 lhs, float rhs)
        {
            return new float2x3(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator +(float lhs, float2x3 rhs)
        {
            return new float2x3(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator -(float2x3 lhs, float2x3 rhs)
        {
            return new float2x3(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator -(float2x3 lhs, float rhs)
        {
            return new float2x3(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator -(float lhs, float2x3 rhs)
        {
            return new float2x3(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator /(float2x3 lhs, float2x3 rhs)
        {
            return new float2x3(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator /(float2x3 lhs, float rhs)
        {
            return new float2x3(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator /(float lhs, float2x3 rhs)
        {
            return new float2x3(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator %(float2x3 lhs, float2x3 rhs)
        {
            return new float2x3(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator %(float2x3 lhs, float rhs)
        {
            return new float2x3(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator %(float lhs, float2x3 rhs)
        {
            return new float2x3(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator ++(float2x3 val)
        {
            return new float2x3(++val.c0, ++val.c1, ++val.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator --(float2x3 val)
        {
            return new float2x3(--val.c0, --val.c1, --val.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator <(float2x3 lhs, float2x3 rhs)
        {
            return new bool2x3(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator <(float2x3 lhs, float rhs)
        {
            return new bool2x3(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator <(float lhs, float2x3 rhs)
        {
            return new bool2x3(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator <=(float2x3 lhs, float2x3 rhs)
        {
            return new bool2x3(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator <=(float2x3 lhs, float rhs)
        {
            return new bool2x3(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator <=(float lhs, float2x3 rhs)
        {
            return new bool2x3(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator >(float2x3 lhs, float2x3 rhs)
        {
            return new bool2x3(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator >(float2x3 lhs, float rhs)
        {
            return new bool2x3(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator >(float lhs, float2x3 rhs)
        {
            return new bool2x3(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator >=(float2x3 lhs, float2x3 rhs)
        {
            return new bool2x3(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator >=(float2x3 lhs, float rhs)
        {
            return new bool2x3(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator >=(float lhs, float2x3 rhs)
        {
            return new bool2x3(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator -(float2x3 val)
        {
            return new float2x3(-val.c0, -val.c1, -val.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 operator +(float2x3 val)
        {
            return new float2x3(+val.c0, +val.c1, +val.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator ==(float2x3 lhs, float2x3 rhs)
        {
            return new bool2x3(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator ==(float2x3 lhs, float rhs)
        {
            return new bool2x3(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator ==(float lhs, float2x3 rhs)
        {
            return new bool2x3(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator !=(float2x3 lhs, float2x3 rhs)
        {
            return new bool2x3(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator !=(float2x3 lhs, float rhs)
        {
            return new bool2x3(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator !=(float lhs, float2x3 rhs)
        {
            return new bool2x3(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(float2x3 rhs)
        {
            if (c0.Equals(rhs.c0) && c1.Equals(rhs.c1))
            {
                return c2.Equals(rhs.c2);
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (o is float2x3)
            {
                float2x3 rhs = (float2x3)o;
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
            return $"float2x3({c0.x}f, {c1.x}f, {c2.x}f,  {c0.y}f, {c1.y}f, {c2.y}f)";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"float2x3({c0.x.ToString(format, formatProvider)}f, {c1.x.ToString(format, formatProvider)}f, {c2.x.ToString(format, formatProvider)}f,  {c0.y.ToString(format, formatProvider)}f, {c1.y.ToString(format, formatProvider)}f, {c2.y.ToString(format, formatProvider)}f)";
        }
    }
}

#endif