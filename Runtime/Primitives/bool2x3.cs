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
    public struct bool2x3 : IEquatable<bool2x3>
    {
        public bool2 c0;

        public bool2 c1;

        public bool2 c2;

        public unsafe ref bool2 this[int index]
        {
            get
            {
                if ((uint)index >= 3u)
                {
                    throw new ArgumentException("index must be between[0...2]");
                }

                fixed (bool2x3* ptr = &this)
                {
                    return ref *(bool2*)((byte*)ptr + index * sizeof(bool2));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool2x3(bool2 c0, bool2 c1, bool2 c2)
        {
            this.c0 = c0;
            this.c1 = c1;
            this.c2 = c2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool2x3(bool m00, bool m01, bool m02, bool m10, bool m11, bool m12)
        {
            c0 = new bool2(m00, m10);
            c1 = new bool2(m01, m11);
            c2 = new bool2(m02, m12);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool2x3(bool v)
        {
            c0 = v;
            c1 = v;
            c2 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool2x3(bool v)
        {
            return new bool2x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator ==(bool2x3 lhs, bool2x3 rhs)
        {
            return new bool2x3(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator ==(bool2x3 lhs, bool rhs)
        {
            return new bool2x3(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator ==(bool lhs, bool2x3 rhs)
        {
            return new bool2x3(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator !=(bool2x3 lhs, bool2x3 rhs)
        {
            return new bool2x3(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator !=(bool2x3 lhs, bool rhs)
        {
            return new bool2x3(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator !=(bool lhs, bool2x3 rhs)
        {
            return new bool2x3(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator !(bool2x3 val)
        {
            return new bool2x3(!val.c0, !val.c1, !val.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator &(bool2x3 lhs, bool2x3 rhs)
        {
            return new bool2x3(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1, lhs.c2 & rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator &(bool2x3 lhs, bool rhs)
        {
            return new bool2x3(lhs.c0 & rhs, lhs.c1 & rhs, lhs.c2 & rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator &(bool lhs, bool2x3 rhs)
        {
            return new bool2x3(lhs & rhs.c0, lhs & rhs.c1, lhs & rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator |(bool2x3 lhs, bool2x3 rhs)
        {
            return new bool2x3(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1, lhs.c2 | rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator |(bool2x3 lhs, bool rhs)
        {
            return new bool2x3(lhs.c0 | rhs, lhs.c1 | rhs, lhs.c2 | rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator |(bool lhs, bool2x3 rhs)
        {
            return new bool2x3(lhs | rhs.c0, lhs | rhs.c1, lhs | rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator ^(bool2x3 lhs, bool2x3 rhs)
        {
            return new bool2x3(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1, lhs.c2 ^ rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator ^(bool2x3 lhs, bool rhs)
        {
            return new bool2x3(lhs.c0 ^ rhs, lhs.c1 ^ rhs, lhs.c2 ^ rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x3 operator ^(bool lhs, bool2x3 rhs)
        {
            return new bool2x3(lhs ^ rhs.c0, lhs ^ rhs.c1, lhs ^ rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(bool2x3 rhs)
        {
            if (c0.Equals(rhs.c0) && c1.Equals(rhs.c1))
            {
                return c2.Equals(rhs.c2);
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (o is bool2x3)
            {
                bool2x3 rhs = (bool2x3)o;
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
            return $"bool2x3({c0.x}, {c1.x}, {c2.x},  {c0.y}, {c1.y}, {c2.y})";
        }
    }
}

#endif