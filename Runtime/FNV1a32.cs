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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
using Unity.Collections;
#else
#define POINT_COLLECTIONS_NATIVE
#endif

namespace Point.Collections
{
    public static class FNV1a32
    {
#if UNITY_EDITOR || (UNITYENGINE && !UNITY_BURST)
        private const uint
            kPrime32 = 16777619,
            kOffsetBasis32 = 2166136261U;
        private const ulong
            kPrime64 = 1099511628211LU,
            kOffsetBasis64 = 14695981039346656037LU;
#endif

        public static uint Calculate(in string str)
        {
            uint hash;
            unsafe
            {
#if UNITYENGINE
#if UNITY_BURST
                Burst.BurstFNV1a.fnv1a32_str(in str, &hash);
#else
                if (str == null)
                {
                    hash = kOffsetBasis32;
                }

                byte[] bytes = System.Text.Encoding.Default.GetBytes(str);
                int length = bytes.Length;

                fixed (byte* buffer = bytes)
                {
                    hash = Calculate(buffer, length);
                }
#endif
#else
                Native.NativeFNV1a.fnv1a32_str(str, &hash);
#endif
            }
            return hash;
        }
#if UNITYENGINE && UNITY_BURST && UNITY_COLLECTIONS
        public static uint Calculate(in FixedString4096Bytes str)
        {
            uint hash;
            unsafe
            {
#if UNITYENGINE
                Burst.BurstFNV1a.fnv1a32_str(in str, &hash);
#else
                Native.NativeFNV1a.fnv1a32_str(str.ToString(), &hash);
#endif
            }
            return hash;
        }
#endif
        public static uint Calculate(in byte[] bytes)
        {
            int length = bytes.Length;
            uint hash;
            unsafe
            {
                fixed (byte* temp = bytes)
                {
#if POINT_COLLECTIONS_NATIVE
                    Native.NativeFNV1a.fnv1a32_byte(temp, &length, &hash);
#elif UNITYENGINE && UNITY_BURST
#if UNITY_EDITOR
                    if (!UnityEditorInternal.InternalEditorUtility.CurrentThreadIsMainThread() ||
                        !UnityEngine.Application.isPlaying)
                    {
                        if (bytes == null || bytes.Length == 0)
                        {
                            hash = kOffsetBasis32;
                        }
                        else
                        {
                            hash = kOffsetBasis32;

                            for (int i = 0; i < length; i++)
                            {
                                hash *= kPrime32;
                                hash ^= (uint)temp[i];
                            }
                        }
                        return hash;
                    }
#endif
                    Burst.BurstFNV1a.fnv1a32_byte(temp, &length, &hash);
#else
                    if (bytes == null || bytes.Length == 0)
                    {
                        hash = kOffsetBasis32;
                    }
                    else
                    {
                        hash = kOffsetBasis32;

                        for (int i = 0; i < length; i++)
                        {
                            hash *= kPrime32;
                            hash ^= (uint)temp[i];
                        }
                    }
#endif
                }
            }
            return hash;
        }
        public static unsafe uint Calculate(byte* bytes, int length)
        {
            uint hash;
#if UNITYENGINE
#if UNITY_BURST
            Burst.BurstFNV1a.fnv1a32_byte(bytes, &length, &hash);
#else
            if (bytes == null)
            {
                hash = kOffsetBasis32;
            }

            hash = kOffsetBasis32;

            for (int i = 0; i < length; i++)
            {
                hash *= kPrime32;
                hash ^= (uint)bytes[i];
            }
#endif
#else
            Native.NativeFNV1a.fnv1a32_byte(bytes, &length, &hash);
#endif
            return hash;
        }
    }
}
