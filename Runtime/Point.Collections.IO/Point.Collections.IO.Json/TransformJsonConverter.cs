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

#if UNITY_2020
#define UNITYENGINE
#endif

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
#if UNITYENGINE
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Scripting;

namespace Point.Collections.IO.Json
{
    [Preserve]
    internal sealed class TransformJsonConverter : JsonConverter<UnityEngine.Transform>
    {
#if UNITY_EDITOR

#endif

        public override bool CanRead => false;
        public override bool CanWrite => false;

        public override Transform ReadJson(JsonReader reader, Type objectType, Transform existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var json = serializer.Deserialize<Transformation>(reader);

            existingValue.localPosition = json.localPosition;
            existingValue.localRotation = json.localRotation;
            existingValue.localScale = json.localScale;

            return existingValue;
        }
        public override void WriteJson(JsonWriter writer, Transform value, JsonSerializer serializer)
        {
            Transformation json = new Transformation(value);
            serializer.Serialize(writer, json);
        }
    }
    internal sealed class GameObjectJsonConverter : JsonConverter<GameObject>
    {
        public override bool CanRead => false;
        public override bool CanWrite => false;

        private sealed class GameObjectJson
        {
            public string name;
            public string tagString;
            public HideFlags objectHideFlags;
            public bool staticEditorFlags;
            public int layer;
            public bool isActive;

            public Transformation transform;
            public string[] components;

            public GameObjectJson(GameObject obj)
            {
                name = obj.name;
                tagString = obj.tag;
                objectHideFlags = obj.hideFlags;
                staticEditorFlags = obj.isStatic;
                layer = obj.layer;
                isActive = obj.activeSelf;
            }
        }
        //private struct BaseComponentJson
        //{
        //    public HideFlags objectHideFlags;
        //    public bool enabled;
        //}
        //private sealed class ComponentJson
        //{

        //}
        public override GameObject ReadJson(JsonReader reader, Type objectType, GameObject existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            JObject jo = JObject.Load(reader);
            existingValue.name = jo["name"].ToString();
            existingValue.tag = jo["tagString"].ToString();
            existingValue.hideFlags = jo["objectHideFlags"].ToObject<HideFlags>();
            existingValue.isStatic = jo["staticEditorFlags"].ToObject<bool>();
            existingValue.layer = jo["layer"].ToObject<int>();
            existingValue.SetActive(jo["isActive"].ToObject<bool>());

            Transform tr = existingValue.transform;
            Transformation trJson = jo["transform"].ToObject<Transformation>();
            tr.localPosition = trJson.localPosition;
            tr.localRotation = trJson.localRotation;
            tr.localScale = trJson.localScale;

            throw new NotImplementedException();
        }
        public override void WriteJson(JsonWriter writer, GameObject value, JsonSerializer serializer)
        {
            writer.WriteStartObject();

            Guid guid = TypeHelper.TypeOf<GameObject>.Type.GUID;


            writer.WriteEndObject();
        }

        

    }
}

#endif