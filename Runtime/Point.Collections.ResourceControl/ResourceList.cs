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
#endif

using System.Collections.Generic;
using UnityEngine;
#if UNITY_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using AddressableReference = UnityEngine.AddressableAssets.AssetReference;
#endif

namespace Point.Collections.ResourceControl
{
    public sealed class ResourceList : ScriptableObject
    {
#if UNITY_ADDRESSABLES
        [SerializeField] private GroupReference m_Group;
#endif
        [SerializeField] private List<AddressableAsset> m_AssetList = new List<AddressableAsset>();

        public int Count => m_AssetList.Count;
        public AssetReference this[int index]
        {
            get => m_AssetList[index].AssetReference;
        }
        public AssetReference this[string friendlyName]
        {
            get
            {
                for (int i = 0; i < m_AssetList.Count; i++)
                {
                    if (m_AssetList[i].FriendlyName.Equals(friendlyName))
                    {
                        return m_AssetList[i].AssetReference;
                    }
                }
                return AssetReference.Empty;
            }
        }

#if UNITY_EDITOR
#if UNITY_ADDRESSABLES
        /// <summary>
        /// Editor only
        /// </summary>
        public string Group => m_Group;
#endif
        /// <summary>
        /// Editor only
        /// </summary>
        public void Clear()
        {
            m_AssetList.Clear();
        }
        /// <summary>
        /// Editor only
        /// </summary>
        /// <param name="name"></param>
        /// <param name="asset"></param>
        public void AddAsset(string name, string assetGuid)
        {
            AddressableAsset temp = new AddressableAsset(name, assetGuid);
            m_AssetList.Add(temp);
        }
        /// <summary>
        /// Editor only
        /// </summary>
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public void AddAsset(string name, UnityEngine.Object obj)
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(obj);
#if !UNITY_2020_1_OR_NEWER
            var guid = UnityEditor.AssetDatabase.GUIDToAssetPath(path);
#else
            var guid = UnityEditor.AssetDatabase.GUIDFromAssetPath(path);
#endif
            AddressableAsset temp = new AddressableAsset(name, guid.ToString());

            m_AssetList.Add(temp);
        }

#if UNITY_ADDRESSABLES
        /// <summary>
        /// Editor only
        /// </summary>
        /// <param name="name"></param>
        /// <param name="asset"></param>
        public void AddAsset(string name, AddressableReference asset)
        {
            AddressableAsset temp = new AddressableAsset(name, asset.AssetGUID);
            m_AssetList.Add(temp);
        }
        /// <summary>
        /// Editor only
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public bool Contains(string guid)
        {
            for (int i = 0; i < m_AssetList.Count; i++)
            {
                if (m_AssetList[i].AssetReference.AssetGUID.Equals(guid)) return true;
            }
            return false;
        }
#endif
#endif
        public AddressableAsset GetAddressableAsset(int index)
        {
            return m_AssetList[index];
        }

//#if UNITY_ADDRESSABLES
//        public AsyncOperationHandle<IList<UnityEngine.Object>> LoadAssetsAsync(Action<UnityEngine.Object> callback)
//        {
//            if (m_AssetReferences.Count == 0)
//            {
//                IList<UnityEngine.Object> temp = Array.Empty<UnityEngine.Object>();
//                return ResourceManager.CreateCompletedOperation(temp);
//            }

//            var result = Addressables.LoadAssetsAsync<UnityEngine.Object>(m_AssetReferences, callback);

//            return result;
//        }
//#endif
    }
}

#endif