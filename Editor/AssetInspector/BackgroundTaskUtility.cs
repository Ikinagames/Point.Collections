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


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Compilation;

namespace Point.Collections.Editor
{
    [InitializeOnLoad]
    public static class BackgroundTaskUtility
    {
        static BackgroundTaskUtility()
        {
            EditorApplication.update -= Update;
            EditorApplication.update += Update;

            AssemblyReloadEvents.beforeAssemblyReload -= AssemblyReloadEvents_beforeAssemblyReload;
            AssemblyReloadEvents.beforeAssemblyReload += AssemblyReloadEvents_beforeAssemblyReload;
        }

        internal static List<BackgroundTask> s_Tasks = new List<BackgroundTask>();

        private static void AssemblyReloadEvents_beforeAssemblyReload()
        {
            EditorApplication.update -= Update;

            for (int i = 0; i < s_Tasks.Count; i++)
            {
                s_Tasks[i].Dispose();
            }
            s_Tasks.Clear();
        }
        private static void Update()
        {
            for (int i = s_Tasks.Count - 1; i >= 0; i--)
            {
                bool result = s_Tasks[i].Update();

                if (!result)
                {
                    s_Tasks.RemoveAt(i);
                }
            }
        }
    }
    public sealed class BackgroundTask : IDisposable
    {
        private int m_ProgressID;
        private List<BackgroundTask> m_Childs = new List<BackgroundTask>();

        private IEnumerator m_Task;
        private bool m_Disposed = false;

        public int ProgressID => m_ProgressID;
        public bool IsRunning => !m_Disposed;

        public BackgroundTask(string name)
        {
            m_ProgressID = Progress.Start(name);
        }
        public BackgroundTask(string name, IEnumerator enumerator) : this(name)
        {
            m_Task = enumerator;
            BackgroundTaskUtility.s_Tasks.Add(this);
        }
        ~BackgroundTask()
        {
            Dispose();
        }
        public void Dispose()
        {
            Progress.Remove(m_ProgressID);
            m_Task = null;
            m_Disposed = true;
        }

        public void StartTask(IEnumerator task)
        {
            if (m_Disposed || m_Task != null)
            {
                throw new Exception();
            }

            m_Task = task;
            BackgroundTaskUtility.s_Tasks.Add(this);
        }

        internal bool Update()
        {
            if (!m_Task.MoveNext())
            {
                Dispose();
                return false;
            }

            return true;
        }

        public static implicit operator int(BackgroundTask t) => t.ProgressID;
    }
}

#endif