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

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
#if UNITY_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.ResourceManagement.ResourceProviders;
#endif

namespace Point.Collections.ResourceControl
{
    public sealed class ResourceHashMap : StaticScriptableObject<ResourceHashMap>
    {
        [SerializeField] private AssetPathField[] m_StreamingAssetBundles = Array.Empty<AssetPathField>();
#if UNITY_ADDRESSABLES
        [Serializable]
        public sealed class SceneBindedLabel : IValidation
        {
            [SerializeField] private AssetLabelReference m_Label;
            [SerializeField] private SceneReference m_Scene;

            [NonSerialized] private bool m_IsInitialized, m_IsLoaded;
            [NonSerialized] AsyncOperationHandle<IList<IResourceLocation>> m_LocationOperation;
            [NonSerialized] AsyncOperationHandle<IList<UnityEngine.Object>> m_ResourceOperation;
            [NonSerialized] private Action<UnityEngine.Object> m_OnCompleted;

            private sealed class ObjectArrayProvider<TObject> : IPromiseProvider<IEnumerable<TObject>>
                where TObject : UnityEngine.Object
            {
                private SceneBindedLabel m_SceneBindedLabel;
                private Action<IEnumerable<TObject>> m_OnCompleted;
                private Action<object> m_OnCompletedUntyped;

                public ObjectArrayProvider(SceneBindedLabel other)
                {
                    m_SceneBindedLabel = other;
                    m_SceneBindedLabel.OnCompleted += M_SceneBindedLabel_OnCompleted;
                }

                private void M_SceneBindedLabel_OnCompleted(UnityEngine.Object obj)
                {
                    IEnumerable<TObject> iter = m_SceneBindedLabel.Result.Select(t => t as TObject);

                    m_OnCompletedUntyped?.Invoke(iter);
                    m_OnCompleted?.Invoke(iter);
                }

                void IPromiseProvider.OnComplete(Action<object> obj)
                {
                    m_OnCompletedUntyped += obj;
                }
                void IPromiseProvider<IEnumerable<TObject>>.OnComplete(Action<IEnumerable<TObject>> obj)
                {
                    m_OnCompleted += obj;
                }
            }

            public bool IsLoaded => m_IsLoaded;
            public IList<UnityEngine.Object> Result
            {
                get
                {
                    if (!IsLoaded) return null;
                    else if (!m_ResourceOperation.IsDone) return null;

                    return m_ResourceOperation.Result;
                }
            }

            public event Action<UnityEngine.Object> OnCompleted
            {
                add
                {
                    if (!IsLoaded)
                    {
                        m_OnCompleted += value;
                        return;
                    }
                    else if (m_ResourceOperation.IsDone)
                    {
                        var result = m_ResourceOperation.Result;
                        for (int i = 0; i < result.Count; i++)
                        {
                            value?.Invoke(result[i]);
                        }
                        return;
                    }

                    m_OnCompleted += value;
                }
                remove
                {
                    m_OnCompleted -= value;
                }
            }

            public bool IsValid() => m_Label.RuntimeKeyIsValid();
            public void Initialize()
            {
                if (m_IsInitialized) return;

                if (!IsValid())
                {
                    PointHelper.LogError(Channel.Collections,
                        $"Fatal Error. ResourceHashMap has invalid {nameof(SceneBindedLabel)} for scene({m_Scene.ScenePath}). This is not allowed. Please remove it or select valid Label.");

                    throw new InvalidOperationException();
                }

                m_LocationOperation = Addressables.LoadResourceLocationsAsync(m_Label);
                m_IsInitialized = true;
            }

            public void LoadResources(Action<UnityEngine.Object> onCompleted)
            {
                if (!m_IsInitialized)
                {
                    Initialize();
                }

                if (IsLoaded)
                {
                    if (m_ResourceOperation.IsDone)
                    {
                        IList<UnityEngine.Object> objects = m_ResourceOperation.Result;
                        for (int i = 0; i < objects.Count; i++)
                        {
                            onCompleted?.Invoke(objects[i]);
                        }
                    }
                    else
                    {
                        m_OnCompleted += onCompleted;
                        m_ResourceOperation.Completed += M_ResourceOperation_Completed;
                    }
                    return;
                }

                m_OnCompleted += onCompleted;
                if (!m_LocationOperation.IsDone)
                {
                    m_LocationOperation.Completed += IfLocationOperation_IsNot_Completed;
                }
                else
                {
                    IList<IResourceLocation> locations = m_LocationOperation.Result;
                    m_ResourceOperation = Addressables.LoadAssetsAsync<UnityEngine.Object>(locations, null);

                    //var oper = Addressables.ResourceManager
                    //    .CreateChainOperation(
                    //        m_ResourceOperation,
                    //        RegisterResourceManagerOperation.Get
                    //        );


                    m_ResourceOperation.Completed += M_ResourceOperation_Completed;
                }

                m_IsLoaded = true;
            }
            public void UnloadResources()
            {
                if (!IsLoaded) return;

                Addressables.Release(m_ResourceOperation);
                Addressables.Release(m_LocationOperation);

                m_LocationOperation = default;
                m_ResourceOperation = default;
                m_OnCompleted = null;

                m_IsLoaded = false;
            }

            public bool Validate(Scene scene)
            {
                return m_Scene.Equals(scene);
            }
            public bool TryLoadResources(Scene scene, Action<UnityEngine.Object> onCompleted)
            {
                if (!m_Scene.Equals(scene))
                {
                    return false;
                }

                LoadResources(onCompleted);
                return true;
            }

            public Promise<IEnumerable<TObject>> GetObjects<TObject>()
                where TObject : UnityEngine.Object
            {
                var provider = new ObjectArrayProvider<TObject>(this);
                return new Promise<IEnumerable<TObject>>(provider);
            }

            private void IfLocationOperation_IsNot_Completed(AsyncOperationHandle<IList<IResourceLocation>> obj)
            {
                m_ResourceOperation = Addressables.LoadAssetsAsync<UnityEngine.Object>(obj.Result, null);
                m_ResourceOperation.Completed += M_ResourceOperation_Completed;
            }
            private void M_ResourceOperation_Completed(AsyncOperationHandle<IList<UnityEngine.Object>> obj)
            {
                IList<UnityEngine.Object> objects = obj.Result;
                for (int i = 0; i < objects.Count; i++)
                {
                    m_OnCompleted?.Invoke(objects[i]);
                }

                m_OnCompleted = null;
            }
        }
        
        [SerializeField] private List<SceneBindedLabel> m_SceneBindedLabels = new List<SceneBindedLabel>();
#endif
        [SerializeField] private List<ResourceList> m_ResourceLists = new List<ResourceList>();

        public IReadOnlyList<AssetPathField> StreamingAssetBundles => m_StreamingAssetBundles;
        public IReadOnlyList<ResourceList> ResourceLists => m_ResourceLists;
        public ResourceList this[int index]
        {
            get
            {
                return m_ResourceLists[index];
            }
        }

        public AssetReference this[AssetIndex index]
        {
            get
            {
                return m_ResourceLists[index.m_Index.x][index.m_Index.y];
            }
        }
        public AssetReference this[string friendlyName]
        {
            get
            {
                AssetReference asset;
                foreach (var list in m_ResourceLists)
                {
                    asset = list[friendlyName];
                    if (asset.IsEmpty()) continue;

                    return asset;
                }

                return AssetReference.Empty;
            }
        }

        public bool TryGetAssetReference(AssetIndex index, out AssetReference asset)
        {
            asset = AssetReference.Empty;
            if (index.IsEmpty()) return false;

            int x = index.m_Index.x, y = index.m_Index.y;
            if (x < 0 || y < 0 || m_ResourceLists.Count <= x)
            {
                return false;
            }

            ResourceList list = m_ResourceLists[x];
            if (list.Count <= y)
            {
                return false;
            }

            asset = list[y];
            return !asset.IsEmpty();
        }

#if UNITY_ADDRESSABLES
        internal void LoadSceneAssets(Scene scene, Action<UnityEngine.Object> onCompleted)
        {
            for (int i = 0; i < m_SceneBindedLabels.Count; i++)
            {
                if (!m_SceneBindedLabels[i].TryLoadResources(scene, null)) continue;

                m_SceneBindedLabels[i].OnCompleted += onCompleted;
                //
            }
            //
        }
        internal void UnloadSceneAssets(Scene scene)
        {
            for (int i = 0; i < m_SceneBindedLabels.Count; i++)
            {
                if (!m_SceneBindedLabels[i].Validate(scene)) continue;

                m_SceneBindedLabels[i].UnloadResources();
            }
        }
#endif
    }
}

#endif