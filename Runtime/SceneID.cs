﻿// Copyright 2022 Ikina Games
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

using System;

namespace Point.Collections
{
#if UNITYENGINE && UNITY_COLLECTIONS
    [BurstCompatible]
#endif
    public readonly struct SceneID : IEquatable<SceneID>, IEmpty
    {
        private readonly Hash m_Hash;

        public Hash Hash => m_Hash;

        internal SceneID(Hash hash)
        {
            m_Hash = hash;
        }

        public bool IsEmpty() => m_Hash.IsEmpty();

        public bool Equals(SceneID other) => m_Hash.Equals(other.m_Hash);
    }
}
