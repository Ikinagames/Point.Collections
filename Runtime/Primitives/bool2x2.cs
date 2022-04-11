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
using System.Runtime.CompilerServices;

namespace Point.Collections
{
    [Serializable]
    public struct bool2x2 : IEquatable<bool2x2>
    {
        public bool2 c0;

        public bool2 c1;

        public unsafe ref bool2 this[int index]
        {
            get
            {
                if ((uint)index >= 2u)
                {
                    throw new ArgumentException("index must be between[0...1]");
                }

                fixed (bool2x2* ptr = &this)
                {
                    return ref *(bool2*)((byte*)ptr + index * sizeof(bool2));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool2x2(bool2 c0, bool2 c1)
        {
            this.c0 = c0;
            this.c1 = c1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool2x2(bool m00, bool m01, bool m10, bool m11)
        {
            c0 = new bool2(m00, m10);
            c1 = new bool2(m01, m11);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool2x2(bool v)
        {
            c0 = v;
            c1 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool2x2(bool v)
        {
            return new bool2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator ==(bool2x2 lhs, bool2x2 rhs)
        {
            return new bool2x2(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator ==(bool2x2 lhs, bool rhs)
        {
            return new bool2x2(lhs.c0 == rhs, lhs.c1 == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator ==(bool lhs, bool2x2 rhs)
        {
            return new bool2x2(lhs == rhs.c0, lhs == rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator !=(bool2x2 lhs, bool2x2 rhs)
        {
            return new bool2x2(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator !=(bool2x2 lhs, bool rhs)
        {
            return new bool2x2(lhs.c0 != rhs, lhs.c1 != rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator !=(bool lhs, bool2x2 rhs)
        {
            return new bool2x2(lhs != rhs.c0, lhs != rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator !(bool2x2 val)
        {
            return new bool2x2(!val.c0, !val.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator &(bool2x2 lhs, bool2x2 rhs)
        {
            return new bool2x2(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator &(bool2x2 lhs, bool rhs)
        {
            return new bool2x2(lhs.c0 & rhs, lhs.c1 & rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator &(bool lhs, bool2x2 rhs)
        {
            return new bool2x2(lhs & rhs.c0, lhs & rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator |(bool2x2 lhs, bool2x2 rhs)
        {
            return new bool2x2(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator |(bool2x2 lhs, bool rhs)
        {
            return new bool2x2(lhs.c0 | rhs, lhs.c1 | rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator |(bool lhs, bool2x2 rhs)
        {
            return new bool2x2(lhs | rhs.c0, lhs | rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator ^(bool2x2 lhs, bool2x2 rhs)
        {
            return new bool2x2(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator ^(bool2x2 lhs, bool rhs)
        {
            return new bool2x2(lhs.c0 ^ rhs, lhs.c1 ^ rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator ^(bool lhs, bool2x2 rhs)
        {
            return new bool2x2(lhs ^ rhs.c0, lhs ^ rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(bool2x2 rhs)
        {
            if (c0.Equals(rhs.c0))
            {
                return c1.Equals(rhs.c1);
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (o is bool2x2)
            {
                bool2x2 rhs = (bool2x2)o;
                return Equals(rhs);
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return (int)Math.hash(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return $"bool2x2({c0.x}, {c1.x},  {c0.y}, {c1.y})";
        }
    }
}

#endif