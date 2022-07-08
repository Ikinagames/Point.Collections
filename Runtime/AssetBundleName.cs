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
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System;
using Unity.Collections;

namespace Point.Collections
{
    [Serializable]
    public struct AssetBundleName : IEmpty, IEquatable<AssetBundleName>, IEquatable<string>
    {
        [UnityEngine.SerializeField]
        private FixedString128Bytes m_Name;

        public AssetBundleName(string name)
        {
            m_Name = name;
        }

        public bool IsEmpty() => m_Name.Equals(string.Empty);

        public override int GetHashCode() => m_Name.GetHashCode();
        public override string ToString() => m_Name.ToString();

        public bool Equals(AssetBundleName other) => m_Name.Equals(other.m_Name);
        public bool Equals(string other) => m_Name.Equals(other);
        public override bool Equals(object obj)
        {
            if (obj is AssetBundleName t0)
            {
                return Equals(t0);
            }
            else if (obj is string t1) return Equals(t1);

            return false;
        }
    }
}
