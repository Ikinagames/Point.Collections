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

using Point.Collections.ResourceControl.LowLevel;
using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Point.Collections.ResourceControl
{
    [BurstCompatible]
    public struct AssetBundleInfo : IValidation, IEquatable<AssetBundleInfo>
    {
        public static AssetBundleInfo Invalid => new AssetBundleInfo(IntPtr.Zero);

        [NativeDisableUnsafePtrRestriction]
        internal unsafe readonly IntPtr m_Pointer;
        private ref InternalAssetBundleInfo Ref
        {
            get
            {
                unsafe
                {
                    InternalAssetBundleInfo* p = (InternalAssetBundleInfo*)m_Pointer.ToPointer();
                    return ref *p;
                }
            }
        }

        public bool IsLoaded
        {
            get
            {
                if (!IsValid())
                {
                    throw new InvalidOperationException();
                }

                return Ref.m_IsLoaded;
            }
        }
        [NotBurstCompatible]
        public AssetBundle AssetBundle
        {
            get
            {
                if (!IsValid())
                {
                    throw new InvalidOperationException();
                }

                if (!Ref.m_IsLoaded) return null;

                return ResourceManager.GetAssetBundle(Ref);
            }
        }
        [NotBurstCompatible]
        public string Name
        {
            get
            {
                if (!IsValid())
                {
                    throw new InvalidOperationException();
                }

                return ResourceAddresses.GetBundleName(Ref);
            }
        }

        internal AssetBundleInfo(IntPtr p)
        {
            m_Pointer = p;
        }

        [NotBurstCompatible]
        public AssetBundle Load() => ResourceManager.LoadAssetBundle(Ref);
        public void Unload() => ResourceManager.UnloadAssetBundle(Ref);

        [NotBurstCompatible]
        public AssetBundleHandler LoadAsync() => ResourceManager.LoadAssetBundleAsync(Ref);

        public bool IsValid() => !Equals(Invalid);
        public bool Equals(AssetBundleInfo other) => m_Pointer.Equals(other.m_Pointer);
    }
}
