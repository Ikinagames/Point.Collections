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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2020
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

namespace Point.Collections
{
    public abstract class AnimationEventBinder : UnityEngine.MonoBehaviour
    {
        /// <summary>
        /// 이 메소드는 <see cref="AnimationClip"/> 내 <see cref="AnimationEvent"/> 호출용 메소드입니다.
        /// </summary>
        /// <param name="ev"></param>
        [System.Obsolete("Do not use. This method is intended to use only at AnimationClip events.", true)]
        public abstract void TriggerAction(UnityEngine.AnimationEvent ev);
    }
}

#endif