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
    }
}
