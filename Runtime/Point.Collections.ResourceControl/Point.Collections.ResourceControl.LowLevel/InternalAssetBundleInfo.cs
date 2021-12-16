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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Point.Collections.ResourceControl.LowLevel
{
    [BurstCompatible]
    internal struct InternalAssetBundleInfo : IValidation, IEquatable<InternalAssetBundleInfo>, IDisposable
    {
        public static InternalAssetBundleInfo Invalid => new InternalAssetBundleInfo(-1);

        public readonly int m_Index;
        public uint m_Generation;
        public bool m_Using;

        public FixedString4096Bytes m_Path;
        public bool m_IsWebRequest;

        public bool m_IsLoaded;
        public JobHandle m_JobHandle;

        [NativeDisableUnsafePtrRestriction]
        public UnsafeHashMap<Hash, InternalAssetInfo> m_Assets;

        public InternalAssetBundleInfo(int index)
        {
            m_Index = index;
            m_Generation = 0;
            m_Using = false;

            m_Path = string.Empty;
            m_IsWebRequest = false;

            m_IsLoaded = false;
            m_JobHandle = default;

            m_Assets = default;
        }

        public void Dispose()
        {
            if (m_Assets.IsCreated)
            {
                m_Assets.Dispose();
            }
        }
        public bool Equals(InternalAssetBundleInfo other) => m_Index.Equals(other.m_Index);

        public bool IsValid() => m_Index >= 0 && m_Using;
    }
}
