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

using UnityEngine;
using Unity.Collections.LowLevel.Unsafe;
using System;
using Unity.Burst;
using Point.Collections.Native;

namespace Point.Collections
{
    internal sealed class CollectionUtility : CLSSingleTone<CollectionUtility>
    {
        private Unity.Mathematics.Random m_Random;

        public static void Initialize()
        {
            CollectionUtility ins = Instance;

#if POINT_COLLECTIONS_NATIVE
            NativeDebug.Initialize();
#endif
            Instance.m_Random = new Unity.Mathematics.Random();
            Instance.m_Random.InitState();
        }

        public static int CreateHashCode() => Instance.m_Random.NextInt(int.MinValue, int.MaxValue);
    }
}
