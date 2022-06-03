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
    public class ItemComponent : PointMonobehaviour, IItem, IItemCallbacks
    {
        [SerializeField] private string m_Name = string.Empty;
        [SerializeField] private Hash m_Hash = Hash.NewHash();

        [SerializeField] private int2 m_Size = 1;
        [SerializeField] private float m_Weight = 1;
        [SerializeField] private int m_MaxDuplications = 1;

        [NonSerialized] private Hash m_InventoryHash;
        [NonSerialized] private int2 m_Coordinate;

        public string Name => m_Name;
        public Hash Hash => m_Hash;

        public int2 Size => m_Size;
        public float Weight => m_Weight;
        public int MaxDuplications => m_MaxDuplications;

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

        void IItemCallbacks.OnItemAdded(IInventory inventory, int2 coord)
        {
            OnItemAdded(inventory, coord);
        }
        void IItemCallbacks.OnItemRemove(IInventory inventory, int2 coord)
        {
            OnItemRemove(inventory, coord);
        }

        #endregion

        protected virtual void OnItemAdded(IInventory inventory, int2 coord)
        {
            gameObject.SetActive(false);
        }
        protected virtual void OnItemRemove(IInventory inventory, int2 coord)
        {
            gameObject.SetActive(true);
        }

        public bool Equals(IItem other) => m_Hash.Equals(other.Hash);
    }
}

#endif