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
using Newtonsoft.Json.Linq;
using System;
using Unity.Mathematics;
using UnityEngine.Scripting;

namespace Point.Collections.IO.Json
{
    [Preserve]
    internal sealed class AABBJsonConverter : JsonConverter<AABB>
    {
        public override bool CanWrite => true;
        public override bool CanRead => true;

        public override AABB ReadJson(JsonReader reader, Type objectType, AABB existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            JArray center = (JArray)jo["Center"];
            JArray extents = (JArray)jo["Extents"];

            AABB aabb = new AABB
            (
                center: new float3(
                    center[0].ToObject<float>(),
                    center[1].ToObject<float>(),
                    center[2].ToObject<float>()),
                size: float3.zero
            );
            aabb.extents = new float3(
                    extents[0].ToObject<float>(),
                    extents[1].ToObject<float>(),
                    extents[2].ToObject<float>());

            return aabb;
        }
        public override void WriteJson(JsonWriter writer, AABB value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            writer.WritePropertyName("Center");
            writer.WriteStartArray();
            writer.WriteValue(value.center.x);
            writer.WriteValue(value.center.y);
            writer.WriteValue(value.center.z);
            writer.WriteEndArray();

            writer.WritePropertyName("Extents");
            writer.WriteStartArray();
            writer.WriteValue(value.extents.x);
            writer.WriteValue(value.extents.y);
            writer.WriteValue(value.extents.z);
            writer.WriteEndArray();

            writer.WriteEndObject();
        }
    }
}
