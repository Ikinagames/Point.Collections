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
        private List<AssetBundle> m_AssetBundles;

        public override void OnInitialze()
        {
            m_AssetBundleInfos = new NativeList<InternalAssetBundleInfo>(AllocatorManager.Persistent);
            m_AssetBundles = new List<AssetBundle>();
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

        public static AssetBundleInfo RegisterAssetBundle(string path, bool webRequest)
        {
            int index = Instance.m_AssetBundles.Count;

            InternalAssetBundleInfo info = new InternalAssetBundleInfo(index)
            {
                m_Path = path,
                m_IsWebRequest = webRequest
            };

            Instance.m_AssetBundleInfos.Add(info);
            Instance.m_AssetBundles.Add(null);

            return GetAssetBundleInfo(in index);
        }
        public static AssetBundleInfo RegisterAssetBundle(AssetBundle assetBundle)
        {
            int index = Instance.m_AssetBundles.Count;

            InternalAssetBundleInfo info = new InternalAssetBundleInfo(index)
            {
                m_IsLoaded = true
            };

            Instance.m_AssetBundleInfos.Add(info);
            Instance.m_AssetBundles.Add(assetBundle);

            return GetAssetBundleInfo(in index);
        }

        #region Internal

        internal static unsafe AssetBundleInfo GetAssetBundleInfo(in int index)
        {
            ref InternalAssetBundleInfo temp = ref Instance.m_AssetBundleInfos.ElementAt(index);

            AssetBundleInfo info = new AssetBundleInfo((IntPtr)UnsafeUtility.AddressOf(ref temp));

            return info;
        }
        internal static unsafe AssetBundle GetAssetBundle(in int index)
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

                Instance.m_AssetBundles[index] = bundle;
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

            Instance.m_AssetBundles[index].Unload(unloadAllLoadedObjects);
            Instance.m_AssetBundles[index] = null;
        }

        private static unsafe JobHandle UpdateAssetInfos(InternalAssetBundleInfo* p, AssetBundle assetBundle)
        {
            var assetNames = assetBundle.GetAllAssetNames().Select(str => (FixedString4096Bytes)str).ToArray();
            NativeArray<FixedString4096Bytes> names = new NativeArray<FixedString4096Bytes>(assetNames, Allocator.TempJob);

            p->m_Assets = new UnsafeHashMap<Hash, InternalAssetInfo>(names.Length, AllocatorManager.Persistent);

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

        #endregion
    }
}
