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
    public struct half : IEquatable<half>, IFormattable
    {
        public ushort value;

        public static readonly half zero;

        public static float MaxValue => 65504f;

        public static float MinValue => -65504f;

        public static half MaxValueAsHalf => new half(MaxValue);

        public static half MinValueAsHalf => new half(MinValue);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public half(half x)
        {
            value = x.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public half(float v)
        {
            value = (ushort)math.f32tof16(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public half(double v)
        {
            value = (ushort)math.f32tof16((float)v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator half(float v)
        {
            return new half(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static explicit operator half(double v)
        {
            return new half(v);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator float(half d)
        {
            return math.f16tof32(d.value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static implicit operator double(half d)
        {
            return math.f16tof32(d.value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(half lhs, half rhs)
        {
            return lhs.value == rhs.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(half lhs, half rhs)
        {
            return lhs.value != rhs.value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(half rhs)
        {
            return value == rhs.value;
        }

        public override bool Equals(object o)
        {
            if (o is half)
            {
                half rhs = (half)o;
                return Equals(rhs);
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override int GetHashCode()
        {
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public override string ToString()
        {
            return math.f16tof32(value).ToString();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string ToString(string format, IFormatProvider formatProvider)
        {
            return math.f16tof32(value).ToString(format, formatProvider);
        }
    }
}
#endif