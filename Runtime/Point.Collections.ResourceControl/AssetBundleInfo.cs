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

#if UNITY_2020_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

using Point.Collections.Buffer.LowLevel;
using Point.Collections.ResourceControl.LowLevel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Point.Collections.ResourceControl
{
    /// <summary>
    /// <see cref="UnityEngine.AssetBundle"/> 의 정보입니다.
    /// </summary>
    [BurstCompatible]
    [Guid("42f15dc1-0626-4c38-84f8-641a3740fd0b")]
    public struct AssetBundleInfo : IValidation, IEquatable<AssetBundleInfo>
    {
        public static AssetBundleInfo Invalid => default(AssetBundleInfo);

        internal readonly UnsafeReference<UnsafeAssetBundleInfo> pointer;
        internal readonly uint generation;

        internal unsafe ref UnsafeAssetBundleInfo Ref
        {
            get
            {
                pointer.Value.m_JobHandle.Complete();

                return ref pointer.Value;
            }
        }

        //public AssetInfo this[FixedString512Bytes key]
        //{
        //    get
        //    {

        //    }
        //}
        public bool IsLoaded
        {
            get
            {
                this.ThrowIfIsNotValid();

                return Ref.loaded;
            }
        }
        [NotBurstCompatible]
        public AssetBundle AssetBundle
        {
            get
            {
                this.ThrowIfIsNotValid();

                if (!Ref.loaded) return null;

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
            this.ThrowIfIsNotValid();

            if (Ref.loaded)
            {
                PointHelper.LogError(LogChannel.Collections,
                    $"This Assetbundle({AssetBundle.name}) already loaded but you trying to override. " +
                    $"This is not allowed.");

                return AssetBundle;
            }

            unsafe
            {
                return ResourceManager.LoadAssetBundle(pointer);
            }
        }
        [NotBurstCompatible]
        public AsyncOperation LoadAsync()
        {
            this.ThrowIfIsNotValid();

            if (Ref.loaded)
            {
                PointHelper.LogError(LogChannel.Collections,
                    $"This Assetbundle({AssetBundle.name}) already loaded but you trying to override. " +
                    $"This is not allowed.");

                return null;
            }

            unsafe
            {
                return ResourceManager.LoadAssetBundleAsync(pointer);
            }
        }

        /// <summary>
        /// 에셋 번들을 메모리에서 해제합니다.
        /// </summary>
        /// <param name="unloadAllLoadedObjects">이 번들을 통해 로드된 모든 객체도
        /// 해제할지 설정합니다.</param>
        public void Unload(bool unloadAllLoadedObjects)
        {
            this.ThrowIfIsNotValid();

#if DEBUG_MODE
            if (!Ref.assets.IsCreated)
            {
                throw new Exception();
            }

            for (int i = 0; i < Ref.assets.Length; i++)
            {
                if (Ref.assets[i].checkSum != 0)
                {
                    PointHelper.LogError(LogChannel.Collections,
                        $"Asset({Ref.assets[i].key}) has references that didn\'t reserved. " +
                        $"This is not allowed.");
                }
            }
#endif
            ResourceManager.UnloadAssetBundle(ref Ref, unloadAllLoadedObjects);
        }

        /// <summary>
        /// 이 에셋 번들 내 모든 에셋의 이름을 반환합니다.
        /// </summary>
        /// <remarks>
        /// <seealso cref="ResourceManager"/> 에서는 여기서 반환한 
        /// 에셋의 이름(에디터상 에셋의 상대 경로를 의미합니다. Assets/.../)을 키 값으로 <seealso cref="LoadAsset(in FixedString4096Bytes)"/> 등에 
        /// 사용 될 수 있습니다.
        /// </remarks>
        /// <returns></returns>
        [NotBurstCompatible]
        public string[] GetAllAssetNames()
        {
            this.ThrowIfIsNotValid();

            if (!IsLoaded)
            {
                PointHelper.LogError(LogChannel.Collections,
                    $"You\'re trying to get all asset names that didn\'t loaded AssetBundle. " +
                    $"This is not allowed.");

                return Array.Empty<string>();
            }

            var assets = Ref.assets;
            string[] arr = new string[assets.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = assets[i].key.ToString();
            }

            return arr;
        }
        /// <inheritdoc cref="GetAllAssetNames"/>
        [NotBurstCompatible]
        public IEnumerable<FixedString4096Bytes> GetAllAssetNamesWithoutAllocation()
        {
            this.ThrowIfIsNotValid();

            if (!IsLoaded)
            {
                PointHelper.LogError(LogChannel.Collections,
                    $"You\'re trying to get all asset names that didn\'t loaded AssetBundle. " +
                    $"This is not allowed.");

                yield break;
            }

            for (int i = 0; i < Ref.assets.Length; i++)
            {
                yield return Ref.assets[i].key;
            }
        }

        public bool HasAsset(in Hash key)
        {
            this.ThrowIfIsNotValid();

            return ResourceManager.HasAsset(pointer, in key);
        }
        public bool HasAsset(in FixedString4096Bytes key)
        {
            this.ThrowIfIsNotValid();

            return ResourceManager.HasAsset(pointer, in key);
        }
        /// <summary>
        /// 에셋을 로드합니다.
        /// </summary>
        /// <remarks>
        /// 로드되어 반환된 <see cref="AssetInfo"/> 는 사용이 끝난 후 반드시 <see cref="Reserve(in AssetInfo)"/> 를 통해 반환되어야합니다.
        /// </remarks>
        /// <param name="key"></param>
        /// <returns></returns>
        public AssetInfo LoadAsset(in Hash key)
        {
            this.ThrowIfIsNotValid();

            return ResourceManager.LoadAsset(pointer, in key);
        }
        /// <inheritdoc cref="LoadAsset(in Hash)"/>
        /// <param name="key"></param>
        /// <returns></returns>
        public AssetInfo LoadAsset(in FixedString4096Bytes key)
        {
            this.ThrowIfIsNotValid();

            return ResourceManager.LoadAsset(pointer, in key);
        }
        /// <summary>
        /// 에셋을 반환합니다.
        /// </summary>
        /// <param name="asset"></param>
        /// <exception cref="Exception"></exception>
        public void Reserve(in AssetInfo asset)
        {
            this.ThrowIfIsNotValid();

            if (!asset.bundlePointer.Equals(pointer))
            {
                throw new Exception();
            }

            ResourceManager.Reserve(pointer, in asset);
        }

        public bool IsValid() => !Equals(Invalid);
        public bool Equals(AssetBundleInfo other) => pointer.Equals(other.pointer);
    }
}

#endif