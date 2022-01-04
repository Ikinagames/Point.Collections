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
using Unity.Collections;

namespace Point.Collections
{
    [Flags]
    public enum LogChannel : int
    {
        None        = 0,
        User01      = 0b0000000000000000000000000000001,
        User02      = 0b0000000000000000000000000000010,
        User03      = 0b0000000000000000000000000000100,
        User04      = 0b0000000000000000000000000001000,
        User05      = 0b0000000000000000000000000010000,
        User06      = 0b0000000000000000000000000100000,
        User07      = 0b0000000000000000000000001000000,
        User08      = 0b0000000000000000000000010000000,
        User09      = 0b0000000000000000000000100000000,
        User10      = 0b0000000000000000000001000000000,
        User11      = 0b0000000000000000000010000000000,
        User12      = 0b0000000000000000000100000000000,
        User13      = 0b0000000000000000001000000000000,
        User14      = 0b0000000000000000010000000000000,
        User15      = 0b0000000000000000100000000000000,
        User16      = 0b0000000000000001000000000000000,
        User17      = 0b0000000000000010000000000000000,
        User18      = 0b0000000000000100000000000000000,
        User19      = 0b0000000000001000000000000000000,
        User20      = 0b0000000000010000000000000000000,
        User21      = 0b0000000000100000000000000000000,
        User22      = 0b0000000001000000000000000000000,
        User23      = 0b0000000010000000000000000000000,
        User24      = 0b0000000100000000000000000000000,
        User25      = 0b0000001000000000000000000000000,
        User26      = 0b0000010000000000000000000000000,
        User27      = 0b0000100000000000000000000000000,
        Audio       = 0b0001000000000000000000000000000,
        Collections = 0b0010000000000000000000000000000,
        Core        = 0b0100000000000000000000000000000,
        Editor      = 0b1000000000000000000000000000000,
        All         = ~0
    }

    [BurstCompatible]
    public struct Channel
    {
        public static Channel None => new Channel();
        public static Channel Audio => new Channel(LogChannel.Audio);
        public static Channel Collections => new Channel(LogChannel.Collections);
        public static Channel Core => new Channel(LogChannel.Core);
        public static Channel Editor => new Channel(LogChannel.Editor);

        private int m_LogChannel;
        private FixedString512Bytes m_Name;

        [NotBurstCompatible]
        private Channel(LogChannel channel)
        {
            m_LogChannel = (int)channel;
            m_Name = TypeHelper.Enum<LogChannel>.ToString(channel);
        }

        public static implicit operator Channel(string t)
        {
            if (string.IsNullOrEmpty(t)) return None;

            LogChannel channel = PointSettings.Instance.GetLogChannel(t);
            return new Channel(channel);
        }
        public static implicit operator LogChannel(Channel t) => (LogChannel)t.m_LogChannel;
        public static implicit operator Channel(LogChannel t) => new Channel(t);
        public static implicit operator Channel(int t) => new Channel((LogChannel)t);

        [NotBurstCompatible]
        public override string ToString() => m_Name.ToString();
    }
}
