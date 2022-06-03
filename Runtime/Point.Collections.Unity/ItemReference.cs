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

using Unity.Mathematics;
using UnityEngine;

namespace Point.Collections.Unity
{
    [CreateAssetMenu(menuName = "Point/Inventory/Create Item Reference")]
    public class ItemReference : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] protected string m_ItemName = string.Empty;
        [SerializeField] protected Hash m_Hash;

        [Space]
        [SerializeField] protected int2 m_Size = 1;
        [SerializeField] protected float m_Weight = 1;
        [SerializeField] protected int m_MaxDuplications = 1;

        public string Name => m_ItemName;
        public Hash Hash => m_Hash;

        public int2 Size => m_Size;
        public float Weight => m_Weight;
        public int MaxDuplications => m_MaxDuplications;

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
    }
}

#endif