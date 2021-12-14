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

using Point.Collections.Native;
using UnityEngine;

namespace Point.Collections
{
    public abstract class StaticMonobehaviour<T> : UnityEngine.MonoBehaviour, IStaticMonobehaviour
        where T : UnityEngine.MonoBehaviour, IStaticMonobehaviour
    {
        private static T s_Instance;
        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
#if UNITY_EDITOR
                    if (!UnityEditorInternal.InternalEditorUtility.CurrentThreadIsMainThread())
                    {
                        Point.LogError(Point.LogChannel.Collections,
                            $"{TypeHelper.TypeOf<StaticMonobehaviour<T>>.ToString()} is only can be initialized in main thread but current thread looks like outside of UnityEngine. This is not allowed.");

                        throw new System.Exception("Internal error. See error log.");
                    }
#endif

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

                    t.OnInitialize();

                    s_Instance = t;
                }
                return s_Instance;
            }
        }

        public static bool HasInstance => s_Instance != null;

        public virtual bool EnableLog => true;
        public virtual bool HideInInspector => false;

        void IStaticMonobehaviour.OnInitialize()
        {
            OnInitialze();

            if (EnableLog)
            {
                Point.Log(Point.LogChannel.Default,
                    $"Initialized {TypeHelper.TypeOf<T>.ToString()}");
            }
        }
        void IStaticMonobehaviour.OnShutdown()
        {
            OnShutdown();

            if (EnableLog)
            {
                Point.Log(Point.LogChannel.Default,
                    $"Shutdown {TypeHelper.TypeOf<T>.ToString()}");
            }
        }

        public virtual void OnInitialze() { }
        public virtual void OnShutdown() { }
    }
}
