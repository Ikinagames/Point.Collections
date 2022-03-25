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

#if UNITY_2020
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

using NUnit.Framework;
using UnityEngine;

namespace Point.Collections.Tests
{
    public sealed class MathTests
    {
        [Test]
        public void TodBTest()
        {
            float linear = .4f;

            float dB = Math.TodB(linear);

            Debug.Log($"linear:{linear} :: dB:{dB}");
            Debug.Log($"linear:0 :: dB:{Math.TodB(0)}");
            Debug.Log($"linear:1 :: dB:{Math.TodB(1)}");
        }
        [Test]
        public void FromdBTest()
        {
            float dB = -5;

            float linear = Math.FromdB(dB);

            Debug.Log($"linear:{linear} :: dB:{dB}");
        }
    }
}

#endif