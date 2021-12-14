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

using System;

namespace Point.Collections
{
    public abstract class Datastore<TProvider> : IDatastore
        where TProvider : IDataProvider
    {
        private TProvider m_DataProvider;

        IDataProvider IDatastore.DataProvider => m_DataProvider;

        public Datastore(TProvider provider, params object[] args)
        {
            m_DataProvider = provider;

            ((IDatastore)this).Initialize(provider, args);
            DatastoreManager.Instance.RegisterDatastore(this);
        }

        void IDatastore.Initialize(IDataProvider dataProvider, params object[] args)
        {
            m_DataProvider.OnInitialize(args);
        }
    }
    public interface IDatastore
    {
        IDataProvider DataProvider { get; }

        void Initialize(IDataProvider dataProvider, params object[] args);
    }
    
    public abstract class DataProviderBase : IDataProvider
    {
        internal protected IDataStrategy m_DataStrategy;

        public IDataStrategy DataStrategy => m_DataStrategy;

        void IDataProvider.OnInitialize(params object[] args)
        {
            OnInitialize();
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
        }

        internal protected abstract void InternalInitializeStrategy();

        protected virtual void OnInitialize() { }
        protected virtual void OnInitialize(params object[] args) { }

        protected virtual void Load() { }
        protected virtual void Unload() { }

        protected virtual void OnDispose() { }
    }
    public abstract class DataProvider<TStrategy> : DataProviderBase
        where TStrategy : IDataStrategy
    {
        public new TStrategy DataStrategy => (TStrategy)base.DataStrategy;

        protected internal override sealed void InternalInitializeStrategy()
        {
            m_DataStrategy = InitializeStrategy();
        }

        protected abstract TStrategy InitializeStrategy();
    }
}
