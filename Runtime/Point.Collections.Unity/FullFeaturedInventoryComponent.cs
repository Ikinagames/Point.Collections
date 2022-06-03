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

using System;
using System.Collections.Generic;

namespace Point.Collections.Unity
{
    public class FullFeaturedInventoryComponent : InventoryComponentBase
    {
        [NonSerialized] private FullFeaturedInventory m_Inventory;

        public override Hash Hash => m_Inventory.Hash;
        public override int Count => m_Inventory.Count;

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

                IItemCallbacks callbacks = item;
                callbacks.OnItemAdded(this);
            }
        }
#endif

        public override void Add(IItem item)
        {
            m_Inventory.Add(item);
        }
        public override bool Remove(IItem item)
        {
            bool result = m_Inventory.Remove(item);
            return result;
        }

        public override IEnumerable<IItem> GetItems() => m_Inventory.GetItems();
        public override IEnumerable<IItem> GetItems(Predicate<IItem> predicate) => m_Inventory.GetItems(predicate);
    }
}

#endif