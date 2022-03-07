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

using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Point.Collections.ResourceControl.LowLevel
{
    [BurstCompatible]
    [Guid("14ad9ef9-3c7c-4e60-96a3-2f3d602f9846")]
    internal struct UnsafeAssetBundleInfo : IValidation, IEquatable<UnsafeAssetBundleInfo>, IDisposable
    {
        public static UnsafeAssetBundleInfo Invalid => new UnsafeAssetBundleInfo(-1);

        public readonly int index;
        public uint m_Generation;
        public bool m_Using;

        public FixedString4096Bytes uri;
        public uint crc;

        public bool loaded;
        public JobHandle m_JobHandle;

        [NativeDisableUnsafePtrRestriction]
        public UnsafeList<UnsafeAssetInfo> assets;

        public UnsafeAssetBundleInfo(int index)
        {
            this.index = index;
            m_Generation = 0;
            m_Using = false;

            uri = string.Empty;
            crc = 0;

            loaded = false;
            m_JobHandle = default;

            assets = default;
        }

        public void Dispose()
        {
            if (assets.IsCreated)
            {
                assets.Dispose();
            }
        }
        public bool Equals(UnsafeAssetBundleInfo other) => index.Equals(other.index);

        public bool IsValid() => index >= 0 && m_Using;
    }
}

#endif