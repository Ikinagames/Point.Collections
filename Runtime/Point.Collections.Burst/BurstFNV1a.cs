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

#if UNITY_2019_1_OR_NEWER && UNITY_BURST
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

using AOT;
using System.Text;
using Unity.Burst;
using Unity.Collections;

namespace Point.Collections.Burst
{
    [BurstCompile(CompileSynchronously = true)]
    public static unsafe class BurstFNV1a
    {
        private const uint
            kPrime32 = 16777619,
            kOffsetBasis32 = 2166136261U;
        private const ulong
            kPrime64 = 1099511628211LU,
            kOffsetBasis64 = 14695981039346656037LU;

        public delegate void BurstFNV1aDelegate(byte* buffer, int* length, uint* output);

        [BurstDiscard]
        public static void fnv1a32_str(in string str, uint* output)
        {
            if (str == null)
            {
                *output = kOffsetBasis32;
                return;
            }

            byte[] bytes = Encoding.Default.GetBytes(str);
            int length = bytes.Length;

            fixed (byte* buffer = bytes)
            {
                fnv1a32_byte(buffer, &length, output);
            }
        }
        [BurstCompile]
        [MonoPInvokeCallback(typeof(BurstFNV1aDelegate))]
        public static void fnv1a32_byte(byte* buffer, int* length, uint* output)
        {
            if (buffer == null)
            {
                *output = kOffsetBasis32;
                return;
            }

            uint hash = kOffsetBasis32;

            for (int i = 0; i < *length; i++)
            {
                hash *= kPrime32;
                hash ^= (uint)buffer[i];
            }

			*output = hash;
        }
#if UNITY_COLLECTIONS
        [BurstCompile]
        public static void fnv1a32_str(in FixedString4096Bytes str, uint* output)
        {
            if (str.Length == 0)
            {
                *output = kOffsetBasis32;
                return;
            }

            uint hash = kOffsetBasis32;

            for (int i = 0; i < str.Length; i++)
            {
                hash *= kPrime32;
                hash ^= (uint)str[i];
            }

			*output = hash;
        }
#endif

        [BurstDiscard]
        public static void fnv1a64_str(in string str, ulong* output)
        {
            if (str == null)
            {
                *output = kOffsetBasis64;
                return;
            }

            byte[] bytes = Encoding.Default.GetBytes(str);
            int length = bytes.Length;

            fixed (byte* buffer = bytes)
            {
                fnv1a64_byte(buffer, &length, output);
            }
        }
        [BurstCompile]
        public static void fnv1a64_byte(byte* buffer, int* length, ulong* output)
        {
            if (buffer == null)
            {
                *output = kOffsetBasis64;
                return;
            }

            ulong hash = kOffsetBasis64;

            for (int i = 0; i < *length; i++)
            {
                hash *= kPrime64;
                hash ^= (ulong)buffer[i];
            }

            *output = hash;
        }
    }
}

#endif