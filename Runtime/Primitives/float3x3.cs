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

#if UNITY_2019 && !UNITY_2020_OR_NEWER

using System;
using System.Runtime.CompilerServices;

namespace Point.Collections
{
    [Serializable]
    public struct float3x3 : IEquatable<float3x3>, IFormattable
    {
        public float3 c0;

        public float3 c1;

        public float3 c2;

        public static readonly float3x3 identity = new float3x3(1f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 1f);

        public static readonly float3x3 zero;

        public unsafe ref float3 this[int index]
        {
            get
            {
                if ((uint)index >= 3u)
                {
                    throw new ArgumentException("index must be between[0...2]");
                }

                fixed (float3x3* ptr = &this)
                {
                    return ref *(float3*)((byte*)ptr + index * sizeof(float3));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x3(float3 c0, float3 c1, float3 c2)
        {
            this.c0 = c0;
            this.c1 = c1;
            this.c2 = c2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x3(float m00, float m01, float m02, float m10, float m11, float m12, float m20, float m21, float m22)
        {
            c0 = new float3(m00, m10, m20);
            c1 = new float3(m01, m11, m21);
            c2 = new float3(m02, m12, m22);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x3(float v)
        {
            c0 = v;
            c1 = v;
            c2 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x3(bool v)
        {
            c0 = Math.select(new float3(0f), new float3(1f), v);
            c1 = Math.select(new float3(0f), new float3(1f), v);
            c2 = Math.select(new float3(0f), new float3(1f), v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x3(bool3x3 v)
        {
            c0 = Math.select(new float3(0f), new float3(1f), v.c0);
            c1 = Math.select(new float3(0f), new float3(1f), v.c1);
            c2 = Math.select(new float3(0f), new float3(1f), v.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x3(int v)
        {
            c0 = v;
            c1 = v;
            c2 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x3(int3x3 v)
        {
            c0 = v.c0;
            c1 = v.c1;
            c2 = v.c2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x3(uint v)
        {
            c0 = v;
            c1 = v;
            c2 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x3(uint3x3 v)
        {
            c0 = v.c0;
            c1 = v.c1;
            c2 = v.c2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x3(double v)
        {
            c0 = (float3)v;
            c1 = (float3)v;
            c2 = (float3)v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float3x3(double3x3 v)
        {
            c0 = (float3)v.c0;
            c1 = (float3)v.c1;
            c2 = (float3)v.c2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float3x3(float v)
        {
            return new float3x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float3x3(bool v)
        {
            return new float3x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float3x3(bool3x3 v)
        {
            return new float3x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float3x3(int v)
        {
            return new float3x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float3x3(int3x3 v)
        {
            return new float3x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float3x3(uint v)
        {
            return new float3x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float3x3(uint3x3 v)
        {
            return new float3x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float3x3(double v)
        {
            return new float3x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float3x3(double3x3 v)
        {
            return new float3x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator *(float3x3 lhs, float3x3 rhs)
        {
            return new float3x3(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator *(float3x3 lhs, float rhs)
        {
            return new float3x3(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator *(float lhs, float3x3 rhs)
        {
            return new float3x3(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator +(float3x3 lhs, float3x3 rhs)
        {
            return new float3x3(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator +(float3x3 lhs, float rhs)
        {
            return new float3x3(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator +(float lhs, float3x3 rhs)
        {
            return new float3x3(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator -(float3x3 lhs, float3x3 rhs)
        {
            return new float3x3(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator -(float3x3 lhs, float rhs)
        {
            return new float3x3(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator -(float lhs, float3x3 rhs)
        {
            return new float3x3(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator /(float3x3 lhs, float3x3 rhs)
        {
            return new float3x3(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator /(float3x3 lhs, float rhs)
        {
            return new float3x3(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator /(float lhs, float3x3 rhs)
        {
            return new float3x3(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator %(float3x3 lhs, float3x3 rhs)
        {
            return new float3x3(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator %(float3x3 lhs, float rhs)
        {
            return new float3x3(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator %(float lhs, float3x3 rhs)
        {
            return new float3x3(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator ++(float3x3 val)
        {
            return new float3x3(++val.c0, ++val.c1, ++val.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator --(float3x3 val)
        {
            return new float3x3(--val.c0, --val.c1, --val.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator <(float3x3 lhs, float3x3 rhs)
        {
            return new bool3x3(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator <(float3x3 lhs, float rhs)
        {
            return new bool3x3(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator <(float lhs, float3x3 rhs)
        {
            return new bool3x3(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator <=(float3x3 lhs, float3x3 rhs)
        {
            return new bool3x3(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator <=(float3x3 lhs, float rhs)
        {
            return new bool3x3(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator <=(float lhs, float3x3 rhs)
        {
            return new bool3x3(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator >(float3x3 lhs, float3x3 rhs)
        {
            return new bool3x3(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator >(float3x3 lhs, float rhs)
        {
            return new bool3x3(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator >(float lhs, float3x3 rhs)
        {
            return new bool3x3(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator >=(float3x3 lhs, float3x3 rhs)
        {
            return new bool3x3(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator >=(float3x3 lhs, float rhs)
        {
            return new bool3x3(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator >=(float lhs, float3x3 rhs)
        {
            return new bool3x3(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator -(float3x3 val)
        {
            return new float3x3(-val.c0, -val.c1, -val.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 operator +(float3x3 val)
        {
            return new float3x3(+val.c0, +val.c1, +val.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator ==(float3x3 lhs, float3x3 rhs)
        {
            return new bool3x3(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator ==(float3x3 lhs, float rhs)
        {
            return new bool3x3(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator ==(float lhs, float3x3 rhs)
        {
            return new bool3x3(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator !=(float3x3 lhs, float3x3 rhs)
        {
            return new bool3x3(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator !=(float3x3 lhs, float rhs)
        {
            return new bool3x3(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x3 operator !=(float lhs, float3x3 rhs)
        {
            return new bool3x3(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(float3x3 rhs)
        {
            if (c0.Equals(rhs.c0) && c1.Equals(rhs.c1))
            {
                return c2.Equals(rhs.c2);
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (o is float3x3)
            {
                float3x3 rhs = (float3x3)o;
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
            return $"float3x3({c0.x}f, {c1.x}f, {c2.x}f,  {c0.y}f, {c1.y}f, {c2.y}f,  {c0.z}f, {c1.z}f, {c2.z}f)";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"float3x3({c0.x.ToString(format, formatProvider)}f, {c1.x.ToString(format, formatProvider)}f, {c2.x.ToString(format, formatProvider)}f,  {c0.y.ToString(format, formatProvider)}f, {c1.y.ToString(format, formatProvider)}f, {c2.y.ToString(format, formatProvider)}f,  {c0.z.ToString(format, formatProvider)}f, {c1.z.ToString(format, formatProvider)}f, {c2.z.ToString(format, formatProvider)}f)";
        }

        public float3x3(float4x4 f4x4)
        {
            c0 = f4x4.c0.xyz;
            c1 = f4x4.c1.xyz;
            c2 = f4x4.c2.xyz;
        }

        public float3x3(quaternion q)
        {
            float4 value = q.value;
            float4 @float = value + value;
            uint3 rhs = new uint3(2147483648u, 0u, 2147483648u);
            uint3 rhs2 = new uint3(2147483648u, 2147483648u, 0u);
            uint3 rhs3 = new uint3(0u, 2147483648u, 2147483648u);
            c0 = @float.y * Math.asfloat(Math.asuint(value.yxw) ^ rhs) - @float.z * Math.asfloat(Math.asuint(value.zwx) ^ rhs3) + new float3(1f, 0f, 0f);
            c1 = @float.z * Math.asfloat(Math.asuint(value.wzy) ^ rhs2) - @float.x * Math.asfloat(Math.asuint(value.yxw) ^ rhs) + new float3(0f, 1f, 0f);
            c2 = @float.x * Math.asfloat(Math.asuint(value.zwx) ^ rhs3) - @float.y * Math.asfloat(Math.asuint(value.wzy) ^ rhs2) + new float3(0f, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 AxisAngle(float3 axis, float angle)
        {
            Math.sincos(angle, out float s, out float c);
            float3 lhs = axis;
            _ = lhs.yzx;
            _ = lhs.zxy;
            float3 rhs = lhs - lhs * c;
            float4 @float = new float4(lhs * s, c);
            uint3 rhs2 = new uint3(0u, 0u, 2147483648u);
            uint3 rhs3 = new uint3(2147483648u, 0u, 0u);
            uint3 rhs4 = new uint3(0u, 2147483648u, 0u);
            return new float3x3(lhs.x * rhs + Math.asfloat(Math.asuint(@float.wzy) ^ rhs2), lhs.y * rhs + Math.asfloat(Math.asuint(@float.zwx) ^ rhs3), lhs.z * rhs + Math.asfloat(Math.asuint(@float.yxw) ^ rhs4));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 EulerXYZ(float3 xyz)
        {
            Math.sincos(xyz, out float3 s, out float3 c);
            return new float3x3(c.y * c.z, c.z * s.x * s.y - c.x * s.z, c.x * c.z * s.y + s.x * s.z, c.y * s.z, c.x * c.z + s.x * s.y * s.z, c.x * s.y * s.z - c.z * s.x, 0f - s.y, c.y * s.x, c.x * c.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 EulerXZY(float3 xyz)
        {
            Math.sincos(xyz, out float3 s, out float3 c);
            return new float3x3(c.y * c.z, s.x * s.y - c.x * c.y * s.z, c.x * s.y + c.y * s.x * s.z, s.z, c.x * c.z, (0f - c.z) * s.x, (0f - c.z) * s.y, c.y * s.x + c.x * s.y * s.z, c.x * c.y - s.x * s.y * s.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 EulerYXZ(float3 xyz)
        {
            Math.sincos(xyz, out float3 s, out float3 c);
            return new float3x3(c.y * c.z - s.x * s.y * s.z, (0f - c.x) * s.z, c.z * s.y + c.y * s.x * s.z, c.z * s.x * s.y + c.y * s.z, c.x * c.z, s.y * s.z - c.y * c.z * s.x, (0f - c.x) * s.y, s.x, c.x * c.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 EulerYZX(float3 xyz)
        {
            Math.sincos(xyz, out float3 s, out float3 c);
            return new float3x3(c.y * c.z, 0f - s.z, c.z * s.y, s.x * s.y + c.x * c.y * s.z, c.x * c.z, c.x * s.y * s.z - c.y * s.x, c.y * s.x * s.z - c.x * s.y, c.z * s.x, c.x * c.y + s.x * s.y * s.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 EulerZXY(float3 xyz)
        {
            Math.sincos(xyz, out float3 s, out float3 c);
            return new float3x3(c.y * c.z + s.x * s.y * s.z, c.z * s.x * s.y - c.y * s.z, c.x * s.y, c.x * s.z, c.x * c.z, 0f - s.x, c.y * s.x * s.z - c.z * s.y, c.y * c.z * s.x + s.y * s.z, c.x * c.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 EulerZYX(float3 xyz)
        {
            Math.sincos(xyz, out float3 s, out float3 c);
            return new float3x3(c.y * c.z, (0f - c.y) * s.z, s.y, c.z * s.x * s.y + c.x * s.z, c.x * c.z - s.x * s.y * s.z, (0f - c.y) * s.x, s.x * s.z - c.x * c.z * s.y, c.z * s.x + c.x * s.y * s.z, c.x * c.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 EulerXYZ(float x, float y, float z)
        {
            return EulerXYZ(new float3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 EulerXZY(float x, float y, float z)
        {
            return EulerXZY(new float3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 EulerYXZ(float x, float y, float z)
        {
            return EulerYXZ(new float3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 EulerYZX(float x, float y, float z)
        {
            return EulerYZX(new float3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 EulerZXY(float x, float y, float z)
        {
            return EulerZXY(new float3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 EulerZYX(float x, float y, float z)
        {
            return EulerZYX(new float3(x, y, z));
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static float3x3 Euler(float3 xyz, math.RotationOrder order = math.RotationOrder.ZXY)
        //{
        //    switch (order)
        //    {
        //        case math.RotationOrder.XYZ:
        //            return EulerXYZ(xyz);
        //        case math.RotationOrder.XZY:
        //            return EulerXZY(xyz);
        //        case math.RotationOrder.YXZ:
        //            return EulerYXZ(xyz);
        //        case math.RotationOrder.YZX:
        //            return EulerYZX(xyz);
        //        case math.RotationOrder.ZXY:
        //            return EulerZXY(xyz);
        //        case math.RotationOrder.ZYX:
        //            return EulerZYX(xyz);
        //        default:
        //            return identity;
        //    }
        //}

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static float3x3 Euler(float x, float y, float z, math.RotationOrder order = math.RotationOrder.ZXY)
        //{
        //    return Euler(new float3(x, y, z), order);
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 RotateX(float angle)
        {
            Math.sincos(angle, out float s, out float c);
            return new float3x3(1f, 0f, 0f, 0f, c, 0f - s, 0f, s, c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 RotateY(float angle)
        {
            Math.sincos(angle, out float s, out float c);
            return new float3x3(c, 0f, s, 0f, 1f, 0f, 0f - s, 0f, c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 RotateZ(float angle)
        {
            Math.sincos(angle, out float s, out float c);
            return new float3x3(c, 0f - s, 0f, s, c, 0f, 0f, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 Scale(float s)
        {
            return new float3x3(s, 0f, 0f, 0f, s, 0f, 0f, 0f, s);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 Scale(float x, float y, float z)
        {
            return new float3x3(x, 0f, 0f, 0f, y, 0f, 0f, 0f, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 Scale(float3 v)
        {
            return Scale(v.x, v.y, v.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 LookRotation(float3 forward, float3 up)
        {
            float3 y = Math.normalize(Math.cross(up, forward));
            return new float3x3(y, Math.cross(forward, y), forward);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 LookRotationSafe(float3 forward, float3 up)
        {
            float x = Math.dot(forward, forward);
            float num = Math.dot(up, up);
            forward *= Math.rsqrt(x);
            up *= Math.rsqrt(num);
            float3 @float = Math.cross(up, forward);
            float num2 = Math.dot(@float, @float);
            @float *= Math.rsqrt(num2);
            float num3 = Math.min(Math.min(x, num), num2);
            float num4 = Math.max(Math.max(x, num), num2);
            bool c = num3 > 1E-35f && num4 < 1E+35f && Math.isfinite(x) && Math.isfinite(num) && Math.isfinite(num2);
            return new float3x3(Math.select(new float3(1f, 0f, 0f), @float, c), Math.select(new float3(0f, 1f, 0f), Math.cross(forward, @float), c), Math.select(new float3(0f, 0f, 1f), forward, c));
        }

        public static explicit operator float3x3(float4x4 f4x4)
        {
            return new float3x3(f4x4);
        }
    }
}

#endif