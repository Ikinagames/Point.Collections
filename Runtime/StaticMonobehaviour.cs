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

using Point.Collections.Native;
using UnityEngine;

namespace Point.Collections
{
    /// <summary>
    /// <seealso cref="UnityEngine.MonoBehaviour"/> (<typeparamref name="T"/>) 를 
    /// Single-tone 으로 wrapping 하는 <see langword="abstract"/> 입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class StaticMonobehaviour<T> : UnityEngine.MonoBehaviour, IStaticMonobehaviour
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
                if (s_Instance == null)
                {
#if UNITY_EDITOR
                    if (!UnityEditorInternal.InternalEditorUtility.CurrentThreadIsMainThread())
                    {
                        PointHelper.LogError(LogChannel.Collections,
                            $"{TypeHelper.TypeOf<StaticMonobehaviour<T>>.ToString()} is only can be initialized in main thread but current thread looks like outside of UnityEngine. This is not allowed.");

                        throw new System.Exception("Internal error. See error log.");
                    }
#endif
                    if (PointApplication.IsShutdown)
                    {
#if UNITY_EDITOR
                        PointHelper.LogError(Channel.Collections,
                            $"You\'re trying to call {TypeHelper.TypeOf<T>.ToString()} while exitting application. This is not allowed.");
#endif
                        return s_Instance;
                    }

                    UnityEngine.GameObject obj = new UnityEngine.GameObject();
                    DontDestroyOnLoad(obj);
                    T t = obj.AddComponent<T>();

#if UNITY_EDITOR
                    obj.name = $"{nameof(T)}: StaticMonobehaviour";
                    if (t.HideInInspector)
                    {
                        obj.hideFlags = HideFlags.HideInHierarchy;
                    }
#endif

                    Application.quitting += t.OnShutdown;

                    s_Instance = t;

                    t.OnInitialize();
                }
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

        void IStaticMonobehaviour.OnInitialize()
        {
            OnInitialze();

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
        protected virtual void OnInitialze() { }
        /// <inheritdoc cref="IStaticMonobehaviour.OnShutdown"/>
        protected virtual void OnShutdown() { }

        /// <summary>
        /// Editor Only, 만약 Runtime 에서 이 메소드가 호출되면 무조건 true 를 반환합니다.
        /// </summary>
        /// <returns></returns>
        protected static bool IsThisMainThread()
        {
#if UNITY_EDITOR
            if (!UnityEditorInternal.InternalEditorUtility.CurrentThreadIsMainThread())
            {
                return false;
            }
#endif
            return true;
        }
    }
}
