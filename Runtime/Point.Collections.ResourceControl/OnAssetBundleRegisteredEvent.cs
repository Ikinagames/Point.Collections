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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#if UNITY_2019 && !UNITY_2020_1_OR_NEWER
#define UNITYENGINE_OLD
#else
#if UNITY_MATHEMATICS
#endif
#endif

#if !UNITYENGINE_OLD

using Point.Collections.Events;
#if UNITY_ADDRESSABLES
using UnityEngine.ResourceManagement.AsyncOperations;
#endif

namespace Point.Collections.ResourceControl
{
    /// <summary>
    /// <see cref="AssetBundle"/> 이 <see cref="ResourceManager"/> 에 등록되면 발생하는 이벤트입니다.
    /// </summary>
    public sealed class OnAssetBundleRegisteredEvent : SynchronousEvent<OnAssetBundleRegisteredEvent>
    {
        public AssetBundleInfo AssetBundle { get; private set; }

        public static OnAssetBundleRegisteredEvent GetEvent(AssetBundleInfo assetBundle)
        {
            var ev = Dequeue();

            ev.AssetBundle = assetBundle;

            return ev;
        }
        protected override void OnReserve()
        {
            AssetBundle = default(AssetBundleInfo);
        }
    }
}

#endif
#endif