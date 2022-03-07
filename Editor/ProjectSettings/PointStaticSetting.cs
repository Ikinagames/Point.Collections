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

#if UNITYENGINE

using System;
using UnityEngine;

namespace Point.Collections.Editor
{
    /// <summary>
    /// Editor 상의 Project Settings 값을 추가 할 수 있는 상속 클래스입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class PointStaticSetting<T> : EditorStaticScriptableObject<T>, IPointStaticSetting
        where T : ScriptableObject
    {
        void IPointStaticSetting.OnSettingGUI(string searchContext) => OnSettingGUI(searchContext);

        /// <inheritdoc cref="IPointStaticSetting.OnSettingGUI(string)"/>
        protected virtual void OnSettingGUI(string searchContext) { }
    }
}

#endif