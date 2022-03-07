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

#if UNITY_2020
#define UNITYENGINE
#endif

#if !UNITYENGINE
using System.Runtime.CompilerServices;

namespace Point.Collections
{
    [Serializable]
    public struct bool4x3 : IEquatable<bool4x3>
    {
        public bool4 c0;

        public bool4 c1;

        public bool4 c2;

        public unsafe ref bool4 this[int index]
        {
            get
            {
                if ((uint)index >= 3u)
                {
                    throw new ArgumentException("index must be between[0...2]");
                }

                fixed (bool4x3* ptr = &this)
                {
                    return ref *(bool4*)((byte*)ptr + (nint)index * (nint)sizeof(bool4));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool4x3(bool4 c0, bool4 c1, bool4 c2)
        {
            this.c0 = c0;
            this.c1 = c1;
            this.c2 = c2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool4x3(bool m00, bool m01, bool m02, bool m10, bool m11, bool m12, bool m20, bool m21, bool m22, bool m30, bool m31, bool m32)
        {
            c0 = new bool4(m00, m10, m20, m30);
            c1 = new bool4(m01, m11, m21, m31);
            c2 = new bool4(m02, m12, m22, m32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool4x3(bool v)
        {
            c0 = v;
            c1 = v;
            c2 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool4x3(bool v)
        {
            return new bool4x3(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x3 operator ==(bool4x3 lhs, bool4x3 rhs)
        {
            return new bool4x3(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1, lhs.c2 == rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x3 operator ==(bool4x3 lhs, bool rhs)
        {
            return new bool4x3(lhs.c0 == rhs, lhs.c1 == rhs, lhs.c2 == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x3 operator ==(bool lhs, bool4x3 rhs)
        {
            return new bool4x3(lhs == rhs.c0, lhs == rhs.c1, lhs == rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x3 operator !=(bool4x3 lhs, bool4x3 rhs)
        {
            return new bool4x3(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1, lhs.c2 != rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x3 operator !=(bool4x3 lhs, bool rhs)
        {
            return new bool4x3(lhs.c0 != rhs, lhs.c1 != rhs, lhs.c2 != rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x3 operator !=(bool lhs, bool4x3 rhs)
        {
            return new bool4x3(lhs != rhs.c0, lhs != rhs.c1, lhs != rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x3 operator !(bool4x3 val)
        {
            return new bool4x3(!val.c0, !val.c1, !val.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x3 operator &(bool4x3 lhs, bool4x3 rhs)
        {
            return new bool4x3(lhs.c0 & rhs.c0, lhs.c1 & rhs.c1, lhs.c2 & rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x3 operator &(bool4x3 lhs, bool rhs)
        {
            return new bool4x3(lhs.c0 & rhs, lhs.c1 & rhs, lhs.c2 & rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x3 operator &(bool lhs, bool4x3 rhs)
        {
            return new bool4x3(lhs & rhs.c0, lhs & rhs.c1, lhs & rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x3 operator |(bool4x3 lhs, bool4x3 rhs)
        {
            return new bool4x3(lhs.c0 | rhs.c0, lhs.c1 | rhs.c1, lhs.c2 | rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x3 operator |(bool4x3 lhs, bool rhs)
        {
            return new bool4x3(lhs.c0 | rhs, lhs.c1 | rhs, lhs.c2 | rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x3 operator |(bool lhs, bool4x3 rhs)
        {
            return new bool4x3(lhs | rhs.c0, lhs | rhs.c1, lhs | rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x3 operator ^(bool4x3 lhs, bool4x3 rhs)
        {
            return new bool4x3(lhs.c0 ^ rhs.c0, lhs.c1 ^ rhs.c1, lhs.c2 ^ rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x3 operator ^(bool4x3 lhs, bool rhs)
        {
            return new bool4x3(lhs.c0 ^ rhs, lhs.c1 ^ rhs, lhs.c2 ^ rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool4x3 operator ^(bool lhs, bool4x3 rhs)
        {
            return new bool4x3(lhs ^ rhs.c0, lhs ^ rhs.c1, lhs ^ rhs.c2);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(bool4x3 rhs)
        {
            if (c0.Equals(rhs.c0) && c1.Equals(rhs.c1))
            {
                return c2.Equals(rhs.c2);
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (o is bool4x3)
            {
                bool4x3 rhs = (bool4x3)o;
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
            return $"bool4x3({c0.x}, {c1.x}, {c2.x},  {c0.y}, {c1.y}, {c2.y},  {c0.z}, {c1.z}, {c2.z},  {c0.w}, {c1.w}, {c2.w})";
        }
    }
}

#endif