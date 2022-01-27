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

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Point.Collections.Events
{
    public sealed class EventBroadcaster : CLRSingleTone<EventBroadcaster>
    {
        private ConcurrentQueue<ISynchronousEvent> m_Events;

        #region Class Instructions

        protected override void OnInitialize()
        {
            m_Events = new ConcurrentQueue<ISynchronousEvent>();

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
            if (ev.Reserved)
            {
                throw new Exception();
            }
#endif
            try
            {
                ev.Execute();
            }
            catch (Exception)
            {
                throw;
            }

            ReserveEvent(ev);
        }
        private void ReserveEvent(ISynchronousEvent ev)
        {
            ev.Reserve();
        }

        #endregion

        public static void Broadcast<TEvent>(TEvent ev)
            where TEvent : ISynchronousEvent
        {
            PointHelper.AssertMainThread();

            if (PointApplication.IsShutdown)
            {
                // TODO : 
                return;
            }

            Instance.m_Events.Enqueue(ev);
        }

        private void TestMethod()
        {
            Broadcast(TestEvent.GetEvent());
        }
    }

    public sealed class TestEvent : SynchronousEvent<TestEvent>
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
}
