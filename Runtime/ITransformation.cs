﻿// Copyright 2022 Ikina Games
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
#if UNITY_MATHEMATICS
using Unity.Mathematics;
#else
using math = Point.Collections.Math;
#endif
#else
#define POINT_COLLECTIONS_NATIVE
using math = Point.Collections.Math;
#endif


namespace Point.Collections
{
    public interface ITransformation
    {
#pragma warning disable IDE1006 // Naming Styles
        float4x4 localToWorld { get; }
        float4x4 worldToLocal { get; }
        float3 localPosition { get; set; }
        quaternion localRotation { get; set; }
        float3 localEulerAngles { get; set; }
        float3 localScale { get; set; }
#pragma warning restore IDE1006 // Naming Styles
    }
}
