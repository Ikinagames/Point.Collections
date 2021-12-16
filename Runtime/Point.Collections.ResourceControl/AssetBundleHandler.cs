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
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Point.Collections.ResourceControl
{
    [BurstCompatible]
    public struct AssetBundleHandler : IValidation
    {
        public static AssetBundleHandler Invalid => default(AssetBundleHandler);

        [NativeDisableUnsafePtrRestriction]
        internal unsafe readonly IntPtr m_Pointer;
        private readonly bool m_ExpectedLoadState;
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

        public bool IsDone
        {
            get
            {
                if (!IsValid())
                {
                    return true;
                }

                return Ref.m_IsLoaded == m_ExpectedLoadState;
            }
        }
        [NotBurstCompatible]
        public AssetBundle AssetBundle
        {
            get
            {
                if (!IsValid())
                {
                    return null;
                }

                if (!Ref.m_IsLoaded) return null;

                return ResourceAddresses.GetAssetBundle(Ref);
            }
        }

        internal unsafe AssetBundleHandler(InternalAssetBundleInfo* p, bool expectedLoadState)
        {
            m_Pointer = (IntPtr)p;
            m_ExpectedLoadState = expectedLoadState;
        }

        public bool IsValid() => !m_Pointer.Equals(IntPtr.Zero);
    }
}
