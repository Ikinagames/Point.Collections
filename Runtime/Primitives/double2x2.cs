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
    public struct double2x2 : IEquatable<double2x2>, IFormattable
    {
        public double2 c0;

        public double2 c1;

        public static readonly double2x2 identity = new double2x2(1.0, 0.0, 0.0, 1.0);

        public static readonly double2x2 zero;

        public unsafe ref double2 this[int index]
        {
            get
            {
                if ((uint)index >= 2u)
                {
                    throw new ArgumentException("index must be between[0...1]");
                }

                fixed (double2x2* ptr = &this)
                {
                    return ref *(double2*)((byte*)ptr + (nint)index * (nint)sizeof(double2));
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x2(double2 c0, double2 c1)
        {
            this.c0 = c0;
            this.c1 = c1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x2(double m00, double m01, double m10, double m11)
        {
            c0 = new double2(m00, m10);
            c1 = new double2(m01, m11);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x2(double v)
        {
            c0 = v;
            c1 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x2(bool v)
        {
            c0 = Math.select(new double2(0.0), new double2(1.0), v);
            c1 = Math.select(new double2(0.0), new double2(1.0), v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x2(bool2x2 v)
        {
            c0 = Math.select(new double2(0.0), new double2(1.0), v.c0);
            c1 = Math.select(new double2(0.0), new double2(1.0), v.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x2(int v)
        {
            c0 = v;
            c1 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x2(int2x2 v)
        {
            c0 = v.c0;
            c1 = v.c1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x2(uint v)
        {
            c0 = v;
            c1 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x2(uint2x2 v)
        {
            c0 = v.c0;
            c1 = v.c1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x2(float v)
        {
            c0 = v;
            c1 = v;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public double2x2(float2x2 v)
        {
            c0 = v.c0;
            c1 = v.c1;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double2x2(double v)
        {
            return new double2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator double2x2(bool v)
        {
            return new double2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator double2x2(bool2x2 v)
        {
            return new double2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double2x2(int v)
        {
            return new double2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double2x2(int2x2 v)
        {
            return new double2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double2x2(uint v)
        {
            return new double2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double2x2(uint2x2 v)
        {
            return new double2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double2x2(float v)
        {
            return new double2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double2x2(float2x2 v)
        {
            return new double2x2(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator *(double2x2 lhs, double2x2 rhs)
        {
            return new double2x2(lhs.c0 * rhs.c0, lhs.c1 * rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator *(double2x2 lhs, double rhs)
        {
            return new double2x2(lhs.c0 * rhs, lhs.c1 * rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator *(double lhs, double2x2 rhs)
        {
            return new double2x2(lhs * rhs.c0, lhs * rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator +(double2x2 lhs, double2x2 rhs)
        {
            return new double2x2(lhs.c0 + rhs.c0, lhs.c1 + rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator +(double2x2 lhs, double rhs)
        {
            return new double2x2(lhs.c0 + rhs, lhs.c1 + rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator +(double lhs, double2x2 rhs)
        {
            return new double2x2(lhs + rhs.c0, lhs + rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator -(double2x2 lhs, double2x2 rhs)
        {
            return new double2x2(lhs.c0 - rhs.c0, lhs.c1 - rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator -(double2x2 lhs, double rhs)
        {
            return new double2x2(lhs.c0 - rhs, lhs.c1 - rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator -(double lhs, double2x2 rhs)
        {
            return new double2x2(lhs - rhs.c0, lhs - rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator /(double2x2 lhs, double2x2 rhs)
        {
            return new double2x2(lhs.c0 / rhs.c0, lhs.c1 / rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator /(double2x2 lhs, double rhs)
        {
            return new double2x2(lhs.c0 / rhs, lhs.c1 / rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator /(double lhs, double2x2 rhs)
        {
            return new double2x2(lhs / rhs.c0, lhs / rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator %(double2x2 lhs, double2x2 rhs)
        {
            return new double2x2(lhs.c0 % rhs.c0, lhs.c1 % rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator %(double2x2 lhs, double rhs)
        {
            return new double2x2(lhs.c0 % rhs, lhs.c1 % rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator %(double lhs, double2x2 rhs)
        {
            return new double2x2(lhs % rhs.c0, lhs % rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator ++(double2x2 val)
        {
            return new double2x2(++val.c0, ++val.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator --(double2x2 val)
        {
            return new double2x2(--val.c0, --val.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator <(double2x2 lhs, double2x2 rhs)
        {
            return new bool2x2(lhs.c0 < rhs.c0, lhs.c1 < rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator <(double2x2 lhs, double rhs)
        {
            return new bool2x2(lhs.c0 < rhs, lhs.c1 < rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator <(double lhs, double2x2 rhs)
        {
            return new bool2x2(lhs < rhs.c0, lhs < rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator <=(double2x2 lhs, double2x2 rhs)
        {
            return new bool2x2(lhs.c0 <= rhs.c0, lhs.c1 <= rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator <=(double2x2 lhs, double rhs)
        {
            return new bool2x2(lhs.c0 <= rhs, lhs.c1 <= rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator <=(double lhs, double2x2 rhs)
        {
            return new bool2x2(lhs <= rhs.c0, lhs <= rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator >(double2x2 lhs, double2x2 rhs)
        {
            return new bool2x2(lhs.c0 > rhs.c0, lhs.c1 > rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator >(double2x2 lhs, double rhs)
        {
            return new bool2x2(lhs.c0 > rhs, lhs.c1 > rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator >(double lhs, double2x2 rhs)
        {
            return new bool2x2(lhs > rhs.c0, lhs > rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator >=(double2x2 lhs, double2x2 rhs)
        {
            return new bool2x2(lhs.c0 >= rhs.c0, lhs.c1 >= rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator >=(double2x2 lhs, double rhs)
        {
            return new bool2x2(lhs.c0 >= rhs, lhs.c1 >= rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator >=(double lhs, double2x2 rhs)
        {
            return new bool2x2(lhs >= rhs.c0, lhs >= rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator -(double2x2 val)
        {
            return new double2x2(-val.c0, -val.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double2x2 operator +(double2x2 val)
        {
            return new double2x2(+val.c0, +val.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator ==(double2x2 lhs, double2x2 rhs)
        {
            return new bool2x2(lhs.c0 == rhs.c0, lhs.c1 == rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator ==(double2x2 lhs, double rhs)
        {
            return new bool2x2(lhs.c0 == rhs, lhs.c1 == rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator ==(double lhs, double2x2 rhs)
        {
            return new bool2x2(lhs == rhs.c0, lhs == rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator !=(double2x2 lhs, double2x2 rhs)
        {
            return new bool2x2(lhs.c0 != rhs.c0, lhs.c1 != rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator !=(double2x2 lhs, double rhs)
        {
            return new bool2x2(lhs.c0 != rhs, lhs.c1 != rhs);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool2x2 operator !=(double lhs, double2x2 rhs)
        {
            return new bool2x2(lhs != rhs.c0, lhs != rhs.c1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(double2x2 rhs)
        {
            if (c0.Equals(rhs.c0))
            {
                return c1.Equals(rhs.c1);
            }

            return false;
        }

        public override bool Equals(object o)
        {
            if (o is double2x2)
            {
                double2x2 rhs = (double2x2)o;
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
            return $"double2x2({c0.x}, {c1.x},  {c0.y}, {c1.y})";
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return $"double2x2({c0.x.ToString(format, formatProvider)}, {c1.x.ToString(format, formatProvider)},  {c0.y.ToString(format, formatProvider)}, {c1.y.ToString(format, formatProvider)})";
        }
    }
}

#endif