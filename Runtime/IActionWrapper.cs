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

#if UNITY_2020_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Point.Collections.Buffer;
using System;

namespace Point.Collections
{
    public interface IActionWrapper
    {
        void Reserve();
        void Invoke(params object[] args);
    }
    public sealed class ActionWrapper : IActionWrapper
    {
        private static readonly ObjectPool<ActionWrapper> s_Container;
#if !UNITYENGINE
        [System.Diagnostics.CodeAnalysis.AllowNull]
#endif
        public Action Action;

#if UNITYENGINE
        private bool m_MarkerSet = false;
        private Unity.Profiling.ProfilerMarker m_Marker;
#endif

        static ActionWrapper()
        {
            s_Container = new ObjectPool<ActionWrapper>(Factory, null, null, null);
        }
        private static ActionWrapper Factory()
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return new ActionWrapper();
#pragma warning restore CS0618 // Type or member is obsolete
        }
        /// <summary>
        /// <see cref="GetWrapper"/> 를 사용하세요.
        /// </summary>
        [Obsolete("Use ActionWrapper.GetWrapper")]
        public ActionWrapper() { }
        public static ActionWrapper GetWrapper() => s_Container.Get();
        public void Reserve()
        {
            Action = null;
#if UNITYENGINE
            m_MarkerSet = false;
#endif
            s_Container.Reserve(this);
        }

        public void SetProfiler(string name)
        {
#if UNITYENGINE
            m_MarkerSet = true;
            m_Marker = new Unity.Profiling.ProfilerMarker(name);
#endif
        }
        public void SetAction(Action action)
        {
            Action = action;
        }
        public void Invoke()
        {
#if UNITYENGINE
            if (m_MarkerSet) m_Marker.Begin();
#endif
            Action?.Invoke();
#if UNITYENGINE
            if (m_MarkerSet) m_Marker.End();
#endif
        }

        void IActionWrapper.Invoke(params object[] args) => Invoke();
    }
    public sealed class ActionWrapper<T> : IActionWrapper
    {
        private static readonly ObjectPool<ActionWrapper<T>> s_Container;
#if !UNITYENGINE
        [System.Diagnostics.CodeAnalysis.AllowNull]
#endif
        public Action<T> Action;

#if UNITYENGINE
        private bool m_MarkerSet = false;
        private Unity.Profiling.ProfilerMarker m_Marker;
#endif

        static ActionWrapper()
        {
            s_Container = new ObjectPool<ActionWrapper<T>>(Factory, null, null, null);
        }
        private static ActionWrapper<T> Factory()
        {
            return new ActionWrapper<T>();
        }
        /// <summary>
        /// <see cref="GetWrapper"/> 를 사용하세요.
        /// </summary>
        public ActionWrapper() { }
        public static ActionWrapper<T> GetWrapper() => s_Container.Get();
        public void Reserve()
        {
            Action = null;
#if UNITYENGINE
            m_MarkerSet = false;
#endif
            s_Container.Reserve(this);
        }

        public void SetProfiler(string name)
        {
#if UNITYENGINE
            m_MarkerSet = true;
            m_Marker = new Unity.Profiling.ProfilerMarker(name);
#endif
        }
        public void SetAction(Action<T> action)
        {
            Action = action;
        }
        public void Invoke(T t)
        {
#if UNITYENGINE
            if (m_MarkerSet) m_Marker.Begin();
#endif
            try
            {
                Action?.Invoke(t);
            }
            catch (Exception ex)
            {
#if UNITYENGINE
                UnityEngine.Debug.LogException(ex);
#else
                throw;
#endif
            }
#if UNITYENGINE
            if (m_MarkerSet) m_Marker.End();
#endif
        }

        void IActionWrapper.Invoke(params object[] args) => Invoke((T)args[0]);
    }
}
