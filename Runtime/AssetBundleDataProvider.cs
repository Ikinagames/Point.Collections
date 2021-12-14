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

using System.Collections.Generic;
using UnityEngine;

namespace Point.Collections
{
    public sealed class AssetBundleDataProvider : DataProvider<AssetBundleStrategy>
    {
        private string m_AssetbundlePath;

        private bool m_Loaded;
        private AssetBundle m_AssetBundle;
        private AssetBundleCreateRequest m_Request;

        private Dictionary<Hash, uint> m_ReferenceCounts;
        private Dictionary<Hash, UnityEngine.Object> m_LoadedAssets;

        protected override void OnInitialize(params object[] args)
        {
            m_AssetbundlePath = (string)args[0];

            m_Loaded = false;
        }
        protected override AssetBundleStrategy InitializeStrategy()
        {
            return new AssetBundleStrategy();
        }

        protected override void Load()
        {
            if (m_Loaded)
            {

                return;
            }

            m_Request = AssetBundle.LoadFromFileAsync(m_AssetbundlePath);
            m_Request.completed += Request_completed;
        }
        private void Request_completed(AsyncOperation obj)
        {
            m_Loaded = true;

            m_ReferenceCounts = new Dictionary<Hash, uint>();
            m_LoadedAssets = new Dictionary<Hash, Object>();

            m_AssetBundle = m_Request.assetBundle;
            string[] allAssetsName = m_AssetBundle.GetAllAssetNames();
            
            for (int i = 0; i < allAssetsName.Length; i++)
            {
                Hash key = new Hash(allAssetsName[i]);

                m_ReferenceCounts.Add(key, 0);
            }

            m_Request = null;
        }

        public T LoadAsset<T>(string name) where T : UnityEngine.Object
        {
            if (!m_Loaded) throw new System.Exception();

            Hash key = new Hash(name);
            if (m_LoadedAssets.TryGetValue(key, out UnityEngine.Object asset))
            {
                if (asset is T t)
                {
                    AddReferenceCount(key);

                    return t;
                }

                throw new System.Exception();
            }

            T target = m_AssetBundle.LoadAsset<T>(name);
            m_LoadedAssets.Add(key, target);
            AddReferenceCount(key);

            return target;
        }
        public void ReserveAsset<T>(T asset) where T : UnityEngine.Object
        {
            Hash key = new Hash(asset.name);
            if (!m_ReferenceCounts.ContainsKey(key))
            {
                throw new System.Exception();
            }

            SubReferenceCount(key);
        }

        protected override void Unload()
        {
            m_LoadedAssets = null;
            m_ReferenceCounts = null;

            m_AssetBundle.Unload(true);

            m_Loaded = false;
        }

        private void AddReferenceCount(Hash key)
        {
            if (!m_ReferenceCounts.ContainsKey(key))
            {
                m_ReferenceCounts.Add(key, 1);
            }
            else m_ReferenceCounts[key] += 1;
        }
        private void SubReferenceCount(Hash key)
        {
            if (m_ReferenceCounts[key] == 1)
            {
                m_ReferenceCounts.Remove(key);

                DataStrategy.ZeroReferenceAtReserve(m_AssetBundle, m_LoadedAssets[key]);
            }
            else m_ReferenceCounts[key] -= 1;
        }
    }
}
