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
using UnityEngine.Scripting;
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Point.Collections.IO.Json
{
#if UNITYENGINE
    [Preserve]
#endif
    internal sealed class Float3JsonConverter : JsonConverter<float3>
    {
        public static Float3JsonConverter Static = new Float3JsonConverter();

        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override float3 ReadJson(JsonReader reader, Type objectType, float3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JArray jo = (JArray)JToken.Load(reader);
            return new float3(jo[0].Value<float>(), jo[1].Value<float>(), jo[2].Value<float>());
        }

        public override void WriteJson(JsonWriter writer, float3 value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(value.x);
            writer.WriteValue(value.y);
            writer.WriteValue(value.z);
            writer.WriteEndArray();
        }
    }
}
