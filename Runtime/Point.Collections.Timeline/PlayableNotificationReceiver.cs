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

using UnityEngine;
using UnityEngine.Playables;

namespace Point.Collections.Timeline
{
    [RequireComponent(typeof(PlayableDirector))]
    [AddComponentMenu("Point/Collections/Timeline/Playable Notification Receiver")]
    public class PlayableNotificationReceiver : PointMonobehaviour, INotificationReceiver
    {
        private PlayableDirector m_Director;
        private bool m_Enabled = false;

        protected virtual void Awake()
        {
            m_Director = GetComponent<PlayableDirector>();
        }
        protected virtual void OnEnable()
        {
            m_Enabled = true;
        }
        protected virtual void OnDisable()
        {
            m_Enabled = false;
        }

        void INotificationReceiver.OnNotify(Playable origin, INotification notification, object context)
        {
            if (!m_Enabled) return;

            OnNotification(m_Director, origin, notification, context);
            if (notification is INotificationExecute execute)
            {
                execute.Execute(m_Director, origin, context);
            }
        }

        protected virtual void OnNotification(PlayableDirector director, Playable origin, INotification notification, object context) { }
    }
}

#endif