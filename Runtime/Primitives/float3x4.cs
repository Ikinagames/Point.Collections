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

#if UNITY_2019 && !UNITY_2020_OR_NEWER

using System;
using System.Runtime.CompilerServices;

namespace Point.Collections
{
    [Serializable]
    public struct float3x4 : IEquatable<float3x4>, IFormattable
    {
        public float3 c0;

        public float3 c1;

        public float3 c2;

        public float3 c3;

        public static readonly float3x4 zero;

        public unsafe ref float3 this[int index]
        {
            get
            {
                if ((uint)index >= 4u)
                {
                    throw new ArgumentException("index must be between[0...3]");
                }

                fixed (float3x4* ptr = &this)
                {
                    return ref *(float3*)((byte*)ptr + index * sizeof(float3));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x4(float3 c0, float3 c1, float3 c2, float3 c3)
        {
            this.c0 = c0;
            this.c1 = c1;
            this.c2 = c2;
            this.c3 = c3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23)
        {
            c0 = new float3(m00, m10, m20);
            c1 = new float3(m01, m11, m21);
            c2 = new float3(m02, m12, m22);
            c3 = new float3(m03, m13, m23);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x4(float v)
        {
            c0 = v;
            c1 = v;
            c2 = v;
            c3 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x4(bool v)
        {
            c0 = Math.select(new float3(0f), new float3(1f), v);
            c1 = Math.select(new float3(0f), new float3(1f), v);
            c2 = Math.select(new float3(0f), new float3(1f), v);
            c3 = Math.select(new float3(0f), new float3(1f), v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x4(bool3x4 v)
        {
            c0 = Math.select(new float3(0f), new float3(1f), v.c0);
            c1 = Math.select(new float3(0f), new float3(1f), v.c1);
            c2 = Math.select(new float3(0f), new float3(1f), v.c2);
            c3 = Math.select(new float3(0f), new float3(1f), v.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x4(int v)
        {
            c0 = v;
            c1 = v;
            c2 = v;
            c3 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x4(int3x4 v)
        {
            c0 = v.c0;
            c1 = v.c1;
            c2 = v.c2;
            c3 = v.c3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x4(uint v)
        {
            c0 = v;
            c1 = v;
            c2 = v;
            c3 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x4(uint3x4 v)
        {
            c0 = v.c0;
            c1 = v.c1;
            c2 = v.c2;
            c3 = v.c3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x4(double v)
        {
            c0 = (float3)v;
            c1 = (float3)v;
            c2 = (float3)v;
            c3 = (float3)v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x4(double3x4 v)
        {
            c0 = (float3)v.c0;
            c1 = (float3)v.c1;
            c2 = (float3)v.c2;
            c3 = (float3)v.c3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float3x4(float v)
        {
            return new float3x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float3x4(bool v)
        {
            return new float3x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float3x4(bool3x4 v)
        {
            return new float3x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float3x4(int v)
        {
            return new float3x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float3x4(int3x4 v)
        {
            return new float3x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float3x4(uint v)
        {
            return new float3x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float3x4(uint3x4 v)
        {
            return new float3x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float3x4(double v)
        {
            return new float3x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float3x4(double3x4 v)
        {
            return new float3x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator *(float3x4 lhs, float3x4 rhs)
        {
            return new float3x4(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2, lhs.c3 * rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator *(float3x4 lhs, float rhs)
        {
            return new float3x4(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs, lhs.c3 * rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator *(float lhs, float3x4 rhs)
        {
            return new float3x4(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2, lhs * rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator +(float3x4 lhs, float3x4 rhs)
        {
            return new float3x4(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2, lhs.c3 + rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator +(float3x4 lhs, float rhs)
        {
            return new float3x4(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs, lhs.c3 + rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator +(float lhs, float3x4 rhs)
        {
            return new float3x4(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2, lhs + rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator -(float3x4 lhs, float3x4 rhs)
        {
            return new float3x4(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2, lhs.c3 - rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator -(float3x4 lhs, float rhs)
        {
            return new float3x4(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs, lhs.c3 - rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator -(float lhs, float3x4 rhs)
        {
            return new float3x4(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2, lhs - rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator /(float3x4 lhs, float3x4 rhs)
        {
            return new float3x4(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2, lhs.c3 / rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator /(float3x4 lhs, float rhs)
        {
            return new float3x4(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs, lhs.c3 / rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator /(float lhs, float3x4 rhs)
        {
            return new float3x4(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2, lhs / rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator %(float3x4 lhs, float3x4 rhs)
        {
            return new float3x4(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2, lhs.c3 % rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator %(float3x4 lhs, float rhs)
        {
            return new float3x4(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs, lhs.c3 % rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator %(float lhs, float3x4 rhs)
        {
            return new float3x4(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2, lhs % rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator ++(float3x4 val)
        {
            return new float3x4(++val.c0, ++val.c1, ++val.c2, ++val.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator --(float3x4 val)
        {
            return new float3x4(--val.c0, --val.c1, --val.c2, --val.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator <(float3x4 lhs, float3x4 rhs)
        {
            return new bool3x4(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2, lhs.c3 < rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator <(float3x4 lhs, float rhs)
        {
            return new bool3x4(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs, lhs.c3 < rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator <(float lhs, float3x4 rhs)
        {
            return new bool3x4(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2, lhs < rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator <=(float3x4 lhs, float3x4 rhs)
        {
            return new bool3x4(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2, lhs.c3 <= rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator <=(float3x4 lhs, float rhs)
        {
            return new bool3x4(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs, lhs.c3 <= rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator <=(float lhs, float3x4 rhs)
        {
            return new bool3x4(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2, lhs <= rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator >(float3x4 lhs, float3x4 rhs)
        {
            return new bool3x4(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2, lhs.c3 > rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator >(float3x4 lhs, float rhs)
        {
            return new bool3x4(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs, lhs.c3 > rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator >(float lhs, float3x4 rhs)
        {
            return new bool3x4(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2, lhs > rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator >=(float3x4 lhs, float3x4 rhs)
        {
            return new bool3x4(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2, lhs.c3 >= rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator >=(float3x4 lhs, float rhs)
        {
            return new bool3x4(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs, lhs.c3 >= rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator >=(float lhs, float3x4 rhs)
        {
            return new bool3x4(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2, lhs >= rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator -(float3x4 val)
        {
            return new float3x4(-val.c0, -val.c1, -val.c2, -val.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 operator +(float3x4 val)
        {
            return new float3x4(+val.c0, +val.c1, +val.c2, +val.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator ==(float3x4 lhs, float3x4 rhs)
        {
            return new bool3x4(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2, lhs.c3 == rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator ==(float3x4 lhs, float rhs)
        {
            return new bool3x4(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs, lhs.c3 == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator ==(float lhs, float3x4 rhs)
        {
            return new bool3x4(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2, lhs == rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator !=(float3x4 lhs, float3x4 rhs)
        {
            return new bool3x4(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2, lhs.c3 != rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator !=(float3x4 lhs, float rhs)
        {
            return new bool3x4(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs, lhs.c3 != rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator !=(float lhs, float3x4 rhs)
        {
            return new bool3x4(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2, lhs != rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(float3x4 rhs)
        {
            if (c0.Equals(rhs.c0) && c1.Equals(rhs.c1) && c2.Equals(rhs.c2))
            {
                return c3.Equals(rhs.c3);
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (o is float3x4)
            {
                float3x4 rhs = (float3x4)o;
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
            return $"float3x4({c0.x}f, {c1.x}f, {c2.x}f, {c3.x}f,  {c0.y}f, {c1.y}f, {c2.y}f, {c3.y}f,  {c0.z}f, {c1.z}f, {c2.z}f, {c3.z}f)";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"float3x4({c0.x.ToString(format, formatProvider)}f, {c1.x.ToString(format, formatProvider)}f, {c2.x.ToString(format, formatProvider)}f, {c3.x.ToString(format, formatProvider)}f,  {c0.y.ToString(format, formatProvider)}f, {c1.y.ToString(format, formatProvider)}f, {c2.y.ToString(format, formatProvider)}f, {c3.y.ToString(format, formatProvider)}f,  {c0.z.ToString(format, formatProvider)}f, {c1.z.ToString(format, formatProvider)}f, {c2.z.ToString(format, formatProvider)}f, {c3.z.ToString(format, formatProvider)}f)";
        }
    }
}

#endif