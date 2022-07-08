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
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_ADDRESSABLES
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace Point.Collections.ResourceControl
{
    /// <summary>
    /// 에셋을 프리로드 할 수 있는 로더의 기본 클래스입니다.
    /// </summary>
    /// <typeparam name="TObject"></typeparam>
    public abstract class AssetReferenceLoaderBase<TObject> : PointMonobehaviour
        where TObject : UnityEngine.Object
    {
        [Serializable]
        public sealed class Asset : IValidation
        {
            [SerializeField] private AssetIndex m_Asset;
            /// <summary>
            /// <see cref="m_Asset"/> 의 로드가 완료되었을 때 실행되는 콜백입니다.
            /// </summary>
            [SerializeField] private UnityEvent<TObject> m_OnCompleted;

            [NonSerialized] private bool m_IsLoaded;

            public AssetIndex AssetIndex => m_Asset;

            public bool IsValid() => m_Asset.IsValid();

#if UNITY_ADDRESSABLES
            [NonSerialized] private AsyncOperationHandle<TObject> m_LoadHandle;

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
#else
            [NonSerialized] private AssetInfo m_AssetInfo;

            public AssetInfo Load()
            {
                if (m_IsLoaded) return m_AssetInfo;

                m_IsLoaded = true;
                m_AssetInfo = m_Asset.AssetReference.LoadAsset();
                return m_AssetInfo;
            }
#endif
            public void Release()
            {
                if (!m_IsLoaded) return;

#if UNITY_ADDRESSABLES
                ResourceManager.Release(m_LoadHandle);
#else
                m_AssetInfo.Reserve();
#endif
                m_IsLoaded = false;
            }
        }

        /// <summary>
        /// <see cref="MonoBehaviour"/> 가 시작할 떄 로드를 시작할 것인지 결정합니다.
        /// </summary>
        [SerializeField] private bool m_StartOnAwake = true;
        /// <summary>
        /// 로드를 시작하기 전, 해당 <see cref="AssetBundle"/> 이 <see cref="ResourceManager"/> 에 등록되었는지 확인하고, 
        /// 등록된 후에 로드를 시작할 지 결정합니다.
        /// </summary>
        [SerializeField] private ArrayWrapper<AssetBundleName> m_WaitForBundleLoads = ArrayWrapper<AssetBundleName>.Empty;

        [Space]
        [SerializeField] private ArrayWrapper<Asset> m_Assets = ArrayWrapper<Asset>.Empty;
        [SerializeField] private UnityEvent m_OnAssetLoadCompleted;

        private int m_TotalAssetLoadedCounter = 0, m_Counter;

        protected IEnumerator Start()
        {
            if (!m_StartOnAwake) yield break;

            Timer timer;
            for (int i = 0; i < m_WaitForBundleLoads.Length; i++)
            {
                timer = Timer.Start();
                while (!ResourceManager.IsLoadedAssetBundle(m_WaitForBundleLoads[i]))
                {
                    if (timer.IsExceeded(10f))
                    {
                        $"{m_WaitForBundleLoads[i]} waiting more than 10 seconds".ToLogError();
                        timer = Timer.Start();
                    }

                    yield return null;
                }
            }

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
            for (int i = 0; i < m_Assets.Length; i++)
            {
                m_Assets[i].Release();
            }
        }

        protected virtual void OnLoadAsset(Asset asset) { }
        protected virtual void OnLoadCompleted(TObject obj) { }
    }
}

#endif
#endif