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

using Unity.Collections;

namespace Point.Collections
{
    public static class FNV1a32
    {
        public static uint Calculate(in string str)
        {
            uint hash;
            unsafe
            {
#if POINT_COLLECTIONS_NATIVE
                Native.NativeFNV1a.fnv1a32_str(str, &hash);
#else
                Burst.BurstFNV1a.fnv1a32_str(in str, &hash);
#endif
            }
            return hash;
        }
        public static uint Calculate(in FixedString4096Bytes str)
        {
            uint hash;
            unsafe
            {
#if POINT_COLLECTIONS_NATIVE
                Native.NativeFNV1a.fnv1a32_str(str, &hash);
#else
                
                Burst.BurstFNV1a.fnv1a32_str(in str, &hash);
#endif
            }
            return hash;
        }
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
#else
                    Burst.BurstFNV1a.fnv1a32_byte(temp, &length, &hash);
#endif
                }
            }
            return hash;
        }
        public static unsafe uint Calculate(byte* bytes, int length)
        {
            uint hash;
#if POINT_COLLECTIONS_NATIVE
            Native.NativeFNV1a.fnv1a32_byte(temp, &length, &hash);
#else
            Burst.BurstFNV1a.fnv1a32_byte(bytes, &length, &hash);
#endif
            return hash;
        }
    }
}
