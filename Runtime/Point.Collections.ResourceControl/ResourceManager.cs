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

using Point.Collections.ResourceControl.LowLevel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.Networking;

namespace Point.Collections.ResourceControl
{
    public sealed class ResourceManager : StaticMonobehaviour<ResourceManager>
    {
        private const string c_FileUri = "file:///";

        protected override bool EnableLog => false;
        protected override bool HideInInspector => true;

        private NativeList<UnsafeAssetBundleInfo> m_AssetBundleInfos;
        private List<AssetContainer> m_AssetBundles;

        internal sealed class AssetContainer
        {
            private AssetBundle m_AssetBundle;
            public Dictionary<Hash, UnityEngine.Object> m_Assets;

            public AssetBundle AssetBundle
            {
                get => m_AssetBundle;
                set => m_AssetBundle = value;
            }

            public AssetContainer()
            {
                m_AssetBundle = null;
                m_Assets = new Dictionary<Hash, UnityEngine.Object>();
            }
            public AssetContainer(AssetBundle assetBundle)
            {
                m_AssetBundle = assetBundle;
                m_Assets = new Dictionary<Hash, UnityEngine.Object>();
            }

            public UnityEngine.Object GetAsset(Hash hash)
            {
                return m_Assets[hash];
            }
            public UnityEngine.Object LoadAsset(string key)
            {
                Hash hash = new Hash(key);

                if (m_Assets.TryGetValue(hash, out var obj)) return obj;

                obj = m_AssetBundle.LoadAsset(key);
                m_Assets.Add(hash, obj);

                return obj;
            }

            public void Clear()
            {
                m_AssetBundle = null;
                m_Assets.Clear();
            }
        }

        protected override void OnInitialze()
        {
            m_AssetBundleInfos = new NativeList<UnsafeAssetBundleInfo>(AllocatorManager.Persistent);
            m_AssetBundles = new List<AssetContainer>();
        }
        protected override void OnShutdown()
        {
            for (int i = 0; i < m_AssetBundleInfos.Length; i++)
            {
                m_AssetBundleInfos[i].Dispose();
            }

            m_AssetBundleInfos.Dispose();
            m_AssetBundles = null;
        }

        private static int GetUnusedAssetBundleBuffer()
        {
            for (int i = 0; i < Instance.m_AssetBundleInfos.Length; i++)
            {
                if (!Instance.m_AssetBundleInfos[i].IsValid())
                {
                    return i;
                }
            }
            return -1;
        }
        private static bool TryGetAssetBundleWithPath(string path, out AssetBundleInfo assetBundle)
        {
            FixedString4096Bytes temp = path;
            for (int i = 0; i < Instance.m_AssetBundleInfos.Length; i++)
            {
                if (Instance.m_AssetBundleInfos[i].uri.Equals(temp))
                {
                    assetBundle = GetAssetBundleInfo(i);
                    return true;
                }
            }

            assetBundle = default;
            return false;
        }
        private static bool TryGetAssetBundleWithBundle(AssetBundle bundle, out AssetBundleInfo assetBundle)
        {
            for (int i = 0; i < Instance.m_AssetBundles.Count; i++)
            {
                if (Instance.m_AssetBundles[i]?.AssetBundle == null) continue;
                else if (Instance.m_AssetBundles[i].AssetBundle.Equals(bundle))
                {
                    assetBundle = GetAssetBundleInfo(i);
                    return true;
                }
            }

            assetBundle = default;
            return false;
        }

        public static AssetBundleInfo RegisterAssetBundlePath(string path)
        {
            
            if (path.StartsWith("Assets"))
            {
                path = path.Substring(6, path.Length - 6);
            }

            string uri = c_FileUri + Application.dataPath + path;
            return RegisterAssetBundleUri(uri);
        }
        public static AssetBundleInfo RegisterAssetBundleUri(string uri, uint crc = 0)
        {
            if (TryGetAssetBundleWithPath(uri, out var bundle)) return bundle;

            int index = GetUnusedAssetBundleBuffer();
            if (index < 0)
            {
                index = Instance.m_AssetBundles.Count;
                var info = new UnsafeAssetBundleInfo(index)
                {
                    m_Using = true,

                    uri = uri,
                    crc = crc
                };

                Instance.m_AssetBundleInfos.Add(info);
                Instance.m_AssetBundles.Add(null);
            }
            else
            {
                ref var info = ref Instance.m_AssetBundleInfos.ElementAt(index);
                info.m_Generation++;

                info.m_Using = true;

                info.uri = uri;
                info.crc = crc;

                Instance.m_AssetBundles[index] = null;
            }

            return GetAssetBundleInfo(in index);
        }
        public static AssetBundleInfo RegisterAssetBundle(AssetBundle assetBundle)
        {
            if (TryGetAssetBundleWithBundle(assetBundle, out var bundle)) return bundle;

            int index = GetUnusedAssetBundleBuffer();
            if (index < 0)
            {
                index = Instance.m_AssetBundles.Count;
                var info = new UnsafeAssetBundleInfo(index)
                {
                    m_Using = true,

                    m_IsLoaded = true
                };

                Instance.m_AssetBundleInfos.Add(info);
                Instance.m_AssetBundles.Add(new AssetContainer(assetBundle));
            }
            else
            {
                ref var info = ref Instance.m_AssetBundleInfos.ElementAt(index);
                info.m_Generation++;

                info.m_Using = true;

                info.m_IsLoaded = true;

                Instance.m_AssetBundles[index] = new AssetContainer(assetBundle);
            }

            AssetBundleInfo bundleInfo = GetAssetBundleInfo(in index);
            unsafe
            {
                UpdateAssetInfos(bundleInfo.pointer, assetBundle);
            }

            return bundleInfo;
        }
        public static void UnregisterAssetBundle(AssetBundleInfo assetBundle)
        {
            int index = assetBundle.Ref.index;

            ref var info = ref Instance.m_AssetBundleInfos.ElementAt(index);

            info.m_Using = false;

            info.uri = string.Empty;
            info.m_IsLoaded = false;

            if (info.assets.IsCreated)
            {
                info.assets.Dispose();
            }

            Instance.m_AssetBundles[index] = null;
        }

        #region Internal

        internal static unsafe AssetBundleInfo GetAssetBundleInfo(in int index)
        {
            ref UnsafeAssetBundleInfo temp = ref Instance.m_AssetBundleInfos.ElementAt(index);

            AssetBundleInfo info 
                = new AssetBundleInfo((UnsafeAssetBundleInfo*)UnsafeUtility.AddressOf(ref temp), temp.m_Generation);

            return info;
        }
        internal static unsafe AssetContainer GetAssetBundle(in int index)
        {
            if (Instance.m_AssetBundles[index] == null)
            {
                Instance.m_AssetBundles[index] = new AssetContainer();
            }
            return Instance.m_AssetBundles[index];
        }

        internal static unsafe AssetBundle LoadAssetBundle(ref UnsafeAssetBundleInfo p)
            => LoadAssetBundle((UnsafeAssetBundleInfo*)UnsafeUtility.AddressOf(ref p));
        internal static unsafe AssetBundle LoadAssetBundle(UnsafeAssetBundleInfo* p)
        {
            int index = p->index;
            AssetBundle bundle = AssetBundle.LoadFromFile(p->uri.ToString().Replace(c_FileUri, string.Empty));

            p->m_IsLoaded = true;
            AssetContainer container = GetAssetBundle(in index);
            container.AssetBundle = bundle;

            UpdateAssetInfos(p, bundle);

            return bundle;
        }
        internal static unsafe AsyncOperation LoadAssetBundleAsync(UnsafeAssetBundleInfo* p)
        {
            int index = p->index;
            var request = UnityWebRequestAssetBundle.GetAssetBundle(p->uri.ToString(), p->crc);
            //AssetBundle bundle = AssetBundle.LoadFromFile(p->uri.ToString());

            var handler = new AssetBundleLoadAsyncHandler();
            return handler.Initialize(p, GetAssetBundle(in index), request);
        }

        internal static unsafe void UnloadAssetBundle(ref UnsafeAssetBundleInfo p, bool unloadAllLoadedObjects)
            => UnloadAssetBundle((UnsafeAssetBundleInfo*)UnsafeUtility.AddressOf(ref p), unloadAllLoadedObjects);
        internal static unsafe void UnloadAssetBundle(UnsafeAssetBundleInfo* p, bool unloadAllLoadedObjects)
        {
            int index = p->index;

            p->m_IsLoaded = false;

            Instance.m_AssetBundles[index].AssetBundle.Unload(unloadAllLoadedObjects);
            Instance.m_AssetBundles[index].Clear();
        }

        internal static unsafe JobHandle UpdateAssetInfos(UnsafeAssetBundleInfo* p, AssetBundle assetBundle)
        {
            var assetNames = assetBundle.GetAllAssetNames().Select(str => (FixedString4096Bytes)str).ToArray();
            NativeArray<FixedString4096Bytes> names = new NativeArray<FixedString4096Bytes>(assetNames, Allocator.TempJob);

            if (!p->assets.IsCreated)
            {
                p->assets = new UnsafeHashMap<Hash, UnsafeAssetInfo>(names.Length, AllocatorManager.Persistent);
            }
            else
            {
                p->assets.Clear();
            }

            UpdateAssetInfoJob job = new UpdateAssetInfoJob()
            {
                m_Names = names,
                m_HashMap = p->assets.AsParallelWriter()
            };

            JobHandle handle = job.Schedule(names.Length, 64);
            p->m_JobHandle = JobHandle.CombineDependencies(p->m_JobHandle, handle);

            return handle;
        }

        [BurstCompile(CompileSynchronously = true)]
        private struct UpdateAssetInfoJob : IJobParallelFor
        {
            [ReadOnly, DeallocateOnJobCompletion] public NativeArray<FixedString4096Bytes> m_Names;
            [WriteOnly] public UnsafeHashMap<Hash, UnsafeAssetInfo>.ParallelWriter m_HashMap;

            public void Execute(int i)
            {
                UnsafeAssetInfo assetInfo = new UnsafeAssetInfo()
                {
                    m_Key = m_Names[i],
                    loaded = false,
                };

                Hash hash = new Hash(FNV1a32.Calculate(m_Names[i]));

                m_HashMap.TryAdd(hash, assetInfo);
            }
        }

        internal static unsafe AssetInfo LoadAsset(ref UnsafeAssetBundleInfo bundleP, in FixedString4096Bytes key)
            => LoadAsset((UnsafeAssetBundleInfo*)UnsafeUtility.AddressOf(ref bundleP), key);
        internal static unsafe AssetInfo LoadAsset(UnsafeAssetBundleInfo* bundleP, in FixedString4096Bytes key)
        {
            if (!bundleP->m_IsLoaded)
            {
                throw new InvalidOperationException();
            }

            int index = bundleP->index;
            AssetContainer bundle = Instance.m_AssetBundles[index];

            string stringKey = key.ToString();
            Hash hash = new Hash(stringKey);
            if (!bundleP->assets.TryGetValue(hash, out UnsafeAssetInfo assetInfo))
            {
                throw new Exception();
            }

            if (!assetInfo.loaded)
            {
                assetInfo.referencedCount = 1;
                assetInfo.loaded = true;
            }
            else assetInfo.referencedCount++;

            bundleP->assets[hash] = assetInfo;

            UnityEngine.Object obj = bundle.LoadAsset(stringKey);

            AssetInfo asset = new AssetInfo(bundleP, hash);
            return asset;
        }
        internal static unsafe void Reserve(UnsafeAssetBundleInfo* bundleP, in Hash key)
        {
            if (!bundleP->m_IsLoaded)
            {
                throw new InvalidOperationException();
            }

            int index = bundleP->index;
            AssetContainer bundle = Instance.m_AssetBundles[index];

            if (!bundleP->assets.TryGetValue(key, out UnsafeAssetInfo assetInfo))
            {
                throw new Exception("1");
            }

            if (!assetInfo.loaded)
            {
                throw new Exception("2");
            }

            assetInfo.referencedCount--;
            bundleP->assets[key] = assetInfo;
        }

        #endregion
    }
}
