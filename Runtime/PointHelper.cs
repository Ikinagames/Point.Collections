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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Point.Collections
{
    /// <summary>
    /// 개발에 필요한 각종 유틸과 Point Framework 의 기본 유틸 클래스입니다.
    /// </summary>
    public static class PointHelper
    {
        internal const string
            c_WhiteSpace = " ",
            c_MessagePrefix = "[<color=lime>Point</color>]",
            c_Context = "[{0}]",
            c_InvalidString = "Invalid";

        public static string StringFormat(string msg)
        {
            return c_MessagePrefix + c_WhiteSpace + msg;
        }

        #region Log

#line hidden

        internal static LowLevel.LogHandler s_LogHandler = new LowLevel.LogHandler();

        private static LogChannel LogChannel => PointSettings.Instance.LogChannel;

        private static string LogStringFormat(LogChannel channel, in string msg, int type)
        {
            string chan = PointSettings.Instance.GetUserChannelNames(channel);

            string context;
            switch (type)
            {
                // norm
                default:
                case 0:
                    context = string.Format(c_Context, chan);
                    break;
                // warn
                case 1:
                    context = string.Format(c_Context, HTMLString.String(in chan, StringColor.maroon));
                    break;
                // err
                case 2:
                    context = string.Format(c_Context, HTMLString.String(in chan, StringColor.teal));
                    break;
            }

            return c_MessagePrefix + string.Format(c_Context, channel.ToString()) + c_WhiteSpace + msg;
        }

        /// <summary>
        /// 해당 채널(<paramref name="channel"/>) 에 <paramref name="msg"/> 로그를 보냅니다.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(Channel channel, in string msg) => Log(channel, in msg, null);
        /// <summary><inheritdoc cref="Log(Channel, in string)"/>></summary>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <param name="context"></param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(Channel channel, in string msg, UnityEngine.Object context)
        {
            if ((LogChannel & channel) != channel) return;

            UnityEngine.Debug.Log(LogStringFormat(channel, in msg, 0), context);
        }

        /// <summary>
        /// 해당 채널(<paramref name="channel"/>) 에 <paramref name="msg"/> 주의 로그를 보냅니다.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogWarning(Channel channel, in string msg) => LogWarning(channel, in msg, null);
        /// <summary><inheritdoc cref="LogWarning(Channel, in string)"/></summary>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <param name="context"></param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogWarning(Channel channel, in string msg, UnityEngine.Object context)
        {
            if ((LogChannel & channel) != channel) return;

            UnityEngine.Debug.LogWarning(LogStringFormat(channel, in msg, 1), context);
        }

        public static string LogErrorString(Channel channel, in string msg) => LogStringFormat(channel, in msg, 2);
        /// <summary>
        /// 해당 채널(<paramref name="channel"/>) 에 <paramref name="msg"/> 에러 로그를 보냅니다.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogError(Channel channel, in string msg) => LogError(channel, in msg, null);
        /// <summary><inheritdoc cref="LogError(Channel, in string)"/></summary>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        /// <param name="context"></param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogError(Channel channel, in string msg, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogError(LogStringFormat(channel, in msg, 2), context);
        }

        /// <summary><inheritdoc cref="Log(Channel, in string)"/>></summary>
        /// <param name="msg"></param>
        /// <param name="channel"></param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void ToLog(this string msg, LogChannel channel = LogChannel.Core)
        {
            if ((LogChannel & channel) != channel) return;

            Log(channel, in msg);
        }
        /// <summary><inheritdoc cref="LogWarning(Channel, in string)"/></summary>
        /// <param name="msg"></param>
        /// <param name="channel"></param>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void ToLogError(this string msg, LogChannel channel = LogChannel.Core)
        {
            LogError(channel, in msg);
        }

#line default

        #endregion

        #region Threading

        /// <summary>
        /// 이 메소드가 실행된 <seealso cref="System.Threading.Thread"/> 가 Unity 의 메인 스크립트 스레드인지 반환합니다.
        /// </summary>
        /// <returns></returns>
        [Unity.Collections.NotBurstCompatible]
        public static bool AssertIsMainThread()
        {
            Threading.ThreadInfo currentThread = Threading.ThreadInfo.CurrentThread;

            return PointApplication.Instance.MainThread.Equals(currentThread);
        }
        /// <summary><inheritdoc cref="AssertIsMainThread"/></summary>
        /// <remarks>
        /// 만약 메인 스레드가 아닐 경우 에러 로그를 발생시킵니다.
        /// </remarks>
        [Unity.Collections.NotBurstCompatible]
        [System.Diagnostics.Conditional("DEBUG_MODE")]
        public static void AssertMainThread()
        {
            AssertThreadAffinity(PointApplication.Instance.MainThread);
        }
        /// <summary>
        /// 해당 스레드의 정보를 통해, 이 메소드가 실행된 <seealso cref="System.Threading.Thread"/> 가 일치하는지 반환합니다.
        /// </summary>
        /// <remarks>
        /// 만약 일치하지 않는다면 에러 로그를 발생시킵니다. 
        /// <br/><br/>
        /// C# 에서는 스레드의 선호도를 직접적으로 가져올 수 있는 방법이 제한적입니다. 
        /// <see cref="System.Diagnostics.Process"/> 를 통하여 선호도를 확인 할 수 있지만, 이는 Win32.dll 등 과 같은 
        /// 현재 프로그램이 실행되는 Operating 시스템에 크게 영향을 받기 때문에 적합하지 않습니다.
        /// <see cref="System.Threading.Thread.ManagedThreadId"/> 는 <see cref="GC"/> 에서 관리되는 Low-Level 관리 인덱스이며, 
        /// 이는 스레드 선호도를 의미하지 않습니다. 해당 인덱스를 통해 다른 스레드임을 확인하고, 만약 다른 스레드라면 다른 선호도를 갖고있다고 판단합니다.
        /// <br/><br/>
        /// 다른 인덱스이어도 같은 스레드 선호도를 공유하는 예외사항이 있습니다. (ex. id = 1(Affinity 0), id = 8(Affinity 0)) 
        /// </remarks>
        /// <param name="expectedAffinity"></param>
        [Unity.Collections.NotBurstCompatible]
        [System.Diagnostics.Conditional("DEBUG_MODE")]
        public static void AssertThreadAffinity(in Threading.ThreadInfo expectedAffinity)
        {
            Threading.ThreadInfo currentThread = Threading.ThreadInfo.CurrentThread;

            if (expectedAffinity.Equals(currentThread)) return;

            LogError(LogChannel.Core,
                $"Thread affinity error. Expected thread({expectedAffinity}) but {currentThread}");
        }
        /// <summary>
        /// 이 스레드가 <paramref name="other"/> 의 스레드와 같은 스레드인지 확인합니다. 
        /// 만약 다르다면 로그 에러를 표시합니다.
        /// </summary>
        /// <param name="other"></param>
        public static void Validate(this in Threading.ThreadInfo other)
        {
            AssertThreadAffinity(in other);
        }

        #endregion
    }
}
