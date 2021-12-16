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

using Point.Collections.ResourceControl.LowLevel;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Point.Collections.ResourceControl
{
    public sealed class ResourceManager : StaticMonobehaviour<ResourceManager>
    {
        public override bool EnableLog => false;
        public override bool HideInInspector => true;

        private NativeArray<InternalAssetBundleInfo> m_AssetBundleInfos;
        private AssetBundle[] m_LoadedAssetBundles = Array.Empty<AssetBundle>();

        public override void OnInitialze()
        {
            var trackedAssetBundleNames = ResourceAddresses.Instance.TrackedAssetBundleNames;

            m_AssetBundleInfos = new NativeArray<InternalAssetBundleInfo>(trackedAssetBundleNames.Count, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
            m_LoadedAssetBundles = new AssetBundle[trackedAssetBundleNames.Count];

            for (int i = 0; i < m_AssetBundleInfos.Length; i++)
            {
                m_AssetBundleInfos[i] = new InternalAssetBundleInfo(i, false);
            }
        }
        private void OnDestroy()
        {
            for (int i = 0; i < m_LoadedAssetBundles.Length; i++)
            {
                if (m_LoadedAssetBundles[i] == null) continue;

                m_LoadedAssetBundles[i].Unload(true);
            }

            m_AssetBundleInfos.Dispose();
            m_LoadedAssetBundles = null;
        }

        #region Internal

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

            string path = Path.Combine(Application.streamingAssetsPath, ResourceAddresses.GetBundleName(in assetBundle));
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

            string path = Path.Combine(Application.streamingAssetsPath, ResourceAddresses.GetBundleName(in assetBundle));
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

        public static bool IsTrackedAssetBundle(in string bundleName, out AssetBundleInfo assetBundle)
        {
            var trackedAssetBundleNames = ResourceAddresses.Instance.TrackedAssetBundleNames;

            for (int i = 0; i < trackedAssetBundleNames.Count; i++)
            {
                if (trackedAssetBundleNames[i].Equals(bundleName))
                {
                    unsafe
                    {
                        assetBundle = new AssetBundleInfo((IntPtr)GetAssetBundleInfoAtPointer(i));
                    }
                    return true;
                }
            }

            assetBundle = AssetBundleInfo.Invalid;
            return false;
        }
        public static AssetBundleInfo GetAssetBundleInfo(string bundleName)
        {
            var trackedAssetBundleNames = ResourceAddresses.Instance.TrackedAssetBundleNames;

            for (int i = 0; i < trackedAssetBundleNames.Count; i++)
            {
                if (trackedAssetBundleNames[i].Equals(bundleName))
                {
                    unsafe
                    {
                        return new AssetBundleInfo((IntPtr)GetAssetBundleInfoAtPointer(i));
                    }
                }
            }

            return AssetBundleInfo.Invalid;
        }

        #endregion

        //public AssetBundle LoadAssetBundle(string assetBundlePath)
        //{
        //    string name = Path.GetFileName(assetBundlePath);
            
        //    if (IsTrackedAssetBundle(in assetBundlePath, out AssetBundleInfo bundleInfo))
        //    {
        //        return bundleInfo.Load();
        //    }
        //}
    }
}
