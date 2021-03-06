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

using System;
using System.Threading;

namespace Point.Collections.Threading
{
    /// <summary>
    /// Thread-safe ValueType <typeparamref name="T"/> 입니다.
    /// </summary>
    /// <remarks>
    /// <typeparamref name="T"/> 가 ValueType 일 경우 stack 에 할당되며, ReferenceType 일 경우에는 heap 에 할당됩니다.
    /// </remarks>
    public struct AtomicSafeReference<T> where T : IEquatable<T>
    {
        private AtomicOperator m_AtomicOp;
        private T m_Value;

        public T Value
        {
            get
            {
                m_AtomicOp.Enter();
                T temp = m_Value;
                m_AtomicOp.Exit();

                return temp;
            }
            set
            {
                m_AtomicOp.Enter();
                m_Value = value;
                m_AtomicOp.Exit();
            }
        }

        public AtomicSafeReference(T value)
        {
            m_AtomicOp = new AtomicOperator();
            m_Value = value;
        }

        public bool Equals(AtomicSafeReference<T> other) => Value.Equals(other.Value);
        public override int GetHashCode() => Value.GetHashCode();
        public override bool Equals(object obj)
        {
            if (obj is int integer)
            {
                return Value.Equals(integer);
            }
            else if (obj is AtomicSafeReference<T> atomicInteger)
            {
                return Value.Equals(atomicInteger.Value);
            }

            return false;
        }
    }
}
