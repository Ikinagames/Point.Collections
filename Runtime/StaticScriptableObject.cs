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

#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;
using System.Reflection;
using System.IO;

namespace Point.Collections
{
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
    public abstract class StaticScriptableObject<T> : ScriptableObject
        where T : ScriptableObject
    {
        private static T s_Instance;
        /// <summary>
        /// <typeparamref name="T"/> 의 인스턴스를 반환합니다.
        /// </summary>
        /// <remarks>
        /// 만약 인스턴스가 생성되지 않았다면 즉시 생성하여 반환합니다.
        /// </remarks>
        public static T Instance
        {
            get
            {
                if (s_Instance == null)
                {
#if UNITY_EDITOR
                    const string c_EditorAssetPath = "Assets/Resources/";

                    if (!IsThisMainThread())
                    {
                        PointCore.LogError(PointCore.LogChannel.Collections,
                            $"{TypeHelper.TypeOf<StaticScriptableObject<T>>.ToString()} is only can be initialized in main thread but current thread looks like outside of UnityEngine. This is not allowed.");

                        throw new System.Exception("Internal error. See error log.");
                    }
#endif
                    string path;
                    var customPath = TypeHelper.TypeOf<T>.Type.GetCustomAttribute<AssetPathAttribute>();
                    if (customPath != null)
                    {
                        path = (customPath.Path);
                    }
                    else path = ("Point");

#if UNITY_EDITOR
                    if (!Directory.Exists(Path.Combine(c_EditorAssetPath, path)))
                    {
                        Directory.CreateDirectory(Path.Combine(c_EditorAssetPath, path));
                    }

#endif
                    s_Instance = Resources.Load<T>($"{path}/{TypeHelper.TypeOf<T>.ToString()}");
                    if (s_Instance == null)
                    {
                        PointCore.Log(PointCore.LogChannel.Collections,
                            $"Creating new StaticScriptableObject({TypeHelper.TypeOf<T>.ToString()}) asset");

                        s_Instance = CreateInstance<T>();
                        s_Instance.name = $"{TypeHelper.TypeOf<T>.Name}";

#if UNITY_EDITOR
                        AssetDatabase.CreateAsset(s_Instance, $"{Path.Combine(c_EditorAssetPath, path)}/{TypeHelper.TypeOf<T>.ToString()}.asset");
#endif
                    }

#if UNITY_EDITOR
                    if (Application.isPlaying)
#endif
                    {
                        if (!(s_Instance as StaticScriptableObject<T>).RuntimeModifiable) s_Instance = Instantiate(s_Instance);
                    }

                    (s_Instance as StaticScriptableObject<T>).OnInitialize();
                }

                return s_Instance;
            }
        }

        public static bool HasInstance => s_Instance != null;

        /// <summary>
        /// <see langword="true"/> 일 경우, 런타임에서도 원본이 수정되어 저장됩니다.<br/>
        /// 기본값은 <see langword="false"/>입니다.
        /// </summary>
        protected virtual bool RuntimeModifiable { get; } = false;

        protected virtual void OnInitialize() { }

        /// <summary>
        /// Editor Only, 만약 Runtime 에서 이 메소드가 호출되면 무조건 true 를 반환합니다.
        /// </summary>
        /// <returns></returns>
        protected static bool IsThisMainThread()
        {
#if UNITY_EDITOR
            if (!UnityEditorInternal.InternalEditorUtility.CurrentThreadIsMainThread())
            {
                return false;
            }
#endif
            return true;
        }
    }
}
