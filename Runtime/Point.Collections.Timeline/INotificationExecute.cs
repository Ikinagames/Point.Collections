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

#if UNITY_2019_1_OR_NEWER
#define UNITYENGINE
#endif

#if UNITYENGINE

using UnityEngine.Playables;

namespace Point.Collections.Timeline
{
    /// <summary>
    /// <see cref="INotification"/> 을 가진 <see cref="PlayableAsset"/> 재생될 때, 부모의 오브젝트가 
    /// <see cref="PlayableNotificationReceiver"/> 을 가지고 있으면 재생되는 구현부입니다.
    /// </summary>
    /// <remarks>
    /// <seealso cref="INotification"/> 과 함께 사용합니다.
    /// </remarks>
    public interface INotificationExecute
    {
        /// <summary>
        /// 조건(ex. <seealso cref="INotification"/>)을 만족하면 실행되는 메소드입니다.
        /// </summary>
        /// <param name="director"></param>
        /// <param name="origin"></param>
        /// <param name="context"></param>
        void Execute(PlayableDirector director, Playable origin, object context);
    }
}

#endif