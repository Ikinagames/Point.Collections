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

#if UNITY_2019 && !UNITY_2020_OR_NEWER

using System;
using System.Runtime.InteropServices;

namespace Point.Collections
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public struct FixedBytes16
    {
        [FieldOffset(0)]
        public byte byte0000;

        [FieldOffset(1)]
        public byte byte0001;

        [FieldOffset(2)]
        public byte byte0002;

        [FieldOffset(3)]
        public byte byte0003;

        [FieldOffset(4)]
        public byte byte0004;

        [FieldOffset(5)]
        public byte byte0005;

        [FieldOffset(6)]
        public byte byte0006;

        [FieldOffset(7)]
        public byte byte0007;

        [FieldOffset(8)]
        public byte byte0008;

        [FieldOffset(9)]
        public byte byte0009;

        [FieldOffset(10)]
        public byte byte0010;

        [FieldOffset(11)]
        public byte byte0011;

        [FieldOffset(12)]
        public byte byte0012;

        [FieldOffset(13)]
        public byte byte0013;

        [FieldOffset(14)]
        public byte byte0014;

        [FieldOffset(15)]
        public byte byte0015;
    }
}

#endif