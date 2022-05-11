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
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Point.Collections.ResourceControl
{
    //////////////////////////////////////////////////////////////////////////////////////////
    /*                                   Critical Section                                   */
    /*                                       수정금지                                        */
    /*                                                                                      */
    /*                          Unsafe pointer를 포함하는 코드입니다                          */
    //////////////////////////////////////////////////////////////////////////////////////////

    /// <summary>
    /// <see cref="AssetBundleInfo"/> 가 담고있는 에셋의 정보입니다.
    /// </summary>
    [BurstCompatible]
    [Guid("b92cc9a9-b577-4759-b623-d794bd86d430")]
    public struct AssetInfo : IValidation, IEquatable<AssetInfo>, IDisposable
    {
        public static AssetInfo Invalid => default(AssetInfo);

        [NativeDisableUnsafePtrRestriction, NonSerialized]
        internal readonly UnsafeReference<UnsafeAssetBundleInfo> m_BundlePointer;
        [NonSerialized]
        internal readonly Hash m_Key;
        private readonly Timer m_CreationTime;
        private Timer m_LastUsedTime;

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

                ResourceManager.AssetContainer bundleInfo = ResourceManager.GetAssetBundle(m_BundlePointer.Value.index);

                m_LastUsedTime = Timer.Start();
                return bundleInfo.m_Assets[m_Key];
            }
        }

        internal unsafe AssetInfo(UnsafeReference<UnsafeAssetBundleInfo> bundle, Hash key)
        {
            this = default(AssetInfo);

            m_BundlePointer = bundle;
            this.m_Key = key;

            m_CreationTime = Timer.Start();
            m_LastUsedTime = m_CreationTime;
        }

        public float GetElapsedTimeSinceLastUsage()
        {
            return m_LastUsedTime.ElapsedTime;
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
            if (!m_BundlePointer.IsCreated) return false;

            bundle = ResourceManager.GetAssetBundle(m_BundlePointer.Value.index);

            return bundle.m_Assets.ContainsKey(m_Key);
        }
        public bool Equals(AssetInfo other)
        {
            return m_BundlePointer.Equals(other.m_BundlePointer) && m_Key.Equals(other.m_Key);
        }

        void IDisposable.Dispose()
        {
            this.ThrowIfIsNotValid();

            ResourceManager.Reserve(m_BundlePointer, in this);
        }

        [NotBurstCompatible]
        public override string ToString() => m_Key.ToString();
    }

    //////////////////////////////////////////////////////////////////////////////////////////
    /*                                End of Critical Section                               */
    //////////////////////////////////////////////////////////////////////////////////////////
}

#endif