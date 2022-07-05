// Copyright 2021 Ikina Games
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
#if UNITYENGINE && UNITY_COLLECTIONS
    [BurstCompatible]
#endif
    [StructLayout(LayoutKind.Sequential)]
    public struct BitArray32 : IEquatable<BitArray32>
    {
        private byte
            x01, x02, y01, y02;

        public unsafe bool this[int index]
        {
            get
            {
                int mul = 0;
                int left = index;
                while (left > 7)
                {
                    left -= 8;
                    mul++;
                }

                fixed (byte* ptr = &x01)
                {
                    byte* p = ptr + mul;
                    var target = 1 << left;
                    return (*p & target) == target;
                }
            }
            set
            {
                int mul = 0;
                int left = index;
                while (left > 7)
                {
                    left -= 8;
                    mul++;
                }

                fixed (byte* ptr = &x01)
                {
                    byte* p = ptr + mul;
                    var target = 1 << left;
                    if (((*p & target) == target) != value)
                    {
                        *p = (byte)(value ? *p + target : *p - target);
                    }

                    return;
                }
            }
        }
        public uint Value
        {
            get => ReadValue(0, 32);
            set
            {
                for (int i = 0; i < 32; i++)
                {
                    this[i] = ((value & (1 << i)) == (1 << i));
                }
            }
        }

        public BitArray32(uint value)
        {
            this = default(BitArray32);

            Value = value;
        }

        public uint ReadValue(int index, int length = 1)
        {
            uint result = 0;
            for (int i = index, j = 0; j < length; i++, j++)
            {
                uint x = this[i] ? 1u : 0;
                result += x << j;
            }
            return result;
        }
        public void SetValue(int index, uint value, int length = 1)
        {
#if DEBUG_MODE
            if (index < 0 || length + index > 32) throw new IndexOutOfRangeException();
            uint maxValue = 0;
            for (int i = 0; i < length; i++)
            {
                maxValue += 1u << i;
            }
            if (value > maxValue)
            {
                throw new ArgumentOutOfRangeException();
            }
#endif
            for (int i = index, j = 0; j < length; i++, j++)
            {
                this[i] = (value & (1 << j)) == (1 << j);
            }
        }

#if UNITYENGINE && UNITY_COLLECTIONS
        [NotBurstCompatible]
#endif
        public override string ToString()
        {
            return Convert.ToString(Value, 2);
        }
#if UNITYENGINE && UNITY_COLLECTIONS
        [NotBurstCompatible]
#endif
        public override bool Equals(object obj)
        {
            if (!(obj is uint)) return false;
            uint other = (uint)obj;
            return (this == other);
        }
        public bool Equals(BitArray32 other)
            => x01 == other.x01 && x02 == other.x02 && y01 == other.y01 && y02 == other.y02;
        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(BitArray32 x, BitArray32 y) => x.Equals(y);
        public static bool operator !=(BitArray32 x, BitArray32 y) => !x.Equals(y);

        public static implicit operator BitArray32(uint other) => new BitArray32(other);
        public static implicit operator uint(BitArray32 other) => other.Value;

        public static implicit operator BitArray64(BitArray32 other) => new BitArray64(other.Value);
    }
}
