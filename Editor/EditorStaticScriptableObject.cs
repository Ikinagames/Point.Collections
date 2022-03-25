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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2020
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using System.IO;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    /// <summary>
    /// 에디터에서만 사용되는 설정 값을 저장하는 <see cref="ScriptableObject"/> 입니다.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class EditorStaticScriptableObject<T> : ScriptableObject
        where T : ScriptableObject
    {
        private const string c_DefaultPath = "Assets/Editor/";

        private static T s_Instance;
        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    string filename = TypeHelper.TypeOf<T>.ToString();
                    if (!Directory.Exists(c_DefaultPath))
                    {
                        Directory.CreateDirectory(c_DefaultPath);
                    }

                    string path = Path.Combine(c_DefaultPath) + filename + ".asset";
                    T obj = AssetDatabase.LoadAssetAtPath<T>(path);
                    if (obj == null)
                    {
                        obj = ScriptableObject.CreateInstance<T>();

                        AssetDatabase.CreateAsset(obj, path);
                        AssetDatabase.SaveAssets();
                    }

                    (obj as EditorStaticScriptableObject<T>).OnInitialize();

                    s_Instance = obj;
                }

                return s_Instance;
            }
        }

        protected virtual void OnInitialize() { }
    }
}

#endif