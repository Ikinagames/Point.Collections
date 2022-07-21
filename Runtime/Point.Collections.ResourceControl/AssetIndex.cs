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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#if UNITY_2019 && !UNITY_2020_1_OR_NEWER
#define UNITYENGINE_OLD
#else
#if UNITY_MATHEMATICS
using Unity.Mathematics;
#endif
#endif

using System;
using UnityEngine;

namespace Point.Collections.ResourceControl
{
    /// <summary>
    /// <see cref="ResourceHashMap"/> 내 리소스 좌표를 저장하는 구조체입니다.
    /// </summary>
    /// <remarks>
    /// 런타임에서는 효율을 위해 <seealso cref="AssetRuntimeKey"/> 가 사용될 수 있습니다. <seealso cref="AssetReference"/> 다음으로 메모리 효율이 낮습니다.
    /// </remarks>
    [Serializable]
    public struct AssetIndex : IEmpty, IValidation
    {
        public static AssetIndex Empty = default(AssetIndex);

        [SerializeField] internal int2 m_Index;
        [SerializeField] private bool m_IsCreated;

        public AssetReference AssetReference
        {
            get
            {
                ResourceHashMap.Instance.TryGetAssetReference(this, out var asset);
                return asset;
            }
        }

        public AssetIndex(int2 index)
        {
            m_Index = index;
            m_IsCreated = true;
        }
        public AssetIndex(int x, int y)
        {
            m_Index = new int2(x, y);
            m_IsCreated = true;
        }

        public bool IsEmpty() => !m_IsCreated;
        public bool IsValid() => ResourceHashMap.Instance.TryGetAssetReference(this, out _);

        public override string ToString()
        {
            string msg = string.Empty;
#if UNITY_EDITOR
            if (IsValid())
            {
                msg += AssetReference.RuntimeKey.ToString();
            }
#endif
            msg += $"({m_Index.x}, {m_Index.y})";
            return base.ToString();
        }
    }
}

#endif