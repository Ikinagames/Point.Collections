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
#else
#if UNITY_MATHEMATICS
#endif
#endif

#if !UNITYENGINE_OLD

using System;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_ADDRESSABLES
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace Point.Collections.ResourceControl
{
    public abstract class AssetReferenceLoaderBase<TObject> : PointMonobehaviour
        where TObject : UnityEngine.Object
    {
        [Serializable]
        public sealed class Asset : IValidation
        {
            [SerializeField] private AssetIndex m_Asset;
            [SerializeField] private UnityEvent<TObject> m_OnCompleted;

            public bool IsValid() => m_Asset.IsValid();

#if UNITY_ADDRESSABLES
            [NonSerialized] private bool m_IsLoaded;
            [NonSerialized] private AsyncOperationHandle<TObject> m_LoadHandle;

            public AssetIndex AssetIndex => m_Asset;


            public bool TryLoadAsync(out AsyncOperationHandle<TObject> handle)
            {
                if (m_IsLoaded)
                {
                    handle = m_LoadHandle;
                    return true;
                }

                if (!m_Asset.IsValid())
                {
                    handle = ResourceManager.CreateCompletedOperationExeception<TObject>(null,
                        m_Asset, TypeHelper.TypeOf<TObject>.Type);
                    return false;
                }

                m_LoadHandle = m_Asset.AssetReference.LoadAssetAsync<TObject>();
                m_LoadHandle.Completed += M_LoadHandle_Completed;
                handle = m_LoadHandle;
                m_IsLoaded = true;

                return true;
            }
            private void M_LoadHandle_Completed(AsyncOperationHandle<TObject> obj)
            {
                TObject result = obj.Result;

                m_OnCompleted?.Invoke(result);
            }
            public void Release()
            {
                if (!m_IsLoaded) return;

                ResourceManager.Release(m_LoadHandle);

                m_IsLoaded = false;
            }
#endif
            public AssetInfo Load()
            {
                return m_Asset.AssetReference.LoadAsset();
            }
        }

        [SerializeField] private bool m_StartOnAwake = true;
        [SerializeField] private ArrayWrapper<Asset> m_Assets = ArrayWrapper<Asset>.Empty;
        [SerializeField] private UnityEvent m_OnAssetLoadCompleted;

        private int m_TotalAssetLoadedCounter = 0, m_Counter;

        protected virtual void OnEnable()
        {
            if (!m_StartOnAwake) return;

            LoadAssets();
        }
        public void LoadAssets()
        {
            m_TotalAssetLoadedCounter = m_Assets.Length;
#if UNITY_ADDRESSABLES
            AsyncOperationHandle<TObject> handle;
            for (int i = 0; i < m_Assets.Length; i++)
            {
#if UNITY_EDITOR
                if (!m_Assets[i].IsValid())
                {
                    PointHelper.LogError(Channel.Collections,
                        $"Asset({m_Assets[i].AssetIndex}) is not valid. This is not allowed.", this);
                    Debug.Break();
                    continue;
                }
#endif
                if (!m_Assets[i].TryLoadAsync(out handle))
                {
                    continue;
                }

                handle.Completed += M_LoadHandle_Completed;
            }
#else
            for (int i = 0; i < m_Assets.Length; i++)
            {
                OnLoadAsset(m_Assets[i]);
                AssetInfo assetInfo = m_Assets[i].Load();
#if UNITY_EDITOR
                if (!assetInfo.IsValid())
                {
                    $"Asset(Index of {i}) is not valid. This is not allowed.".ToLogError();

                    m_TotalAssetLoadedCounter--;
                }
#endif
                assetInfo.OnLoaded += AssetInfo_OnLoaded;
            }
#endif
        }

#if UNITY_ADDRESSABLES
        private void M_LoadHandle_Completed(AsyncOperationHandle<TObject> obj)
        {
            TObject result = obj.Result;

            OnLoadCompleted(result);
        }
#else
        private void AssetInfo_OnLoaded(UnityEngine.Object obj)
        {
            m_Counter++;
            OnLoadCompleted(obj as TObject);
            $"{obj.name} has loaded".ToLog();

            if (m_TotalAssetLoadedCounter == m_Counter)
            {
                m_OnAssetLoadCompleted?.Invoke();
            }
        }
#endif

        protected virtual void OnDestroy()
        {
#if UNITY_ADDRESSABLES
            for (int i = 0; i < m_Assets.Length; i++)
            {
                m_Assets[i].Release();
            }
#endif
        }

        protected virtual void OnLoadAsset(Asset asset) { }
        protected virtual void OnLoadCompleted(TObject obj) { }
    }
}

#endif
#endif