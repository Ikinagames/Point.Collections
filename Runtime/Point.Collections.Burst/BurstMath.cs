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

using Unity.Burst;
using Unity.Mathematics;

namespace Point.Collections.Burst
{
#if !POINT_COLLECTIONS_NATIVE
    [BurstCompile]
#endif
    public static unsafe class BurstMath
    {
#if !POINT_COLLECTIONS_NATIVE
        public static void unity_todB(double* linear, double* output)
        {
            const double kMindB = -144;

            if (*linear == 0) *output = kMindB;
            else
            {
                *output = 20 * math.log10(*linear);
            }
        }
        public static void unity_fromdB(double* dB, double* output)
        {
            *output = math.pow(10, *dB / 20);
        }
#endif
    }
}
