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
    public readonly struct DatastoreID : IValidation, IEquatable<DatastoreID>
    {
        internal readonly int m_Index;
        internal readonly Hash m_Hash;

        public IDatastore Datastore => ResourceManager.Instance.GetDatastore(this);

        internal DatastoreID(int index, Hash hash)
        {
            m_Index = index;
            m_Hash = hash;
        }

        public bool Equals(DatastoreID other)
        {
            return m_Index.Equals(other) && m_Hash.Equals(other);
        }

        public bool IsValid() => ResourceManager.Instance.IsValidID(this);
    }
}
