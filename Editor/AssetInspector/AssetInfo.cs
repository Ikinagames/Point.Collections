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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2019_1_OR_NEWER
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using Point.Collections.Threading;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Point.Collections.Editor
{
    [Serializable]
    public sealed class AssetInfo : ISerializationCallbackReceiver
    {
        [NonSerialized]
        private HashSet<string>
            // 내가 참조하는 모든 에셋의 경로들
            m_ReferenceSet = new HashSet<string>(),
            // 나를 참조하는 모든 에셋의 경로들
            m_DependencySet = new HashSet<string>();
        [NonSerialized]
        private BuildStatus m_BuildStatus = BuildStatus.Unknown;

        [SerializeField]
        private AssetPathField m_Asset = new AssetPathField(string.Empty);
        [SerializeField]
        private string[]
            m_References = Array.Empty<string>(),
            m_Dependencies = Array.Empty<string>();

        private AtomicOperator m_Op = new AtomicOperator();

        public AssetPathField Asset => m_Asset;
        public HashSet<string> References => m_ReferenceSet;
        public HashSet<string> Dependencies => m_DependencySet;

        public BuildStatus BuildStatus
        {
            get
            {
                if (m_BuildStatus == BuildStatus.Unknown)
                {
                    if (m_Asset.IsEmpty()) return BuildStatus.Unknown;

                    if (m_Asset.IsInEditorFolder())
                    {
                        m_BuildStatus = BuildStatus.NotIncludable;
                    }
                    else
                    {
                        m_BuildStatus = BuildStatus.Includable;
                    }

                    if (m_ReferenceSet.Count > 0 || m_DependencySet.Count > 0)
                    {
                        m_BuildStatus |= BuildStatus.Referenced;
                    }
                }

                return m_BuildStatus;
            }
        }

        public AssetInfo(string assetPath)
        {
            m_Asset = new AssetPathField(assetPath);
            
            string[] dependencies = m_Asset.GetDependencies();
            m_DependencySet = new HashSet<string>(dependencies.Where(t => !t.Equals(assetPath)));
        }
        internal void BuildReferenceSet(ConcurrentDictionary<string, AssetInfo> assetDatabase)
        {
            foreach (var item in m_DependencySet)
            {
                if (!assetDatabase.TryGetValue(item, out AssetInfo dep))
                {
                    continue;
                }

                dep.AddToReferenceSet(m_Asset.AssetPath);
            }
        }
        internal void RemoveReferenceSet(ConcurrentDictionary<string, AssetInfo> assetDatabase)
        {
            foreach (var item in m_DependencySet)
            {
                if (!assetDatabase.TryGetValue(item, out AssetInfo dep))
                {
                    continue;
                }

                dep.RemoveFromReferenceSet(m_Asset.AssetPath);
            }
        }

        private void AddToReferenceSet(string assetPath)
        {
            m_Op.Enter();

            m_ReferenceSet.Add(assetPath);

            m_Op.Exit();
        }
        private void RemoveFromReferenceSet(string assetPath)
        {
            m_Op.Enter();

            m_ReferenceSet.Remove(assetPath);

            m_Op.Exit();
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            m_References = m_ReferenceSet.ToArray();
            m_Dependencies = m_DependencySet.ToArray();
        }
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            m_ReferenceSet = new HashSet<string>(m_References);
            m_DependencySet = new HashSet<string>(m_Dependencies);
        }
    }
}

#endif