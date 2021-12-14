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

namespace Point.Collections.ResourceControl
{
    public abstract class Datastore<TProvider> : IDatastore
        where TProvider : IDataProvider
    {
        private readonly DatastoreID m_ID;
        private TProvider m_DataProvider;

        DatastoreID IDatastore.ID => m_ID;
        IDataProvider IDatastore.DataProvider => m_DataProvider;

        public Datastore(TProvider provider, params object[] args)
        {
            m_DataProvider = provider;

            m_ID = ResourceManager.Instance.RegisterDatastore(this);

            ((IDatastore)this).Initialize(provider, args);
        }

        void IDatastore.Initialize(IDataProvider dataProvider, params object[] args)
        {
            m_DataProvider.OnInitialize(args);
        }

        void IDisposable.Dispose()
        {
            OnDispose();
            m_DataProvider.Dispose();

            ResourceManager.Instance.UnregisterDatastore(this);
        }

        protected virtual void OnDispose() { }
    }
}
