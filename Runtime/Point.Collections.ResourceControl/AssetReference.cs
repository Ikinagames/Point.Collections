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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#if UNITY_2019 && !UNITY_2020_1_OR_NEWER
#define UNITYENGINE_OLD
#endif

using System;
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
    [Serializable]
    public struct AssetReference : IValidation, IEmpty, IEquatable<AssetReference>
#if UNITY_ADDRESSABLES
        , IKeyEvaluator
#endif
    {
        public static AssetReference Empty => new AssetReference();

        [SerializeField] private FixedString128Bytes m_Key;
        [SerializeField] private FixedString128Bytes m_SubAssetName;

#if UNITY_ADDRESSABLES
        object IKeyEvaluator.RuntimeKey
        {
            get
            {
                if (m_Key.IsEmpty) return string.Empty;

                const string c_Format = "{0}[{1}]";
                if (!m_SubAssetName.IsEmpty)
                {
                    return string.Format(c_Format, m_Key.ToString(), m_SubAssetName.ToString());
                }
                return m_Key.ToString();
            }
        }
        public AssetRuntimeKey RuntimeKey => new AssetRuntimeKey(FNV1a32.Calculate(((IKeyEvaluator)this).RuntimeKey.ToString()));
#else
        public AssetRuntimeKey RuntimeKey => new AssetRuntimeKey(FNV1a32.Calculate(m_Key.ToString().ToLowerInvariant()));
#endif
        public bool IsSubAsset => !m_SubAssetName.IsEmpty;
#if UNITY_EDITOR
        public UnityEngine.Object editorAsset
        {
            get
            {
                return UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(m_Key.ToString());
            }
        }
#endif

#if UNITY_ADDRESSABLES
        public AsyncOperationHandle<IResourceLocation> Location => ResourceManager.GetLocation(this, TypeHelper.TypeOf<UnityEngine.Object>.Type);
#endif

        public AssetReference(FixedString128Bytes key) : this(key, default) { }
        public AssetReference(FixedString128Bytes key, FixedString128Bytes subAssetName)
        {
            m_Key = key;
            m_SubAssetName = subAssetName;
        }

        public bool IsEmpty()
        {
            return m_Key.IsEmpty || (m_Key.IsEmpty && m_SubAssetName.IsEmpty);
        }
        public bool IsValid()
        {
            if (m_Key.IsEmpty) return false;

#if UNITY_ADDRESSABLES
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
#if UNITY_ADDRESSABLES
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