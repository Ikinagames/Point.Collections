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
using UnityEngine;
using UnityEngine.Networking;

namespace Point.Collections.ResourceControl.LowLevel
{
    [BurstCompatible]
    internal struct UnsafeAssetBundleInfo : IValidation, IEquatable<UnsafeAssetBundleInfo>, IDisposable
    {
        public static UnsafeAssetBundleInfo Invalid => new UnsafeAssetBundleInfo(-1);

        public readonly int index;
        public uint m_Generation;
        public bool m_Using;

        public FixedString4096Bytes uri;
        public uint crc;

        public bool m_IsLoaded;
        public JobHandle m_JobHandle;

        [NativeDisableUnsafePtrRestriction]
        public UnsafeHashMap<Hash, UnsafeAssetInfo> assets;

        public UnsafeAssetBundleInfo(int index)
        {
            this.index = index;
            m_Generation = 0;
            m_Using = false;

            uri = string.Empty;
            crc = 0;

            m_IsLoaded = false;
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
    internal unsafe sealed class AssetBundleLoadAsyncHandler
    {
        private UnsafeAssetBundleInfo* m_Bundle;
        private ResourceManager.AssetContainer m_AssetContainer;

        private UnityWebRequest m_WebRequest;

        public void Initialize(UnsafeAssetBundleInfo* bundle, ResourceManager.AssetContainer container, UnityWebRequest webRequest)
        {
            m_Bundle = bundle;
            m_AssetContainer = container;

            m_WebRequest = webRequest;
            m_WebRequest.SendWebRequest().completed += M_WebRequest_completed;
        }

        private void M_WebRequest_completed(UnityEngine.AsyncOperation obj)
        {
            m_Bundle->m_IsLoaded = true;
            //byte[] assetBundleBytes = m_WebRequest.downloadHandler.data;
            AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(m_WebRequest);

            ResourceManager.GetAssetBundle(m_Bundle->index).AssetBundle = bundle;
            ResourceManager.UpdateAssetInfos(m_Bundle, bundle);
        }
    }
}
