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

using Point.Collections.ResourceControl;
using System;

namespace Point.Collections
{
    public class Promise : IPromise, IDisposable
    {
        private object m_Value;
        private Action<object> m_OnCompleted;

        public bool HasValue => m_Value != null;
        public object Value => m_Value;

        public event Action<object> OnCompleted
        {
            add
            {
                if (m_Value != null)
                {
                    value?.Invoke(m_Value);
                    return;
                }

                m_OnCompleted += value;
            }
            remove
            {
                m_OnCompleted -= value;
            }
        }

        public Promise(IPromiseProvider provider)
        {
            provider.OnComplete(OnCompleteMethod);
        }
        ~Promise()
        {
            Dispose();
        }
        public void Dispose()
        {
            m_Value = null;
            m_OnCompleted = null;
        }

        protected virtual void OnCompleteMethod(object obj)
        {
            m_Value = obj;

            m_OnCompleted?.Invoke(obj);
            m_OnCompleted = null;
        }
    }
    public class Promise<T> : IPromise, IDisposable
    {
        private T m_Value;
        private bool m_IsCompleted;
        private Action<T> m_OnCompleted;

        public bool HasValue => m_IsCompleted;
        public T Value => m_Value;
        object IPromise.Value => m_Value;

        public event Action<T> OnCompleted
        {
            add
            {
                if (m_IsCompleted)
                {
                    value?.Invoke(m_Value);
                    return;
                }

                m_OnCompleted += value;
            }
            remove
            {
                m_OnCompleted -= value;
            }
        }

        public Promise(T obj)
        {
            m_Value = obj;
            m_IsCompleted = true;
        }
        public Promise(IPromiseProvider provider)
        {
            provider.OnComplete(OnCompleteMethod);

            m_IsCompleted = false;
        }
        public Promise(IPromiseProvider<T> provider)
        {
            provider.OnComplete(OnCompleteMethod);

            m_IsCompleted = false;
        }
        ~Promise()
        {
            Dispose();
        }
        public void Dispose()
        {
            m_Value = default(T);
            m_IsCompleted = false;
            m_OnCompleted = null;
        }

        private void OnCompleteMethod(object obj) => OnCompleteMethod((T)obj);
        protected virtual void OnCompleteMethod(T obj)
        {
            m_Value = (T)obj;

            m_OnCompleted?.Invoke((T)obj);
            m_IsCompleted = true;

            m_OnCompleted = null;
        }

        public static implicit operator Promise<T>(T t)
        {
            return new Promise<T>(t);
        }
        public static explicit operator T(Promise<T> t) => t.Value;
    }

    internal interface IPromise
    {
        bool HasValue { get; }
        object Value { get; }
    }

    public interface IPromiseProvider
    {
        void OnComplete(Action<object> obj);
    }
    public interface IPromiseProvider<T>
    {
        void OnComplete(Action<T> obj);
    }
}
