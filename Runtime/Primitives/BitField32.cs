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
using System.Diagnostics;

using math = Point.Collections.Math;

namespace Point.Collections
{
    [DebuggerTypeProxy(typeof(BitField32.BitField32DebugView))]
    public struct BitField32
    {
        internal sealed class BitField32DebugView
        {
            private BitField32 BitField;

            public bool[] Bits
            {
                get
                {
                    bool[] array = new bool[32];
                    for (int i = 0; i < 32; i++)
                    {
                        array[i] = BitField.IsSet(i);
                    }

                    return array;
                }
            }

            public BitField32DebugView(BitField32 bitfield)
            {
                BitField = bitfield;
            }
        }

        public uint Value;

        public BitField32(uint initialValue = 0u)
        {
            Value = initialValue;
        }

        public void Clear()
        {
            Value = 0u;
        }

        public void SetBits(int pos, bool value)
        {
            CheckArgs(pos, 1);
            Value = Bitwise.SetBits(Value, pos, 1u, value);
        }

        public void SetBits(int pos, bool value, int numBits)
        {
            CheckArgs(pos, numBits);
            uint mask = uint.MaxValue >> 32 - numBits;
            Value = Bitwise.SetBits(Value, pos, mask, value);
        }

        public uint GetBits(int pos, int numBits = 1)
        {
            CheckArgs(pos, numBits);
            uint mask = uint.MaxValue >> 32 - numBits;
            return Bitwise.ExtractBits(Value, pos, mask);
        }

        public bool IsSet(int pos)
        {
            return GetBits(pos) != 0;
        }

        public bool TestNone(int pos, int numBits = 1)
        {
            return GetBits(pos, numBits) == 0;
        }

        public bool TestAny(int pos, int numBits = 1)
        {
            return GetBits(pos, numBits) != 0;
        }

        public bool TestAll(int pos, int numBits = 1)
        {
            CheckArgs(pos, numBits);
            uint num = uint.MaxValue >> 32 - numBits;
            return num == Bitwise.ExtractBits(Value, pos, num);
        }

        public int CountBits()
        {
            return math.countbits(Value);
        }

        public int CountLeadingZeros()
        {
            return math.lzcnt(Value);
        }

        public int CountTrailingZeros()
        {
            return math.tzcnt(Value);
        }

        [Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        private static void CheckArgs(int pos, int numBits)
        {
            if (pos > 31 || numBits == 0 || numBits > 32 || pos + numBits > 32)
            {
                throw new ArgumentException($"BitField32 invalid arguments: pos {pos} (must be 0-31), numBits {numBits} (must be 1-32).");
            }
        }
    }
}
#endif