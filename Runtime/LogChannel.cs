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

namespace Point.Collections
{
    /// <summary>
    /// 로그 채널입니다.
    /// </summary>
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
}
