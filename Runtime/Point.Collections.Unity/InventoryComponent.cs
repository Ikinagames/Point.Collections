﻿// Copyright 2022 Ikina Games
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
using System;
using System.Collections.Generic;

namespace Point.Collections.Unity
{
    public class InventoryComponent : PointMonobehaviour, IInventory, ISerializationCallbackReceiver
    {
        [SerializeField] protected Vector2Int m_Size;
        [SerializeField] protected float m_MaximumWeight = 100;
        [SerializeField] protected float m_DefaultWeight;

        [Space]
        [SerializeField] protected List<ItemComponent> m_Items = new List<ItemComponent>();

        [NonSerialized] private FullFeaturedInventory m_Inventory;

        public Hash Hash => m_Inventory.Hash;
        public int Count => m_Inventory.Count;

        protected virtual void Awake()
        {
            m_Inventory = new FullFeaturedInventory(m_Size.x, m_Size.y, m_MaximumWeight, m_DefaultWeight);
            foreach (var item in m_Items)
            {
                Add(item);
            }
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            m_Inventory = new FullFeaturedInventory(m_Size.x, m_Size.y, m_MaximumWeight, m_DefaultWeight);
            foreach (var item in m_Items)
            {
                if (item == null) continue;

                //Add(item);
                //if (item.transform.parent != transform)
                //{
                //    item.transform.parent = transform;
                //    item.transform.localPosition = Vector3.zero;
                //}

                IItemCallbacks callbacks = item;
                //m_Inventory.Add(new Item(item));
                callbacks.OnItemAdded(this);
            }
        }
#endif

        #region ISerializationCallbackReceiver Implements

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (m_Size.Equals(Vector2Int.zero))
            {
                m_Size = new Vector2Int(1, 1);
            }
            if (m_MaximumWeight <= 0) m_MaximumWeight = 100;

            //if (m_Inventory != null)
            //{
            //    foreach (var item in m_Inventory.Items)
            //    {
            //        if (item is ItemComponent component)
            //        {

            //        }
            //    }
            //}
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
        }

        #endregion

        public void Add(IItem item)
        {
            m_Inventory.Add(item);
        }
        public bool Remove(IItem item)
        {
            bool result = m_Inventory.Remove(item);
            return result;
        }

        public IEnumerable<IItem> GetItems() => m_Inventory.GetItems();
        public IEnumerable<IItem> GetItems(Predicate<IItem> predicate) => m_Inventory.GetItems(predicate);
    }
}

#endif