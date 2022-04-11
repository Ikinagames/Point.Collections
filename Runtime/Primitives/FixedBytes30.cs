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
    [StructLayout(LayoutKind.Explicit, Size = 30)]
    public struct FixedBytes30
    {
        [FieldOffset(0)]
        public FixedBytes16 offset0000;

        [FieldOffset(16)]
        public byte byte0016;

        [FieldOffset(17)]
        public byte byte0017;

        [FieldOffset(18)]
        public byte byte0018;

        [FieldOffset(19)]
        public byte byte0019;

        [FieldOffset(20)]
        public byte byte0020;

        [FieldOffset(21)]
        public byte byte0021;

        [FieldOffset(22)]
        public byte byte0022;

        [FieldOffset(23)]
        public byte byte0023;

        [FieldOffset(24)]
        public byte byte0024;

        [FieldOffset(25)]
        public byte byte0025;

        [FieldOffset(26)]
        public byte byte0026;

        [FieldOffset(27)]
        public byte byte0027;

        [FieldOffset(28)]
        public byte byte0028;

        [FieldOffset(29)]
        public byte byte0029;
    }
}

#endif