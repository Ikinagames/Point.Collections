﻿// Copyright 2021 Ikina Games
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

using Point.Collections.ResourceControl.LowLevel;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Point.Collections.ResourceControl
{
    [PreferBinarySerialization]
    public sealed class ResourceAddresses : StaticScriptableObject<ResourceAddresses>
    {
        // AssetBundle.name
        [SerializeField] private string[] m_TrackedAssetBundles = Array.Empty<string>();
        [SerializeField] private TrackedAsset[] m_TrackedAssets = Array.Empty<TrackedAsset>();

        private NativeArray<InternalAssetBundleInfo> m_AssetBundleInfos;
        private AssetBundle[] m_LoadedAssetBundles = Array.Empty<AssetBundle>();

        public IReadOnlyList<string> TrackedAssetBundleNames => m_TrackedAssetBundles;

        private void OnEnable()
        {
            m_AssetBundleInfos = new NativeArray<InternalAssetBundleInfo>(m_TrackedAssetBundles.Length, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_LoadedAssetBundles = new AssetBundle[m_TrackedAssetBundles.Length];

            for (int i = 0; i < m_AssetBundleInfos.Length; i++)
            {
                m_AssetBundleInfos[i] = new InternalAssetBundleInfo(i, false);
            }
        }
        private void OnDisable()
        {
            for (int i = 0; i < m_LoadedAssetBundles.Length; i++)
            {
                if (m_LoadedAssetBundles[i] == null) continue;

                m_LoadedAssetBundles[i].Unload(true);
            }

            m_AssetBundleInfos.Dispose();
            m_LoadedAssetBundles = null;
        }

        internal static unsafe ref InternalAssetBundleInfo GetAssetBundleInfoAt(int index)
        {
            InternalAssetBundleInfo* buffer
                    = (InternalAssetBundleInfo*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(Instance.m_AssetBundleInfos);

            return ref UnsafeUtility.ArrayElementAsRef<InternalAssetBundleInfo>(buffer, index);
        }
        internal static unsafe InternalAssetBundleInfo* GetAssetBundleInfoAtPointer(int index)
        {
            InternalAssetBundleInfo* buffer
                    = (InternalAssetBundleInfo*)NativeArrayUnsafeUtility.GetUnsafeBufferPointerWithoutChecks(Instance.m_AssetBundleInfos);

            return buffer + index;
        }

        internal static void RegisterAssetBundleAssetAt(int index, AssetBundle assetBundle)
        {
            Instance.m_LoadedAssetBundles[index] = assetBundle;
        }

        internal static string GetBundleName(in InternalAssetBundleInfo assetBundle)
        {
            return Instance.m_TrackedAssetBundles[assetBundle.m_Index];
        }
        internal static bool IsAssetBundleLoaded(in InternalAssetBundleInfo assetBundle)
        {
            return Instance.m_LoadedAssetBundles[assetBundle.m_Index] != null;
        }
        internal static AssetBundle GetAssetBundle(in InternalAssetBundleInfo assetBundle)
        {
            return Instance.m_LoadedAssetBundles[assetBundle.m_Index];
        }
        internal static unsafe AssetBundleHandler LoadAssetBundleAsync(in InternalAssetBundleInfo assetBundle)
        {
            AssetBundleHandler handler = new AssetBundleHandler(GetAssetBundleInfoAtPointer(assetBundle.m_Index), true);

            if (IsAssetBundleLoaded(in assetBundle))
            {
                return handler;
            }

            string path = Path.Combine(Application.streamingAssetsPath, Instance.m_TrackedAssetBundles[assetBundle.m_Index]);
            if (!File.Exists(path))
            {
                return new AssetBundleHandler(null, true);
            }

            AssetBundleCreateRequest request = AssetBundle.LoadFromFileAsync(path);
            // TODO : Temp
            AssetBundleLoadAsyncHandler asyncLoadHandler = new AssetBundleLoadAsyncHandler();
            asyncLoadHandler.Initialize(assetBundle.m_Index, request);

            return handler;
        }
        internal static unsafe AssetBundle LoadAssetBundle(in InternalAssetBundleInfo assetBundle)
        {
            if (IsAssetBundleLoaded(in assetBundle))
            {
                return Instance.m_LoadedAssetBundles[assetBundle.m_Index];
            }

            string path = Path.Combine(Application.streamingAssetsPath, Instance.m_TrackedAssetBundles[assetBundle.m_Index]);
            if (!File.Exists(path))
            {
                return null;
            }
            AssetBundle bundle = AssetBundle.LoadFromFile(path);

            Instance.m_LoadedAssetBundles[assetBundle.m_Index] = bundle;

            ref InternalAssetBundleInfo target = ref GetAssetBundleInfoAt(assetBundle.m_Index);
            target.m_IsLoaded = true;

            return bundle;
        }
        internal static unsafe void UnloadAssetBundle(in InternalAssetBundleInfo assetBundle)
        {
            if (!IsAssetBundleLoaded(in assetBundle))
            {
                return;
            }

            Instance.m_LoadedAssetBundles[assetBundle.m_Index].Unload(true);
            Instance.m_LoadedAssetBundles[assetBundle.m_Index] = null;

            ref InternalAssetBundleInfo target = ref GetAssetBundleInfoAt(assetBundle.m_Index);
            target.m_IsLoaded = false;
        }

        public static AssetBundleInfo GetAssetBundleInfo(string bundleName)
        {
            ResourceAddresses resources = Instance;
            for (int i = 0; i < resources.m_TrackedAssetBundles.Length; i++)
            {
                if (resources.m_AssetBundleInfos[i].Name.ToString().Equals(bundleName))
                {
                    unsafe
                    {
                        return new AssetBundleInfo((IntPtr)GetAssetBundleInfoAtPointer(i));
                    }
                }
            }

            return AssetBundleInfo.Invalid;
        }
    }
}
