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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

#if UNITY_2019 || !UNITY_2020_OR_NEWER
#define UNITYENGINE_OLD
#endif

using UnityEngine;
using System;

namespace Point.Collections
{
    public sealed class PointSettings : StaticScriptableObject<PointSettings>
    {
        [Header("Logger")]
        [SerializeField] private LogChannel m_LogChannel = LogChannel.All;

        // Before 2020 version of unity cannot serialize generic classes.
#if UNITY_2020_1_OR_NEWER
        [SerializeField] private ArrayWrapper<string> m_UserChannelNames = new string[27];
#else
        [SerializeField] private string[] m_UserChannelNames = new string[27];
#endif

        [SerializeField] internal bool m_EnableLogFile = false;
        /// <summary>
        /// 경로는 <see cref="Application.persistentDataPath"/> 다음부터 시작됩니다. (ex. m_LogFilePath = "test.txt" => Application.persistentDataPath/test.txt
        /// </summary>
        [SerializeField] internal string m_LogFilePath = string.Empty;
        [Tooltip("PointApplication editor 에서 보여질 로그 줄 최대")]
        [SerializeField] public int m_LogDisplayLines = 30;

        [Header("Ingame")]
        [Tooltip("Point Application 객체를 Hierarchy 에 표시할지 결정합니다.")]
        [SerializeField] internal bool m_DisplayMainApplication = false; 
#if ENABLE_INPUT_SYSTEM
        [Tooltip("해당 시간(초) 만큼 인풋이 없을 경우 이벤트(ApplicationInActiveEvent)를 발생합니다.")]
        [SerializeField] private float m_InActiveTime = 120f;
#endif

        public LogChannel LogChannel { get => m_LogChannel; set => m_LogChannel = value; }
#if ENABLE_INPUT_SYSTEM
        public float InActiveTime { get => m_InActiveTime; set => m_InActiveTime = value; }
#endif

        public string GetUserChannelNames(LogChannel channel)
        {
            string names = string.Empty;
            var values = TypeHelper.Enum<LogChannel>.Values;
            for (int i = 0; i < values.Length; i++)
            {
                if ((channel & values[i]) != values[i])
                {
                    continue;
                }
                else if (!string.IsNullOrEmpty(names)) names += ", ";

                names += TypeHelper.Enum<LogChannel>.ToString(values[i]);
            }

            return name;
        }
        public string GetUserChannelName(LogChannel channel)
        {
#if UNITYENGINE_OLD
            switch (channel)
            {
                case LogChannel.User01:
                    return string.IsNullOrEmpty(m_UserChannelNames[0]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User01) : m_UserChannelNames[0];
                case LogChannel.User02:
                    return string.IsNullOrEmpty(m_UserChannelNames[1]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User02) : m_UserChannelNames[1];
                case LogChannel.User03:
                    return string.IsNullOrEmpty(m_UserChannelNames[2]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User03) : m_UserChannelNames[2];
                case LogChannel.User04:
                    return string.IsNullOrEmpty(m_UserChannelNames[3]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User04) : m_UserChannelNames[3];
                case LogChannel.User05:
                    return string.IsNullOrEmpty(m_UserChannelNames[4]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User05) : m_UserChannelNames[4];
                case LogChannel.User06:
                    return string.IsNullOrEmpty(m_UserChannelNames[5]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User06) : m_UserChannelNames[5];
                case LogChannel.User07:
                    return string.IsNullOrEmpty(m_UserChannelNames[6]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User07) : m_UserChannelNames[6];
                case LogChannel.User08:
                    return string.IsNullOrEmpty(m_UserChannelNames[7]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User08) : m_UserChannelNames[7];
                case LogChannel.User09:
                    return string.IsNullOrEmpty(m_UserChannelNames[8]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User09) : m_UserChannelNames[8];
                case LogChannel.User10:
                    return string.IsNullOrEmpty(m_UserChannelNames[9]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User10) : m_UserChannelNames[9];
                case LogChannel.User11:
                    return string.IsNullOrEmpty(m_UserChannelNames[10]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User11) : m_UserChannelNames[10];
                case LogChannel.User12:
                    return string.IsNullOrEmpty(m_UserChannelNames[11]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User12) : m_UserChannelNames[11];
                case LogChannel.User13:
                    return string.IsNullOrEmpty(m_UserChannelNames[12]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User13) : m_UserChannelNames[12];
                case LogChannel.User14:
                    return string.IsNullOrEmpty(m_UserChannelNames[13]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User14) : m_UserChannelNames[13];
                case LogChannel.User15:
                    return string.IsNullOrEmpty(m_UserChannelNames[14]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User15) : m_UserChannelNames[14];
                case LogChannel.User16:
                    return string.IsNullOrEmpty(m_UserChannelNames[15]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User16) : m_UserChannelNames[15];
                case LogChannel.User17:
                    return string.IsNullOrEmpty(m_UserChannelNames[16]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User17) : m_UserChannelNames[16];
                case LogChannel.User18:
                    return string.IsNullOrEmpty(m_UserChannelNames[17]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User18) : m_UserChannelNames[17];
                case LogChannel.User19:
                    return string.IsNullOrEmpty(m_UserChannelNames[18]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User19) : m_UserChannelNames[18];
                case LogChannel.User20:
                    return string.IsNullOrEmpty(m_UserChannelNames[19]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User20) : m_UserChannelNames[19];
                case LogChannel.User21:
                    return string.IsNullOrEmpty(m_UserChannelNames[20]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User21) : m_UserChannelNames[20];
                case LogChannel.User22:
                    return string.IsNullOrEmpty(m_UserChannelNames[21]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User22) : m_UserChannelNames[21];
                case LogChannel.User23:
                    return string.IsNullOrEmpty(m_UserChannelNames[22]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User23) : m_UserChannelNames[22];
                case LogChannel.User24:
                    return string.IsNullOrEmpty(m_UserChannelNames[23]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User24) : m_UserChannelNames[23];
                case LogChannel.User25:
                    return string.IsNullOrEmpty(m_UserChannelNames[24]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User25) : m_UserChannelNames[24];
                case LogChannel.User26:
                    return string.IsNullOrEmpty(m_UserChannelNames[25]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User26) : m_UserChannelNames[25];
                case LogChannel.User27:
                    return string.IsNullOrEmpty(m_UserChannelNames[26]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User27) : m_UserChannelNames[26];
                case LogChannel.Audio:
                    return TypeHelper.Enum<LogChannel>.ToString(LogChannel.Audio);
                case LogChannel.Collections:
                    return TypeHelper.Enum<LogChannel>.ToString(LogChannel.Collections);
                case LogChannel.Core:
                    return TypeHelper.Enum<LogChannel>.ToString(LogChannel.Core);
                case LogChannel.Editor:
                    return TypeHelper.Enum<LogChannel>.ToString(LogChannel.Editor);
                case LogChannel.None:
                case LogChannel.All:
                default:
                    return "None";
            }
#else
            return channel switch
            {
                LogChannel.User01 => string.IsNullOrEmpty(m_UserChannelNames[0]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User01) : m_UserChannelNames[0],
                LogChannel.User02 => string.IsNullOrEmpty(m_UserChannelNames[1]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User02) : m_UserChannelNames[1],
                LogChannel.User03 => string.IsNullOrEmpty(m_UserChannelNames[2]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User03) : m_UserChannelNames[2],
                LogChannel.User04 => string.IsNullOrEmpty(m_UserChannelNames[3]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User04) : m_UserChannelNames[3],
                LogChannel.User05 => string.IsNullOrEmpty(m_UserChannelNames[4]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User05) : m_UserChannelNames[4],
                LogChannel.User06 => string.IsNullOrEmpty(m_UserChannelNames[5]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User06) : m_UserChannelNames[5],
                LogChannel.User07 => string.IsNullOrEmpty(m_UserChannelNames[6]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User07) : m_UserChannelNames[6],
                LogChannel.User08 => string.IsNullOrEmpty(m_UserChannelNames[7]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User08) : m_UserChannelNames[7],
                LogChannel.User09 => string.IsNullOrEmpty(m_UserChannelNames[8]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User09) : m_UserChannelNames[8],
                LogChannel.User10 => string.IsNullOrEmpty(m_UserChannelNames[9]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User10) : m_UserChannelNames[9],
                LogChannel.User11 => string.IsNullOrEmpty(m_UserChannelNames[10]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User11) : m_UserChannelNames[10],
                LogChannel.User12 => string.IsNullOrEmpty(m_UserChannelNames[11]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User12) : m_UserChannelNames[11],
                LogChannel.User13 => string.IsNullOrEmpty(m_UserChannelNames[12]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User13) : m_UserChannelNames[12],
                LogChannel.User14 => string.IsNullOrEmpty(m_UserChannelNames[13]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User14) : m_UserChannelNames[13],
                LogChannel.User15 => string.IsNullOrEmpty(m_UserChannelNames[14]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User15) : m_UserChannelNames[14],
                LogChannel.User16 => string.IsNullOrEmpty(m_UserChannelNames[15]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User16) : m_UserChannelNames[15],
                LogChannel.User17 => string.IsNullOrEmpty(m_UserChannelNames[16]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User17) : m_UserChannelNames[16],
                LogChannel.User18 => string.IsNullOrEmpty(m_UserChannelNames[17]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User18) : m_UserChannelNames[17],
                LogChannel.User19 => string.IsNullOrEmpty(m_UserChannelNames[18]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User19) : m_UserChannelNames[18],
                LogChannel.User20 => string.IsNullOrEmpty(m_UserChannelNames[19]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User20) : m_UserChannelNames[19],
                LogChannel.User21 => string.IsNullOrEmpty(m_UserChannelNames[20]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User21) : m_UserChannelNames[20],
                LogChannel.User22 => string.IsNullOrEmpty(m_UserChannelNames[21]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User22) : m_UserChannelNames[21],
                LogChannel.User23 => string.IsNullOrEmpty(m_UserChannelNames[22]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User23) : m_UserChannelNames[22],
                LogChannel.User24 => string.IsNullOrEmpty(m_UserChannelNames[23]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User24) : m_UserChannelNames[23],
                LogChannel.User25 => string.IsNullOrEmpty(m_UserChannelNames[24]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User25) : m_UserChannelNames[24],
                LogChannel.User26 => string.IsNullOrEmpty(m_UserChannelNames[25]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User26) : m_UserChannelNames[25],
                LogChannel.User27 => string.IsNullOrEmpty(m_UserChannelNames[26]) ? TypeHelper.Enum<LogChannel>.ToString(LogChannel.User27) : m_UserChannelNames[26],
                LogChannel.Audio => TypeHelper.Enum<LogChannel>.ToString(LogChannel.Audio),
                LogChannel.Collections => TypeHelper.Enum<LogChannel>.ToString(LogChannel.Collections),
                LogChannel.Core => TypeHelper.Enum<LogChannel>.ToString(LogChannel.Core),
                LogChannel.Editor => TypeHelper.Enum<LogChannel>.ToString(LogChannel.Editor),
                _ => "None",
            };
#endif
        }
    }
}

#endif