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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#if UNITY_COLLECTIONS
#endif

using UnityEngine;
using Point.Collections;
using System;

namespace Point.Collections
{
    [AddComponentMenu("Point/Collections/GameObject Pool Receiver")]
    public sealed class GameObjectPoolReceiver : PointMonobehaviour
    {
        [SerializeField] private bool m_ReserveOnPaticleStop = false;

        private bool m_Reserved = false;

        internal GameObjectPool.Pool Parent { get; set; }
        public bool Reserved { get => m_Reserved; internal set => m_Reserved = value; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (m_ReserveOnPaticleStop)
            {
                ParticleSystem particleSystem = GetComponent<ParticleSystem>();
                var main = particleSystem.main;
                main.stopAction = ParticleSystemStopAction.Callback;
            }
        }
#endif

        private void OnParticleSystemStopped()
        {
            if (!Reserved && m_ReserveOnPaticleStop)
            {
                Reserve();
            }
        }
        public void Reserve()
        {
            if (Reserved)
            {
                throw new Exception("??");
            }

            Parent.Reserve(gameObject);
        }
    }
}

#endif