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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2020
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using Point.Collections.Buffer.LowLevel;
using Unity.Burst;

namespace Point.Collections.Burst
{
    [BurstCompile(CompileSynchronously = true)]
    internal static unsafe class BurstComplieHelper
    {
        [BurstCompile]
        public static void Compile()
        {
            UnsafeReference<int> temp;
            UnsafeReference temp1;
            UnsafeAllocator temp2;
            UnsafeAllocator<int> temp3;
        }
    }
}

#endif