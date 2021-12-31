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

#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif

using Point.Collections;
using Point.Collections.Buffer.LowLevel;
using Point.Collections.LowLevel;
using Point.Collections.Threading;
using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Jobs;
using UnityEngine.SceneManagement;

namespace Point.Collections
{
    [NativeContainer]
    public struct TransformScene : IDisposable
    {
        private readonly ThreadInfo m_Owner;
        private readonly int m_SceneBuildIndex;
        private UnsafeLinearHashMap<SceneID, UnsafeTransform> m_HashMap;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle m_SafetyHandle;
        [NativeSetClassTypeToNullOnSchedule]
        private DisposeSentinel m_DisposeSentinel;
#endif

        public Scene Scene => SceneManager.GetSceneByBuildIndex(m_SceneBuildIndex);

        public TransformScene(Scene scene, int initialCount = 128)
        {
            m_Owner = ThreadInfo.CurrentThread;
            m_SceneBuildIndex = scene.buildIndex;
            m_HashMap = new UnsafeLinearHashMap<SceneID, UnsafeTransform>(initialCount, Allocator.Persistent);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Create(out m_SafetyHandle, out m_DisposeSentinel, 1, Allocator.Persistent);
#endif
        }

        [WriteAccessRequired]
        public NativeTransform Add(Transformation tr)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            PointCore.AssertThreadAffinity(in m_Owner);

            AtomicSafetyHandle.CheckExistsAndThrow(in m_SafetyHandle);
            AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
            SceneID id = new SceneID(Hash.NewHash());

            m_HashMap.Add(id, new UnsafeTransform(tr));

            return new NativeTransform(m_HashMap, id
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                , m_SafetyHandle
#endif
                );
        }
        [WriteAccessRequired]
        public void Remove(NativeTransform transform)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            PointCore.AssertThreadAffinity(in m_Owner);

            AtomicSafetyHandle.CheckExistsAndThrow(in m_SafetyHandle);
            AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
            m_HashMap.Remove(transform.ID);
        }

        public void Dispose()
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            PointCore.AssertThreadAffinity(in m_Owner);

            AtomicSafetyHandle.CheckExistsAndThrow(in m_SafetyHandle);
            DisposeSentinel.Dispose(ref m_SafetyHandle, ref m_DisposeSentinel);
#endif
            m_HashMap.Dispose();
        }
    }
}
