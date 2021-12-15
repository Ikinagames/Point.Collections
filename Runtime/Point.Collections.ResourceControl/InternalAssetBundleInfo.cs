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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

using System;
using Unity.Collections;

namespace Point.Collections.ResourceControl
{
    internal struct InternalAssetBundleInfo : IEquatable<InternalAssetBundleInfo>
    {
        internal readonly int m_Index;
        internal bool m_IsLoaded;
        private bool m_IsWebBundle;

        [NotBurstCompatible]
        public string Name => ResourceAddresses.GetBundleName(in this);
        
        internal InternalAssetBundleInfo(int index, bool isWeb)
        {
            m_Index = index;
            m_IsLoaded = false;
            m_IsWebBundle = isWeb;
        }

        public bool Equals(InternalAssetBundleInfo other) => m_Index.Equals(other.m_Index);
    }
}
