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
#if UNITY_COLLECTIONS
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Point.Collections.Actions
{
    [DisplayName("Unity/Destroy")]
    [Guid("AC591D0A-88AE-45DB-8915-929876111840")]
    internal sealed class DestroyObjectConstAction : ConstAction<int, UnityEngine.Object>
    {
        protected override int Execute(UnityEngine.Object arg0)
        {
            UnityEngine.Object.Destroy(arg0);
            return 0;
        }
    }
}
