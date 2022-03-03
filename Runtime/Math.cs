// Copyright 2021 Ikina Games
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

//#define POINT_COLLECTIONS_NATIVE

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2020
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Point.Collections
{
    public static class Math
    {
        public static float TodB(float value)
        {
            double
                linear = value,
                output = 0;
            unsafe
            {
#if POINT_COLLECTIONS_NATIVE
                Native.NativeMath.unity_todB(&linear, &output);
#else
                Burst.BurstMath.unity_todB(&linear, &output);
#endif
            }
            return (float)output;
        }
        public static float FromdB(float dB)
        {
            double
                decibel = dB,
                output = 0;
            unsafe
            {
#if POINT_COLLECTIONS_NATIVE
                Native.NativeMath.unity_fromdB(&decibel, &output);
#else
                Burst.BurstMath.unity_fromdB(&decibel, &output);
#endif
            }
            return (float)output;
        }

        public static long min(in long x, in long y)
        {
#if UNITYENGINE
            return Unity.Mathematics.math.min(x, y);
#else
            return x < y ? x : y;
#endif
        }

#if !UNITYENGINE
        #region Unity.Mathematics

        public const float EPSILON = 1.1920929E-07f;

        #region Hash

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(bool2 v)
        {
            return csum(select(new uint2(2426570171u, 1561977301u), new uint2(4205774813u, 1650214333u), v));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(bool3 v)
        {
            return csum(select(new uint3(2716413241u, 1166264321u, 2503385333u), new uint3(2944493077u, 2599999021u, 3814721321u), v));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(bool4 v)
        {
            return csum(select(new uint4(1610574617u, 1584185147u, 3041325733u, 3150930919u), new uint4(3309258581u, 1770373673u, 3778261171u, 3286279097u), v));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(double2 v)
        {
            return csum(fold_to_uint(v) * new uint2(2503385333u, 2944493077u)) + 2599999021u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(double3 v)
        {
            return csum(fold_to_uint(v) * new uint3(2937008387u, 3835713223u, 2216526373u)) + 3375971453u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(double4 v)
        {
            return csum(fold_to_uint(v) * new uint4(2669441947u, 1260114311u, 2650080659u, 4052675461u)) + 2652487619u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(float2 v)
        {
            return csum(asuint(v) * new uint2(4198118021u, 2908068253u)) + 3705492289u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(float3 v)
        {
            return csum(asuint(v) * new uint3(2601761069u, 1254033427u, 2248573027u)) + 3612677113u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(float4 v)
        {
            return csum(asuint(v) * new uint4(3868600063u, 3170963179u, 2632835537u, 1136528209u)) + 2944626401u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(half v)
        {
            return (uint)(v.value * 1952372791 + -2123433123);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(half2 v)
        {
            return csum(new uint2(v.x.value, v.y.value) * new uint2(1851936439u, 1938025801u)) + 3712598587u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(half3 v)
        {
            return csum(new uint3(v.x.value, v.y.value, v.z.value) * new uint3(1750611407u, 3285396193u, 3110507567u)) + 4271396531u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(half4 v)
        {
            return csum(new uint4(v.x.value, v.y.value, v.z.value, v.w.value) * new uint4(1952372791u, 2631698677u, 4200781601u, 2119021007u)) + 1760485621;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(int2 v)
        {
            return csum(asuint(v) * new uint2(2209710647u, 2201894441u)) + 2849577407u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(int3 v)
        {
            return csum(asuint(v) * new uint3(1283419601u, 1210229737u, 2864955997u)) + 3525118277u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(int4 v)
        {
            return csum(asuint(v) * new uint4(1845824257u, 1963973621u, 2134758553u, 1391111867u)) + 1167706003;
        }

        #endregion

        #region Select

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int select(int a, int b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 select(int2 a, int2 b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 select(int3 a, int3 b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 select(int4 a, int4 b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 select(int2 a, int2 b, bool2 c)
        {
            return new int2(c.x ? b.x : a.x, c.y ? b.y : a.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 select(int3 a, int3 b, bool3 c)
        {
            return new int3(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 select(int4 a, int4 b, bool4 c)
        {
            return new int4(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z, c.w ? b.w : a.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint select(uint a, uint b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2 select(uint2 a, uint2 b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3 select(uint3 a, uint3 b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 select(uint4 a, uint4 b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2 select(uint2 a, uint2 b, bool2 c)
        {
            return new uint2(c.x ? b.x : a.x, c.y ? b.y : a.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3 select(uint3 a, uint3 b, bool3 c)
        {
            return new uint3(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 select(uint4 a, uint4 b, bool4 c)
        {
            return new uint4(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z, c.w ? b.w : a.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long select(long a, long b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong select(ulong a, ulong b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float select(float a, float b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 select(float2 a, float2 b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 select(float3 a, float3 b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 select(float4 a, float4 b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 select(float2 a, float2 b, bool2 c)
        {
            return new float2(c.x ? b.x : a.x, c.y ? b.y : a.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 select(float3 a, float3 b, bool3 c)
        {
            return new float3(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 select(float4 a, float4 b, bool4 c)
        {
            return new float4(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z, c.w ? b.w : a.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double select(double a, double b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 select(double2 a, double2 b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 select(double3 a, double3 b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 select(double4 a, double4 b, bool c)
        {
            if (!c)
            {
                return a;
            }

            return b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 select(double2 a, double2 b, bool2 c)
        {
            return new double2(c.x ? b.x : a.x, c.y ? b.y : a.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 select(double3 a, double3 b, bool3 c)
        {
            return new double3(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 select(double4 a, double4 b, bool4 c)
        {
            return new double4(c.x ? b.x : a.x, c.y ? b.y : a.y, c.z ? b.z : a.z, c.w ? b.w : a.w);
        }


        #endregion

        #region csum

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int csum(int2 x)
        {
            return x.x + x.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int csum(int3 x)
        {
            return x.x + x.y + x.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int csum(int4 x)
        {
            return x.x + x.y + x.z + x.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint csum(uint2 x)
        {
            return x.x + x.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint csum(uint3 x)
        {
            return x.x + x.y + x.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint csum(uint4 x)
        {
            return x.x + x.y + x.z + x.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float csum(float2 x)
        {
            return x.x + x.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float csum(float3 x)
        {
            return x.x + x.y + x.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float csum(float4 x)
        {
            return x.x + x.y + (x.z + x.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double csum(double2 x)
        {
            return x.x + x.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double csum(double3 x)
        {
            return x.x + x.y + x.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double csum(double4 x)
        {
            return x.x + x.y + (x.z + x.w);
        }

        #endregion

        #region asint

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int asint(uint x)
        {
            return (int)x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 asint(uint2 x)
        {
            return new int2((int)x.x, (int)x.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 asint(uint3 x)
        {
            return new int3((int)x.x, (int)x.y, (int)x.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 asint(uint4 x)
        {
            return new int4((int)x.x, (int)x.y, (int)x.z, (int)x.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int asint(float x)
        {
            IntFloatUnion intFloatUnion = default(IntFloatUnion);
            intFloatUnion.intValue = 0;
            intFloatUnion.floatValue = x;
            return intFloatUnion.intValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 asint(float2 x)
        {
            return new int2(asint(x.x), asint(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 asint(float3 x)
        {
            return new int3(asint(x.x), asint(x.y), asint(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 asint(float4 x)
        {
            return new int4(asint(x.x), asint(x.y), asint(x.z), asint(x.w));
        }

        #endregion

        #region asuint

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint asuint(int x)
        {
            return (uint)x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2 asuint(int2 x)
        {
            return new uint2((uint)x.x, (uint)x.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3 asuint(int3 x)
        {
            return new uint3((uint)x.x, (uint)x.y, (uint)x.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 asuint(int4 x)
        {
            return new uint4((uint)x.x, (uint)x.y, (uint)x.z, (uint)x.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint asuint(float x)
        {
            return (uint)asint(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2 asuint(float2 x)
        {
            return new uint2(asuint(x.x), asuint(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3 asuint(float3 x)
        {
            return new uint3(asuint(x.x), asuint(x.y), asuint(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 asuint(float4 x)
        {
            return new uint4(asuint(x.x), asuint(x.y), asuint(x.z), asuint(x.w));
        }

        #endregion

        #region mul



        #endregion

        #region sin cos

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float cos(float x)
        {
            return (float)System.Math.Cos(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 cos(float2 x)
        {
            return new float2(cos(x.x), cos(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 cos(float3 x)
        {
            return new float3(cos(x.x), cos(x.y), cos(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 cos(float4 x)
        {
            return new float4(cos(x.x), cos(x.y), cos(x.z), cos(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double cos(double x)
        {
            return System.Math.Cos(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 cos(double2 x)
        {
            return new double2(cos(x.x), cos(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 cos(double3 x)
        {
            return new double3(cos(x.x), cos(x.y), cos(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 cos(double4 x)
        {
            return new double4(cos(x.x), cos(x.y), cos(x.z), cos(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float cosh(float x)
        {
            return (float)System.Math.Cosh(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 cosh(float2 x)
        {
            return new float2(cosh(x.x), cosh(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 cosh(float3 x)
        {
            return new float3(cosh(x.x), cosh(x.y), cosh(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 cosh(float4 x)
        {
            return new float4(cosh(x.x), cosh(x.y), cosh(x.z), cosh(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double cosh(double x)
        {
            return System.Math.Cosh(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 cosh(double2 x)
        {
            return new double2(cosh(x.x), cosh(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 cosh(double3 x)
        {
            return new double3(cosh(x.x), cosh(x.y), cosh(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 cosh(double4 x)
        {
            return new double4(cosh(x.x), cosh(x.y), cosh(x.z), cosh(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float acos(float x)
        {
            return (float)System.Math.Acos(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 acos(float2 x)
        {
            return new float2(acos(x.x), acos(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 acos(float3 x)
        {
            return new float3(acos(x.x), acos(x.y), acos(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 acos(float4 x)
        {
            return new float4(acos(x.x), acos(x.y), acos(x.z), acos(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double acos(double x)
        {
            return System.Math.Acos(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 acos(double2 x)
        {
            return new double2(acos(x.x), acos(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 acos(double3 x)
        {
            return new double3(acos(x.x), acos(x.y), acos(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 acos(double4 x)
        {
            return new double4(acos(x.x), acos(x.y), acos(x.z), acos(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float sin(float x)
        {
            return (float)System.Math.Sin(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 sin(float2 x)
        {
            return new float2(sin(x.x), sin(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 sin(float3 x)
        {
            return new float3(sin(x.x), sin(x.y), sin(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 sin(float4 x)
        {
            return new float4(sin(x.x), sin(x.y), sin(x.z), sin(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double sin(double x)
        {
            return System.Math.Sin(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 sin(double2 x)
        {
            return new double2(sin(x.x), sin(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 sin(double3 x)
        {
            return new double3(sin(x.x), sin(x.y), sin(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 sin(double4 x)
        {
            return new double4(sin(x.x), sin(x.y), sin(x.z), sin(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float sinh(float x)
        {
            return (float)System.Math.Sinh(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 sinh(float2 x)
        {
            return new float2(sinh(x.x), sinh(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 sinh(float3 x)
        {
            return new float3(sinh(x.x), sinh(x.y), sinh(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 sinh(float4 x)
        {
            return new float4(sinh(x.x), sinh(x.y), sinh(x.z), sinh(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double sinh(double x)
        {
            return System.Math.Sinh(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 sinh(double2 x)
        {
            return new double2(sinh(x.x), sinh(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 sinh(double3 x)
        {
            return new double3(sinh(x.x), sinh(x.y), sinh(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 sinh(double4 x)
        {
            return new double4(sinh(x.x), sinh(x.y), sinh(x.z), sinh(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float asin(float x)
        {
            return (float)System.Math.Asin(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 asin(float2 x)
        {
            return new float2(asin(x.x), asin(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 asin(float3 x)
        {
            return new float3(asin(x.x), asin(x.y), asin(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 asin(float4 x)
        {
            return new float4(asin(x.x), asin(x.y), asin(x.z), asin(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double asin(double x)
        {
            return System.Math.Asin(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 asin(double2 x)
        {
            return new double2(asin(x.x), asin(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 asin(double3 x)
        {
            return new double3(asin(x.x), asin(x.y), asin(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 asin(double4 x)
        {
            return new double4(asin(x.x), asin(x.y), asin(x.z), asin(x.w));
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void sincos(float x, out float s, out float c)
        {
            s = sin(x);
            c = cos(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void sincos(float2 x, out float2 s, out float2 c)
        {
            s = sin(x);
            c = cos(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void sincos(float3 x, out float3 s, out float3 c)
        {
            s = sin(x);
            c = cos(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void sincos(float4 x, out float4 s, out float4 c)
        {
            s = sin(x);
            c = cos(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void sincos(double x, out double s, out double c)
        {
            s = sin(x);
            c = cos(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void sincos(double2 x, out double2 s, out double2 c)
        {
            s = sin(x);
            c = cos(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void sincos(double3 x, out double3 s, out double3 c)
        {
            s = sin(x);
            c = cos(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void sincos(double4 x, out double4 s, out double4 c)
        {
            s = sin(x);
            c = cos(x);
        }

        #endregion

        #region dot

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int dot(int x, int y)
        {
            return x * y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int dot(int2 x, int2 y)
        {
            return x.x * y.x + x.y * y.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int dot(int3 x, int3 y)
        {
            return x.x * y.x + x.y * y.y + x.z * y.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int dot(int4 x, int4 y)
        {
            return x.x * y.x + x.y * y.y + x.z * y.z + x.w * y.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint dot(uint x, uint y)
        {
            return x * y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint dot(uint2 x, uint2 y)
        {
            return x.x * y.x + x.y * y.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint dot(uint3 x, uint3 y)
        {
            return x.x * y.x + x.y * y.y + x.z * y.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint dot(uint4 x, uint4 y)
        {
            return x.x * y.x + x.y * y.y + x.z * y.z + x.w * y.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float dot(float x, float y)
        {
            return x * y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float dot(float2 x, float2 y)
        {
            return x.x * y.x + x.y * y.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float dot(float3 x, float3 y)
        {
            return x.x * y.x + x.y * y.y + x.z * y.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float dot(float4 x, float4 y)
        {
            return x.x * y.x + x.y * y.y + x.z * y.z + x.w * y.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double dot(double x, double y)
        {
            return x * y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double dot(double2 x, double2 y)
        {
            return x.x * y.x + x.y * y.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double dot(double3 x, double3 y)
        {
            return x.x * y.x + x.y * y.y + x.z * y.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double dot(double4 x, double4 y)
        {
            return x.x * y.x + x.y * y.y + x.z * y.z + x.w * y.w;
        }



        #endregion

        #region cross

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 cross(float3 x, float3 y)
        {
            return (x * y.yzx - x.yzx * y).yzx;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 cross(double3 x, double3 y)
        {
            return (x * y.yzx - x.yzx * y).yzx;
        }



        #endregion

        #region min max

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int min(int x, int y)
        {
            if (x >= y)
            {
                return y;
            }

            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 min(int2 x, int2 y)
        {
            return new int2(min(x.x, y.x), min(x.y, y.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 min(int3 x, int3 y)
        {
            return new int3(min(x.x, y.x), min(x.y, y.y), min(x.z, y.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 min(int4 x, int4 y)
        {
            return new int4(min(x.x, y.x), min(x.y, y.y), min(x.z, y.z), min(x.w, y.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint min(uint x, uint y)
        {
            if (x >= y)
            {
                return y;
            }

            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2 min(uint2 x, uint2 y)
        {
            return new uint2(min(x.x, y.x), min(x.y, y.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3 min(uint3 x, uint3 y)
        {
            return new uint3(min(x.x, y.x), min(x.y, y.y), min(x.z, y.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 min(uint4 x, uint4 y)
        {
            return new uint4(min(x.x, y.x), min(x.y, y.y), min(x.z, y.z), min(x.w, y.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long min(long x, long y)
        {
            if (x >= y)
            {
                return y;
            }

            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong min(ulong x, ulong y)
        {
            if (x >= y)
            {
                return y;
            }

            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float min(float x, float y)
        {
            if (!float.IsNaN(y) && !(x < y))
            {
                return y;
            }

            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 min(float2 x, float2 y)
        {
            return new float2(min(x.x, y.x), min(x.y, y.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 min(float3 x, float3 y)
        {
            return new float3(min(x.x, y.x), min(x.y, y.y), min(x.z, y.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 min(float4 x, float4 y)
        {
            return new float4(min(x.x, y.x), min(x.y, y.y), min(x.z, y.z), min(x.w, y.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double min(double x, double y)
        {
            if (!double.IsNaN(y) && !(x < y))
            {
                return y;
            }

            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 min(double2 x, double2 y)
        {
            return new double2(min(x.x, y.x), min(x.y, y.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 min(double3 x, double3 y)
        {
            return new double3(min(x.x, y.x), min(x.y, y.y), min(x.z, y.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 min(double4 x, double4 y)
        {
            return new double4(min(x.x, y.x), min(x.y, y.y), min(x.z, y.z), min(x.w, y.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int max(int x, int y)
        {
            if (x <= y)
            {
                return y;
            }

            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 max(int2 x, int2 y)
        {
            return new int2(max(x.x, y.x), max(x.y, y.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 max(int3 x, int3 y)
        {
            return new int3(max(x.x, y.x), max(x.y, y.y), max(x.z, y.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 max(int4 x, int4 y)
        {
            return new int4(max(x.x, y.x), max(x.y, y.y), max(x.z, y.z), max(x.w, y.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint max(uint x, uint y)
        {
            if (x <= y)
            {
                return y;
            }

            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2 max(uint2 x, uint2 y)
        {
            return new uint2(max(x.x, y.x), max(x.y, y.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3 max(uint3 x, uint3 y)
        {
            return new uint3(max(x.x, y.x), max(x.y, y.y), max(x.z, y.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 max(uint4 x, uint4 y)
        {
            return new uint4(max(x.x, y.x), max(x.y, y.y), max(x.z, y.z), max(x.w, y.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long max(long x, long y)
        {
            if (x <= y)
            {
                return y;
            }

            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong max(ulong x, ulong y)
        {
            if (x <= y)
            {
                return y;
            }

            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float max(float x, float y)
        {
            if (!float.IsNaN(y) && !(x > y))
            {
                return y;
            }

            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 max(float2 x, float2 y)
        {
            return new float2(max(x.x, y.x), max(x.y, y.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 max(float3 x, float3 y)
        {
            return new float3(max(x.x, y.x), max(x.y, y.y), max(x.z, y.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 max(float4 x, float4 y)
        {
            return new float4(max(x.x, y.x), max(x.y, y.y), max(x.z, y.z), max(x.w, y.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double max(double x, double y)
        {
            if (!double.IsNaN(y) && !(x > y))
            {
                return y;
            }

            return x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 max(double2 x, double2 y)
        {
            return new double2(max(x.x, y.x), max(x.y, y.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 max(double3 x, double3 y)
        {
            return new double3(max(x.x, y.x), max(x.y, y.y), max(x.z, y.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 max(double4 x, double4 y)
        {
            return new double4(max(x.x, y.x), max(x.y, y.y), max(x.z, y.z), max(x.w, y.w));
        }



        #endregion

        #region sqrt

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float sqrt(float x)
        {
            return (float)System.Math.Sqrt(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 sqrt(float2 x)
        {
            return new float2(sqrt(x.x), sqrt(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 sqrt(float3 x)
        {
            return new float3(sqrt(x.x), sqrt(x.y), sqrt(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 sqrt(float4 x)
        {
            return new float4(sqrt(x.x), sqrt(x.y), sqrt(x.z), sqrt(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double sqrt(double x)
        {
            return System.Math.Sqrt(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 sqrt(double2 x)
        {
            return new double2(sqrt(x.x), sqrt(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 sqrt(double3 x)
        {
            return new double3(sqrt(x.x), sqrt(x.y), sqrt(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 sqrt(double4 x)
        {
            return new double4(sqrt(x.x), sqrt(x.y), sqrt(x.z), sqrt(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float rsqrt(float x)
        {
            return 1f / sqrt(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 rsqrt(float2 x)
        {
            return 1f / sqrt(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 rsqrt(float3 x)
        {
            return 1f / sqrt(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 rsqrt(float4 x)
        {
            return 1f / sqrt(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double rsqrt(double x)
        {
            return 1.0 / sqrt(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 rsqrt(double2 x)
        {
            return 1.0 / sqrt(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 rsqrt(double3 x)
        {
            return 1.0 / sqrt(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 rsqrt(double4 x)
        {
            return 1.0 / sqrt(x);
        }



        #endregion

        #region normalize

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 normalize(float2 x)
        {
            return rsqrt(dot(x, x)) * x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 normalize(float3 x)
        {
            return rsqrt(dot(x, x)) * x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 normalize(float4 x)
        {
            return rsqrt(dot(x, x)) * x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 normalize(double2 x)
        {
            return rsqrt(dot(x, x)) * x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 normalize(double3 x)
        {
            return rsqrt(dot(x, x)) * x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 normalize(double4 x)
        {
            return rsqrt(dot(x, x)) * x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 normalizesafe(float2 x, float2 defaultvalue = default(float2))
        {
            float num = dot(x, x);
            return select(defaultvalue, x * rsqrt(num), num > 1.17549435E-38f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 normalizesafe(float3 x, float3 defaultvalue = default(float3))
        {
            float num = dot(x, x);
            return select(defaultvalue, x * rsqrt(num), num > 1.17549435E-38f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 normalizesafe(float4 x, float4 defaultvalue = default(float4))
        {
            float num = dot(x, x);
            return select(defaultvalue, x * rsqrt(num), num > 1.17549435E-38f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 normalizesafe(double2 x, double2 defaultvalue = default(double2))
        {
            double num = dot(x, x);
            return select(defaultvalue, x * rsqrt(num), num > 1.1754943508222875E-38);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 normalizesafe(double3 x, double3 defaultvalue = default(double3))
        {
            double num = dot(x, x);
            return select(defaultvalue, x * rsqrt(num), num > 1.1754943508222875E-38);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 normalizesafe(double4 x, double4 defaultvalue = default(double4))
        {
            double num = dot(x, x);
            return select(defaultvalue, x * rsqrt(num), num > 1.1754943508222875E-38);
        }



        #endregion

        #region Internal

        [StructLayout(LayoutKind.Explicit)]
        internal struct IntFloatUnion
        {
            [FieldOffset(0)]
            public int intValue;

            [FieldOffset(0)]
            public float floatValue;
        }
        [StructLayout(LayoutKind.Explicit)]
        internal struct LongDoubleUnion
        {
            [FieldOffset(0)]
            public long longValue;

            [FieldOffset(0)]
            public double doubleValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint fold_to_uint(double x)
        {
            LongDoubleUnion longDoubleUnion = default(LongDoubleUnion);
            longDoubleUnion.longValue = 0L;
            longDoubleUnion.doubleValue = x;
            return (uint)((int)(longDoubleUnion.longValue >> 32) ^ (int)longDoubleUnion.longValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint2 fold_to_uint(double2 x)
        {
            return new uint2(fold_to_uint(x.x), fold_to_uint(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint3 fold_to_uint(double3 x)
        {
            return new uint3(fold_to_uint(x.x), fold_to_uint(x.y), fold_to_uint(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static uint4 fold_to_uint(double4 x)
        {
            return new uint4(fold_to_uint(x.x), fold_to_uint(x.y), fold_to_uint(x.z), fold_to_uint(x.w));
        }


        #endregion

        #endregion
#endif
    }
}
