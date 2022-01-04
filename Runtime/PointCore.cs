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

[assembly: System.Runtime.InteropServices.ComVisible(true)]

namespace Point.Collections
{
    public static class PointCore
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

        private static LogChannel LogChannel => PointSettings.Instance.LogChannel;

        private static string LogStringFormat(LogChannel channel, in string msg, int type)
        {
            string chan = TypeHelper.Enum<LogChannel>.ToString(channel);
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

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(LogChannel channel, in string msg) => Log(channel, in msg, null);
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(LogChannel channel, in string msg, UnityEngine.Object context)
        {
            if ((LogChannel & channel) != channel) return;

            UnityEngine.Debug.Log(LogStringFormat(channel, in msg, 0), context);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogWarning(LogChannel channel, in string msg) => LogWarning(channel, in msg, null);
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogWarning(LogChannel channel, in string msg, UnityEngine.Object context)
        {
            if ((LogChannel & channel) != channel) return;

            UnityEngine.Debug.LogWarning(LogStringFormat(channel, in msg, 1), context);
        }

        public static string LogErrorString(LogChannel channel, in string msg) => LogStringFormat(channel, in msg, 2);
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogError(LogChannel channel, in string msg) => LogError(channel, in msg, null);
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogError(LogChannel channel, in string msg, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogError(LogStringFormat(channel, in msg, 2), context);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void ToLog(this string msg, LogChannel channel = LogChannel.Core)
        {
            if ((LogChannel & channel) != channel) return;

            Log(channel, in msg);
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void ToLogError(this string msg, LogChannel channel = LogChannel.Core)
        {
            LogError(channel, in msg);
        }

#line default

        #endregion

        #region Threading

        [Unity.Collections.NotBurstCompatible]
        public static bool AssertIsMainThread()
        {
            Threading.ThreadInfo currentThread = Threading.ThreadInfo.CurrentThread;

            return PointApplication.Instance.MainThread.Equals(currentThread);
        }
        [Unity.Collections.NotBurstCompatible]
        [System.Diagnostics.Conditional("DEBUG_MODE")]
        public static void AssertMainThread()
        {
            AssertThreadAffinity(PointApplication.Instance.MainThread);
        }
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
