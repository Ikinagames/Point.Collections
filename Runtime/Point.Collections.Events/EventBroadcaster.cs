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

using Point.Collections.Diagnostics;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Point.Collections.Events
{
    public sealed class EventBroadcaster : CLRSingleTone<EventBroadcaster>
    {
        private ConcurrentQueue<ISynchronousEvent> m_Events;

        private Dictionary<Type, EventDescriptionBase> m_EventActions;

        #region Class Instructions

        protected override void OnInitialize()
        {
            m_Events = new ConcurrentQueue<ISynchronousEvent>();
            m_EventActions = new Dictionary<Type, EventDescriptionBase>();

#if DEBUG_MODE
            PointApplication.Instance.OnFrameUpdate -= OnFrameUpdate;
            PointApplication.Instance.OnApplicationShutdown -= Instance_OnApplicationShutdown;
#endif
            PointApplication.Instance.OnFrameUpdate += OnFrameUpdate;
            PointApplication.Instance.OnApplicationShutdown += Instance_OnApplicationShutdown;
        }
        private void Instance_OnApplicationShutdown()
        {
            PointApplication.Instance.OnFrameUpdate -= OnFrameUpdate;
            PointApplication.Instance.OnApplicationShutdown -= Instance_OnApplicationShutdown;

            ((IDisposable)this).Dispose();
        }
        protected override void OnDispose()
        {
            for (int i = 0; i < m_Events.Count; i++)
            {
                m_Events.TryDequeue(out var ev);
                ReserveEvent(ev);
            }

            base.OnDispose();
        }
        private void OnFrameUpdate()
        {
            int eCount = m_Events.Count;
            for (int i = 0; i < eCount; i++)
            {
                m_Events.TryDequeue(out var ev);
                ExecuteEvent(ev);
            }
        }

        #endregion

        #region Private Methods

        private void ExecuteEvent(ISynchronousEvent ev)
        {
#if DEBUG_MODE
            const string c_LogFormat = "Event({0}) posted from {1}";
            PointHelper.Log(Channel.Collections,
                string.Format(c_LogFormat, TypeHelper.ToString(ev.GetType()), ScriptUtils.ToStringFormat(ev.GetStackFrame()))
                );

            if (ev.Reserved)
            {
                throw new Exception("reserve err");
            }
#endif
            try
            {
                ev.Execute();
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
                //throw ex;
            }

            try
            {
                if (m_EventActions.TryGetValue(ev.GetType(), out var actions))
                {
                    actions.Execute(ev);
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.LogException(ex);
                //throw ex;
            }

            ReserveEvent(ev);
        }
        private void ReserveEvent(ISynchronousEvent ev)
        {
            ev.Reserve();
        }

        #endregion

        /// <summary>
        /// 이벤트 <typeparamref name="TEvent"/> 를 호출합니다.
        /// </summary>
        /// <remarks>
        /// 이벤트가 즉시 발생되는 것이 아닌, 실제로는 다음 프레임에 발생합니다.
        /// </remarks>
        /// <typeparam name="TEvent"></typeparam>
        /// <param name="ev"></param>
        public static void PostEvent<TEvent>(TEvent ev)
            where TEvent : SynchronousEvent<TEvent>, new()
        {
            PointHelper.AssertMainThread();

            if (PointApplication.IsShutdown)
            {
                // TODO : 
                return;
            }

#if DEBUG_MODE
            ((IStackDebugger)ev).SetStackFrame(ScriptUtils.GetCallerFrame(1));
#endif
            Instance.m_Events.Enqueue(ev);
        }
        public static void AddEvent<TEvent>(Action<TEvent> action)
            where TEvent : SynchronousEvent<TEvent>, new()
        {
            PointHelper.AssertMainThread();

            if (PointApplication.IsShutdown)
            {
                // TODO : 
                return;
            }

            if (!Instance.m_EventActions.TryGetValue(TypeHelper.TypeOf<TEvent>.Type, out var desc))
            {
                desc = new EventDescription<TEvent>();
                Instance.m_EventActions.Add(TypeHelper.TypeOf<TEvent>.Type, desc);
            }

            EventDescription<TEvent> description = (EventDescription<TEvent>)desc;
            description.action += action;
        }
        public static void RemoveEvent<TEvent>(Action<TEvent> action)
            where TEvent : SynchronousEvent<TEvent>, new()
        {
            PointHelper.AssertMainThread();

            if (PointApplication.IsShutdown)
            {
                // TODO : 
                return;
            }

            if (!Instance.m_EventActions.TryGetValue(TypeHelper.TypeOf<TEvent>.Type, out var desc))
            {
                return;
            }

            EventDescription<TEvent> description = (EventDescription<TEvent>)desc;
            description.action -= action;
        }

        private void TestMethod()
        {
            PostEvent(TestEvent.GetEvent());
            AddEvent<TestEvent>((ev) => { });
            RemoveEvent<TestEvent>((ev) => { });
        }

        #region Inner Classes

        private abstract class EventDescriptionBase
        {
            public abstract void Execute(ISynchronousEvent ev);
        }
        private sealed class EventDescription<TEvent> : EventDescriptionBase
            where TEvent : SynchronousEvent<TEvent>, new()
        {
            public event Action<TEvent> action;

            public override void Execute(ISynchronousEvent ev)
            {
                action?.Invoke((TEvent)ev);
            }
        }

        class TestEvent : SynchronousEvent<TestEvent>
        {
            public static TestEvent GetEvent()
            {
                var ev = Dequeue();

                return ev;
            }
            protected override void Execute()
            {

            }
        }

        #endregion
    }
}
