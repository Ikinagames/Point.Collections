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
using System.Runtime.InteropServices;

namespace Point.Collections
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit, Size = 510)]
    public unsafe struct Char510 : IChar
    {
        [FieldOffset(0)]
        public Char16 offset0000;
        [FieldOffset(16)]
        public Char16 offset0016;
        [FieldOffset(32)]
        public Char16 offset0032;
        [FieldOffset(48)]
        public Char16 offset0048;
        [FieldOffset(64)]
        public Char16 offset0064;
        [FieldOffset(80)]
        public Char16 offset0080;
        [FieldOffset(96)]
        public Char16 offset0096;
        [FieldOffset(112)]
        public Char16 offset0112;
        [FieldOffset(128)]
        public Char16 offset0128;
        [FieldOffset(144)]
        public Char16 offset0144;
        [FieldOffset(160)]
        public Char16 offset0160;
        [FieldOffset(176)]
        public Char16 offset0176;
        [FieldOffset(192)]
        public Char16 offset0192;
        [FieldOffset(208)]
        public Char16 offset0208;
        [FieldOffset(224)]
        public Char16 offset0224;
        [FieldOffset(240)]
        public Char16 offset0240;
        [FieldOffset(256)]
        public Char16 offset0256;
        [FieldOffset(272)]
        public Char16 offset0272;
        [FieldOffset(288)]
        public Char16 offset0288;
        [FieldOffset(304)]
        public Char16 offset0304;
        [FieldOffset(320)]
        public Char16 offset0320;
        [FieldOffset(336)]
        public Char16 offset0336;
        [FieldOffset(352)]
        public Char16 offset0352;
        [FieldOffset(368)]
        public Char16 offset0368;
        [FieldOffset(384)]
        public Char16 offset0384;
        [FieldOffset(400)]
        public Char16 offset0400;
        [FieldOffset(416)]
        public Char16 offset0416;
        [FieldOffset(432)]
        public Char16 offset0432;
        [FieldOffset(448)]
        public Char16 offset0448;
        [FieldOffset(464)]
        public Char16 offset0464;
        [FieldOffset(480)]
        public Char16 offset0480;

        [FieldOffset(496)]
        public char char0496;
        [FieldOffset(497)]
        public char char0497;
        [FieldOffset(498)]
        public char char0498;
        [FieldOffset(499)]
        public char char0499;
        [FieldOffset(500)]
        public char char0500;
        [FieldOffset(501)]
        public char char0501;
        [FieldOffset(502)]
        public char char0502;
        [FieldOffset(503)]
        public char char0503;
        [FieldOffset(504)]
        public char char0504;
        [FieldOffset(505)]
        public char char0505;
        [FieldOffset(506)]
        public char char0506;
        [FieldOffset(507)]
        public char char0507;
        [FieldOffset(508)]
        public char char0508;
        [FieldOffset(509)]
        public char char0509;

        public int Length => 510;
        public char this[int index]
        {
            get
            {
                fixed (char* ptr = &offset0000.char0000)
                {
                    return ptr[index];
                }
            }
        }

        public Char510(string str)
        {
            this = default(Char510);

            CharExtensions.CheckIsValidString(in this, in str);
            CharExtensions.Apply(ref this, in str, str.Length);
        }

        void IChar.Set(int index, char value)
        {
            fixed (char* ptr = &offset0000.char0000)
            {
                ptr[index] = value;
            }
        }
        public string Read(int from, int to)
        {
            string sum = string.Empty;
            fixed (char* ptr = &offset0000.char0000)
            {
                for (int i = from; i < to; i++)
                {
                    sum += ptr[i];
                }
            }
            return sum;
        }

        public override string ToString()
        {
            string sum = string.Empty;

            fixed (char* ptr = &offset0000.char0000)
            {
                for (int i = 0; i < Length; i++)
                {
                    sum += ptr[i];
                }
            }
            
            return sum.Trim();
        }

        public static implicit operator Char510(string str) => new Char510(str);
        public static implicit operator string(Char510 str) => str.ToString();
    }
}
