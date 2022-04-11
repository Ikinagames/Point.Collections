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
    [StructLayout(LayoutKind.Explicit, Size = 62)]
    public unsafe struct Char62 : IChar
    {
        [FieldOffset(0)]
        public Char16 offset0000;
        [FieldOffset(16)]
        public Char16 offset0016;
        [FieldOffset(32)]
        public Char16 offset0032;

        [FieldOffset(48)]
        public char char0048;
        [FieldOffset(49)]
        public char char0049;
        [FieldOffset(50)]
        public char char0050;
        [FieldOffset(51)]
        public char char0051;
        [FieldOffset(52)]
        public char char0052;
        [FieldOffset(53)]
        public char char0053;
        [FieldOffset(54)]
        public char char0054;
        [FieldOffset(55)]
        public char char0055;
        [FieldOffset(56)]
        public char char0056;
        [FieldOffset(57)]
        public char char0057;
        [FieldOffset(58)]
        public char char0058;
        [FieldOffset(59)]
        public char char0059;
        [FieldOffset(60)]
        public char char0060;
        [FieldOffset(61)]
        public char char0061;

        public int Length => 62;
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

        public Char62(string str)
        {
            this = default(Char62);

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
