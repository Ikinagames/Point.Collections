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
#if UNITY_COLLECTIONS
#endif


using System;

namespace Point.Collections.IO
{
    public struct Identifier : IEmpty, IEquatable<Identifier>
    {
        private readonly Hash m_Hash;

        public Identifier(string name)
        {
            m_Hash = new Hash(name);
        }

        public bool IsEmpty() => m_Hash.IsEmpty();

        public bool Equals(Identifier other) => m_Hash.Equals(other.m_Hash);
        public override string ToString() => m_Hash.Value.ToString();

        public static implicit operator Identifier(string name) => new Identifier(name);
        public static implicit operator uint(Identifier identifier) => identifier.m_Hash.Value;
    }
}

#endif