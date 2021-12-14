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

namespace Point.Collections.ResourceControl
{
    public abstract class DataProviderBase : IDataProvider
    {
        internal protected IDataStrategy m_DataStrategy;

        public IDataStrategy DataStrategy => m_DataStrategy;

        void IDataProvider.OnInitialize(params object[] args)
        {
            OnInitialize(args);

            InternalInitializeStrategy();
        }
        void IDataProvider.Load()
        {
            Load();
        }
        void IDataProvider.Unload()
        {
            Unload();
        }
        void IDisposable.Dispose()
        {
            OnDispose();

            m_DataStrategy.Dispose();
        }

        internal protected abstract void InternalInitializeStrategy();

        protected virtual void OnInitialize(params object[] args) { }

        protected virtual void Load() { }
        protected virtual void Unload() { }

        protected virtual void OnDispose() { }
    }
}