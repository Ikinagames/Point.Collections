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
using Unity.Mathematics;

namespace Point.Collections.Unity
{
    public class FullFeaturedInventory : InventoryBase
    {
        private sealed class Entry
        {
            private IItem m_Item;
            private int2 m_OriginCoordinate;
            private List<IItem> m_Items;

            public IItem Item => m_Item;
            public int2 OriginCoordinate => m_OriginCoordinate;

            public Entry(IItem item, int2 startIndex)
            {
                m_Item = item;
                m_OriginCoordinate = startIndex;
                m_Items = new List<IItem>();
            }

            public bool Acceptable(IItem item)
            {
                if (m_Item == null) return true;

                if (!m_Item.Hash.Equals(item.Hash))
                {
                    return false;
                }

                int count = 1 + m_Items.Count;
                if (count <= item.MaxDuplications)
                {
                    return true;
                }
                return false;
            }

            public void Add(IItem item, int2 startIndex)
            {
                if (m_Item == null)
                {
                    m_Item = item;
                }
                else
                {
                    m_Items.Add(item);
                }

                m_OriginCoordinate = startIndex;
            }
            public void Remove(IItem item)
            {
                if (m_Item.Equals(item))
                {
                    m_Item = null;
                    return;
                }

                m_Items.Remove(item);
            }
        }

        private List<IItem> m_Items = new List<IItem>();
        private Entry[,] m_Slots;

        private float m_MaximumWeight, m_Weight;
        private int m_Count;

        public override int Count => m_Count;
        public override IReadOnlyList<IItem> Items => m_Items;

        public FullFeaturedInventory(int x, int y, float maximumWeight, float defaultWeight) : base()
        {
            m_Slots = new Entry[x, y];

            m_MaximumWeight = maximumWeight;
            m_Weight = math.min(m_MaximumWeight, defaultWeight);
        }

        private bool TryFindEmptySpaceCoord(IItem item, out int2 coord)
        {
            coord = 0;

            int x = m_Slots.GetLength(0), y = m_Slots.GetLength(1);
            for (int yy = 0; yy < y; yy++)
            {
                for (int xx = 0; xx < x; xx++)
                {
                    coord = new int2(xx, yy);
                    if (!IsSuitable(m_Slots, x, y, coord, item)) continue;

                    return true;
                }
            }

            return false;
        }
        private static bool IsSuitable(Entry[,] slots, int x, int y, int2 coord, IItem item)
        {
            int maxX = coord.x + item.Size.x, maxY = coord.y + item.Size.y;
            for (int yy = coord.y; yy < y && y < maxY; yy++)
            {
                for (int xx = coord.x; xx < x && x < maxX; xx++)
                {
                    if (slots[xx, yy] != null && 
                        !slots[xx, yy].Acceptable(item))
                    {
                        return false;
                    }
                }
            }

            return true;
        }
        private static bool IsExceedingWeight(FullFeaturedInventory inventory, IItem item)
        {
            float sum = inventory.m_Weight + item.Weight;

            if (inventory.m_MaximumWeight < sum) return true;
            return false;
        }

        private static void InsertItemAt(FullFeaturedInventory inventory, int2 coord, IItem item)
        {
            int2 size = item.Size;
            for (int yy = 0; yy < size.y; yy++)
            {
                for (int xx = 0; xx < size.x; xx++)
                {
                    int2 targetCoord = coord + new int2(xx, yy);

                    if (inventory.m_Slots[targetCoord.x, targetCoord.y] == null)
                    {
                        inventory.m_Slots[targetCoord.x, targetCoord.y] = new Entry(item, coord);
                        continue;
                    }

                    inventory.m_Slots[targetCoord.x, targetCoord.y].Add(item, coord);
                }
            }
        }
        private static void RemoveItemAt(FullFeaturedInventory inventory, int2 coord, IItem item)
        {
            Entry entry = inventory.m_Slots[coord.x, coord.y];
            coord = entry.OriginCoordinate;

            int2 size = item.Size;

            for (int yy = 0; yy < size.y; yy++)
            {
                for (int xx = 0; xx < size.x; xx++)
                {
                    int2 targetCoord = coord + new int2(xx, yy);
                    inventory.m_Slots[targetCoord.x, targetCoord.y].Remove(item);
                }
            }
        }

        public override void Add(IItem item)
        {
            if (IsExceedingWeight(this, item))
            {
                "exceed weight".ToLog();
                return;
            }
            if (!TryFindEmptySpaceCoord(item, out int2 coord))
            {
                "?? is full".ToLog();
                return;
            }

            InsertItemAt(this, coord, item);
            item.SetInventoryCoordinate(Hash, coord);
            m_Items.Add(item);

            m_Weight += item.Weight;
            m_Count++;

            if (item is IItemCallbacks callbacks)
            {
                callbacks.OnItemAdded(this);
            }
            OnItemAdded(item);
        }
        public override bool Remove(IItem item)
        {
            if (!item.IsInInventory(Hash))
            {
                "validate falid.".ToLog();
                return false;
            }

            int2 coord = item.GetInventoryCoordinate();
            if (item is IItemCallbacks callbacks)
            {
                callbacks.OnItemRemove(this);
            }
            OnItemRemove(item);

            RemoveItemAt(this, coord, item);
            item.SetInventoryCoordinate(Hash.Empty, -1);
            m_Items.Remove(item);

            m_Weight -= item.Weight;
            m_Count--;

            return true;
        }
        public override IEnumerable<IItem> GetItems() => m_Items;
        public override IEnumerable<IItem> GetItems(Predicate<IItem> predicate)
        {
            for (int i = 0; i < m_Items.Count; i++)
            {
                if (predicate.Invoke(m_Items[i])) yield return m_Items[i];
            }
        }

        protected virtual void OnItemAdded(IItem item) { }
        protected virtual void OnItemRemove(IItem item) { }
    }
}

#endif