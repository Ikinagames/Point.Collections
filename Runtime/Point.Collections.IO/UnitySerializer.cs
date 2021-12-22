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

using Newtonsoft.Json;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace Point.Collections.IO
{
    public static class UnitySerializer
    {
        public static void Serialize(UnityEngine.Object obj)
        {

        }
    }



    public struct TransformationJson
    {
        [JsonProperty(Order = 0, PropertyName = "localRotation")]
        public quaternion localRotation;
        [JsonProperty(Order = 1, PropertyName = "localPosition")]
        public float3 localPosition;
        [JsonProperty(Order = 2, PropertyName = "localScale")]
        public float3 localScale;

        public TransformationJson(Transform tr)
        {
            localPosition = tr.localPosition;
            localRotation = tr.localRotation;
            localScale = tr.localScale;
        }
    }
}
