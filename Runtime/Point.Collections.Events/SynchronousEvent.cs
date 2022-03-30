﻿// Copyright 2021 Ikina Games
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

using Point.Collections.Buffer;
using Point.Collections.Diagnostics;
using System;

namespace Point.Collections.Events
{
    public abstract class SynchronousEvent<TEvent> : ISynchronousEvent, IValidation, IDisposable
        , IStackDebugger
        where TEvent : class, ISynchronousEvent, new()
    {
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
            return s_Pool.Get();
        }
        public static void ReserveEvent(TEvent ev)
        {
            ev.Reserve();
        }

        #endregion

        private bool m_Reserved;
        private System.Diagnostics.StackFrame m_Caller;

        public bool Reserved => m_Reserved;

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
            m_Reserved = true;
        }
        bool IValidation.IsValid() => IsValid();

        void IDisposable.Dispose()
        {
            OnDispose();
        }

        #endregion

        System.Diagnostics.StackFrame IStackDebugger.GetStackFrame() => m_Caller;
        void IStackDebugger.SetStackFrame(System.Diagnostics.StackFrame frame)
        {
            m_Caller = frame;
        }

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
        protected virtual bool IsValid() => true;
    }
}