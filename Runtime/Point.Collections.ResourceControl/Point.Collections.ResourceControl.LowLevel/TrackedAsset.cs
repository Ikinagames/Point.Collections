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

using UnityEngine;
using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Point.Collections.ResourceControl.LowLevel
{
    //[Serializable]
    //public struct TrackedAsset : IValidation
    //{
    //    [NativeDisableUnsafePtrRestriction]
    //    private unsafe InternalAssetBundleInfo* m_BundlePointer;
    //    private unsafe ref InternalAssetBundleInfo BundleRef => ref *m_BundlePointer;

    //    public bool IsLoaded
    //    {
    //        get
    //        {
    //            if (!IsValid())
    //            {
    //                Point.LogError(Point.LogChannel.Collections,
    //                    $"You are trying to un invalid tracked asset. This is not allowed.");

    //                return false;
    //            }

    //            if (!BundleRef.m_IsLoaded) return false;


    //            return true;
    //        }
    //    }

    //    public bool IsValid()
    //    {
    //        unsafe
    //        {
    //            if (m_BundlePointer == null) return false;
    //        }

    //        return true;
    //    }
    //}
}
