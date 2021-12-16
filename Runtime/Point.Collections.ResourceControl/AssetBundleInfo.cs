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
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Point.Collections.ResourceControl
{
    [BurstCompatible]
    public struct AssetBundleInfo : IValidation, IEquatable<AssetBundleInfo>
    {
        public static AssetBundleInfo Invalid => new AssetBundleInfo(IntPtr.Zero, 0);

        [NativeDisableUnsafePtrRestriction]
        internal unsafe readonly IntPtr m_Pointer;
        internal readonly uint m_Generation;

        internal unsafe ref InternalAssetBundleInfo Ref
        {
            get
            {
                InternalAssetBundleInfo* p = (InternalAssetBundleInfo*)m_Pointer.ToPointer();
                p->m_JobHandle.Complete();

                return ref *p;
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

                return ResourceManager.GetAssetBundle(Ref.m_Index).AssetBundle;
            }
        }

        internal AssetBundleInfo(IntPtr p, uint generation)
        {
            m_Pointer = p;
            m_Generation = generation;
        }

        [NotBurstCompatible]
        public AssetBundle Load()
        {
            if (!IsValid())
            {
                throw new Exception();
            }

            return ResourceManager.LoadAssetBundle(ref Ref);
        }
        public void Unload(bool unloadAllLoadedObjects)
        {
#if DEBUG_MODE
            if (!IsValid())
            {
                throw new Exception();
            }

            UnsafeHashMap<Hash, InternalAssetInfo>.Enumerator iter = Ref.m_Assets.GetEnumerator();
            while (iter.MoveNext())
            {
                KeyValue<Hash, InternalAssetInfo> temp = iter.Current;

                if (temp.Value.m_ReferencedCount > 0)
                {
                    Point.LogError(Point.LogChannel.Collections,
                        $"Asset({temp.Key}) has {temp.Value.m_ReferencedCount} of references that didn\'t reserved. " +
                        $"This is not allowed.");
                }
            }
#endif
            ResourceManager.UnloadAssetBundle(ref Ref, unloadAllLoadedObjects);
        }

        public string[] GetAllAssetNames()
        {
            if (!IsLoaded)
            {
                Point.LogError(Point.LogChannel.Collections,
                    $"You\'re trying to get all asset names that didn\'t loaded AssetBundle. " +
                    $"This is not allowed.");

                return Array.Empty<string>();
            }

            var values = Ref.m_Assets.GetValueArray(AllocatorManager.Temp);

            string[] arr = values.Select(other => other.m_Key.ToString()).ToArray();
            values.Dispose();

            return arr;
        }
        public AssetInfo LoadAsset(string key)
        {
            return ResourceManager.LoadAsset(ref Ref, key);
        }
        public void Reserve(AssetInfo asset)
        {
            unsafe
            {
                if (asset.m_BundlePointer != (InternalAssetBundleInfo*)m_Pointer.ToPointer())
                {
                    throw new Exception();
                }

                ResourceManager.Reserve(asset.m_BundlePointer, asset.m_Key);
            }
        }

        public bool IsValid() => !Equals(Invalid);
        public bool Equals(AssetBundleInfo other) => m_Pointer.Equals(other.m_Pointer);
    }
}
