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

using System;

namespace Point.Collections
{
    /// <summary>
    /// Common language runtime single-tone <see langword="abstract"/> class
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class CLRSingleTone<T> : IDisposable where T : class, new()
    {
        private static T s_Instance;
        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    s_Instance = new T();

                    (s_Instance as CLRSingleTone<T>).OnInitialize();
                }
                return s_Instance;
            }
        }
        ~CLRSingleTone()
        {
            ((IDisposable)this).Dispose();
        }
        void IDisposable.Dispose()
        {
            OnDispose();
        }

        protected virtual void OnInitialize() { }
        protected virtual void OnDispose() { }
    }
}
