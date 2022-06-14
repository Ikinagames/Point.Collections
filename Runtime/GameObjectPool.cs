// Copyright 2022 Ikina Games
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
#if UNITY_COLLECTIONS
#endif

using UnityEngine;
using Point.Collections;
using System;
using Point.Collections.Buffer;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace Point.Collections
{
    public class GameObjectPool : PointMonobehaviour
    {
        [Serializable]
        public abstract class Pool
        {
            [SerializeField] private string m_FriendlyName = string.Empty;
            //[SerializeField] private GameObject m_Object;
            [SerializeField] private bool m_SpawnAtWorld = false;

            private GameObjectPool m_Parent;
            private ObjectPool<GameObject> m_ObjectPool;

            public Hash FriendlyName => m_FriendlyName.IsNullOrEmpty() ? Hash.Empty : new Hash(m_FriendlyName);
            public abstract GameObject Object { get; }

            internal void Initialize(GameObjectPool parent)
            {
                m_Parent = parent;
                m_ObjectPool = new ObjectPool<GameObject>(
                    Factory,
                    OnGet,
                    OnReserve,
                    null
                    );
            }

            private GameObject Factory()
            {
                GameObject result;
                if (!m_SpawnAtWorld)
                {
                    result = Instantiate(Object, m_Parent.transform);
                }
                else
                {
                    result = Instantiate(Object);
                }
                GameObjectPoolReceiver receiver = result.GetOrAddComponent<GameObjectPoolReceiver>();
                receiver.Parent = this;

                return result;
            }
            private void OnGet(GameObject obj)
            {
                GameObjectPoolReceiver receiver = obj.GetComponent<GameObjectPoolReceiver>();
                receiver.Reserved = false;

                obj.transform.position = m_Parent.transform.position + Object.transform.localPosition;
                obj.SetActive(true);
            }
            private void OnReserve(GameObject obj)
            {
                GameObjectPoolReceiver receiver = obj.GetComponent<GameObjectPoolReceiver>();
                receiver.Reserved = true;

                obj.SetActive(false);
            }

            public GameObject Get() => m_ObjectPool.Get();
            public void Reserve(GameObject obj) => m_ObjectPool.Reserve(obj);
        }
        [Serializable]
        public sealed class PoolDirect : Pool
        {
            [SerializeField] private GameObject m_Object;

            public override GameObject Object => m_Object;
        }
        [Serializable]
        public sealed class PoolReferece : Pool
        {
            [SerializeField] private AssetPathField<GameObject> m_Object;

            public override GameObject Object
            {
                get
                {
                    return m_Object.Asset.Asset as GameObject;
                }
            }
        }

        [FormerlySerializedAs("m_GameObjects")]
        [SerializeField] private ArrayWrapper<PoolDirect> m_DirectReferences = Array.Empty<PoolDirect>();
        [SerializeField] private ArrayWrapper<PoolReferece> m_References = Array.Empty<PoolReferece>();

        private Dictionary<Hash, Pool> m_HashMap = new Dictionary<Hash, Pool>();

        //public IReadOnlyList<Pool> Pools => m_DirectReferences;

#if UNITY_EDITOR
        private void OnValidate()
        {
        }
#endif
        private void Awake()
        {
            for (int i = 0; i < m_DirectReferences.Length; i++)
            {
                m_DirectReferences[i].Initialize(this);

                Hash friendlyName = m_DirectReferences[i].FriendlyName;
                if (!friendlyName.IsEmpty())
                {
                    m_HashMap[friendlyName] = m_DirectReferences[i];
                }
            }
            for (int i = 0; i < m_References.Length; i++)
            {
                m_References[i].Initialize(this);

                Hash friendlyName = m_References[i].FriendlyName;
                if (!friendlyName.IsEmpty())
                {
                    m_HashMap[friendlyName] = m_References[i];
                }
            }
        }

        public int Register(IList<PoolReferece> pools)
        {
            int count = m_References.Count;
            m_References.AddRange(pools);

            return count;
        }

        public GameObject FindObject(string friendlyName)
        {
            m_HashMap.TryGetValue(new Hash(friendlyName), out var value);
            return value != null ? value.Object : null;
        }
        public GameObject FindObject(int index)
        {
            return m_DirectReferences[index].Object;
        }

        public void SpawnAtPosition(string friendlyName) => Spawn(friendlyName);
        public GameObject Spawn(string friendlyName)
        {
            m_HashMap.TryGetValue(new Hash(friendlyName), out var value);

            return value.Get();
        }
        public GameObject Spawn(int index)
        {
            return m_References[index].Get();
        }

        public void Reserve(int index, GameObject obj)
        {
            m_References[index].Reserve(obj);
        }
    }
}

#endif