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
        public static AssetBundleInfo Invalid => default(AssetBundleInfo);

        [NativeDisableUnsafePtrRestriction]
        internal unsafe readonly UnsafeAssetBundleInfo* pointer;
        internal readonly uint generation;

        internal unsafe ref UnsafeAssetBundleInfo Ref
        {
            get
            {
                pointer->m_JobHandle.Complete();

                return ref *pointer;
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

                return ResourceManager.GetAssetBundle(Ref.index).AssetBundle;
            }
        }

        internal unsafe AssetBundleInfo(UnsafeAssetBundleInfo* p, uint generation)
        {
            pointer = p;
            this.generation = generation;
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
        public AsyncOperation LoadAsync()
        {
            if (!IsValid())
            {
                throw new Exception();
            }

            unsafe
            {
                return ResourceManager.LoadAssetBundleAsync(pointer);
            }
        }

        public void Unload(bool unloadAllLoadedObjects)
        {
#if DEBUG_MODE
            if (!IsValid())
            {
                throw new Exception();
            }

            if (!Ref.assets.IsCreated)
            {
                throw new Exception();
            }

            UnsafeHashMap<Hash, UnsafeAssetInfo>.Enumerator iter = Ref.assets.GetEnumerator();
            while (iter.MoveNext())
            {
                KeyValue<Hash, UnsafeAssetInfo> temp = iter.Current;

                if (temp.Value.referencedCount > 0)
                {
                    Point.LogError(Point.LogChannel.Collections,
                        $"Asset({temp.Key}) has {temp.Value.referencedCount} of references that didn\'t reserved. " +
                        $"This is not allowed.");
                }
            }
#endif
            ResourceManager.UnloadAssetBundle(ref Ref, unloadAllLoadedObjects);
        }

        [NotBurstCompatible]
        public string[] GetAllAssetNames()
        {
            if (!IsLoaded)
            {
                Point.LogError(Point.LogChannel.Collections,
                    $"You\'re trying to get all asset names that didn\'t loaded AssetBundle. " +
                    $"This is not allowed.");

                return Array.Empty<string>();
            }

            var values = Ref.assets.GetValueArray(AllocatorManager.Temp);

            string[] arr = values.Select(other => other.m_Key.ToString()).ToArray();
            values.Dispose();

            return arr;
        }
        public AssetInfo LoadAsset(in FixedString4096Bytes key)
        {
            return ResourceManager.LoadAsset(ref Ref, in key);
        }
        public void Reserve(ref AssetInfo asset)
        {
            unsafe
            {
                if (asset.bundlePointer != pointer)
                {
                    throw new Exception();
                }

                Hash key = asset.key;

                //ResourceManager.Reserve(asset.m_BundlePointer, asset.key);
                BurstResourceFunction.reserve_assets(pointer, &key, 1);
            }
        }

        public bool IsValid() => !Equals(Invalid);
        public bool Equals(AssetBundleInfo other)
        {
            unsafe
            {
                return pointer == other.pointer;
            }
        }
    }
}
