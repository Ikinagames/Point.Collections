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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2020
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using Point.Collections.ResourceControl.LowLevel;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Point.Collections.ResourceControl
{
    [PreferBinarySerialization]
    public sealed class ResourceAddresses : StaticScriptableObject<ResourceAddresses>
    {
        [SerializeField] private AssetPathField[] m_StreamingAssetBundles = Array.Empty<AssetPathField>();

        public IReadOnlyList<AssetPathField> StreamingAssetBundles => m_StreamingAssetBundles;
    }
}

#endif