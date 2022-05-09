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
using UnityEngine;
using Unity.Collections;
#if UNITY_MATHEMATICS
using Unity.Mathematics;
#else
using math = Point.Collections.Math;
#endif
#else
#define POINT_COLLECTIONS_NATIVE
using math = Point.Collections.Math;
#endif

using Newtonsoft.Json;

namespace Point.Collections
{
#if UNITYENGINE && UNITY_COLLECTIONS
    [BurstCompatible]
#endif
    public struct Transformation
    {
        [JsonProperty(Order = 0, PropertyName = "localRotation")]
        public quaternion localRotation;
        [JsonProperty(Order = 1, PropertyName = "localPosition")]
        public float3 localPosition;
        [JsonProperty(Order = 2, PropertyName = "localScale")]
        public float3 localScale;

#if UNITYENGINE
#if UNITY_COLLECTIONS
        [NotBurstCompatible]
#endif
        public Transformation(Transform tr)
        {
            localPosition = tr.localPosition;
            localRotation = tr.localRotation;
            localScale = tr.localScale;
        }
        public Transformation(float3 position, quaternion rotation, float3 scale)
        {
            localPosition = position;
            localRotation = rotation;
            localScale = scale;
        }

        public static implicit operator Transformation(Transform t)
        {
            return new Transformation
            {
                localPosition = t.localPosition,
                localRotation = t.localRotation,
                localScale = t.localScale
            };
        }
#endif
    }
}
