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

#if UNITY_2019_1_OR_NEWER
#define UNITYENGINE
#endif

#if UNITYENGINE

namespace Point.Collections
{
    public abstract class SceneMonobehaviour<T> : PointMonobehaviour, IStaticMonobehaviour
        where T : UnityEngine.MonoBehaviour, IStaticMonobehaviour
    {
        private static T s_Instance;
        /// <summary>
        /// <typeparamref name="T"/> 의 인스턴스를 반환합니다.
        /// </summary>
        /// <remarks>
        /// 만약 인스턴스가 생성되지 않았다면 즉시 생성하여 반환합니다.
        /// </remarks>
        public static T Instance
        {
            get
            {
                return s_Instance;
            }
        }

        /// <summary>
        /// 싱글톤 객체가 할당되었는지 반환합니다.
        /// </summary>
        public static bool HasInstance => s_Instance != null;

        bool IStaticMonobehaviour.EnableLog => EnableLog;
        bool IStaticMonobehaviour.HideInInspector => HideInInspector;

        /// <inheritdoc cref="IStaticMonobehaviour.EnableLog"/>
        protected virtual bool EnableLog => true;
        /// <inheritdoc cref="IStaticMonobehaviour.HideInInspector"/>
        protected virtual bool HideInInspector => false;

        protected virtual void Awake()
        {
            s_Instance = this as T;
            s_Instance.OnInitialize();
        }
        protected virtual void OnDestroy()
        {
            s_Instance.OnShutdown();
            s_Instance = null;
        }

        void IStaticMonobehaviour.OnInitialize()
        {
            OnInitialize();

            if (EnableLog)
            {
                PointHelper.Log(LogChannel.Collections,
                    $"Initialized {TypeHelper.TypeOf<T>.ToString()}");
            }
        }
        void IStaticMonobehaviour.OnShutdown()
        {
            OnShutdown();

            if (EnableLog)
            {
                PointHelper.Log(LogChannel.Collections,
                    $"Shutdown {TypeHelper.TypeOf<T>.ToString()}");
            }
        }

        /// <inheritdoc cref="IStaticMonobehaviour.OnInitialize"/>
        protected virtual void OnInitialize() { }

        protected virtual void OnShutdown() { }
    }
}

#endif