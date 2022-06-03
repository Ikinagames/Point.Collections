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
using Unity.Mathematics;
using System;

namespace Point.Collections.Unity
{
    public class ItemComponent : PointMonobehaviour, IItem, IItemCallbacks, ISerializationCallbackReceiver
    {
        [SerializeField] protected ItemReference m_ItemReference = null;

        [Space]
        [SerializeField] protected string m_ItemName = string.Empty;
        [SerializeField] protected Hash m_Hash;

        [Space]
        [SerializeField] protected int2 m_Size = 1;
        [SerializeField] protected float m_Weight = 1;
        [SerializeField] protected int m_MaxDuplications = 1;

        [NonSerialized] private Hash m_InventoryHash;
        [NonSerialized] private int2 m_Coordinate;

        public string Name => m_ItemReference != null ? m_ItemReference.Name : m_ItemName;
        public Hash Hash => m_ItemReference != null ? m_ItemReference.Hash : m_Hash;

        public int2 Size => m_ItemReference != null ? m_ItemReference.Size : m_Size;
        public float Weight => m_ItemReference != null ? m_ItemReference.Weight : m_Weight;
        public int MaxDuplications => m_ItemReference != null ? m_ItemReference.MaxDuplications : m_MaxDuplications;

        #region IItem Implements

        bool IItem.IsInInventory(Hash inventoryHash)
        {
            if (m_InventoryHash.IsEmpty()) return false;

            return m_InventoryHash.Equals(inventoryHash);
        }
        void IItem.SetInventoryCoordinate(Hash inventoryHash, int2 coord)
        {
            m_InventoryHash = inventoryHash;
            m_Coordinate = coord;
        }
        int2 IItem.GetInventoryCoordinate() => m_Coordinate;

        #endregion

        #region IItemCallbacks Implements

        void IItemCallbacks.OnItemAdded(IInventory inventory)
        {
            OnItemAdded(inventory);
        }
        void IItemCallbacks.OnItemRemove(IInventory inventory)
        {
            OnItemRemove(inventory);
        }

        #endregion

        #region ISerializationCallbackReceiver Implements

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (m_Hash.IsEmpty()) m_Hash = Hash.NewHash();

            if (m_Size.Equals(0)) m_Size = 1;

            if (m_MaxDuplications <= 0) m_MaxDuplications = 1;
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
        }

        #endregion

        public bool IsInInventory()
        {
            return !m_InventoryHash.IsEmpty();
        }

        protected virtual void OnItemAdded(IInventory inventory)
        {
            gameObject.SetActive(false);

            if (inventory is FullFeaturedInventoryComponent component)
            {
                transform.parent = component.transform;
            }
        }
        protected virtual void OnItemRemove(IInventory inventory)
        {
            gameObject.SetActive(true);

            if (inventory is FullFeaturedInventoryComponent component)
            {
                transform.localPosition = Vector3.zero;
                transform.parent = null;
            }
        }

        public bool Equals(IItem other) => Hash.Equals(other.Hash);
    }
    public struct Item : IItem
    {
        public string name;
        public Hash hash;
        
        public int2 size;
        public float weight;
        public int maxDuplications;

        public Hash inventoryHash;
        public int2 coordinate;

        string IItem.Name => name;
        Hash IItem.Hash => hash;

        int2 IItem.Size => size;
        float IItem.Weight => weight;
        int IItem.MaxDuplications => maxDuplications;

        public Item(ItemComponent component)
        {
            this = default(Item);

            name = component.Name;
            hash = component.Hash;

            size = component.Size;
            weight = component.Weight;
            maxDuplications = component.MaxDuplications;
        }

        bool IItem.IsInInventory(Hash inventoryHash)
        {
            if (inventoryHash.IsEmpty()) return false;

            return inventoryHash.Equals(inventoryHash);
        }
        void IItem.SetInventoryCoordinate(Hash inventoryHash, int2 coord)
        {
            this.inventoryHash = inventoryHash;
            this.coordinate = coord;
        }
        int2 IItem.GetInventoryCoordinate() => coordinate;

        public bool Equals(IItem other) => hash.Equals(other.Hash);
    }
}

#endif