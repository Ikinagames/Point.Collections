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

#if UNITY_2019_1_OR_NEWER && UNITY_ADDRESSABLES
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#if UNITY_2019 && !UNITY_2020_1_OR_NEWER
#define UNITYENGINE_OLD
#else
#if UNITY_MATHEMATICS
#endif
#endif

using System;
using UnityEngine;
using AddressableReference = UnityEngine.AddressableAssets.AssetReference;

namespace Point.Collections.ResourceControl
{
    [Serializable]
    public sealed class AddressableAsset
    {
        [SerializeField] private string m_FriendlyName;
        [SerializeField] private AddressableReference m_AssetReference;

        public string FriendlyName { get => m_FriendlyName; set => m_FriendlyName = value; }
        public AddressableReference AssetReference => m_AssetReference;
#if UNITY_EDITOR
        public UnityEngine.Object EditorAsset => m_AssetReference.editorAsset;
#endif

        public AddressableAsset() : base() { }
#if UNITY_EDITOR
        public AddressableAsset(string name, UnityEditor.GUID guid) : this(name, guid.ToString()) { }
#endif
        public AddressableAsset(string name, string guid)
        {
            m_FriendlyName = name;
            m_AssetReference = new AddressableReference(guid);
        }
    }
}

#endif