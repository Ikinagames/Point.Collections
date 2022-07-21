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
#if UNITY_COLLECTIONS
using Unity.Collections;
#endif

namespace Point.Collections.ResourceControl
{
    /// <summary>
    /// <see cref="ResourceManager"/> 에서 사용되는 런타임 리소스 키 입니다.
    /// </summary>
    /// <remarks>
    /// 런타임 키는 메모리와 연산 효율을 위해 <seealso cref="AssetReference"/> 에서 <seealso cref="uint">
    /// 해시 값으로 연산된 값입니다. 에디터에서는 디버그 용도로 원래의 키 값을 확인 할 수 있지만, 배포에서는 제공되지 않습니다.
    /// 
    /// 리소스 접근 권한을 가진 구조체 중 메모리 효율이 가장 높습니다. <seealso cref="AssetReference"/>, <seealso cref="AssetIndex"/> 를 참조하세요.
    /// </remarks>
#if UNITY_COLLECTIONS
    [BurstCompatible]
#endif
    public readonly struct AssetRuntimeKey : IEmpty, IEquatable<AssetRuntimeKey>
    {
        const string c_Format = "{0}[{1}]";

        public static AssetRuntimeKey Empty => new AssetRuntimeKey(0);

        private readonly Hash m_Key;
        public Hash Key => m_Key;

#if UNITY_COLLECTIONS
        [NotBurstCompatible]
#endif
        public AssetRuntimeKey(string path)
        {
            if (path.IsNullOrEmpty())
            {
                m_Key = Hash.Empty;
                return;
            }

            m_Key = new Hash(path);
        }
        public AssetRuntimeKey(string path, string subAssetName)
        {
            if (path.IsNullOrEmpty())
            {
                m_Key = Hash.Empty;
                return;
            }

            m_Key = new Hash(string.Format(c_Format, path.ToLowerInvariant(), subAssetName));
        }
        public AssetRuntimeKey(AssetPathField path)
        {
            if (!path.IsSubAsset)
            {
                m_Key = new Hash(path.AssetPath.ToLowerInvariant());
            }

            m_Key = new Hash(string.Format(c_Format, 
                path.AssetPath.ToLowerInvariant(), 
                path.SubAssetName));
        }
#if UNITY_COLLECTIONS
        public AssetRuntimeKey(FixedString512Bytes path)
        {
            m_Key = new Hash(path.ToString().ToLowerInvariant());
        }
#endif
        public AssetRuntimeKey(Hash hash)
        {
            m_Key = hash;
        }
        public AssetRuntimeKey(uint key)
        {
            m_Key = new Hash(key);
        }

        public bool IsEmpty() => m_Key == 0;

        public bool Equals(AssetRuntimeKey other) => m_Key.Equals(other.m_Key);
#if UNITY_COLLECTIONS
        [NotBurstCompatible]
#endif
        public override bool Equals(object obj)
        {
            if (!(obj is AssetRuntimeKey other)) return false;
            else if (!m_Key.Equals(other.m_Key)) return false;

            return true;
        }
        public override int GetHashCode() => unchecked((int)m_Key.Value);
#if UNITY_COLLECTIONS
        [NotBurstCompatible]
#endif
        public override string ToString() => m_Key.ToString(true);
    }
}

#endif