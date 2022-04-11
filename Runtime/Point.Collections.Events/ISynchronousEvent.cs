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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System;

namespace Point.Collections.Events
{
    public interface ISynchronousEvent : IValidation, IDisposable
        , Point.Collections.Diagnostics.IStackDebugger
    {
        bool Reserved { get; }
        bool InternalEnableLog { get; }

        /// <summary>
        /// 이벤트 객체가 생성될 때 실행되는 메소드입니다.
        /// </summary>
        void OnCreated();
        void OnInitialize();

        /// <summary>
        /// 이벤트의 행동부입니다.
        /// </summary>
        void Execute();
        void Reserve();

        /// <summary>
        /// 이벤트가 반환될 때 실행되는 메소드입니다.
        /// </summary>
        void OnReserve();
    }
}
