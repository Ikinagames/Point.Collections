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

#if UNITY_2019_1_OR_NEWER && UNITY_ADDRESSABLES
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#if UNITY_2019 && !UNITY_2020_1_OR_NEWER
#define UNITYENGINE_OLD
#else
#if UNITY_MATHEMATICS
#endif
#endif

using System;
using Unity.Collections;
using UnityEngine;

namespace Point.Collections.ResourceControl
{
    [Serializable]
    public struct GroupReference : IEmpty
    {
        public static GroupReference Empty = default(GroupReference);

        [SerializeField] private FixedString128Bytes m_Name;

        public GroupReference(FixedString128Bytes name)
        {
            m_Name = name;
        }

        public bool IsEmpty() => m_Name.IsEmpty;
        public override string ToString() => m_Name.ToString();

        public static implicit operator string(GroupReference t) => t.m_Name.ToString();
    }
}

#endif