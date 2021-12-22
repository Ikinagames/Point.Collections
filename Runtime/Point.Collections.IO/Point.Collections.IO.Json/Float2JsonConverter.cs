﻿// Copyright 2021 Ikina Games
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
    internal sealed class Float2JsonConverter : JsonConverter<float2>
    {
        public static Float2JsonConverter Static = new Float2JsonConverter();

        public override bool CanRead => true;
        public override bool CanWrite => true;

        public override float2 ReadJson(JsonReader reader, Type objectType, float2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JArray jo = (JArray)JToken.Load(reader);
            return new float2(jo[0].Value<float>(), jo[1].Value<float>());
        }

        public override void WriteJson(JsonWriter writer, float2 value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(value.x);
            writer.WriteValue(value.y);
            writer.WriteEndArray();
        }
    }
}
