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

namespace Point.Collections
{
    public class GameObjectPool : PointMonobehaviour
    {
        [Serializable]
        public sealed class Pool
        {
            [SerializeField] private GameObject m_Object;
            [SerializeField] private bool m_SpawnAtWorld = false;

            private GameObjectPool m_Parent;
            private ObjectPool<GameObject> m_ObjectPool;

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
                    result = Instantiate(m_Object, m_Parent.transform);
                }
                else
                {
                    result = Instantiate(m_Object);
                }
                GameObjectPoolReceiver receiver = result.GetOrAddComponent<GameObjectPoolReceiver>();
                receiver.Parent = this;

                return result;
            }
            private void OnGet(GameObject obj)
            {
                GameObjectPoolReceiver receiver = obj.GetComponent<GameObjectPoolReceiver>();
                receiver.Reserved = false;

                obj.transform.position = m_Object.transform.position;
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

        [SerializeField] private ArrayWrapper<Pool> m_GameObjects = Array.Empty<Pool>();

        public IReadOnlyList<Pool> Pools => m_GameObjects;

#if UNITY_EDITOR
        private void OnValidate()
        {
        }
#endif
        private void Awake()
        {
            for (int i = 0; i < m_GameObjects.Length; i++)
            {
                m_GameObjects[i].Initialize(this);
            }
        }

        public void Spawn(int index)
        {
            m_GameObjects[index].Get();
        }
    }
}

#endif