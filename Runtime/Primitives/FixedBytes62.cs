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
    [StructLayout(LayoutKind.Explicit, Size = 62)]
    public struct FixedBytes62
    {
        [FieldOffset(0)]
        public FixedBytes16 offset0000;

        [FieldOffset(16)]
        public FixedBytes16 offset0016;

        [FieldOffset(32)]
        public FixedBytes16 offset0032;

        [FieldOffset(48)]
        public byte byte0048;

        [FieldOffset(49)]
        public byte byte0049;

        [FieldOffset(50)]
        public byte byte0050;

        [FieldOffset(51)]
        public byte byte0051;

        [FieldOffset(52)]
        public byte byte0052;

        [FieldOffset(53)]
        public byte byte0053;

        [FieldOffset(54)]
        public byte byte0054;

        [FieldOffset(55)]
        public byte byte0055;

        [FieldOffset(56)]
        public byte byte0056;

        [FieldOffset(57)]
        public byte byte0057;

        [FieldOffset(58)]
        public byte byte0058;

        [FieldOffset(59)]
        public byte byte0059;

        [FieldOffset(60)]
        public byte byte0060;

        [FieldOffset(61)]
        public byte byte0061;
    }
}

#endif