﻿// Copyright 2022 Ikina Games
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
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Point.Collections
{
    [Serializable]
    [DebuggerTypeProxy(typeof(DebuggerProxy))]
    public struct bool2 : IEquatable<bool2>
    {
        internal sealed class DebuggerProxy
        {
            public bool x;

            public bool y;

            public DebuggerProxy(bool2 v)
            {
                x = v.x;
                y = v.y;
            }
        }

        [MarshalAs(UnmanagedType.U1)]
        public bool x;

        [MarshalAs(UnmanagedType.U1)]
        public bool y;

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool4 xxxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool4(x, x, x, x);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool4 xxxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool4(x, x, x, y);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool4 xxyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool4(x, x, y, x);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool4 xxyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool4(x, x, y, y);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool4 xyxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool4(x, y, x, x);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool4 xyxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool4(x, y, x, y);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool4 xyyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool4(x, y, y, x);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool4 xyyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool4(x, y, y, y);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool4 yxxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool4(y, x, x, x);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool4 yxxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool4(y, x, x, y);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool4 yxyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool4(y, x, y, x);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool4 yxyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool4(y, x, y, y);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool4 yyxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool4(y, y, x, x);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool4 yyxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool4(y, y, x, y);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool4 yyyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool4(y, y, y, x);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool4 yyyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool4(y, y, y, y);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool3 xxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool3(x, x, x);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool3 xxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool3(x, x, y);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool3 xyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool3(x, y, x);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool3 xyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool3(x, y, y);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool3 yxx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool3(y, x, x);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool3 yxy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool3(y, x, y);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool3 yyx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool3(y, y, x);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool3 yyy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool3(y, y, y);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool2 xx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool2(x, x);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool2 xy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool2(x, y);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                x = value.x;
                y = value.y;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool2 yx
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool2(y, x);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                y = value.x;
                x = value.y;
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool2 yy
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return new bool2(y, y);
            }
        }

        public unsafe bool this[int index]
        {
            get
            {
                if ((uint)index >= 2u)
                {
                    throw new ArgumentException("index must be between[0...1]");
                }

                fixed (bool2* ptr = &this)
                {
                    return ((byte*)ptr)[index] != 0;
                }
            }
            set
            {
                if ((uint)index >= 2u)
                {
                    throw new ArgumentException("index must be between[0...1]");
                }

                fixed (bool* ptr = &x)
                {
                    ptr[index] = value;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool2(bool x, bool y)
        {
            this.x = x;
            this.y = y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool2(bool2 xy)
        {
            x = xy.x;
            y = xy.y;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool2(bool v)
        {
            x = v;
            y = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator bool2(bool v)
        {
            return new bool2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator ==(bool2 lhs, bool2 rhs)
        {
            return new bool2(lhs.x == rhs.x, lhs.y == rhs.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator ==(bool2 lhs, bool rhs)
        {
            return new bool2(lhs.x == rhs, lhs.y == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator ==(bool lhs, bool2 rhs)
        {
            return new bool2(lhs == rhs.x, lhs == rhs.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator !=(bool2 lhs, bool2 rhs)
        {
            return new bool2(lhs.x != rhs.x, lhs.y != rhs.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator !=(bool2 lhs, bool rhs)
        {
            return new bool2(lhs.x != rhs, lhs.y != rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator !=(bool lhs, bool2 rhs)
        {
            return new bool2(lhs != rhs.x, lhs != rhs.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator !(bool2 val)
        {
            return new bool2(!val.x, !val.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator &(bool2 lhs, bool2 rhs)
        {
            return new bool2(lhs.x & rhs.x, lhs.y & rhs.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator &(bool2 lhs, bool rhs)
        {
            return new bool2(lhs.x && rhs, lhs.y && rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator &(bool lhs, bool2 rhs)
        {
            return new bool2(lhs & rhs.x, lhs & rhs.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator |(bool2 lhs, bool2 rhs)
        {
            return new bool2(lhs.x | rhs.x, lhs.y | rhs.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator |(bool2 lhs, bool rhs)
        {
            return new bool2(lhs.x || rhs, lhs.y || rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator |(bool lhs, bool2 rhs)
        {
            return new bool2(lhs | rhs.x, lhs | rhs.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator ^(bool2 lhs, bool2 rhs)
        {
            return new bool2(lhs.x ^ rhs.x, lhs.y ^ rhs.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator ^(bool2 lhs, bool rhs)
        {
            return new bool2(lhs.x ^ rhs, lhs.y ^ rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2 operator ^(bool lhs, bool2 rhs)
        {
            return new bool2(lhs ^ rhs.x, lhs ^ rhs.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(bool2 rhs)
        {
            if (x == rhs.x)
            {
                return y == rhs.y;
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (o is bool2)
            {
                bool2 rhs = (bool2)o;
                return Equals(rhs);
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return (int)math.hash(this);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return $"bool2({x}, {y})";
        }
    }
}
#endif