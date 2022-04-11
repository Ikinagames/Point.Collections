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
    [StructLayout(LayoutKind.Explicit, Size = 30)]
    public unsafe struct Char30 : IChar
    {
        [FieldOffset(0)]
        public Char16 offset0000;

        [FieldOffset(16)]
        public char char0016;
        [FieldOffset(17)]
        public char char0017;
        [FieldOffset(18)]
        public char char0018;
        [FieldOffset(19)]
        public char char0019;
        [FieldOffset(20)]
        public char char0020;
        [FieldOffset(21)]
        public char char0021;
        [FieldOffset(22)]
        public char char0022;
        [FieldOffset(23)]
        public char char0023;
        [FieldOffset(24)]
        public char char0024;
        [FieldOffset(25)]
        public char char0025;
        [FieldOffset(26)]
        public char char0026;
        [FieldOffset(27)]
        public char char0027;
        [FieldOffset(28)]
        public char char0028;
        [FieldOffset(29)]
        public char char0029;

        public int Length => 30;
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

        public Char30(string str)
        {
            this = default(Char30);

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
