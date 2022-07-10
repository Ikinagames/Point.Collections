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
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Point.Collections.Buffer;
using Point.Collections.Diagnostics;
using Point.Collections.Threading;
using System;
using UnityEngine;

namespace Point.Collections.Events
{
    public abstract class SynchronousEvent<TEvent> : CustomYieldInstruction,
        ISynchronousEvent, IValidation, IDisposable, IStackDebugger
        where TEvent : class, ISynchronousEvent, new()
    {
        private static AtomicOperator s_Op = new AtomicOperator();

        #region Pool

        private static readonly ObjectPool<TEvent> 
            s_Pool = new ObjectPool<TEvent>(Factory, InternalOnInitialize, InternalOnReserve, InternalOnDispose);
        private static TEvent Factory()
        {
            TEvent ev = new TEvent();

            ev.OnCreated();

            return ev;
        }
        private static void InternalOnInitialize(TEvent ev)
        {
            ev.OnInitialize();
        }
        private static void InternalOnReserve(TEvent ev)
        {
            ev.OnReserve();
        }
        private static void InternalOnDispose(TEvent ev)
        {
            ev.Dispose();
        }

        protected static TEvent Dequeue()
        {
            s_Op.Enter();
            var obj = s_Pool.Get();
            s_Op.Exit();

            return obj;
        }
        //public static void ReserveEvent(TEvent ev)
        //{
        //    ev.Reserve();
        //}

        #endregion

        private bool m_Reserved;
        private System.Diagnostics.StackFrame m_Caller;
        private object m_ContextObject;

        public bool Reserved => m_Reserved;
        bool ISynchronousEvent.InternalEnableLog => EnableLog;
        public override bool keepWaiting => !Reserved;

        #region Interface Instructions

        void ISynchronousEvent.OnCreated()
        {
            OnCreated();

            m_Reserved = false;
        }
        void ISynchronousEvent.OnInitialize()
        {
            OnInitialize();

            m_Reserved = false;
        }

        void ISynchronousEvent.Execute()
        {
            Execute();
        }
        void ISynchronousEvent.Reserve()
        {
            s_Pool.Reserve(this as TEvent);
        }

        void ISynchronousEvent.OnReserve()
        {
            OnReserve();

            m_Caller = null;
            m_ContextObject = null;
            m_Reserved = true;
        }
        ISynchronousEvent ISynchronousEvent.Copy()
        {
            return Copy();
        }
        bool IValidation.IsValid() => IsValid();

        void IDisposable.Dispose()
        {
            OnDispose();
        }

        #endregion

        public void SetContextObject(object contextObject) => m_ContextObject = this;

        System.Diagnostics.StackFrame IStackDebugger.GetStackFrame() => m_Caller;
        object IStackDebugger.GetContextObject() => m_ContextObject;
        void IStackDebugger.SetStackFrame(System.Diagnostics.StackFrame frame)
        {
            m_Caller = frame;
        }

        protected virtual bool EnableLog => true;

        /// <inheritdoc cref="ISynchronousEvent.OnCreated"/>
        protected virtual void OnCreated() { }
        /// <inheritdoc cref="ISynchronousEvent.OnInitialize"/>
        protected virtual void OnInitialize() { }
        protected virtual void OnDispose() { }

        /// <summary>
        /// Broadcast 직전 수행되는 메소드입니다.
        /// </summary>
        protected virtual void Execute() { }
        protected virtual void OnReserve() { }
        protected virtual ISynchronousEvent Copy() { throw new NotImplementedException(); }
        protected virtual bool IsValid() => true;
    }
}