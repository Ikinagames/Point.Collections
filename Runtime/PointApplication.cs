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
using UnityEngine;
using UnityEngine.Scripting;
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Point.Collections.Threading;
using System;
using System.Reflection;
using Point.Collections.Events;

[assembly: System.Runtime.InteropServices.ComVisible(true)]
namespace Point.Collections
{
#if UNITYENGINE
    [AddComponentMenu("")]
#endif
    [XmlSettings(SaveToDisk = false)]
    public sealed class PointApplication :
#if UNITYENGINE
        StaticMonobehaviour<PointApplication>
#else
        CLRSingleTone<PointApplication>
#endif
    {
        #region Statics

#if UNITYENGINE
        [Preserve, RuntimeInitializeOnLoadMethod]
#endif
        public static void Initialize()
        {
            PointApplication app = Instance;
        }
#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        public static void EditorInitialize()
        {
            CollectionUtility.Initialize();
        }
#endif

        private static bool s_IsShutdown = false;

        public static bool IsShutdown => s_IsShutdown;

        #endregion

#if UNITYENGINE
        protected override bool EnableLog => false;
        protected override bool HideInInspector => !PointSettings.Instance.m_DisplayMainApplication;
#endif

        private ThreadInfo m_MainThread;
#if UNITYENGINE
#if ENABLE_INPUT_SYSTEM
        private Timer m_InActiveTimer;
        private bool m_IsInActive = false;
        public event Action<bool> OnInActive;
#endif
        // FPS
        [SerializeField, XmlField(PropertyName = "DisplayFPS")] private bool m_DisplayFPS = false;
#endif

        public ThreadInfo MainThread => m_MainThread;

#if UNITYENGINE
        public static event Action OnFrameUpdate;
        public static event Action OnFixedUpdate;
        public static event Action OnLateUpdate;
        public static event Action OnApplicationShutdown;

        public static bool DisplayFPS { get => Instance.m_DisplayFPS; set => Instance.m_DisplayFPS = value; }
#endif

        protected override void OnInitialize()
        {
            const string c_Instance = "Instance";

            XmlSettings.LoadSettings(this);
            if (m_DisplayFPS)
            {
                object temp = HUDFPS.Instance;
            }
            CollectionUtility.Initialize();

            Type[] types = TypeHelper.GetTypes(other => TypeHelper.TypeOf<IStaticInitializer>.Type.IsAssignableFrom(other));
            for (int i = 0; i < types.Length; i++)
            {
                if (TypeHelper.TypeOf<IStaticMonobehaviour>.Type.IsAssignableFrom(types[i]))
                {
                    types[i].GetProperty(c_Instance, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                        .GetValue(null);
                }
                else System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(types[i].TypeHandle);
            }

#if !UNITYENGINE
            Main();   
#else
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += SceneManager_sceneUnloaded;
#endif
        }

#if UNITYENGINE
        private void Awake()
#else
        private void Main()
#endif
        {
            m_MainThread = ThreadInfo.CurrentThread;
#if UNITYENGINE
#if UNITY_EDITOR
            PointHelper.s_EditorLogs = string.Empty;
#endif

#if ENABLE_INPUT_SYSTEM
            m_InActiveTimer = Timer.Start();
            UnityEngine.InputSystem.InputSystem.onActionChange += InputSystem_onActionChange;
#endif
            if (PointSettings.Instance.m_EnableLogFile)
            {
#if DEBUG_MODE
                if (string.IsNullOrEmpty(PointSettings.Instance.m_LogFilePath))
                {
                    PointHelper.LogError(Channel.Core,
                        "You\'re trying to save logs in local without any path. This is not allowed. Please set log path at the PointSettings.");
                }
                else
#endif

                {
                    PointHelper.s_LogHandler = new LowLevel.LogHandler();
                    PointHelper.s_LogHandler.SetLogFile(PointSettings.Instance.m_LogFilePath);
                }
            }
#endif
        }

#if UNITYENGINE && ENABLE_INPUT_SYSTEM

        private void InputSystem_onActionChange(object arg1, UnityEngine.InputSystem.InputActionChange arg2)
        {
            switch (arg2)
            {
                case UnityEngine.InputSystem.InputActionChange.ActionEnabled:
                    break;
                case UnityEngine.InputSystem.InputActionChange.ActionDisabled:
                    break;
                case UnityEngine.InputSystem.InputActionChange.ActionMapEnabled:
                    break;
                case UnityEngine.InputSystem.InputActionChange.ActionMapDisabled:
                    break;
                case UnityEngine.InputSystem.InputActionChange.ActionStarted:
                    break;
                case UnityEngine.InputSystem.InputActionChange.ActionPerformed:
                    UnityEngine.InputSystem.InputAction inputAction = (UnityEngine.InputSystem.InputAction)arg1;
                    if (inputAction.IsMouseMoveAction())
                    {
                        break;
                    }

                    m_InActiveTimer.Reset();

                    if (m_IsInActive)
                    {
                        m_IsInActive = false;
                        OnInActive?.Invoke(false);
                        EventBroadcaster.PostEvent(ApplicationInActiveEvent.GetEvent(false));
                    }

                    break;
                case UnityEngine.InputSystem.InputActionChange.ActionCanceled:
                    break;
                case UnityEngine.InputSystem.InputActionChange.BoundControlsAboutToChange:
                    break;
                case UnityEngine.InputSystem.InputActionChange.BoundControlsChanged:
                    break;
                default:
                    break;
            }
        }

#endif

#if UNITYENGINE
        private void OnApplicationFocus(bool focus)
        {
            if (IsShutdown) return;

            EventBroadcaster.PostEvent(ApplicationOutFocusEvent.GetEvent(focus));
        }
        private void Update()
        {
            OnFrameUpdate?.Invoke();

#if ENABLE_INPUT_SYSTEM
            InActiveHandler();
#endif
        }
        private void FixedUpdate()
        {
            OnFixedUpdate?.Invoke();
        }
        private void LateUpdate()
        {
            OnLateUpdate?.Invoke();
        }
        protected override void OnShutdown()
        {
            s_IsShutdown = true;
            
            OnApplicationShutdown?.Invoke();

            if (PointSettings.Instance.m_EnableLogFile)
            {
                PointHelper.s_LogHandler.CloseLogFile();
            }
            XmlSettings.SaveSettings(this);
        }

        private void SceneManager_sceneLoaded(UnityEngine.SceneManagement.Scene arg0, UnityEngine.SceneManagement.LoadSceneMode arg1)
        {
            EventBroadcaster.PostEvent(OnSceneLoadedEvent.GetEvent(arg0));
        }
        private void SceneManager_sceneUnloaded(UnityEngine.SceneManagement.Scene arg0)
        {
            EventBroadcaster.PostEvent(OnSceneUnloadedEvent.GetEvent());
        }

#if UNITYENGINE && ENABLE_INPUT_SYSTEM
        private void InActiveHandler()
        {
            if (m_IsInActive) return;
            else if (m_InActiveTimer.ElapsedTime > PointSettings.Instance.InActiveTime)
            {
                m_IsInActive = true;
                OnInActive?.Invoke(true);
                EventBroadcaster.PostEvent(ApplicationInActiveEvent.GetEvent(true));
            }
        }
#endif
#endif
    }
}
