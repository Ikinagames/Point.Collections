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

using Point.Collections.Buffer.LowLevel;
using Point.Collections.ResourceControl.LowLevel;
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Point.Collections.ResourceControl
{
    [BurstCompatible]
    [Guid("b92cc9a9-b577-4759-b623-d794bd86d430")]
    public readonly struct AssetInfo : IValidation, IEquatable<AssetInfo>
    {
        public static AssetInfo Invalid => default(AssetInfo);

        [NativeDisableUnsafePtrRestriction, NonSerialized]
        internal readonly UnsafeReference<UnsafeAssetBundleInfo> bundlePointer;
        [NonSerialized]
        internal readonly Hash key;

        public UnityEngine.Object Asset
        {
            get
            {
                this.ThrowIfIsNotValid();

                ResourceManager.AssetContainer bundleInfo = ResourceManager.GetAssetBundle(bundlePointer.Value.index);
                return bundleInfo.m_Assets[key];
            }
        }

        internal unsafe AssetInfo(UnsafeReference<UnsafeAssetBundleInfo> bundle, Hash key)
        {
            bundlePointer = bundle;
            this.key = key;
        }

        public void Reserve()
        {
            this.ThrowIfIsNotValid();

            ResourceManager.Reserve(bundlePointer, in this);
        }

        public bool IsValid()
        {
            ResourceManager.AssetContainer bundle;
            if (!bundlePointer.IsCreated) return false;

            bundle = ResourceManager.GetAssetBundle(bundlePointer.Value.index);

            return bundle.m_Assets.ContainsKey(key);
        }
        public bool Equals(AssetInfo other)
        {
            return bundlePointer.Equals(other.bundlePointer) && key.Equals(other.key);
        }
    }
}
