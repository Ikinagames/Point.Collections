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
    [StructLayout(LayoutKind.Explicit, Size = 126)]
    public unsafe struct Char126 : IChar
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
        public char char0112;
        [FieldOffset(113)]
        public char char0113;
        [FieldOffset(114)]
        public char char0114;
        [FieldOffset(115)]
        public char char0115;
        [FieldOffset(116)]
        public char char0116;
        [FieldOffset(117)]
        public char char0117;
        [FieldOffset(118)]
        public char char0118;
        [FieldOffset(119)]
        public char char0119;
        [FieldOffset(120)]
        public char char0120;
        [FieldOffset(121)]
        public char char0121;
        [FieldOffset(122)]
        public char char0122;
        [FieldOffset(123)]
        public char char0123;
        [FieldOffset(124)]
        public char char0124;
        [FieldOffset(125)]
        public char char0125;

        public int Length => 126;
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

        public Char126(string str)
        {
            this = default(Char126);

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
    }
}
