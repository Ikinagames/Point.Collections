﻿// Copyright 2021 Ikina Games
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

using Point.Collections.ResourceControl.LowLevel;
using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Point.Collections.ResourceControl
{
    public struct AssetInfo : IValidation, IEquatable<AssetInfo>
    {
        [NativeDisableUnsafePtrRestriction]
        internal unsafe readonly UnsafeAssetBundleInfo* bundlePointer;
        internal readonly Hash key;

        public UnityEngine.Object Asset
        {
            get
            {
                this.ThrowIfIsNotValid();

                unsafe
                {
                    ResourceManager.AssetContainer bundleInfo = ResourceManager.GetAssetBundle(bundlePointer->index);
                    return bundleInfo.m_Assets[key];
                }
            }
        }

        internal unsafe AssetInfo(UnsafeAssetBundleInfo* bundle, Hash key)
        {
            bundlePointer = bundle;
            this.key = key;
        }

        public void Reserve()
        {
            this.ThrowIfIsNotValid();

            unsafe
            {
                ResourceManager.Reserve(bundlePointer, in this);
            }
        }

        public bool IsValid()
        {
            ResourceManager.AssetContainer bundle;
            unsafe
            {
                if (bundlePointer == null) return false;

                bundle = ResourceManager.GetAssetBundle(bundlePointer->index);
            }

            return bundle.m_Assets.ContainsKey(key);
        }
        public bool Equals(AssetInfo other)
        {
            unsafe
            {
                return bundlePointer == other.bundlePointer && key.Equals(other.key);
            }
        }
    }
}
