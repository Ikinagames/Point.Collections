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
using Unity.Collections;
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System;

namespace Point.Collections
{
    [Serializable]
    public struct FixedChar128Bytes
    {
        [UnityEngine.SerializeField]
        private Char126 bytes;
        [UnityEngine.SerializeField]
        private ushort utf8LengthInBytes;

        public int Length => utf8LengthInBytes;
        public bool IsEmpty => utf8LengthInBytes == 0;

        public FixedChar128Bytes(string str)
        {
            bytes = str;
            utf8LengthInBytes = (ushort)str.Length;
        }

        public override string ToString() => bytes.Read(0, utf8LengthInBytes);

        public static implicit operator FixedChar128Bytes(string t) => new FixedChar128Bytes(t);
        public static implicit operator string(FixedChar128Bytes t) => t.ToString();
    }
}
