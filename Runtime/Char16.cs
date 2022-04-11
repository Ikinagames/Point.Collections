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
    [StructLayout(LayoutKind.Explicit, Size = 16)]
    public unsafe struct Char16 : IChar
    {
        [FieldOffset(0)]
        public char char0000;
        [FieldOffset(1)]
        public char char0001;
        [FieldOffset(2)]
        public char char0002;
        [FieldOffset(3)]
        public char char0003;
        [FieldOffset(4)]
        public char char0004;
        [FieldOffset(5)]
        public char char0005;
        [FieldOffset(6)]
        public char char0006;
        [FieldOffset(7)]
        public char char0007;
        [FieldOffset(8)]
        public char char0008;
        [FieldOffset(9)]
        public char char0009;
        [FieldOffset(10)]
        public char char0010;
        [FieldOffset(11)]
        public char char0011;
        [FieldOffset(12)]
        public char char0012;
        [FieldOffset(13)]
        public char char0013;
        [FieldOffset(14)]
        public char char0014;
        [FieldOffset(15)]
        public char char0015;

        public int Length => 16;
        public char this[int index]
        {
            get
            {
                fixed (char* ptr = &char0000)
                {
                    return ptr[index];
                }
            }
        }

        public Char16(string str)
        {
            this = default(Char16);

            CharExtensions.CheckIsValidString(in this, in str);
            CharExtensions.Apply(ref this, in str, str.Length);
        }

        void IChar.Set(int index, char value)
        {
            fixed (char* ptr = &char0000)
            {
                ptr[index] = value;
            }
        }
        public string Read(int from, int to)
        {
            string sum = string.Empty;
            fixed (char* ptr = &char0000)
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

            fixed (char* ptr = &char0000)
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
