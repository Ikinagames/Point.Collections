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

#if UNITY_2020_1_OR_NEWER && ENABLE_INPUT_SYSTEM
#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

namespace Point.Collections.Events
{
    public sealed class ApplicationInActiveEvent : SynchronousEvent<ApplicationInActiveEvent>
    {
        public bool InActive { get; private set; }

        internal static ApplicationInActiveEvent GetEvent(bool inActive)
        {
            var ev = Dequeue();

            ev.InActive = inActive;

            return ev;
        }
    }
}

#endif