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

#define POINT_COLLECTIONS_NATIVE

using System.Runtime.InteropServices;

namespace Point.Collections.Native
{
    public static unsafe class NativeMath
    {
#if POINT_COLLECTIONS_NATIVE
        [DllImport("Point.Collections.Native.Internal")]
        public static extern void unity_todB(double* linear, double* output);
        [DllImport("Point.Collections.Native.Internal")]
        public static extern void unity_fromdB(double* dB, double* output);

        [DllImport("Point.Collections.Native.Internal")]
        public static extern void min(in long x, in long y, long* output);

        [DllImport("Point.Collections.Native.Internal")]
        public static extern bool binaryComparer(void* x, void* y, in int length, bool* output);
#endif
    }
}
