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
    public struct bool3x4 : IEquatable<bool3x4>
    {
        public bool3 c0;

        public bool3 c1;

        public bool3 c2;

        public bool3 c3;

        public unsafe ref bool3 this[int index]
        {
            get
            {
                if ((uint)index >= 4u)
                {
                    throw new ArgumentException("index must be between[0...3]");
                }

                fixed (bool3x4* ptr = &this)
                {
                    return ref *(bool3*)((byte*)ptr + index * sizeof(bool3));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool3x4(bool3 c0, bool3 c1, bool3 c2, bool3 c3)
        {
            this.c0 = c0;
            this.c1 = c1;
            this.c2 = c2;
            this.c3 = c3;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool3x4(bool m00, bool m01, bool m02, bool m03, bool m10, bool m11, bool m12, bool m13, bool m20, bool m21, bool m22, bool m23)
        {
            c0 = new bool3(m00, m10, m20);
            c1 = new bool3(m01, m11, m21);
            c2 = new bool3(m02, m12, m22);
            c3 = new bool3(m03, m13, m23);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool3x4(bool v)
        {
            c0 = v;
            c1 = v;
            c2 = v;
            c3 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool3x4(bool v)
        {
            return new bool3x4(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator ==(bool3x4 lhs, bool3x4 rhs)
        {
            return new bool3x4(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2, lhs.c3 == rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator ==(bool3x4 lhs, bool rhs)
        {
            return new bool3x4(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs, lhs.c3 == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator ==(bool lhs, bool3x4 rhs)
        {
            return new bool3x4(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2, lhs == rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator !=(bool3x4 lhs, bool3x4 rhs)
        {
            return new bool3x4(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2, lhs.c3 != rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator !=(bool3x4 lhs, bool rhs)
        {
            return new bool3x4(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs, lhs.c3 != rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator !=(bool lhs, bool3x4 rhs)
        {
            return new bool3x4(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2, lhs != rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator !(bool3x4 val)
        {
            return new bool3x4(!val.c0, !val.c1, !val.c2, !val.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator &(bool3x4 lhs, bool3x4 rhs)
        {
            return new bool3x4(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1, lhs.c2 & rhs.c2, lhs.c3 & rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator &(bool3x4 lhs, bool rhs)
        {
            return new bool3x4(lhs.c0 & rhs, lhs.c1 & rhs, lhs.c2 & rhs, lhs.c3 & rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator &(bool lhs, bool3x4 rhs)
        {
            return new bool3x4(lhs & rhs.c0, lhs & rhs.c1, lhs & rhs.c2, lhs & rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator |(bool3x4 lhs, bool3x4 rhs)
        {
            return new bool3x4(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1, lhs.c2 | rhs.c2, lhs.c3 | rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator |(bool3x4 lhs, bool rhs)
        {
            return new bool3x4(lhs.c0 | rhs, lhs.c1 | rhs, lhs.c2 | rhs, lhs.c3 | rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator |(bool lhs, bool3x4 rhs)
        {
            return new bool3x4(lhs | rhs.c0, lhs | rhs.c1, lhs | rhs.c2, lhs | rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator ^(bool3x4 lhs, bool3x4 rhs)
        {
            return new bool3x4(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1, lhs.c2 ^ rhs.c2, lhs.c3 ^ rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator ^(bool3x4 lhs, bool rhs)
        {
            return new bool3x4(lhs.c0 ^ rhs, lhs.c1 ^ rhs, lhs.c2 ^ rhs, lhs.c3 ^ rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool3x4 operator ^(bool lhs, bool3x4 rhs)
        {
            return new bool3x4(lhs ^ rhs.c0, lhs ^ rhs.c1, lhs ^ rhs.c2, lhs ^ rhs.c3);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(bool3x4 rhs)
        {
            if (c0.Equals(rhs.c0) && c1.Equals(rhs.c1) && c2.Equals(rhs.c2))
            {
                return c3.Equals(rhs.c3);
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (o is bool3x4)
            {
                bool3x4 rhs = (bool3x4)o;
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
            return $"bool3x4({c0.x}, {c1.x}, {c2.x}, {c3.x},  {c0.y}, {c1.y}, {c2.y}, {c3.y},  {c0.z}, {c1.z}, {c2.z}, {c3.z})";
        }
    }
}

#endif