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
    public struct float4x4 : IEquatable<float4x4>, IFormattable
    {
        public float4 c0;

        public float4 c1;

        public float4 c2;

        public float4 c3;

        public static readonly float4x4 identity = new float4x4(1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);

        public static readonly float4x4 zero;

        public unsafe ref float4 this[int index]
        {
            get
            {
                if ((uint)index >= 4u)
                {
                    throw new ArgumentException("index must be between[0...3]");
                }

                fixed (float4x4* ptr = &this)
                {
                    return ref *(float4*)((byte*)ptr + (nint)index * (nint)sizeof(float4));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float4x4(float4 c0, float4 c1, float4 c2, float4 c3)
        {
            this.c0 = c0;
            this.c1 = c1;
            this.c2 = c2;
            this.c3 = c3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float4x4(float m00, float m01, float m02, float m03, float m10, float m11, float m12, float m13, float m20, float m21, float m22, float m23, float m30, float m31, float m32, float m33)
        {
            c0 = new float4(m00, m10, m20, m30);
            c1 = new float4(m01, m11, m21, m31);
            c2 = new float4(m02, m12, m22, m32);
            c3 = new float4(m03, m13, m23, m33);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float4x4(float v)
        {
            c0 = v;
            c1 = v;
            c2 = v;
            c3 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float4x4(bool v)
        {
            c0 = Math.select(new float4(0f), new float4(1f), v);
            c1 = Math.select(new float4(0f), new float4(1f), v);
            c2 = Math.select(new float4(0f), new float4(1f), v);
            c3 = Math.select(new float4(0f), new float4(1f), v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float4x4(bool4x4 v)
        {
            c0 = Math.select(new float4(0f), new float4(1f), v.c0);
            c1 = Math.select(new float4(0f), new float4(1f), v.c1);
            c2 = Math.select(new float4(0f), new float4(1f), v.c2);
            c3 = Math.select(new float4(0f), new float4(1f), v.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float4x4(int v)
        {
            c0 = v;
            c1 = v;
            c2 = v;
            c3 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float4x4(int4x4 v)
        {
            c0 = v.c0;
            c1 = v.c1;
            c2 = v.c2;
            c3 = v.c3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float4x4(uint v)
        {
            c0 = v;
            c1 = v;
            c2 = v;
            c3 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float4x4(uint4x4 v)
        {
            c0 = v.c0;
            c1 = v.c1;
            c2 = v.c2;
            c3 = v.c3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float4x4(double v)
        {
            c0 = (float4)v;
            c1 = (float4)v;
            c2 = (float4)v;
            c3 = (float4)v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public float4x4(double4x4 v)
        {
            c0 = (float4)v.c0;
            c1 = (float4)v.c1;
            c2 = (float4)v.c2;
            c3 = (float4)v.c3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float4x4(float v)
        {
            return new float4x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float4x4(bool v)
        {
            return new float4x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float4x4(bool4x4 v)
        {
            return new float4x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float4x4(int v)
        {
            return new float4x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float4x4(int4x4 v)
        {
            return new float4x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float4x4(uint v)
        {
            return new float4x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float4x4(uint4x4 v)
        {
            return new float4x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float4x4(double v)
        {
            return new float4x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator float4x4(double4x4 v)
        {
            return new float4x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator *(float4x4 lhs, float4x4 rhs)
        {
            return new float4x4(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1, lhs.c2 * rhs.c2, lhs.c3 * rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator *(float4x4 lhs, float rhs)
        {
            return new float4x4(lhs.c0 * rhs, lhs.c1 * rhs, lhs.c2 * rhs, lhs.c3 * rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator *(float lhs, float4x4 rhs)
        {
            return new float4x4(lhs * rhs.c0, lhs * rhs.c1, lhs * rhs.c2, lhs * rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator +(float4x4 lhs, float4x4 rhs)
        {
            return new float4x4(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1, lhs.c2 + rhs.c2, lhs.c3 + rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator +(float4x4 lhs, float rhs)
        {
            return new float4x4(lhs.c0 + rhs, lhs.c1 + rhs, lhs.c2 + rhs, lhs.c3 + rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator +(float lhs, float4x4 rhs)
        {
            return new float4x4(lhs + rhs.c0, lhs + rhs.c1, lhs + rhs.c2, lhs + rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator -(float4x4 lhs, float4x4 rhs)
        {
            return new float4x4(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1, lhs.c2 - rhs.c2, lhs.c3 - rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator -(float4x4 lhs, float rhs)
        {
            return new float4x4(lhs.c0 - rhs, lhs.c1 - rhs, lhs.c2 - rhs, lhs.c3 - rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator -(float lhs, float4x4 rhs)
        {
            return new float4x4(lhs - rhs.c0, lhs - rhs.c1, lhs - rhs.c2, lhs - rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator /(float4x4 lhs, float4x4 rhs)
        {
            return new float4x4(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1, lhs.c2 / rhs.c2, lhs.c3 / rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator /(float4x4 lhs, float rhs)
        {
            return new float4x4(lhs.c0 / rhs, lhs.c1 / rhs, lhs.c2 / rhs, lhs.c3 / rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator /(float lhs, float4x4 rhs)
        {
            return new float4x4(lhs / rhs.c0, lhs / rhs.c1, lhs / rhs.c2, lhs / rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator %(float4x4 lhs, float4x4 rhs)
        {
            return new float4x4(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1, lhs.c2 % rhs.c2, lhs.c3 % rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator %(float4x4 lhs, float rhs)
        {
            return new float4x4(lhs.c0 % rhs, lhs.c1 % rhs, lhs.c2 % rhs, lhs.c3 % rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator %(float lhs, float4x4 rhs)
        {
            return new float4x4(lhs % rhs.c0, lhs % rhs.c1, lhs % rhs.c2, lhs % rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator ++(float4x4 val)
        {
            return new float4x4(++val.c0, ++val.c1, ++val.c2, ++val.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator --(float4x4 val)
        {
            return new float4x4(--val.c0, --val.c1, --val.c2, --val.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator <(float4x4 lhs, float4x4 rhs)
        {
            return new bool4x4(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1, lhs.c2 < rhs.c2, lhs.c3 < rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator <(float4x4 lhs, float rhs)
        {
            return new bool4x4(lhs.c0 < rhs, lhs.c1 < rhs, lhs.c2 < rhs, lhs.c3 < rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator <(float lhs, float4x4 rhs)
        {
            return new bool4x4(lhs < rhs.c0, lhs < rhs.c1, lhs < rhs.c2, lhs < rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator <=(float4x4 lhs, float4x4 rhs)
        {
            return new bool4x4(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1, lhs.c2 <= rhs.c2, lhs.c3 <= rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator <=(float4x4 lhs, float rhs)
        {
            return new bool4x4(lhs.c0 <= rhs, lhs.c1 <= rhs, lhs.c2 <= rhs, lhs.c3 <= rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator <=(float lhs, float4x4 rhs)
        {
            return new bool4x4(lhs <= rhs.c0, lhs <= rhs.c1, lhs <= rhs.c2, lhs <= rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator >(float4x4 lhs, float4x4 rhs)
        {
            return new bool4x4(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1, lhs.c2 > rhs.c2, lhs.c3 > rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator >(float4x4 lhs, float rhs)
        {
            return new bool4x4(lhs.c0 > rhs, lhs.c1 > rhs, lhs.c2 > rhs, lhs.c3 > rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator >(float lhs, float4x4 rhs)
        {
            return new bool4x4(lhs > rhs.c0, lhs > rhs.c1, lhs > rhs.c2, lhs > rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator >=(float4x4 lhs, float4x4 rhs)
        {
            return new bool4x4(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1, lhs.c2 >= rhs.c2, lhs.c3 >= rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator >=(float4x4 lhs, float rhs)
        {
            return new bool4x4(lhs.c0 >= rhs, lhs.c1 >= rhs, lhs.c2 >= rhs, lhs.c3 >= rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator >=(float lhs, float4x4 rhs)
        {
            return new bool4x4(lhs >= rhs.c0, lhs >= rhs.c1, lhs >= rhs.c2, lhs >= rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator -(float4x4 val)
        {
            return new float4x4(-val.c0, -val.c1, -val.c2, -val.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 operator +(float4x4 val)
        {
            return new float4x4(+val.c0, +val.c1, +val.c2, +val.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator ==(float4x4 lhs, float4x4 rhs)
        {
            return new bool4x4(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2, lhs.c3 == rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator ==(float4x4 lhs, float rhs)
        {
            return new bool4x4(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs, lhs.c3 == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator ==(float lhs, float4x4 rhs)
        {
            return new bool4x4(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2, lhs == rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator !=(float4x4 lhs, float4x4 rhs)
        {
            return new bool4x4(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2, lhs.c3 != rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator !=(float4x4 lhs, float rhs)
        {
            return new bool4x4(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs, lhs.c3 != rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x4 operator !=(float lhs, float4x4 rhs)
        {
            return new bool4x4(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2, lhs != rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(float4x4 rhs)
        {
            if (c0.Equals(rhs.c0) && c1.Equals(rhs.c1) && c2.Equals(rhs.c2))
            {
                return c3.Equals(rhs.c3);
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (o is float4x4)
            {
                float4x4 rhs = (float4x4)o;
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
            return $"float4x4({c0.x}f, {c1.x}f, {c2.x}f, {c3.x}f,  {c0.y}f, {c1.y}f, {c2.y}f, {c3.y}f,  {c0.z}f, {c1.z}f, {c2.z}f, {c3.z}f,  {c0.w}f, {c1.w}f, {c2.w}f, {c3.w}f)";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"float4x4({c0.x.ToString(format, formatProvider)}f, {c1.x.ToString(format, formatProvider)}f, {c2.x.ToString(format, formatProvider)}f, {c3.x.ToString(format, formatProvider)}f,  {c0.y.ToString(format, formatProvider)}f, {c1.y.ToString(format, formatProvider)}f, {c2.y.ToString(format, formatProvider)}f, {c3.y.ToString(format, formatProvider)}f,  {c0.z.ToString(format, formatProvider)}f, {c1.z.ToString(format, formatProvider)}f, {c2.z.ToString(format, formatProvider)}f, {c3.z.ToString(format, formatProvider)}f,  {c0.w.ToString(format, formatProvider)}f, {c1.w.ToString(format, formatProvider)}f, {c2.w.ToString(format, formatProvider)}f, {c3.w.ToString(format, formatProvider)}f)";
        }

        public float4x4(float3x3 rotation, float3 translation)
        {
            c0 = new float4(rotation.c0, 0f);
            c1 = new float4(rotation.c1, 0f);
            c2 = new float4(rotation.c2, 0f);
            c3 = new float4(translation, 1f);
        }

        public float4x4(quaternion rotation, float3 translation)
        {
            float3x3 float3x = new float3x3(rotation);
            c0 = new float4(float3x.c0, 0f);
            c1 = new float4(float3x.c1, 0f);
            c2 = new float4(float3x.c2, 0f);
            c3 = new float4(translation, 1f);
        }

        //public float4x4(RigidTransform transform)
        //{
        //    float3x3 float3x = new float3x3(transform.rot);
        //    c0 = new float4(float3x.c0, 0f);
        //    c1 = new float4(float3x.c1, 0f);
        //    c2 = new float4(float3x.c2, 0f);
        //    c3 = new float4(transform.pos, 1f);
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 AxisAngle(float3 axis, float angle)
        {
            Math.sincos(angle, out float s, out float c);
            float4 lhs = new float4(axis, 0f);
            _ = lhs.yzxx;
            _ = lhs.zxyx;
            float4 rhs = lhs - lhs * c;
            float4 @float = new float4(lhs.xyz * s, c);
            uint4 rhs2 = new uint4(0u, 0u, 2147483648u, 0u);
            uint4 rhs3 = new uint4(2147483648u, 0u, 0u, 0u);
            uint4 rhs4 = new uint4(0u, 2147483648u, 0u, 0u);
            uint4 rhs5 = new uint4(uint.MaxValue, uint.MaxValue, uint.MaxValue, 0u);
            return new float4x4(lhs.x * rhs + Math.asfloat((Math.asuint(@float.wzyx) ^ rhs2) & rhs5), lhs.y * rhs + Math.asfloat((Math.asuint(@float.zwxx) ^ rhs3) & rhs5), lhs.z * rhs + Math.asfloat((Math.asuint(@float.yxwx) ^ rhs4) & rhs5), new float4(0f, 0f, 0f, 1f));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 EulerXYZ(float3 xyz)
        {
            Math.sincos(xyz, out float3 s, out float3 c);
            return new float4x4(c.y * c.z, c.z * s.x * s.y - c.x * s.z, c.x * c.z * s.y + s.x * s.z, 0f, c.y * s.z, c.x * c.z + s.x * s.y * s.z, c.x * s.y * s.z - c.z * s.x, 0f, 0f - s.y, c.y * s.x, c.x * c.y, 0f, 0f, 0f, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 EulerXZY(float3 xyz)
        {
            Math.sincos(xyz, out float3 s, out float3 c);
            return new float4x4(c.y * c.z, s.x * s.y - c.x * c.y * s.z, c.x * s.y + c.y * s.x * s.z, 0f, s.z, c.x * c.z, (0f - c.z) * s.x, 0f, (0f - c.z) * s.y, c.y * s.x + c.x * s.y * s.z, c.x * c.y - s.x * s.y * s.z, 0f, 0f, 0f, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 EulerYXZ(float3 xyz)
        {
            Math.sincos(xyz, out float3 s, out float3 c);
            return new float4x4(c.y * c.z - s.x * s.y * s.z, (0f - c.x) * s.z, c.z * s.y + c.y * s.x * s.z, 0f, c.z * s.x * s.y + c.y * s.z, c.x * c.z, s.y * s.z - c.y * c.z * s.x, 0f, (0f - c.x) * s.y, s.x, c.x * c.y, 0f, 0f, 0f, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 EulerYZX(float3 xyz)
        {
            Math.sincos(xyz, out float3 s, out float3 c);
            return new float4x4(c.y * c.z, 0f - s.z, c.z * s.y, 0f, s.x * s.y + c.x * c.y * s.z, c.x * c.z, c.x * s.y * s.z - c.y * s.x, 0f, c.y * s.x * s.z - c.x * s.y, c.z * s.x, c.x * c.y + s.x * s.y * s.z, 0f, 0f, 0f, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 EulerZXY(float3 xyz)
        {
            Math.sincos(xyz, out float3 s, out float3 c);
            return new float4x4(c.y * c.z + s.x * s.y * s.z, c.z * s.x * s.y - c.y * s.z, c.x * s.y, 0f, c.x * s.z, c.x * c.z, 0f - s.x, 0f, c.y * s.x * s.z - c.z * s.y, c.y * c.z * s.x + s.y * s.z, c.x * c.y, 0f, 0f, 0f, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 EulerZYX(float3 xyz)
        {
            Math.sincos(xyz, out float3 s, out float3 c);
            return new float4x4(c.y * c.z, (0f - c.y) * s.z, s.y, 0f, c.z * s.x * s.y + c.x * s.z, c.x * c.z - s.x * s.y * s.z, (0f - c.y) * s.x, 0f, s.x * s.z - c.x * c.z * s.y, c.z * s.x + c.x * s.y * s.z, c.x * c.y, 0f, 0f, 0f, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 EulerXYZ(float x, float y, float z)
        {
            return EulerXYZ(new float3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 EulerXZY(float x, float y, float z)
        {
            return EulerXZY(new float3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 EulerYXZ(float x, float y, float z)
        {
            return EulerYXZ(new float3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 EulerYZX(float x, float y, float z)
        {
            return EulerYZX(new float3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 EulerZXY(float x, float y, float z)
        {
            return EulerZXY(new float3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 EulerZYX(float x, float y, float z)
        {
            return EulerZYX(new float3(x, y, z));
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static float4x4 Euler(float3 xyz, math.RotationOrder order = math.RotationOrder.ZXY)
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
        //public static float4x4 Euler(float x, float y, float z, math.RotationOrder order = math.RotationOrder.ZXY)
        //{
        //    return Euler(new float3(x, y, z), order);
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 RotateX(float angle)
        {
            Math.sincos(angle, out float s, out float c);
            return new float4x4(1f, 0f, 0f, 0f, 0f, c, 0f - s, 0f, 0f, s, c, 0f, 0f, 0f, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 RotateY(float angle)
        {
            Math.sincos(angle, out float s, out float c);
            return new float4x4(c, 0f, s, 0f, 0f, 1f, 0f, 0f, 0f - s, 0f, c, 0f, 0f, 0f, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 RotateZ(float angle)
        {
            Math.sincos(angle, out float s, out float c);
            return new float4x4(c, 0f - s, 0f, 0f, s, c, 0f, 0f, 0f, 0f, 1f, 0f, 0f, 0f, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Scale(float s)
        {
            return new float4x4(s, 0f, 0f, 0f, 0f, s, 0f, 0f, 0f, 0f, s, 0f, 0f, 0f, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Scale(float x, float y, float z)
        {
            return new float4x4(x, 0f, 0f, 0f, 0f, y, 0f, 0f, 0f, 0f, z, 0f, 0f, 0f, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Scale(float3 scales)
        {
            return Scale(scales.x, scales.y, scales.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Translate(float3 vector)
        {
            return new float4x4(new float4(1f, 0f, 0f, 0f), new float4(0f, 1f, 0f, 0f), new float4(0f, 0f, 1f, 0f), new float4(vector.x, vector.y, vector.z, 1f));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 LookAt(float3 eye, float3 target, float3 up)
        {
            float3x3 float3x = float3x3.LookRotation(Math.normalize(target - eye), up);
            float4x4 result = default(float4x4);
            result.c0 = new float4(float3x.c0, 0f);
            result.c1 = new float4(float3x.c1, 0f);
            result.c2 = new float4(float3x.c2, 0f);
            result.c3 = new float4(eye, 1f);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 Ortho(float width, float height, float near, float far)
        {
            float num = 1f / width;
            float num2 = 1f / height;
            float num3 = 1f / (far - near);
            return new float4x4(2f * num, 0f, 0f, 0f, 0f, 2f * num2, 0f, 0f, 0f, 0f, -2f * num3, (0f - (far + near)) * num3, 0f, 0f, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 OrthoOffCenter(float left, float right, float bottom, float top, float near, float far)
        {
            float num = 1f / (right - left);
            float num2 = 1f / (top - bottom);
            float num3 = 1f / (far - near);
            return new float4x4(2f * num, 0f, 0f, (0f - (right + left)) * num, 0f, 2f * num2, 0f, (0f - (top + bottom)) * num2, 0f, 0f, -2f * num3, (0f - (far + near)) * num3, 0f, 0f, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 PerspectiveFov(float verticalFov, float aspect, float near, float far)
        {
            float num = 1f / Math.tan(verticalFov * 0.5f);
            float num2 = 1f / (near - far);
            return new float4x4(num / aspect, 0f, 0f, 0f, 0f, num, 0f, 0f, 0f, 0f, (far + near) * num2, 2f * near * far * num2, 0f, 0f, -1f, 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 PerspectiveOffCenter(float left, float right, float bottom, float top, float near, float far)
        {
            float num = 1f / (near - far);
            float num2 = 1f / (right - left);
            float num3 = 1f / (top - bottom);
            return new float4x4(2f * near * num2, 0f, (left + right) * num2, 0f, 0f, 2f * near * num3, (bottom + top) * num3, 0f, 0f, 0f, (far + near) * num, 2f * near * far * num, 0f, 0f, -1f, 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 TRS(float3 translation, quaternion rotation, float3 scale)
        {
            float3x3 float3x = new float3x3(rotation);
            return new float4x4(new float4(float3x.c0 * scale.x, 0f), new float4(float3x.c1 * scale.y, 0f), new float4(float3x.c2 * scale.z, 0f), new float4(translation, 1f));
        }
    }

}

#endif