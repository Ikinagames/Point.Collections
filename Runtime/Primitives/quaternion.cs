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

#if UNITY_2019 || !UNITY_2020_OR_NEWER

using System;
using System.Runtime.CompilerServices;

namespace Point.Collections
{
    [Serializable]
    public struct quaternion : IEquatable<quaternion>, IFormattable
    {
        public float4 value;

        public static readonly quaternion identity = new quaternion(0f, 0f, 0f, 1f);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public quaternion(float x, float y, float z, float w)
        {
            value.x = x;
            value.y = y;
            value.z = z;
            value.w = w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public quaternion(float4 value)
        {
            this.value = value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator quaternion(float4 v)
        {
            return new quaternion(v);
        }

        public quaternion(float3x3 m)
        {
            float3 c = m.c0;
            float3 c2 = m.c1;
            float3 c3 = m.c2;
            uint num = Math.asuint(c.x) & 2147483648u;
            float x = c2.y + Math.asfloat(Math.asuint(c3.z) ^ num);
            uint4 @uint = new uint4((int)num >> 31);
            uint4 uint2 = new uint4(Math.asint(x) >> 31);
            float x2 = 1f + Math.abs(c.x);
            uint4 rhs = new uint4(0u, 2147483648u, 2147483648u, 2147483648u) ^ (@uint & new uint4(0u, 2147483648u, 0u, 2147483648u)) ^ (uint2 & new uint4(2147483648u, 2147483648u, 2147483648u, 0u));
            value = new float4(x2, c.y, c3.x, c2.z) + Math.asfloat(Math.asuint(new float4(x, c2.x, c.z, c3.y)) ^ rhs);
            value = Math.asfloat((Math.asuint(value) & ~@uint) | (Math.asuint(value.zwxy) & @uint));
            value = Math.asfloat((Math.asuint(value.wzyx) & ~uint2) | (Math.asuint(value) & uint2));
            value = Math.normalize(value);
        }

        public quaternion(float4x4 m)
        {
            float4 c = m.c0;
            float4 c2 = m.c1;
            float4 c3 = m.c2;
            uint num = Math.asuint(c.x) & 2147483648u;
            float x = c2.y + Math.asfloat(Math.asuint(c3.z) ^ num);
            uint4 @uint =new uint4((int)num >> 31);
            uint4 uint2 =new uint4(Math.asint(x) >> 31);
            float x2 = 1f + Math.abs(c.x);
            uint4 rhs = new uint4(0u, 2147483648u, 2147483648u, 2147483648u) ^ (@uint & new uint4(0u, 2147483648u, 0u, 2147483648u)) ^ (uint2 & new uint4(2147483648u, 2147483648u, 2147483648u, 0u));
            value = new float4(x2, c.y, c3.x, c2.z) + Math.asfloat(Math.asuint(new float4(x, c2.x, c.z, c3.y)) ^ rhs);
            value = Math.asfloat((Math.asuint(value) & ~@uint) | (Math.asuint(value.zwxy) & @uint));
            value = Math.asfloat((Math.asuint(value.wzyx) & ~uint2) | (Math.asuint(value) & uint2));
            value = Math.normalize(value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion AxisAngle(float3 axis, float angle)
        {
            Math.sincos(0.5f * angle, out float s, out float c);
            return new quaternion(new float4(axis * s, c));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion EulerXYZ(float3 xyz)
        {
            Math.sincos(0.5f * xyz, out float3 s, out float3 c);
            return new quaternion(new float4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * new float4(c.xyz, s.x) * new float4(-1f, 1f, -1f, 1f));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion EulerXZY(float3 xyz)
        {
            Math.sincos(0.5f * xyz, out float3 s, out float3 c);
            return new quaternion(new float4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * new float4(c.xyz, s.x) * new float4(1f, 1f, -1f, -1f));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion EulerYXZ(float3 xyz)
        {
            Math.sincos(0.5f * xyz, out float3 s, out float3 c);
            return new quaternion(new float4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * new float4(c.xyz, s.x) * new float4(-1f, 1f, 1f, -1f));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion EulerYZX(float3 xyz)
        {
            Math.sincos(0.5f * xyz, out float3 s, out float3 c);
            return new quaternion(new float4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * new float4(c.xyz, s.x) * new float4(-1f, -1f, 1f, 1f));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion EulerZXY(float3 xyz)
        {
            Math.sincos(0.5f * xyz, out float3 s, out float3 c);
            return new quaternion(new float4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * new float4(c.xyz, s.x) * new float4(1f, -1f, -1f, 1f));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion EulerZYX(float3 xyz)
        {
            Math.sincos(0.5f * xyz, out float3 s, out float3 c);
            return new quaternion(new float4(s.xyz, c.x) * c.yxxy * c.zzyz + s.yxxy * s.zzyz * new float4(c.xyz, s.x) * new float4(1f, -1f, 1f, -1f));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion EulerXYZ(float x, float y, float z)
        {
            return EulerXYZ(new float3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion EulerXZY(float x, float y, float z)
        {
            return EulerXZY(new float3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion EulerYXZ(float x, float y, float z)
        {
            return EulerYXZ(new float3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion EulerYZX(float x, float y, float z)
        {
            return EulerYZX(new float3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion EulerZXY(float x, float y, float z)
        {
            return EulerZXY(new float3(x, y, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion EulerZYX(float x, float y, float z)
        {
            return EulerZYX(new float3(x, y, z));
        }

        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        //public static quaternion Euler(float3 xyz, math.RotationOrder order = math.RotationOrder.ZXY)
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
        //public static quaternion Euler(float x, float y, float z, math.RotationOrder order = math.RotationOrder.ZXY)
        //{
        //    return Euler(math.float3(x, y, z), order);
        //}

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion RotateX(float angle)
        {
            Math.sincos(0.5f * angle, out float s, out float c);
            return new quaternion(s, 0f, 0f, c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion RotateY(float angle)
        {
            Math.sincos(0.5f * angle, out float s, out float c);
            return new quaternion(0f, s, 0f, c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion RotateZ(float angle)
        {
            Math.sincos(0.5f * angle, out float s, out float c);
            return new quaternion(0f, 0f, s, c);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion LookRotation(float3 forward, float3 up)
        {
            float3 @float = Math.normalize(Math.cross(up, forward));
            return new quaternion(new float3x3(@float, Math.cross(forward, @float), forward));
        }

        public static quaternion LookRotationSafe(float3 forward, float3 up)
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
            return new quaternion(Math.select(new float4(0f, 0f, 0f, 1f), new quaternion(new float3x3(@float, Math.cross(forward, @float), forward)).value, c));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(quaternion x)
        {
            if (value.x == x.value.x && value.y == x.value.y && value.z == x.value.z)
            {
                return value.w == x.value.w;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override bool Equals(object x)
        {
            if (x is quaternion)
            {
                quaternion x2 = (quaternion)x;
                return Equals(x2);
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
            return $"quaternion({value.x}f, {value.y}f, {value.z}f, {value.w}f)";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"quaternion({value.x.ToString(format, formatProvider)}f, {value.y.ToString(format, formatProvider)}f, {value.z.ToString(format, formatProvider)}f, {value.w.ToString(format, formatProvider)}f)";
        }

        public static implicit operator UnityEngine.Quaternion(quaternion t)
        {
            return new UnityEngine.Quaternion(t.value.x, t.value.y, t.value.z, t.value.w);
        }
        public static implicit operator quaternion(UnityEngine.Quaternion t)
        {
            return new quaternion(t.x, t.y, t.z, t.w);
        }
    }

}

#endif