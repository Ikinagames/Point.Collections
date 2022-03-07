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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2020
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System;
using System.Threading;
#if UNITYENGINE
using Unity.Collections;
#endif

namespace Point.Collections.Threading
{
    /// <summary>
    /// Thread 관련 각종 작업을 할 수 있는 구조체입니다.
    /// </summary>
#if UNITYENGINE
    [BurstCompatible]
#endif
    public struct ThreadInfo : IEquatable<ThreadInfo>, IEquatable<Thread>
    {
        [ThreadStatic]
        private static ThreadInfo s_CurrentThread = new ThreadInfo(Thread.CurrentThread);
        /// <summary>
        /// 현재 스레드 정보를 가져옵니다.
        /// </summary>
        public static ThreadInfo CurrentThread => s_CurrentThread;

        //
        //
        // https://www.sysnet.pe.kr/2/0/492

        private readonly int m_ManagedThreadID;
        private readonly int m_HashCode;
#if UNITYENGINE
        private readonly FixedString512Bytes m_Name;

        /// <summary>
        /// 현재 스레드의 이름입니다.
        /// </summary>
        [NotBurstCompatible]
        public string Name => m_Name.ToString();

        [NotBurstCompatible]
#endif
        public ThreadInfo(Thread thread)
        {
            m_ManagedThreadID = thread.ManagedThreadId;
            m_HashCode = thread.GetHashCode();

#if UNITYENGINE
            if (string.IsNullOrEmpty(thread.Name))
            {
                m_Name = "None";
            }
            else m_Name = thread.Name;
#endif
        }

        public bool Equals(ThreadInfo other)
        {
            if (m_ManagedThreadID != other.m_ManagedThreadID) return false;
            // 아마도 같은 Native thread (Processor) 로 매핑된 새로운 다른 스레드 객체일 것으로 추측됨.
            // 같은 native 로 연결되었으면 같은 스레드라고 판단하는 것으로
            //else if (m_HashCode != other.m_HashCode)
            //{
            //}

            return true;
        }
#if UNITYENGINE
        [NotBurstCompatible]
#endif
        public bool Equals(Thread other)
        {
            if (m_ManagedThreadID != other.ManagedThreadId) return false;
            // 아마도 같은 Native thread (Processor) 로 매핑된 새로운 다른 스레드 객체일 것으로 추측됨.
            // 같은 native 로 연결되었으면 같은 스레드라고 판단하는 것으로
            //else if (m_HashCode != other.GetHashCode())
            //{
            //}

            return true;
        }
#if UNITYENGINE
        [NotBurstCompatible]
#endif
        public override string ToString()
        {
#if UNITYENGINE
            string name = m_Name.ToString();
            return $"Thread({name}, {m_ManagedThreadID}, {m_HashCode})";
#else
            return $"Thread({m_ManagedThreadID}, {m_HashCode})";
#endif
        }
    }
}
