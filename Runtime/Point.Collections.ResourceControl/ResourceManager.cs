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

using System.Collections.Generic;

namespace Point.Collections.ResourceControl
{
    public sealed class ResourceManager : CLSSingleTone<ResourceManager>
    {
        private readonly List<IDatastore> m_Datastores;

        public ResourceManager()
        {
            m_Datastores = new List<IDatastore>();
        }
        protected override void OnInitialize()
        {

        }

        private int FindUnusedSpace()
        {
            for (int i = 0; i < m_Datastores.Count; i++)
            {
                if (m_Datastores[i] == null) return i;
            }
            return -1;
        }

        public bool IsValidID(DatastoreID id)
        {
            if (m_Datastores[id.m_Index] == null ||
                !m_Datastores[id.m_Index].ID.Equals(id)) return false;

            return true;
        }
        public IDatastore GetDatastore(DatastoreID id)
        {
            if (!IsValidID(id)) return null;

            return m_Datastores[id.m_Index];
        }
        public DatastoreID RegisterDatastore(IDatastore datastore)
        {
            int index = FindUnusedSpace();
            if (index < 0)
            {
                index = m_Datastores.Count;
                m_Datastores.Add(datastore);
            }
            
            return new DatastoreID(index, Hash.NewHash());
        }
        public void UnregisterDatastore(IDatastore datastore)
        {
            m_Datastores[datastore.ID.m_Index] = null;
        }
    }
}
