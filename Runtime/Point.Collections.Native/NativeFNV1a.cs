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

using System.Runtime.InteropServices;

namespace Point.Collections.Native
{
    public static class NativeFNV1a
    {
#if POINT_COLLECTIONS_NATIVE
        [DllImport("Point.Collections.Native.Internal")]
        public static unsafe extern void fnv1a32_str([MarshalAs(UnmanagedType.LPWStr)] string str, uint* output);
        [DllImport("Point.Collections.Native.Internal")]
        public static unsafe extern void fnv1a32_byte(byte* buffer, int* length, uint* output);

        [DllImport("Point.Collections.Native.Internal")]
        public static unsafe extern void fnv1a64_str([MarshalAs(UnmanagedType.LPWStr)] string str, ulong* output);
        [DllImport("Point.Collections.Native.Internal")]
        public static unsafe extern void fnv1a64_byte(byte* buffer, int* length, ulong* output);
#endif
    }
}
