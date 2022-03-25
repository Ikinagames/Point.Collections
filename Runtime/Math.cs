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

#if UNITY_2020_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
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
        public const float PI = (float)System.Math.PI;
        public const float Deg2Rad = (float)System.Math.PI / 180f;
        public const float Rad2Deg = 57.29578f;

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

        [System.Obsolete]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(quaternion q)
        {
            return hash(q.value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(bool2x2 v)
        {
            return csum(select(new uint2(2062756937u, 2920485769u), new uint2(1562056283u, 2265541847u), v.c0) + select(new uint2(1283419601u, 1210229737u), new uint2(2864955997u, 3525118277u), v.c1));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(bool2x3 v)
        {
            return csum(select(new uint2(2078515003u, 4206465343u), new uint2(3025146473u, 3763046909u), v.c0) + select(new uint2(3678265601u, 2070747979u), new uint2(1480171127u, 1588341193u), v.c1) + select(new uint2(4234155257u, 1811310911u), new uint2(2635799963u, 4165137857u), v.c2));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(bool2x4 v)
        {
            return csum(select(new uint2(1168253063u, 4228926523u), new uint2(1610574617u, 1584185147u), v.c0) + select(new uint2(3041325733u, 3150930919u), new uint2(3309258581u, 1770373673u), v.c1) + select(new uint2(3778261171u, 3286279097u), new uint2(4264629071u, 1898591447u), v.c2) + select(new uint2(2641864091u, 1229113913u), new uint2(3020867117u, 1449055807u), v.c3));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(bool3x2 v)
        {
            return csum(select(new uint3(2627668003u, 1520214331u, 2949502447u), new uint3(2827819133u, 3480140317u, 2642994593u), v.c0) + select(new uint3(3940484981u, 1954192763u, 1091696537u), new uint3(3052428017u, 4253034763u, 2338696631u), v.c1));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(bool3x3 v)
        {
            return csum(select(new uint3(3881277847u, 4017968839u, 1727237899u), new uint3(1648514723u, 1385344481u, 3538260197u), v.c0) + select(new uint3(4066109527u, 2613148903u, 3367528529u), new uint3(1678332449u, 2918459647u, 2744611081u), v.c1) + select(new uint3(1952372791u, 2631698677u, 4200781601u), new uint3(2119021007u, 1760485621u, 3157985881u), v.c2));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(bool3x4 v)
        {
            return csum(select(new uint3(2209710647u, 2201894441u, 2849577407u), new uint3(3287031191u, 3098675399u, 1564399943u), v.c0) + select(new uint3(1148435377u, 3416333663u, 1750611407u), new uint3(3285396193u, 3110507567u, 4271396531u), v.c1) + select(new uint3(4198118021u, 2908068253u, 3705492289u), new uint3(2497566569u, 2716413241u, 1166264321u), v.c2) + select(new uint3(2503385333u, 2944493077u, 2599999021u), new uint3(3814721321u, 1595355149u, 1728931849u), v.c3));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(bool4x2 v)
        {
            return csum(select(new uint4(3516359879u, 3050356579u, 4178586719u, 2558655391u), new uint4(1453413133u, 2152428077u, 1938706661u, 1338588197u), v.c0) + select(new uint4(3439609253u, 3535343003u, 3546061613u, 2702024231u), new uint4(1452124841u, 1966089551u, 2668168249u, 1587512777u), v.c1));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(bool4x3 v)
        {
            return csum(select(new uint4(3940484981u, 1954192763u, 1091696537u, 3052428017u), new uint4(4253034763u, 2338696631u, 3757372771u, 1885959949u), v.c0) + select(new uint4(3508684087u, 3919501043u, 1209161033u, 4007793211u), new uint4(3819806693u, 3458005183u, 2078515003u, 4206465343u), v.c1) + select(new uint4(3025146473u, 3763046909u, 3678265601u, 2070747979u), new uint4(1480171127u, 1588341193u, 4234155257u, 1811310911u), v.c2));
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(bool4x4 v)
        {
            return csum(select(new uint4(3516359879u, 3050356579u, 4178586719u, 2558655391u), new uint4(1453413133u, 2152428077u, 1938706661u, 1338588197u), v.c0) + select(new uint4(3439609253u, 3535343003u, 3546061613u, 2702024231u), new uint4(1452124841u, 1966089551u, 2668168249u, 1587512777u), v.c1) + select(new uint4(2353831999u, 3101256173u, 2891822459u, 2837054189u), new uint4(3016004371u, 4097481403u, 2229788699u, 2382715877u), v.c2) + select(new uint4(1851936439u, 1938025801u, 3712598587u, 3956330501u), new uint4(2437373431u, 1441286183u, 2426570171u, 1561977301u), v.c3));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(float2x2 v)
        {
            return csum(asuint(v.c0) * new uint2(2627668003u, 1520214331u) + asuint(v.c1) * new uint2(2949502447u, 2827819133u)) + 3480140317u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(float2x3 v)
        {
            return csum(asuint(v.c0) * new uint2(3898072289u, 4129428421u) + asuint(v.c1) * new uint2(2631575897u, 2854656703u) + asuint(v.c2) * new uint2(3578504047u, 4245178297u)) + 2173281923u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(float2x4 v)
        {
            return csum(asuint(v.c0) * new uint2(3546061613u, 2702024231u) + asuint(v.c1) * new uint2(1452124841u, 1966089551u) + asuint(v.c2) * new uint2(2668168249u, 1587512777u) + asuint(v.c3) * new uint2(2353831999u, 3101256173u)) + 2891822459u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(float3x2 v)
        {
            return csum(asuint(v.c0) * new uint3(3777095341u, 3385463369u, 1773538433u) + asuint(v.c1) * new uint3(3773525029u, 4131962539u, 1809525511u)) + 4016293529u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(float3x3 v)
        {
            return csum(asuint(v.c0) * new uint3(1899745391u, 1966790317u, 3516359879u) + asuint(v.c1) * new uint3(3050356579u, 4178586719u, 2558655391u) + asuint(v.c2) * new uint3(1453413133u, 2152428077u, 1938706661u)) + 1338588197;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(float3x4 v)
        {
            return csum(asuint(v.c0) * new uint3(4192899797u, 3271228601u, 1634639009u) + asuint(v.c1) * new uint3(3318036811u, 3404170631u, 2048213449u) + asuint(v.c2) * new uint3(4164671783u, 1780759499u, 1352369353u) + asuint(v.c3) * new uint3(2446407751u, 1391928079u, 3475533443u)) + 3777095341u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(float4x2 v)
        {
            return csum(asuint(v.c0) * new uint4(2864955997u, 3525118277u, 2298260269u, 1632478733u) + asuint(v.c1) * new uint4(1537393931u, 2353355467u, 3441847433u, 4052036147u)) + 2011389559;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(float4x3 v)
        {
            return csum(asuint(v.c0) * new uint4(3309258581u, 1770373673u, 3778261171u, 3286279097u) + asuint(v.c1) * new uint4(4264629071u, 1898591447u, 2641864091u, 1229113913u) + asuint(v.c2) * new uint4(3020867117u, 1449055807u, 2479033387u, 3702457169u)) + 1845824257;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(float4x4 v)
        {
            return csum(asuint(v.c0) * new uint4(3299952959u, 3121178323u, 2948522579u, 1531026433u) + asuint(v.c1) * new uint4(1365086453u, 3969870067u, 4192899797u, 3271228601u) + asuint(v.c2) * new uint4(1634639009u, 3318036811u, 3404170631u, 2048213449u) + asuint(v.c3) * new uint4(4164671783u, 1780759499u, 1352369353u, 2446407751u)) + 1391928079;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(double2x2 v)
        {
            return csum(fold_to_uint(v.c0) * new uint2(4253034763u, 2338696631u) + fold_to_uint(v.c1) * new uint2(3757372771u, 1885959949u)) + 3508684087u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(double2x3 v)
        {
            return csum(fold_to_uint(v.c0) * new uint2(4066109527u, 2613148903u) + fold_to_uint(v.c1) * new uint2(3367528529u, 1678332449u) + fold_to_uint(v.c2) * new uint2(2918459647u, 2744611081u)) + 1952372791;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(double2x4 v)
        {
            return csum(fold_to_uint(v.c0) * new uint2(2437373431u, 1441286183u) + fold_to_uint(v.c1) * new uint2(2426570171u, 1561977301u) + fold_to_uint(v.c2) * new uint2(4205774813u, 1650214333u) + fold_to_uint(v.c3) * new uint2(3388112843u, 1831150513u)) + 1848374953;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(double3x2 v)
        {
            return csum(fold_to_uint(v.c0) * new uint3(3996716183u, 2626301701u, 1306289417u) + fold_to_uint(v.c1) * new uint3(2096137163u, 1548578029u, 4178800919u)) + 3898072289u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(double3x3 v)
        {
            return csum(fold_to_uint(v.c0) * new uint3(2891822459u, 2837054189u, 3016004371u) + fold_to_uint(v.c1) * new uint3(4097481403u, 2229788699u, 2382715877u) + fold_to_uint(v.c2) * new uint3(1851936439u, 1938025801u, 3712598587u)) + 3956330501u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(double3x4 v)
        {
            return csum(fold_to_uint(v.c0) * new uint3(3996716183u, 2626301701u, 1306289417u) + fold_to_uint(v.c1) * new uint3(2096137163u, 1548578029u, 4178800919u) + fold_to_uint(v.c2) * new uint3(3898072289u, 4129428421u, 2631575897u) + fold_to_uint(v.c3) * new uint3(2854656703u, 3578504047u, 4245178297u)) + 2173281923u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(double4x2 v)
        {
            return csum(fold_to_uint(v.c0) * new uint4(1521739981u, 1735296007u, 3010324327u, 1875523709u) + fold_to_uint(v.c1) * new uint4(2937008387u, 3835713223u, 2216526373u, 3375971453u)) + 3559829411u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(double4x3 v)
        {
            return csum(fold_to_uint(v.c0) * new uint4(2057338067u, 2942577577u, 2834440507u, 2671762487u) + fold_to_uint(v.c1) * new uint4(2892026051u, 2455987759u, 3868600063u, 3170963179u) + fold_to_uint(v.c2) * new uint4(2632835537u, 1136528209u, 2944626401u, 2972762423u)) + 1417889653;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(double4x4 v)
        {
            return csum(fold_to_uint(v.c0) * new uint4(1306289417u, 2096137163u, 1548578029u, 4178800919u) + fold_to_uint(v.c1) * new uint4(3898072289u, 4129428421u, 2631575897u, 2854656703u) + fold_to_uint(v.c2) * new uint4(3578504047u, 4245178297u, 2173281923u, 2973357649u) + fold_to_uint(v.c3) * new uint4(3881277847u, 4017968839u, 1727237899u, 1648514723u)) + 1385344481;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(int2x2 v)
        {
            return csum(asuint(v.c0) * new uint2(3784421429u, 1750626223u) + asuint(v.c1) * new uint2(3571447507u, 3412283213u)) + 2601761069u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(int2x3 v)
        {
            return csum(asuint(v.c0) * new uint2(3404170631u, 2048213449u) + asuint(v.c1) * new uint2(4164671783u, 1780759499u) + asuint(v.c2) * new uint2(1352369353u, 2446407751u)) + 1391928079;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(int2x4 v)
        {
            return csum(asuint(v.c0) * new uint2(2057338067u, 2942577577u) + asuint(v.c1) * new uint2(2834440507u, 2671762487u) + asuint(v.c2) * new uint2(2892026051u, 2455987759u) + asuint(v.c3) * new uint2(3868600063u, 3170963179u)) + 2632835537u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(int3x2 v)
        {
            return csum(asuint(v.c0) * new uint3(3678265601u, 2070747979u, 1480171127u) + asuint(v.c1) * new uint3(1588341193u, 4234155257u, 1811310911u)) + 2635799963u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(int3x3 v)
        {
            return csum(asuint(v.c0) * new uint3(2479033387u, 3702457169u, 1845824257u) + asuint(v.c1) * new uint3(1963973621u, 2134758553u, 1391111867u) + asuint(v.c2) * new uint3(1167706003u, 2209736489u, 3261535807u)) + 1740411209;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(int3x4 v)
        {
            return csum(asuint(v.c0) * new uint3(1521739981u, 1735296007u, 3010324327u) + asuint(v.c1) * new uint3(1875523709u, 2937008387u, 3835713223u) + asuint(v.c2) * new uint3(2216526373u, 3375971453u, 3559829411u) + asuint(v.c3) * new uint3(3652178029u, 2544260129u, 2013864031u)) + 2627668003u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(int4x2 v)
        {
            return csum(asuint(v.c0) * new uint4(4205774813u, 1650214333u, 3388112843u, 1831150513u) + asuint(v.c1) * new uint4(1848374953u, 3430200247u, 2209710647u, 2201894441u)) + 2849577407u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(int4x3 v)
        {
            return csum(asuint(v.c0) * new uint4(1773538433u, 3773525029u, 4131962539u, 1809525511u) + asuint(v.c1) * new uint4(4016293529u, 2416021567u, 2828384717u, 2636362241u) + asuint(v.c2) * new uint4(1258410977u, 1952565773u, 2037535609u, 3592785499u)) + 3996716183u;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint hash(int4x4 v)
        {
            return csum(asuint(v.c0) * new uint4(1562056283u, 2265541847u, 1283419601u, 1210229737u) + asuint(v.c1) * new uint4(2864955997u, 3525118277u, 2298260269u, 1632478733u) + asuint(v.c2) * new uint4(1537393931u, 2353355467u, 3441847433u, 4052036147u) + asuint(v.c3) * new uint4(2011389559u, 2252224297u, 3784421429u, 1750626223u)) + 3571447507u;
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

        #region abs

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int abs(int x)
        {
            return max(-x, x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 abs(int2 x)
        {
            return max(-x, x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 abs(int3 x)
        {
            return max(-x, x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 abs(int4 x)
        {
            return max(-x, x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long abs(long x)
        {
            return max(-x, x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float abs(float x)
        {
            return asfloat(asuint(x) & int.MaxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 abs(float2 x)
        {
            return asfloat(asuint(x) & 2147483647u);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 abs(float3 x)
        {
            return asfloat(asuint(x) & 2147483647u);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 abs(float4 x)
        {
            return asfloat(asuint(x) & 2147483647u);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double abs(double x)
        {
            return asdouble(asulong(x) & long.MaxValue);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 abs(double2 x)
        {
            return new double2(asdouble(asulong(x.x) & long.MaxValue), asdouble(asulong(x.y) & long.MaxValue));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 abs(double3 x)
        {
            return new double3(asdouble(asulong(x.x) & long.MaxValue), asdouble(asulong(x.y) & long.MaxValue), asdouble(asulong(x.z) & long.MaxValue));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 abs(double4 x)
        {
            return new double4(asdouble(asulong(x.x) & long.MaxValue), asdouble(asulong(x.y) & long.MaxValue), asdouble(asulong(x.z) & long.MaxValue), asdouble(asulong(x.w) & long.MaxValue));
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

        #region asfloat

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float asfloat(int x)
        {
            IntFloatUnion intFloatUnion = default(IntFloatUnion);
            intFloatUnion.floatValue = 0f;
            intFloatUnion.intValue = x;
            return intFloatUnion.floatValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 asfloat(int2 x)
        {
            return new float2(asfloat(x.x), asfloat(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 asfloat(int3 x)
        {
            return new float3(asfloat(x.x), asfloat(x.y), asfloat(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 asfloat(int4 x)
        {
            return new float4(asfloat(x.x), asfloat(x.y), asfloat(x.z), asfloat(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float asfloat(uint x)
        {
            return asfloat((int)x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 asfloat(uint2 x)
        {
            return new float2(asfloat(x.x), asfloat(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 asfloat(uint3 x)
        {
            return new float3(asfloat(x.x), asfloat(x.y), asfloat(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 asfloat(uint4 x)
        {
            return new float4(asfloat(x.x), asfloat(x.y), asfloat(x.z), asfloat(x.w));
        }

        #endregion

        #region asdouble

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double asdouble(long x)
        {
            LongDoubleUnion longDoubleUnion = default(LongDoubleUnion);
            longDoubleUnion.doubleValue = 0.0;
            longDoubleUnion.longValue = x;
            return longDoubleUnion.doubleValue;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double asdouble(ulong x)
        {
            return asdouble((long)x);
        }

        #endregion

        #region aslong

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long aslong(ulong x)
        {
            return (long)x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static long aslong(double x)
        {
            LongDoubleUnion longDoubleUnion = default(LongDoubleUnion);
            longDoubleUnion.longValue = 0L;
            longDoubleUnion.doubleValue = x;
            return longDoubleUnion.longValue;
        }

        #endregion

        #region asulong

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong asulong(long x)
        {
            return (ulong)x;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong asulong(double x)
        {
            return (ulong)aslong(x);
        }

        #endregion

        #region is

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isfinite(float x)
        {
            return abs(x) < float.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 isfinite(float2 x)
        {
            return abs(x) < float.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 isfinite(float3 x)
        {
            return abs(x) < float.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 isfinite(float4 x)
        {
            return abs(x) < float.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isfinite(double x)
        {
            return abs(x) < double.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 isfinite(double2 x)
        {
            return abs(x) < double.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 isfinite(double3 x)
        {
            return abs(x) < double.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 isfinite(double4 x)
        {
            return abs(x) < double.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isinf(float x)
        {
            return abs(x) == float.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 isinf(float2 x)
        {
            return abs(x) == float.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 isinf(float3 x)
        {
            return abs(x) == float.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 isinf(float4 x)
        {
            return abs(x) == float.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isinf(double x)
        {
            return abs(x) == double.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 isinf(double2 x)
        {
            return abs(x) == double.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 isinf(double3 x)
        {
            return abs(x) == double.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 isinf(double4 x)
        {
            return abs(x) == double.PositiveInfinity;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isnan(float x)
        {
            return (asuint(x) & int.MaxValue) > 2139095040;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 isnan(float2 x)
        {
            return (asuint(x) & 2147483647u) > 2139095040u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 isnan(float3 x)
        {
            return (asuint(x) & 2147483647u) > 2139095040u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 isnan(float4 x)
        {
            return (asuint(x) & 2147483647u) > 2139095040u;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool isnan(double x)
        {
            return (asulong(x) & long.MaxValue) > 9218868437227405312L;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 isnan(double2 x)
        {
            return new bool2((asulong(x.x) & long.MaxValue) > 9218868437227405312L, (asulong(x.y) & long.MaxValue) > 9218868437227405312L);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 isnan(double3 x)
        {
            return new bool3((asulong(x.x) & long.MaxValue) > 9218868437227405312L, (asulong(x.y) & long.MaxValue) > 9218868437227405312L, (asulong(x.z) & long.MaxValue) > 9218868437227405312L);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 isnan(double4 x)
        {
            return new bool4((asulong(x.x) & long.MaxValue) > 9218868437227405312L, (asulong(x.y) & long.MaxValue) > 9218868437227405312L, (asulong(x.z) & long.MaxValue) > 9218868437227405312L, (asulong(x.w) & long.MaxValue) > 9218868437227405312L);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ispow2(int x)
        {
            if (x > 0)
            {
                return (x & (x - 1)) == 0;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 ispow2(int2 x)
        {
            return new bool2(ispow2(x.x), ispow2(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 ispow2(int3 x)
        {
            return new bool3(ispow2(x.x), ispow2(x.y), ispow2(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 ispow2(int4 x)
        {
            return new bool4(ispow2(x.x), ispow2(x.y), ispow2(x.z), ispow2(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool ispow2(uint x)
        {
            if (x != 0)
            {
                return (x & (x - 1)) == 0;
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 ispow2(uint2 x)
        {
            return new bool2(ispow2(x.x), ispow2(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3 ispow2(uint3 x)
        {
            return new bool3(ispow2(x.x), ispow2(x.y), ispow2(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4 ispow2(uint4 x)
        {
            return new bool4(ispow2(x.x), ispow2(x.y), ispow2(x.z), ispow2(x.w));
        }

        #endregion

        #region mul

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float mul(float a, float b)
        {
            return a * b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float mul(float2 a, float2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 mul(float2 a, float2x2 b)
        {
            return new float2(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 mul(float2 a, float2x3 b)
        {
            return new float3(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y, a.x * b.c2.x + a.y * b.c2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 mul(float2 a, float2x4 b)
        {
            return new float4(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y, a.x * b.c2.x + a.y * b.c2.y, a.x * b.c3.x + a.y * b.c3.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float mul(float3 a, float3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 mul(float3 a, float3x2 b)
        {
            return new float2(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 mul(float3 a, float3x3 b)
        {
            return new float3(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 mul(float3 a, float3x4 b)
        {
            return new float4(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z, a.x * b.c3.x + a.y * b.c3.y + a.z * b.c3.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float mul(float4 a, float4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 mul(float4 a, float4x2 b)
        {
            return new float2(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 mul(float4 a, float4x3 b)
        {
            return new float3(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z + a.w * b.c2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 mul(float4 a, float4x4 b)
        {
            return new float4(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z + a.w * b.c2.w, a.x * b.c3.x + a.y * b.c3.y + a.z * b.c3.z + a.w * b.c3.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 mul(float2x2 a, float2 b)
        {
            return a.c0 * b.x + a.c1 * b.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 mul(float2x2 a, float2x2 b)
        {
            return new float2x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 mul(float2x2 a, float2x3 b)
        {
            return new float2x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x4 mul(float2x2 a, float2x4 b)
        {
            return new float2x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 mul(float2x3 a, float3 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 mul(float2x3 a, float3x2 b)
        {
            return new float2x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 mul(float2x3 a, float3x3 b)
        {
            return new float2x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x4 mul(float2x3 a, float3x4 b)
        {
            return new float2x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 mul(float2x4 a, float4 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x2 mul(float2x4 a, float4x2 b)
        {
            return new float2x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x3 mul(float2x4 a, float4x3 b)
        {
            return new float2x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2x4 mul(float2x4 a, float4x4 b)
        {
            return new float2x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 mul(float3x2 a, float2 b)
        {
            return a.c0 * b.x + a.c1 * b.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x2 mul(float3x2 a, float2x2 b)
        {
            return new float3x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 mul(float3x2 a, float2x3 b)
        {
            return new float3x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 mul(float3x2 a, float2x4 b)
        {
            return new float3x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 mul(float3x3 a, float3 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x2 mul(float3x3 a, float3x2 b)
        {
            return new float3x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 mul(float3x3 a, float3x3 b)
        {
            return new float3x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 mul(float3x3 a, float3x4 b)
        {
            return new float3x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 mul(float3x4 a, float4 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x2 mul(float3x4 a, float4x2 b)
        {
            return new float3x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x3 mul(float3x4 a, float4x3 b)
        {
            return new float3x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3x4 mul(float3x4 a, float4x4 b)
        {
            return new float3x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 mul(float4x2 a, float2 b)
        {
            return a.c0 * b.x + a.c1 * b.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x2 mul(float4x2 a, float2x2 b)
        {
            return new float4x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x3 mul(float4x2 a, float2x3 b)
        {
            return new float4x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 mul(float4x2 a, float2x4 b)
        {
            return new float4x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 mul(float4x3 a, float3 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x2 mul(float4x3 a, float3x2 b)
        {
            return new float4x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x3 mul(float4x3 a, float3x3 b)
        {
            return new float4x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 mul(float4x3 a, float3x4 b)
        {
            return new float4x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 mul(float4x4 a, float4 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x2 mul(float4x4 a, float4x2 b)
        {
            return new float4x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x3 mul(float4x4 a, float4x3 b)
        {
            return new float4x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 mul(float4x4 a, float4x4 b)
        {
            return new float4x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double mul(double a, double b)
        {
            return a * b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double mul(double2 a, double2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 mul(double2 a, double2x2 b)
        {
            return new double2(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 mul(double2 a, double2x3 b)
        {
            return new double3(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y, a.x * b.c2.x + a.y * b.c2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 mul(double2 a, double2x4 b)
        {
            return new double4(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y, a.x * b.c2.x + a.y * b.c2.y, a.x * b.c3.x + a.y * b.c3.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double mul(double3 a, double3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 mul(double3 a, double3x2 b)
        {
            return new double2(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 mul(double3 a, double3x3 b)
        {
            return new double3(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 mul(double3 a, double3x4 b)
        {
            return new double4(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z, a.x * b.c3.x + a.y * b.c3.y + a.z * b.c3.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double mul(double4 a, double4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 mul(double4 a, double4x2 b)
        {
            return new double2(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 mul(double4 a, double4x3 b)
        {
            return new double3(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z + a.w * b.c2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 mul(double4 a, double4x4 b)
        {
            return new double4(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z + a.w * b.c2.w, a.x * b.c3.x + a.y * b.c3.y + a.z * b.c3.z + a.w * b.c3.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 mul(double2x2 a, double2 b)
        {
            return a.c0 * b.x + a.c1 * b.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 mul(double2x2 a, double2x2 b)
        {
            return new double2x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 mul(double2x2 a, double2x3 b)
        {
            return new double2x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x4 mul(double2x2 a, double2x4 b)
        {
            return new double2x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 mul(double2x3 a, double3 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 mul(double2x3 a, double3x2 b)
        {
            return new double2x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 mul(double2x3 a, double3x3 b)
        {
            return new double2x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x4 mul(double2x3 a, double3x4 b)
        {
            return new double2x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 mul(double2x4 a, double4 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 mul(double2x4 a, double4x2 b)
        {
            return new double2x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x3 mul(double2x4 a, double4x3 b)
        {
            return new double2x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x4 mul(double2x4 a, double4x4 b)
        {
            return new double2x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 mul(double3x2 a, double2 b)
        {
            return a.c0 * b.x + a.c1 * b.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3x2 mul(double3x2 a, double2x2 b)
        {
            return new double3x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3x3 mul(double3x2 a, double2x3 b)
        {
            return new double3x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3x4 mul(double3x2 a, double2x4 b)
        {
            return new double3x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 mul(double3x3 a, double3 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3x2 mul(double3x3 a, double3x2 b)
        {
            return new double3x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3x3 mul(double3x3 a, double3x3 b)
        {
            return new double3x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3x4 mul(double3x3 a, double3x4 b)
        {
            return new double3x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 mul(double3x4 a, double4 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3x2 mul(double3x4 a, double4x2 b)
        {
            return new double3x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3x3 mul(double3x4 a, double4x3 b)
        {
            return new double3x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3x4 mul(double3x4 a, double4x4 b)
        {
            return new double3x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 mul(double4x2 a, double2 b)
        {
            return a.c0 * b.x + a.c1 * b.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4x2 mul(double4x2 a, double2x2 b)
        {
            return new double4x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4x3 mul(double4x2 a, double2x3 b)
        {
            return new double4x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4x4 mul(double4x2 a, double2x4 b)
        {
            return new double4x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 mul(double4x3 a, double3 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4x2 mul(double4x3 a, double3x2 b)
        {
            return new double4x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4x3 mul(double4x3 a, double3x3 b)
        {
            return new double4x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4x4 mul(double4x3 a, double3x4 b)
        {
            return new double4x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 mul(double4x4 a, double4 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4x2 mul(double4x4 a, double4x2 b)
        {
            return new double4x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4x3 mul(double4x4 a, double4x3 b)
        {
            return new double4x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4x4 mul(double4x4 a, double4x4 b)
        {
            return new double4x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int mul(int a, int b)
        {
            return a * b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int mul(int2 a, int2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 mul(int2 a, int2x2 b)
        {
            return new int2(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 mul(int2 a, int2x3 b)
        {
            return new int3(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y, a.x * b.c2.x + a.y * b.c2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 mul(int2 a, int2x4 b)
        {
            return new int4(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y, a.x * b.c2.x + a.y * b.c2.y, a.x * b.c3.x + a.y * b.c3.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int mul(int3 a, int3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 mul(int3 a, int3x2 b)
        {
            return new int2(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 mul(int3 a, int3x3 b)
        {
            return new int3(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 mul(int3 a, int3x4 b)
        {
            return new int4(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z, a.x * b.c3.x + a.y * b.c3.y + a.z * b.c3.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int mul(int4 a, int4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 mul(int4 a, int4x2 b)
        {
            return new int2(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 mul(int4 a, int4x3 b)
        {
            return new int3(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z + a.w * b.c2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 mul(int4 a, int4x4 b)
        {
            return new int4(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z + a.w * b.c2.w, a.x * b.c3.x + a.y * b.c3.y + a.z * b.c3.z + a.w * b.c3.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 mul(int2x2 a, int2 b)
        {
            return a.c0 * b.x + a.c1 * b.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2x2 mul(int2x2 a, int2x2 b)
        {
            return new int2x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2x3 mul(int2x2 a, int2x3 b)
        {
            return new int2x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2x4 mul(int2x2 a, int2x4 b)
        {
            return new int2x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 mul(int2x3 a, int3 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2x2 mul(int2x3 a, int3x2 b)
        {
            return new int2x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2x3 mul(int2x3 a, int3x3 b)
        {
            return new int2x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2x4 mul(int2x3 a, int3x4 b)
        {
            return new int2x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 mul(int2x4 a, int4 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2x2 mul(int2x4 a, int4x2 b)
        {
            return new int2x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2x3 mul(int2x4 a, int4x3 b)
        {
            return new int2x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2x4 mul(int2x4 a, int4x4 b)
        {
            return new int2x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 mul(int3x2 a, int2 b)
        {
            return a.c0 * b.x + a.c1 * b.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3x2 mul(int3x2 a, int2x2 b)
        {
            return new int3x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3x3 mul(int3x2 a, int2x3 b)
        {
            return new int3x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3x4 mul(int3x2 a, int2x4 b)
        {
            return new int3x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 mul(int3x3 a, int3 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3x2 mul(int3x3 a, int3x2 b)
        {
            return new int3x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3x3 mul(int3x3 a, int3x3 b)
        {
            return new int3x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3x4 mul(int3x3 a, int3x4 b)
        {
            return new int3x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 mul(int3x4 a, int4 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3x2 mul(int3x4 a, int4x2 b)
        {
            return new int3x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3x3 mul(int3x4 a, int4x3 b)
        {
            return new int3x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3x4 mul(int3x4 a, int4x4 b)
        {
            return new int3x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 mul(int4x2 a, int2 b)
        {
            return a.c0 * b.x + a.c1 * b.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4x2 mul(int4x2 a, int2x2 b)
        {
            return new int4x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4x3 mul(int4x2 a, int2x3 b)
        {
            return new int4x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4x4 mul(int4x2 a, int2x4 b)
        {
            return new int4x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 mul(int4x3 a, int3 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4x2 mul(int4x3 a, int3x2 b)
        {
            return new int4x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4x3 mul(int4x3 a, int3x3 b)
        {
            return new int4x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4x4 mul(int4x3 a, int3x4 b)
        {
            return new int4x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 mul(int4x4 a, int4 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4x2 mul(int4x4 a, int4x2 b)
        {
            return new int4x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4x3 mul(int4x4 a, int4x3 b)
        {
            return new int4x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4x4 mul(int4x4 a, int4x4 b)
        {
            return new int4x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint mul(uint a, uint b)
        {
            return a * b;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint mul(uint2 a, uint2 b)
        {
            return a.x * b.x + a.y * b.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2 mul(uint2 a, uint2x2 b)
        {
            return new uint2(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3 mul(uint2 a, uint2x3 b)
        {
            return new uint3(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y, a.x * b.c2.x + a.y * b.c2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 mul(uint2 a, uint2x4 b)
        {
            return new uint4(a.x * b.c0.x + a.y * b.c0.y, a.x * b.c1.x + a.y * b.c1.y, a.x * b.c2.x + a.y * b.c2.y, a.x * b.c3.x + a.y * b.c3.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint mul(uint3 a, uint3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2 mul(uint3 a, uint3x2 b)
        {
            return new uint2(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3 mul(uint3 a, uint3x3 b)
        {
            return new uint3(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 mul(uint3 a, uint3x4 b)
        {
            return new uint4(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z, a.x * b.c3.x + a.y * b.c3.y + a.z * b.c3.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint mul(uint4 a, uint4 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z + a.w * b.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2 mul(uint4 a, uint4x2 b)
        {
            return new uint2(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3 mul(uint4 a, uint4x3 b)
        {
            return new uint3(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z + a.w * b.c2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 mul(uint4 a, uint4x4 b)
        {
            return new uint4(a.x * b.c0.x + a.y * b.c0.y + a.z * b.c0.z + a.w * b.c0.w, a.x * b.c1.x + a.y * b.c1.y + a.z * b.c1.z + a.w * b.c1.w, a.x * b.c2.x + a.y * b.c2.y + a.z * b.c2.z + a.w * b.c2.w, a.x * b.c3.x + a.y * b.c3.y + a.z * b.c3.z + a.w * b.c3.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2 mul(uint2x2 a, uint2 b)
        {
            return a.c0 * b.x + a.c1 * b.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2x2 mul(uint2x2 a, uint2x2 b)
        {
            return new uint2x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2x3 mul(uint2x2 a, uint2x3 b)
        {
            return new uint2x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2x4 mul(uint2x2 a, uint2x4 b)
        {
            return new uint2x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2 mul(uint2x3 a, uint3 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2x2 mul(uint2x3 a, uint3x2 b)
        {
            return new uint2x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2x3 mul(uint2x3 a, uint3x3 b)
        {
            return new uint2x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2x4 mul(uint2x3 a, uint3x4 b)
        {
            return new uint2x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2 mul(uint2x4 a, uint4 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2x2 mul(uint2x4 a, uint4x2 b)
        {
            return new uint2x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2x3 mul(uint2x4 a, uint4x3 b)
        {
            return new uint2x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint2x4 mul(uint2x4 a, uint4x4 b)
        {
            return new uint2x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3 mul(uint3x2 a, uint2 b)
        {
            return a.c0 * b.x + a.c1 * b.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3x2 mul(uint3x2 a, uint2x2 b)
        {
            return new uint3x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3x3 mul(uint3x2 a, uint2x3 b)
        {
            return new uint3x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3x4 mul(uint3x2 a, uint2x4 b)
        {
            return new uint3x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3 mul(uint3x3 a, uint3 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3x2 mul(uint3x3 a, uint3x2 b)
        {
            return new uint3x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3x3 mul(uint3x3 a, uint3x3 b)
        {
            return new uint3x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3x4 mul(uint3x3 a, uint3x4 b)
        {
            return new uint3x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3 mul(uint3x4 a, uint4 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3x2 mul(uint3x4 a, uint4x2 b)
        {
            return new uint3x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3x3 mul(uint3x4 a, uint4x3 b)
        {
            return new uint3x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint3x4 mul(uint3x4 a, uint4x4 b)
        {
            return new uint3x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 mul(uint4x2 a, uint2 b)
        {
            return a.c0 * b.x + a.c1 * b.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4x2 mul(uint4x2 a, uint2x2 b)
        {
            return new uint4x2(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4x3 mul(uint4x2 a, uint2x3 b)
        {
            return new uint4x3(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4x4 mul(uint4x2 a, uint2x4 b)
        {
            return new uint4x4(a.c0 * b.c0.x + a.c1 * b.c0.y, a.c0 * b.c1.x + a.c1 * b.c1.y, a.c0 * b.c2.x + a.c1 * b.c2.y, a.c0 * b.c3.x + a.c1 * b.c3.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 mul(uint4x3 a, uint3 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4x2 mul(uint4x3 a, uint3x2 b)
        {
            return new uint4x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4x3 mul(uint4x3 a, uint3x3 b)
        {
            return new uint4x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4x4 mul(uint4x3 a, uint3x4 b)
        {
            return new uint4x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4 mul(uint4x4 a, uint4 b)
        {
            return a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3 * b.w;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4x2 mul(uint4x4 a, uint4x2 b)
        {
            return new uint4x2(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4x3 mul(uint4x4 a, uint4x3 b)
        {
            return new uint4x3(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint4x4 mul(uint4x4 a, uint4x4 b)
        {
            return new uint4x4(a.c0 * b.c0.x + a.c1 * b.c0.y + a.c2 * b.c0.z + a.c3 * b.c0.w, a.c0 * b.c1.x + a.c1 * b.c1.y + a.c2 * b.c1.z + a.c3 * b.c1.w, a.c0 * b.c2.x + a.c1 * b.c2.y + a.c2 * b.c2.z + a.c3 * b.c2.w, a.c0 * b.c3.x + a.c1 * b.c3.y + a.c2 * b.c3.z + a.c3 * b.c3.w);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion mul(quaternion a, quaternion b)
        {
            return new quaternion(a.value.wwww * b.value + (a.value.xyzx * b.value.wwwx + a.value.yzxy * b.value.zxyy) * new float4(1f, 1f, 1f, -1f) - a.value.zxyz * b.value.yzxz);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 mul(quaternion q, float3 v)
        {
            float3 @float = 2f * cross(q.value.xyz, v);
            return v + q.value.w * @float + cross(q.value.xyz, @float);
        }

        #endregion

        #region tan

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float tan(float x)
        {
            return (float)System.Math.Tan(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 tan(float2 x)
        {
            return new float2(tan(x.x), tan(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 tan(float3 x)
        {
            return new float3(tan(x.x), tan(x.y), tan(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 tan(float4 x)
        {
            return new float4(tan(x.x), tan(x.y), tan(x.z), tan(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double tan(double x)
        {
            return System.Math.Tan(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 tan(double2 x)
        {
            return new double2(tan(x.x), tan(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 tan(double3 x)
        {
            return new double3(tan(x.x), tan(x.y), tan(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 tan(double4 x)
        {
            return new double4(tan(x.x), tan(x.y), tan(x.z), tan(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float tanh(float x)
        {
            return (float)System.Math.Tanh(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 tanh(float2 x)
        {
            return new float2(tanh(x.x), tanh(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 tanh(float3 x)
        {
            return new float3(tanh(x.x), tanh(x.y), tanh(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 tanh(float4 x)
        {
            return new float4(tanh(x.x), tanh(x.y), tanh(x.z), tanh(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double tanh(double x)
        {
            return System.Math.Tanh(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 tanh(double2 x)
        {
            return new double2(tanh(x.x), tanh(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 tanh(double3 x)
        {
            return new double3(tanh(x.x), tanh(x.y), tanh(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 tanh(double4 x)
        {
            return new double4(tanh(x.x), tanh(x.y), tanh(x.z), tanh(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float atan(float x)
        {
            return (float)System.Math.Atan(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 atan(float2 x)
        {
            return new float2(atan(x.x), atan(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 atan(float3 x)
        {
            return new float3(atan(x.x), atan(x.y), atan(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 atan(float4 x)
        {
            return new float4(atan(x.x), atan(x.y), atan(x.z), atan(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double atan(double x)
        {
            return System.Math.Atan(x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 atan(double2 x)
        {
            return new double2(atan(x.x), atan(x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 atan(double3 x)
        {
            return new double3(atan(x.x), atan(x.y), atan(x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 atan(double4 x)
        {
            return new double4(atan(x.x), atan(x.y), atan(x.z), atan(x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float atan2(float y, float x)
        {
            return (float)System.Math.Atan2(y, x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 atan2(float2 y, float2 x)
        {
            return new float2(atan2(y.x, x.x), atan2(y.y, x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 atan2(float3 y, float3 x)
        {
            return new float3(atan2(y.x, x.x), atan2(y.y, x.y), atan2(y.z, x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 atan2(float4 y, float4 x)
        {
            return new float4(atan2(y.x, x.x), atan2(y.y, x.y), atan2(y.z, x.z), atan2(y.w, x.w));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double atan2(double y, double x)
        {
            return System.Math.Atan2(y, x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 atan2(double2 y, double2 x)
        {
            return new double2(atan2(y.x, x.x), atan2(y.y, x.y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 atan2(double3 y, double3 x)
        {
            return new double3(atan2(y.x, x.x), atan2(y.y, x.y), atan2(y.z, x.z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 atan2(double4 y, double4 x)
        {
            return new double4(atan2(y.x, x.x), atan2(y.y, x.y), atan2(y.z, x.z), atan2(y.w, x.w));
        }

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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion normalize(quaternion q)
        {
            float4 value = q.value;
            return new quaternion(rsqrt(dot(value, value)) * value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion normalizesafe(quaternion q)
        {
            float4 value = q.value;
            float num = dot(value, value);
            return new quaternion(select(quaternion.identity.value, value * rsqrt(num), num > 1.17549435E-38f));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static quaternion normalizesafe(quaternion q, quaternion defaultvalue)
        {
            float4 value = q.value;
            float num = dot(value, value);
            return new quaternion(select(defaultvalue.value, value * rsqrt(num), num > 1.17549435E-38f));
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
        public enum ShuffleComponent : byte
        {
            LeftX,
            LeftY,
            LeftZ,
            LeftW,
            RightX,
            RightY,
            RightZ,
            RightW
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float select_shuffle_component(float4 a, float4 b, ShuffleComponent component)
        {
            switch (component)
            {
                case ShuffleComponent.LeftX:
                    return a.x;
                case ShuffleComponent.LeftY:
                    return a.y;
                case ShuffleComponent.LeftZ:
                    return a.z;
                case ShuffleComponent.LeftW:
                    return a.w;
                case ShuffleComponent.RightX:
                    return b.x;
                case ShuffleComponent.RightY:
                    return b.y;
                case ShuffleComponent.RightZ:
                    return b.z;
                case ShuffleComponent.RightW:
                    return b.w;
                default:
                    throw new ArgumentException("Invalid shuffle component: " + component);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float4 unpacklo(float4 a, float4 b)
        {
            return shuffle(a, b, ShuffleComponent.LeftX, ShuffleComponent.RightX, ShuffleComponent.LeftY, ShuffleComponent.RightY);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float4 unpackhi(float4 a, float4 b)
        {
            return shuffle(a, b, ShuffleComponent.LeftZ, ShuffleComponent.RightZ, ShuffleComponent.LeftW, ShuffleComponent.RightW);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float4 movelh(float4 a, float4 b)
        {
            return shuffle(a, b, ShuffleComponent.LeftX, ShuffleComponent.LeftY, ShuffleComponent.RightX, ShuffleComponent.RightY);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float4 movehl(float4 a, float4 b)
        {
            return shuffle(b, a, ShuffleComponent.LeftZ, ShuffleComponent.LeftW, ShuffleComponent.RightZ, ShuffleComponent.RightW);
        }

        #endregion

        #region float math

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 up()
        {
            return new float3(0f, 1f, 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 down()
        {
            return new float3(0f, -1f, 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 forward()
        {
            return new float3(0f, 0f, 1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 back()
        {
            return new float3(0f, 0f, -1f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 left()
        {
            return new float3(-1f, 0f, 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 right()
        {
            return new float3(1f, 0f, 0f);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float shuffle(float4 left, float4 right, ShuffleComponent x)
        {
            return select_shuffle_component(left, right, x);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 shuffle(float4 left, float4 right, ShuffleComponent x, ShuffleComponent y)
        {
            return new float2(select_shuffle_component(left, right, x), select_shuffle_component(left, right, y));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 shuffle(float4 left, float4 right, ShuffleComponent x, ShuffleComponent y, ShuffleComponent z)
        {
            return new float3(select_shuffle_component(left, right, x), select_shuffle_component(left, right, y), select_shuffle_component(left, right, z));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 shuffle(float4 left, float4 right, ShuffleComponent x, ShuffleComponent y, ShuffleComponent z, ShuffleComponent w)
        {
            return new float4(select_shuffle_component(left, right, x), select_shuffle_component(left, right, y), select_shuffle_component(left, right, z), select_shuffle_component(left, right, w));
        }
        

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 rotate(float4x4 a, float3 b)
        {
            return (a.c0 * b.x + a.c1 * b.y + a.c2 * b.z).xyz;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 transform(float4x4 a, float3 b)
        {
            return (a.c0 * b.x + a.c1 * b.y + a.c2 * b.z + a.c3).xyz;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4x4 transpose(float4x4 v)
        {
            return new float4x4(v.c0.x, v.c0.y, v.c0.z, v.c0.w, v.c1.x, v.c1.y, v.c1.z, v.c1.w, v.c2.x, v.c2.y, v.c2.z, v.c2.w, v.c3.x, v.c3.y, v.c3.z, v.c3.w);
        }

        public static float4x4 inverse(float4x4 m)
        {
            float4 c = m.c0;
            float4 c2 = m.c1;
            float4 c3 = m.c2;
            float4 c4 = m.c3;
            float4 @float = movelh(c2, c);
            float4 float2 = movelh(c3, c4);
            float4 float3 = movehl(c, c2);
            float4 float4 = movehl(c4, c3);
            float4 lhs = shuffle(c2, c, ShuffleComponent.LeftY, ShuffleComponent.LeftZ, ShuffleComponent.RightY, ShuffleComponent.RightZ);
            float4 lhs2 = shuffle(c3, c4, ShuffleComponent.LeftY, ShuffleComponent.LeftZ, ShuffleComponent.RightY, ShuffleComponent.RightZ);
            float4 lhs3 = shuffle(c2, c, ShuffleComponent.LeftW, ShuffleComponent.LeftX, ShuffleComponent.RightW, ShuffleComponent.RightX);
            float4 lhs4 = shuffle(c3, c4, ShuffleComponent.LeftW, ShuffleComponent.LeftX, ShuffleComponent.RightW, ShuffleComponent.RightX);
            float4 lhs5 = shuffle(float2, @float, ShuffleComponent.LeftZ, ShuffleComponent.LeftX, ShuffleComponent.RightX, ShuffleComponent.RightZ);
            float4 lhs6 = shuffle(float2, @float, ShuffleComponent.LeftW, ShuffleComponent.LeftY, ShuffleComponent.RightY, ShuffleComponent.RightW);
            float4 lhs7 = shuffle(float4, float3, ShuffleComponent.LeftZ, ShuffleComponent.LeftX, ShuffleComponent.RightX, ShuffleComponent.RightZ);
            float4 lhs8 = shuffle(float4, float3, ShuffleComponent.LeftW, ShuffleComponent.LeftY, ShuffleComponent.RightY, ShuffleComponent.RightW);
            float4 lhs9 = shuffle(@float, float2, ShuffleComponent.LeftZ, ShuffleComponent.LeftX, ShuffleComponent.RightX, ShuffleComponent.RightZ);
            float4 float5 = lhs * float4 - lhs2 * float3;
            float4 float6 = @float * float4 - float2 * float3;
            float4 float7 = lhs4 * @float - lhs3 * float2;
            float4 rhs = shuffle(float5, float5, ShuffleComponent.LeftX, ShuffleComponent.LeftZ, ShuffleComponent.RightZ, ShuffleComponent.RightX);
            float4 rhs2 = shuffle(float5, float5, ShuffleComponent.LeftY, ShuffleComponent.LeftW, ShuffleComponent.RightW, ShuffleComponent.RightY);
            float4 rhs3 = shuffle(float6, float6, ShuffleComponent.LeftX, ShuffleComponent.LeftZ, ShuffleComponent.RightZ, ShuffleComponent.RightX);
            float4 rhs4 = shuffle(float6, float6, ShuffleComponent.LeftY, ShuffleComponent.LeftW, ShuffleComponent.RightW, ShuffleComponent.RightY);
            float4 float8 = lhs8 * rhs - lhs7 * rhs4 + lhs6 * rhs2;
            float4 float9 = lhs9 * float8;
            float9 += shuffle(float9, float9, ShuffleComponent.LeftY, ShuffleComponent.LeftX, ShuffleComponent.RightW, ShuffleComponent.RightZ);
            float9 -= shuffle(float9, float9, ShuffleComponent.LeftZ, ShuffleComponent.LeftZ, ShuffleComponent.RightX, ShuffleComponent.RightX);
            float4 rhs5 = new float4(1f) / float9;
            float4x4 result = default(float4x4);
            result.c0 = float8 * rhs5;
            float4 rhs6 = shuffle(float7, float7, ShuffleComponent.LeftX, ShuffleComponent.LeftZ, ShuffleComponent.RightZ, ShuffleComponent.RightX);
            float4 rhs7 = shuffle(float7, float7, ShuffleComponent.LeftY, ShuffleComponent.LeftW, ShuffleComponent.RightW, ShuffleComponent.RightY);
            float4 lhs10 = lhs7 * rhs6 - lhs5 * rhs2 - lhs8 * rhs3;
            result.c1 = lhs10 * rhs5;
            float4 lhs11 = lhs5 * rhs4 - lhs6 * rhs6 - lhs8 * rhs7;
            result.c2 = lhs11 * rhs5;
            float4 lhs12 = lhs6 * rhs3 - lhs5 * rhs + lhs7 * rhs7;
            result.c3 = lhs12 * rhs5;
            return result;
        }

        public static float4x4 fastinverse(float4x4 m)
        {
            float4 c = m.c0;
            float4 c2 = m.c1;
            float4 c3 = m.c2;
            float4 c4 = m.c3;
            float4 b = new float4(0);
            float4 a = unpacklo(c, c3);
            float4 b2 = unpacklo(c2, b);
            float4 a2 = unpackhi(c, c3);
            float4 b3 = unpackhi(c2, b);
            float4 @float = unpacklo(a, b2);
            float4 float2 = unpackhi(a, b2);
            float4 float3 = unpacklo(a2, b3);
            c4 = -(@float * c4.x + float2 * c4.y + float3 * c4.z);
            c4.w = 1f;
            return new float4x4(@float, float2, float3, c4);
        }

        #endregion

        #region round

        /// <summary>Returns the result of rounding a float value to the nearest integral value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The round to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float round(float x) { return (float)System.Math.Round((float)x); }

        /// <summary>Returns the result of rounding each component of a float2 vector value to the nearest integral value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise round to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 round(float2 x) { return new float2(round(x.x), round(x.y)); }

        /// <summary>Returns the result of rounding each component of a float3 vector value to the nearest integral value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise round to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 round(float3 x) { return new float3(round(x.x), round(x.y), round(x.z)); }

        /// <summary>Returns the result of rounding each component of a float4 vector value to the nearest integral value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise round to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 round(float4 x) { return new float4(round(x.x), round(x.y), round(x.z), round(x.w)); }


        /// <summary>Returns the result of rounding a double value to the nearest integral value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The round to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double round(double x) { return System.Math.Round(x); }

        /// <summary>Returns the result of rounding each component of a double2 vector value to the nearest integral value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise round to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 round(double2 x) { return new double2(round(x.x), round(x.y)); }

        /// <summary>Returns the result of rounding each component of a double3 vector value to the nearest integral value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise round to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 round(double3 x) { return new double3(round(x.x), round(x.y), round(x.z)); }

        /// <summary>Returns the result of rounding each component of a double4 vector value to the nearest integral value.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise round to nearest integral value of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 round(double4 x) { return new double4(round(x.x), round(x.y), round(x.z), round(x.w)); }

        #endregion

        #region sign

        /// <summary>Returns the sign of a int value. -1 if it is less than zero, 0 if it is zero and 1 if it greater than zero.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The sign of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int sign(int x) { return (x > 0 ? 1 : 0) - (x < 0 ? 1 : 0); }

        /// <summary>Returns the componentwise sign of a int2 value. 1 for positive components, 0 for zero components and -1 for negative components.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise sign of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int2 sign(int2 x) { return new int2(sign(x.x), sign(x.y)); }

        /// <summary>Returns the componentwise sign of a int3 value. 1 for positive components, 0 for zero components and -1 for negative components.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise sign of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int3 sign(int3 x) { return new int3(sign(x.x), sign(x.y), sign(x.z)); }

        /// <summary>Returns the componentwise sign of a int4 value. 1 for positive components, 0 for zero components and -1 for negative components.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise sign of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int4 sign(int4 x) { return new int4(sign(x.x), sign(x.y), sign(x.z), sign(x.w)); }

        /// <summary>Returns the sign of a float value. -1.0f if it is less than zero, 0.0f if it is zero and 1.0f if it greater than zero.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The sign of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float sign(float x) { return (x > 0.0f ? 1.0f : 0.0f) - (x < 0.0f ? 1.0f : 0.0f); }

        /// <summary>Returns the componentwise sign of a float2 value. 1.0f for positive components, 0.0f for zero components and -1.0f for negative components.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise sign of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float2 sign(float2 x) { return new float2(sign(x.x), sign(x.y)); }

        /// <summary>Returns the componentwise sign of a float3 value. 1.0f for positive components, 0.0f for zero components and -1.0f for negative components.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise sign of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float3 sign(float3 x) { return new float3(sign(x.x), sign(x.y), sign(x.z)); }

        /// <summary>Returns the componentwise sign of a float4 value. 1.0f for positive components, 0.0f for zero components and -1.0f for negative components.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise sign of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float4 sign(float4 x) { return new float4(sign(x.x), sign(x.y), sign(x.z), sign(x.w)); }


        /// <summary>Returns the sign of a double value. -1.0 if it is less than zero, 0.0 if it is zero and 1.0 if it greater than zero.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The sign of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double sign(double x) { return x == 0 ? 0 : (x > 0.0 ? 1.0 : 0.0) - (x < 0.0 ? 1.0 : 0.0); }

        /// <summary>Returns the componentwise sign of a double2 value. 1.0 for positive components, 0.0 for zero components and -1.0 for negative components.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise sign of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2 sign(double2 x) { return new double2(sign(x.x), sign(x.y)); }

        /// <summary>Returns the componentwise sign of a double3 value. 1.0 for positive components, 0.0 for zero components and -1.0 for negative components.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise sign of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double3 sign(double3 x) { return new double3(sign(x.x), sign(x.y), sign(x.z)); }

        /// <summary>Returns the componentwise sign of a double4 value. 1.0 for positive components, 0.0 for zero components and -1.0 for negative components.</summary>
        /// <param name="x">Input value.</param>
        /// <returns>The componentwise sign of the input.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double4 sign(double4 x) { return new double4(sign(x.x), sign(x.y), sign(x.z), sign(x.w)); }

        #endregion

        #endregion
#endif
    }
}
