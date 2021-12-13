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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Point.Collections
{
    public static class Point
    {
        private const string
            c_WhiteSpace = " ",
            c_MessagePrefix = "[Point]",
            c_Context = "[{0}]";

        public static string StringFormat(string msg)
        {
            return c_MessagePrefix + c_WhiteSpace + msg;
        }

        #region Log

#line hidden

        [System.Flags]
        public enum LogChannel
        {
            Default = 0,

            Collections = 0x0001,
            Audio = 0x0010,
        }
        public static LogChannel s_LogChannel = (LogChannel)~0;

        private static string LogStringFormat(LogChannel channel, in string msg)
        {
            return c_MessagePrefix + string.Format(c_Context, channel.ToString()) + c_WhiteSpace + msg;
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(LogChannel channel, in string msg)
        {
            if ((s_LogChannel & channel) != channel) return;

            UnityEngine.Debug.Log(LogStringFormat(channel, in msg));
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void Log(LogChannel channel, in string msg, UnityEngine.Object context)
        {
            if ((s_LogChannel & channel) != channel) return;

            UnityEngine.Debug.Log(LogStringFormat(channel, in msg), context);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogError(LogChannel channel, in string msg)
        {
            if ((s_LogChannel & channel) != channel) return;

            UnityEngine.Debug.LogError(LogStringFormat(channel, in msg));
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void LogError(LogChannel channel, in string msg, UnityEngine.Object context)
        {
            if ((s_LogChannel & channel) != channel) return;

            UnityEngine.Debug.LogError(LogStringFormat(channel, in msg), context);
        }

        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void ToLog(this string msg, LogChannel channel = LogChannel.Default)
        {
            if ((s_LogChannel & channel) != channel) return;

            Log(channel, in msg);
        }
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void ToLogError(this string msg, LogChannel channel = LogChannel.Default)
        {
            if ((s_LogChannel & channel) != channel) return;

            LogError(channel, in msg);
        }

#line default

        #endregion

        //public static bool IsShutdown => IkinaApplication.IsShutdown;
    }
}
