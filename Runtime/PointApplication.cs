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
using UnityEngine;
using UnityEngine.Scripting;

namespace Point.Collections
{
    [AddComponentMenu("")]
    internal sealed class PointApplication : StaticMonobehaviour<PointApplication>
    {
        #region Statics

        [Preserve, RuntimeInitializeOnLoadMethod]
        private static void Initialize()
        {
            PointApplication app = Instance;
        }
        private static bool s_IsShutdown = false;

        public static bool IsShutdown => s_IsShutdown;

        #endregion

        public override bool EnableLog => false;
        public override bool HideInInspector => true;

        public event Action OnApplicationShutdown;

        public override void OnShutdown()
        {
            s_IsShutdown = true;
            
            OnApplicationShutdown?.Invoke();
        }
    }
}
