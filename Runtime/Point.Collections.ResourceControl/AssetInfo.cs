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

using Point.Collections.Buffer.LowLevel;
using Point.Collections.ResourceControl.LowLevel;
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Point.Collections.ResourceControl
{
    /// <summary>
    /// <see cref="AssetBundleInfo"/> 가 담고있는 에셋의 정보입니다.
    /// </summary>
    [BurstCompatible]
    [Guid("b92cc9a9-b577-4759-b623-d794bd86d430")]
    public readonly struct AssetInfo : IValidation, IEquatable<AssetInfo>, IDisposable
    {
        public static AssetInfo Invalid => default(AssetInfo);

        [NativeDisableUnsafePtrRestriction, NonSerialized]
        internal readonly UnsafeReference<UnsafeAssetBundleInfo> bundlePointer;
        [NonSerialized]
        internal readonly Hash key;

        /// <summary>
        /// <seealso cref="UnityEngine.AssetBundle"/> 내 에셋.
        /// </summary>
        /// <remarks>
        /// 반환된 객체는 <seealso cref="ResourceManager"/> 에서 <see cref="UnityEngine.AssetBundle"/> 단위로 관리되므로, 
        /// <see cref="UnityEngine.Object.Destroy(UnityEngine.Object)"/> 등과 같은 행위가 절때로 일어나서는 안됩니다.
        /// </remarks>
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
        /// <summary>
        /// 에셋의 레퍼런스를 반환합니다.
        /// </summary>
        public void Reserve()
        {
            ((IDisposable)this).Dispose();
        }
        /// <summary>
        /// 유효한 에셋인지 반환합니다.
        /// </summary>
        /// <returns></returns>
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

        void IDisposable.Dispose()
        {
            this.ThrowIfIsNotValid();

            ResourceManager.Reserve(bundlePointer, in this);
        }
    }
}
