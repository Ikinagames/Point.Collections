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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2020
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

//using Newtonsoft.Json;
//using System.Xml.Serialization;
//using UnityEditor;
//using UnityEngine;

//namespace Point.Collections
//{
//    public class TestSerializer : MonoBehaviour
//    {
//        public UnityEngine.GameObject prefab;

//        [TextArea]
//        public string output;

//        private void Start()
//        {
////             UnityEngine.Component[] components = prefab.GetComponents(TypeHelper.TypeOf<UnityEngine.Component>.Type);
////             output = EditorJsonUtility.ToJson(components, true);

//            //for (int i = 1; i < components.Length; i++)
//            {
//                //output += ",\n" + EditorJsonUtility.ToJson(components[i], true);
//                //output += JsonConvert.SerializeObject(prefab, Formatting.Indented) + "\n";
//            }
//            //output = JsonUtility.ToJson(prefab, true);

//            //AssetDatabase.id
//        }
//    }
//}
