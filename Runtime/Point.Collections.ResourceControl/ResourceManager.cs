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
        public override bool EnableLog => false;
        public override bool HideInInspector => true;

        private NativeList<InternalAssetBundleInfo> m_AssetBundleInfos;
        private List<AssetContainer> m_AssetBundles;

        internal sealed class AssetContainer
        {
            private readonly AssetBundle m_AssetBundle;
            public Dictionary<Hash, UnityEngine.Object> m_Assets;

            public AssetBundle AssetBundle => m_AssetBundle;

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
        }

        public override void OnInitialze()
        {
            m_AssetBundleInfos = new NativeList<InternalAssetBundleInfo>(AllocatorManager.Persistent);
            m_AssetBundles = new List<AssetContainer>();
        }
        public override void OnShutdown()
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
                if (Instance.m_AssetBundleInfos[i].m_Path.Equals(temp))
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

        public static AssetBundleInfo RegisterAssetBundle(string path, bool webRequest)
        {
            if (TryGetAssetBundleWithPath(path, out var bundle)) return bundle;

            int index = GetUnusedAssetBundleBuffer();
            if (index < 0)
            {
                index = Instance.m_AssetBundles.Count;
                var info = new InternalAssetBundleInfo(index)
                {
                    m_Using = true,

                    m_Path = path,
                    m_IsWebRequest = webRequest
                };

                Instance.m_AssetBundleInfos.Add(info);
                Instance.m_AssetBundles.Add(null);
            }
            else
            {
                ref var info = ref Instance.m_AssetBundleInfos.ElementAt(index);
                info.m_Generation++;

                info.m_Using = true;

                info.m_Path = path;
                info.m_IsWebRequest = webRequest;

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
                var info = new InternalAssetBundleInfo(index)
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

            return GetAssetBundleInfo(in index);
        }
        public static void UnregisterAssetBundle(AssetBundleInfo assetBundle)
        {
            int index = assetBundle.Ref.m_Index;

            ref var info = ref Instance.m_AssetBundleInfos.ElementAt(index);

            info.m_Using = false;

            info.m_Path = string.Empty;
            info.m_IsLoaded = false;

            if (info.m_Assets.IsCreated)
            {
                info.m_Assets.Dispose();
            }

            Instance.m_AssetBundles[index] = null;
        }

        #region Internal

        internal static unsafe AssetBundleInfo GetAssetBundleInfo(in int index)
        {
            ref InternalAssetBundleInfo temp = ref Instance.m_AssetBundleInfos.ElementAt(index);

            AssetBundleInfo info 
                = new AssetBundleInfo((IntPtr)UnsafeUtility.AddressOf(ref temp), temp.m_Generation);

            return info;
        }
        internal static unsafe AssetContainer GetAssetBundle(in int index)
        {
            return Instance.m_AssetBundles[index];
        }

        internal static unsafe AssetBundle LoadAssetBundle(ref InternalAssetBundleInfo p)
            => LoadAssetBundle((InternalAssetBundleInfo*)UnsafeUtility.AddressOf(ref p));
        internal static unsafe AssetBundle LoadAssetBundle(InternalAssetBundleInfo* p)
        {
            int index = p->m_Index;
            if (!p->m_IsWebRequest)
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(p->m_Path.ToString());

                Instance.m_AssetBundles[index] = new AssetContainer(bundle);
                p->m_IsLoaded = true;

                UpdateAssetInfos(p, bundle);

                return bundle;
            }

            throw new NotImplementedException();
        }

        internal static unsafe void UnloadAssetBundle(ref InternalAssetBundleInfo p, bool unloadAllLoadedObjects)
            => UnloadAssetBundle((InternalAssetBundleInfo*)UnsafeUtility.AddressOf(ref p), unloadAllLoadedObjects);
        internal static unsafe void UnloadAssetBundle(InternalAssetBundleInfo* p, bool unloadAllLoadedObjects)
        {
            int index = p->m_Index;

            p->m_IsLoaded = false;

            Instance.m_AssetBundles[index].AssetBundle.Unload(unloadAllLoadedObjects);
            Instance.m_AssetBundles[index] = null;
        }

        private static unsafe JobHandle UpdateAssetInfos(InternalAssetBundleInfo* p, AssetBundle assetBundle)
        {
            var assetNames = assetBundle.GetAllAssetNames().Select(str => (FixedString4096Bytes)str).ToArray();
            NativeArray<FixedString4096Bytes> names = new NativeArray<FixedString4096Bytes>(assetNames, Allocator.TempJob);

            if (!p->m_Assets.IsCreated)
            {
                p->m_Assets = new UnsafeHashMap<Hash, InternalAssetInfo>(names.Length, AllocatorManager.Persistent);
            }
            else
            {
                p->m_Assets.Clear();
            }

            UpdateAssetInfoJob job = new UpdateAssetInfoJob()
            {
                m_Names = names,
                m_HashMap = p->m_Assets.AsParallelWriter()
            };

            JobHandle handle = job.Schedule(names.Length, 64);
            p->m_JobHandle = JobHandle.CombineDependencies(p->m_JobHandle, handle);

            return handle;
        }

        [BurstCompile(CompileSynchronously = true)]
        private struct UpdateAssetInfoJob : IJobParallelFor
        {
            [ReadOnly, DeallocateOnJobCompletion] public NativeArray<FixedString4096Bytes> m_Names;
            [WriteOnly] public UnsafeHashMap<Hash, InternalAssetInfo>.ParallelWriter m_HashMap;

            public void Execute(int i)
            {
                InternalAssetInfo assetInfo = new InternalAssetInfo()
                {
                    m_Key = m_Names[i],
                    m_IsLoaded = false,
                };

                Hash hash = new Hash(FNV1a32.Calculate(m_Names[i]));

                m_HashMap.TryAdd(hash, assetInfo);
            }
        }

        internal static unsafe AssetInfo LoadAsset(ref InternalAssetBundleInfo bundleP, in FixedString4096Bytes key)
            => LoadAsset((InternalAssetBundleInfo*)UnsafeUtility.AddressOf(ref bundleP), key);
        internal static unsafe AssetInfo LoadAsset(InternalAssetBundleInfo* bundleP, in FixedString4096Bytes key)
        {
            if (!bundleP->m_IsLoaded)
            {
                throw new InvalidOperationException();
            }

            int index = bundleP->m_Index;
            AssetContainer bundle = Instance.m_AssetBundles[index];

            string stringKey = key.ToString();
            Hash hash = new Hash(stringKey);
            if (!bundleP->m_Assets.TryGetValue(hash, out InternalAssetInfo assetInfo))
            {
                throw new Exception();
            }

            if (!assetInfo.m_IsLoaded)
            {
                assetInfo.m_ReferencedCount = 1;
                assetInfo.m_IsLoaded = true;
            }
            else assetInfo.m_ReferencedCount++;

            bundleP->m_Assets[hash] = assetInfo;

            UnityEngine.Object obj = bundle.LoadAsset(stringKey);

            AssetInfo asset = new AssetInfo(bundleP, hash);
            return asset;
        }
        internal static unsafe void Reserve(InternalAssetBundleInfo* bundleP, Hash key)
        {
            if (!bundleP->m_IsLoaded)
            {
                throw new InvalidOperationException();
            }

            int index = bundleP->m_Index;
            AssetContainer bundle = Instance.m_AssetBundles[index];

            if (!bundleP->m_Assets.TryGetValue(key, out InternalAssetInfo assetInfo))
            {
                throw new Exception("1");
            }

            if (!assetInfo.m_IsLoaded)
            {
                throw new Exception("2");
            }

            assetInfo.m_ReferencedCount--;
            bundleP->m_Assets[key] = assetInfo;
        }

        #endregion
    }
}
