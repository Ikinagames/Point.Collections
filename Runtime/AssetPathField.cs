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

#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.IO;
using UnityEngine;
using System.Text.RegularExpressions;
using Point.Collections.ResourceControl;

namespace Point.Collections
{
    /// <summary>
    /// <see cref="UnityEngine.Object"/> 의 reletive path 를 담을 수 있습니다.
    /// </summary>
    [System.Serializable]
    public class AssetPathField : IEmpty, IEquatable<AssetPathField>
#if UNITY_EDITOR
        , ISerializationCallbackReceiver
#endif
    {
#if UNITY_EDITOR
        [NonSerialized] private UnityEngine.Object m_EditorAsset;
        [SerializeField] protected string p_AssetGUID = string.Empty;
#endif
        [SerializeField] protected string p_AssetPath = string.Empty;
        [SerializeField] protected string p_SubAssetName = string.Empty;

#if UNITY_2020_1_OR_NEWER
        [NonSerialized] protected AssetInfo p_AssetInfo;
#endif

        public virtual System.Type TargetType => TypeHelper.TypeOf<UnityEngine.Object>.Type;
        public bool IsSubAsset => !p_SubAssetName.IsNullOrEmpty();
        public string AssetPath
        {
            get => p_AssetPath;
            set
            {
                p_AssetPath = value;
#if UNITY_EDITOR
                m_EditorAsset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(value);
#endif
            }
        }
        public string SubAssetName { get => p_SubAssetName; set => p_SubAssetName = value; }

#if UNITY_EDITOR
        /// <summary>
        /// Editor only
        /// </summary>
        public UnityEngine.Object EditorAsset
        {
            get
            {
                if (m_EditorAsset == null)
                {
                    if (p_AssetPath.IsNullOrEmpty())
                    {
                        return null;
                    }

                    m_EditorAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(p_AssetPath);
                }

                return m_EditorAsset;
            }
            set
            {
                p_AssetPath = UnityEditor.AssetDatabase.GetAssetPath(value);
                m_EditorAsset = value;
            }
        }
#endif
#if UNITY_2020_1_OR_NEWER
        public AssetInfo Asset
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    PointHelper.LogError(Channel.Collections,
                        $"You cannot load AssetInfo from editor.");

                    return AssetInfo.Invalid;
                }
#endif
                if (!p_AssetInfo.IsValid() && !IsEmpty())
                {
                    p_AssetInfo = ResourceManager.LoadAsset(this.ToString());
                }
                return p_AssetInfo;
            }
        }
#endif

        public AssetPathField(string path) : this(path, string.Empty) { }
        public AssetPathField(string path, string subAssetName)
        {
            p_AssetPath = path;
            p_SubAssetName = subAssetName;
        }
        ~AssetPathField()
        {
#if UNITY_2020_1_OR_NEWER
            if (p_AssetInfo.IsValid())
            {
                p_AssetInfo.Reserve();
            }
#endif
        }

        public bool IsInResourceFolder()
        {
            const string c_Str = "resources";
            var vs = p_AssetPath.Split('/', StringSplitOptions.None);
            for (int i = 0; i < vs.Length; i++)
            {
                if (vs[0].ToLowerInvariant().Contains(c_Str)) return true;
            }
            return false;
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editor Only
        /// </summary>
        /// <returns></returns>
        public bool IsInEditorFolder()
        {
            const string c_Str = "editor";
            return p_AssetPath.ToLowerInvariant().Contains(c_Str);
        }
        /// <summary>
        /// Editor Only
        /// </summary>
        /// <returns></returns>
        public bool IsInPluginFolder()
        {
            const string c_Str = "plugins";
            return p_AssetPath.ToLowerInvariant().Contains(c_Str);
        }
        /// <summary>
        /// Editor Only 
        /// <inheritdoc cref="UnityEditor.AssetDatabase.GetDependencies(string)"/>
        /// </summary>
        /// <returns>
        /// <inheritdoc cref="UnityEditor.AssetDatabase.GetDependencies(string)"/>
        /// </returns>
        public string[] GetDependencies()
        {
            return UnityEditor.AssetDatabase.GetDependencies(p_AssetPath);
        }
#endif
#if UNITY_2020_1_OR_NEWER
        public AssetInfo LoadAsset()
        {
            if (p_AssetInfo.IsValid()) return p_AssetInfo;

            p_AssetInfo = ResourceManager.LoadAsset(this.ToString());
            return p_AssetInfo;
        }
#endif

        public bool IsEmpty() => p_AssetPath.IsNullOrEmpty();
        public bool Equals(AssetPathField other) => p_AssetPath.Equals(other.p_AssetPath);

        public static AssetPathField Parse(string value)
        {
            var match = Regex.Match(value, @"(.+)" + Regex.Escape("[") + @"(.+)" + Regex.Escape("]"));
            if (match.Success)
            {
                return new AssetPathField(match.Groups[1].Value, match.Groups[2].Value);
            }
            return new AssetPathField(string.Empty);
        }
        public override string ToString()
        {
            const string c_Format = "{0}[{1}]";

            if (IsSubAsset) return string.Format(c_Format, AssetPath, SubAssetName);
            return AssetPath;
        }

#if UNITY_EDITOR
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            if (!p_AssetGUID.IsNullOrEmpty())
            {
                p_AssetPath = AssetDatabase.GUIDToAssetPath(p_AssetGUID);
                if (p_AssetPath.IsNullOrEmpty())
                {
                    p_AssetGUID = String.Empty;
                }
            }
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            //if (!p_AssetPath.IsNullOrEmpty())
            //{
            //    p_AssetGUID = AssetDatabase.AssetPathToGUID(p_AssetPath);
            //}
        }
#endif
    }
    /// <inheritdoc cref="AssetPathField"/>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class AssetPathField<T> : AssetPathField, IEquatable<AssetPathField<T>>
        where T : UnityEngine.Object
    {
        public override System.Type TargetType => TypeHelper.TypeOf<T>.Type;

#if UNITY_EDITOR
        /// <summary>
        /// Editor only
        /// </summary>
        public new T EditorAsset
        {
            get
            {
                if (string.IsNullOrEmpty(p_AssetPath))
                {
                    return null;
                }

                return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(p_AssetPath);
            }
            set
            {
                p_AssetPath = UnityEditor.AssetDatabase.GetAssetPath(value);
            }
        }
#endif
#if UNITY_2020_1_OR_NEWER
        public new AssetInfo<T> Asset
        {
            get
            {
#if UNITY_EDITOR
                if (!Application.isPlaying)
                {
                    PointHelper.LogError(Channel.Collections,
                        $"You cannot load AssetInfo from editor.");

                    return AssetInfo<T>.Invalid;
                }
#endif
                return LoadAsset();
            }
        }
#endif

        public AssetPathField(string path) : base(path) { }

#if UNITY_2020_1_OR_NEWER
        public new AssetInfo<T> LoadAsset()
        {
            if (p_AssetInfo.IsValid()) return (AssetInfo<T>)p_AssetInfo;

            p_AssetInfo = ResourceManager.LoadAsset(this.ToString());
            return (AssetInfo<T>)p_AssetInfo;
        }
#endif

        public bool Equals(AssetPathField<T> other) => p_AssetPath.Equals(other.p_AssetPath) && TargetType.Equals(other.TargetType);
    }
    [Serializable]
    public sealed class AudioClipPathField : AssetPathField<AudioClip>
    {
        public AudioClipPathField(string path) : base(path) { }
    }
}

#endif