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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#if UNITY_2019 && !UNITY_2020_1_OR_NEWER
#define UNITYENGINE_OLD
#endif

using System;
using System.Linq;
using System.Text.RegularExpressions;
using Unity.Collections;
using UnityEngine;
#if UNITY_ADDRESSABLES
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using AddressableReference = UnityEngine.AddressableAssets.AssetReference;
#endif

namespace Point.Collections.ResourceControl
{
    /// <summary>
    /// <see cref="ResourceManager"/> 에서 <see cref="AssetBundle"/>, 혹은 Unity.Addressables 에 접근하기 위한 키 입니다.
    /// </summary>
    /// <remarks>
    /// 런타임에서는 효율을 위해 <seealso cref="AssetRuntimeKey"/> 가 사용될 수 있습니다. 리소스 접근 권한을 가진 구조체 중 메모리 효율이 가장 낮습니다. <seealso cref="AssetRuntimeKey"/>, <seealso cref="AssetIndex"/> 를 참조하세요.
    /// </remarks>
    [Serializable]
    public struct AssetReference : IValidation, IEmpty, IEquatable<AssetReference>
#if UNITY_ADDRESSABLES && !UNITYENGINE_OLD
        , IKeyEvaluator
#endif
    {
        public static AssetReference Empty => new AssetReference();

#if UNITY_COLLECTIONS
        [SerializeField] private FixedString128Bytes m_Key;
        [SerializeField] private FixedString128Bytes m_SubAssetName;
#else
        [SerializeField] private FixedChar128Bytes m_Key;
        [SerializeField] private FixedChar128Bytes m_SubAssetName;
#endif

#if UNITY_ADDRESSABLES && !UNITYENGINE_OLD
        object IKeyEvaluator.RuntimeKey
        {
            get
            {
                if (m_Key.IsEmpty) return string.Empty;

                const string c_Format = "{0}[{1}]";
                if (!m_SubAssetName.IsEmpty)
                {
                    return string.Format(c_Format, m_Key.ToString().ToLowerInvariant(), m_SubAssetName.ToString());
                }
                return m_Key.ToString();
            }
        }
        public AssetRuntimeKey RuntimeKey => new AssetRuntimeKey(FNV1a32.Calculate(((IKeyEvaluator)this).RuntimeKey.ToString()));
#else
        public AssetRuntimeKey RuntimeKey
        {
            get
            {
                const string c_Format = "{0}[{1}]";

                string key;
                if (!m_SubAssetName.IsEmpty)
                {
                    key = string.Format(c_Format,
                        m_Key.ToString().ToLowerInvariant(),
                        m_SubAssetName.ToString());
                }
                else key = m_Key.ToString().ToLowerInvariant();

                return new AssetRuntimeKey(key);
            }
        }
#endif
        public bool IsSubAsset => !m_SubAssetName.IsEmpty;
        public FixedString128Bytes Key => m_Key;
        public FixedString128Bytes SubAssetName => m_SubAssetName;
#if UNITY_EDITOR
        public UnityEngine.Object editorMainAsset
        {
            get
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(m_Key.ToString());
            }
        }
        public UnityEngine.Object editorAsset
        {
            get
            {
                if (!IsSubAsset) return editorMainAsset;

                string subAssetName = m_SubAssetName.ToString();
                var found = UnityEditor.AssetDatabase
                    .LoadAllAssetsAtPath(m_Key.ToString())
                    .Where(t => t.name.Equals(subAssetName));

                return found.Any() ? found.First() : null;
            }
        }
#endif

#if UNITY_ADDRESSABLES && !UNITYENGINE_OLD
        public AsyncOperationHandle<IResourceLocation> Location => ResourceManager.GetLocation(this, TypeHelper.TypeOf<UnityEngine.Object>.Type);
#endif
#if UNITY_COLLECTIONS
        public AssetReference(FixedString128Bytes key) : this(key, default) { }
        public AssetReference(FixedString128Bytes key, FixedString128Bytes subAssetName)
        {
            m_Key = key;
            m_SubAssetName = subAssetName;
        }
#else
        public AssetReference(FixedChar128Bytes key) : this(key, default) { }
        public AssetReference(FixedChar128Bytes key, FixedChar128Bytes subAssetName)
        {
            m_Key = key;
            m_SubAssetName = subAssetName;
        }
#endif

        public bool IsEmpty()
        {
            return m_Key.IsEmpty || (m_Key.IsEmpty && m_SubAssetName.IsEmpty);
        }
        public bool IsValid()
        {
            if (m_Key.IsEmpty) return false;

#if UNITY_ADDRESSABLES && !UNITYENGINE_OLD
            const char c_guidstart = '[';
            string text = ((IKeyEvaluator)this).RuntimeKey.ToString();
            int num = text.IndexOf(c_guidstart);
            if (num != -1)
            {
                text = text.Substring(0, num);
            }

            return Guid.TryParse(text, out _);
#else
            return true;
#endif
        }
#if UNITY_ADDRESSABLES && !UNITYENGINE_OLD
        bool IKeyEvaluator.RuntimeKeyIsValid() => IsValid();

        public AsyncOperationHandle<UnityEngine.Object> LoadAssetAsync()
        {
            return ResourceManager.LoadAssetAsync<UnityEngine.Object>(this);
        }
        public AsyncOperationHandle<TObject> LoadAssetAsync<TObject>()
            where TObject : UnityEngine.Object
        {
            return ResourceManager.LoadAssetAsync<TObject>(this);
        }
#endif
        public AssetInfo LoadAsset()
        {
            return ResourceManager.LoadAsset(in this);
        }

        public bool Equals(AssetReference other) => m_Key.Equals(other.m_Key);
        public override string ToString()
        {
            if (IsEmpty()) return "Invalid";
            else if (IsSubAsset) return $"{m_Key}[{m_SubAssetName}]";
            return m_Key.ToString();
        }

#if UNITY_ADDRESSABLES
        public static implicit operator AssetReference(AddressableReference t)
        {
            if (t.AssetGUID.IsNullOrEmpty()) return Empty;
            else if (t.SubObjectName.IsNullOrEmpty())
            {
                return new AssetReference(t.AssetGUID);
            }
            return new AssetReference(t.AssetGUID, t.SubObjectName);
        }
#endif
        public static implicit operator AssetReference(string t)
        {
            Match match = Regex.Match(t, @"(^.+)" + Regex.Escape("[") + @"(.+)]");
            if (match.Success)
            {
                return new AssetReference(match.Groups[1].Value, match.Groups[2].Value);
            }
            return new AssetReference(t);
        }
    }
}

#endif