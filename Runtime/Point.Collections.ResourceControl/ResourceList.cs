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

#if UNITY_2019_1_OR_NEWER && UNITY_ADDRESSABLES
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#if UNITY_2019 && !UNITY_2020_1_OR_NEWER
#define UNITYENGINE_OLD
#else
#if UNITY_MATHEMATICS
#endif
#endif

using Point.Collections.Buffer;
using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using AddressableReference = UnityEngine.AddressableAssets.AssetReference;

namespace Point.Collections.ResourceControl
{
    public sealed class ResourceList : ScriptableObject
    {
        [SerializeField] private List<AddressableAsset> m_AssetList = new List<AddressableAsset>();

        public AssetReference this[int index]
        {
            get => m_AssetList[index].AssetReference;
        }

#if UNITY_EDITOR
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
        /// <param name="name"></param>
        /// <param name="obj"></param>
        public void AddAsset(string name, UnityEngine.Object obj)
        {
            string path = UnityEditor.AssetDatabase.GetAssetPath(obj);
            var guid = UnityEditor.AssetDatabase.GUIDFromAssetPath(path);
            AddressableAsset temp = new AddressableAsset(name, guid.ToString());

            m_AssetList.Add(temp);
        }
#endif
    }
    [Serializable]
    public sealed class AddressableAsset
    {
        [SerializeField] private string m_DisplayName;
        [SerializeField] private AddressableReference m_AssetReference;

        public string DisplayName { get => m_DisplayName; set => m_DisplayName = value; }
        public AddressableReference AssetReference => m_AssetReference;

        public AddressableAsset() : base() { }
#if UNITY_EDITOR
        public AddressableAsset(string name, UnityEditor.GUID guid) : this(name, guid.ToString()) { }
#endif
        public AddressableAsset(string name, string guid)
        {
            m_DisplayName = name;
            m_AssetReference = new AddressableReference(guid);
        }
    }

    //internal sealed class RegisterResourceManagerOperation : AsyncOperationBase<IList<UnityEngine.Object>>
    //{
    //    private Dictionary<IResourceLocation, AsyncOperationHandle> m_Resources;

    //    AsyncOperationHandle<IList<UnityEngine.Object>> m_ResourceOperation;

    //    public static AsyncOperationHandle<IList<UnityEngine.Object>> Get(AsyncOperationHandle<IList<UnityEngine.Object>> oper)
    //    {
    //        var ins = ObjectPool<RegisterResourceManagerOperation>.Shared.Get();

    //        ins.m_Resources = ResourceManager.Instance.m_Resources;
    //        ins.m_ResourceOperation = oper;

    //        return Addressables.ResourceManager.StartOperation(ins, oper);
    //    }

    //    protected override void Execute()
    //    {
    //        IList<UnityEngine.Object> list = m_ResourceOperation.Result;
    //        for (int i = 0; i < list.Count; i++)
    //        {
    //            this.
    //            var completed = Addressables.ResourceManager.CreateCompletedOperation(list[i], string.Empty);
    //            m_Resources.Add()
    //        }
            

    //        ObjectPool<RegisterResourceManagerOperation>.Shared.Reserve(this);
    //    }
    //}
}

#endif