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

#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif

using Point.Collections;
using Point.Collections.Buffer.LowLevel;
using Point.Collections.SceneManagement.LowLevel;
using Point.Collections.Threading;
using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.Jobs;
using UnityEngine.SceneManagement;

namespace Point.Collections.SceneManagement
{
    /// <summary>
    /// TRS(Translation, Rotation, Scale) 값으로만으로 이루어진 데이터들을 씬으로서 묶은 구조체입니다.
    /// </summary>
    /// <remarks>
    /// Unity.Jobs 에 대한 모든 Safety 를 지원합니다.
    /// </remarks>
    /// <typeparam name="THandler"></typeparam>
    [NativeContainer]
    public struct TransformScene<THandler> : ITransformScene, IUnsafeTransformScene, IDisposable, IValidation
        where THandler : unmanaged, ITransformSceneHandler
    {
        private readonly ThreadInfo m_Owner;
        private readonly int m_SceneBuildIndex;
        private UnsafeLinearHashMap<SceneID, UnsafeTransform> m_HashMap;
        private UnsafeAllocator<THandler> m_Handler;

        #region IUnsafeTransformScene Implements

        UnsafeLinearHashMap<SceneID, UnsafeTransform> IUnsafeTransformScene.HashMap => m_HashMap;
        TypeInfo IUnsafeTransformScene.HandlerType => TypeHelper.TypeOf<THandler>.TypeInfo;
        UnsafeAllocator IUnsafeTransformScene.Handler => m_Handler;

        #endregion

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        private AtomicSafetyHandle m_SafetyHandle;
        [NativeSetClassTypeToNullOnSchedule]
        private DisposeSentinel m_DisposeSentinel;
#endif
        public Scene Scene => SceneManager.GetSceneByBuildIndex(m_SceneBuildIndex);
        public bool IsCreated => m_HashMap.IsCreated;

        /// <summary>
        /// 해당 씬(<paramref name="scene"/>)에 새로운 TRS 씬을 생성합니다.
        /// </summary>
        /// <param name="scene">목표 씬입니다.</param>
        /// <param name="initialCount">이 TRS 씬의 초기 크기입니다.</param>
        public TransformScene(Scene scene, int initialCount = 128)
        {
            m_Owner = ThreadInfo.CurrentThread;
            m_SceneBuildIndex = scene.buildIndex;
            m_HashMap = new UnsafeLinearHashMap<SceneID, UnsafeTransform>(initialCount, Allocator.Persistent);
            m_Handler = new UnsafeAllocator<THandler>(1, Allocator.Persistent, NativeArrayOptions.ClearMemory);

            m_Handler.Ptr.Value.OnInitialize();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            DisposeSentinel.Create(out m_SafetyHandle, out m_DisposeSentinel, 1, Allocator.Persistent);
#endif
        }

        /// <summary>
        /// 새로운 <see cref="NativeTransform"/> 을 추가합니다.
        /// </summary>
        /// <param name="tr"></param>
        /// <returns></returns>
        [WriteAccessRequired]
        public NativeTransform Add(Transformation tr)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            PointCore.AssertThreadAffinity(in m_Owner);

            AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
            AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
            SceneID id = new SceneID(Hash.NewHash());

            m_HashMap.AddOrUpdate(id, new UnsafeTransform(tr));

            var temp = new NativeTransform(m_HashMap, id
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                , m_SafetyHandle
#endif
                );

            m_Handler.Ptr.Value.OnTransformAdded(in temp);

            return temp;
        }
        /// <summary>
        /// <paramref name="transform"/> 을 이 씬에서 제거합니다.
        /// </summary>
        /// <param name="transform"></param>
        [WriteAccessRequired]
        public void Remove(NativeTransform transform)
        {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            PointCore.AssertThreadAffinity(in m_Owner);

            AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
            AtomicSafetyHandle.CheckWriteAndThrow(m_SafetyHandle);
#endif
            m_Handler.Ptr.Value.OnTransformRemove(in transform);

            m_HashMap.Remove(transform.ID);
        }

        [WriteAccessRequired]
        public void Dispose()
        {
            foreach (KeyValue<SceneID, UnsafeTransform> item in m_HashMap)
            {
                var temp = new NativeTransform(m_HashMap, item.Key
#if ENABLE_UNITY_COLLECTIONS_CHECKS
                , m_SafetyHandle
#endif
                );
                m_Handler.Ptr.Value.OnTransformRemove(in temp);
            }
            m_Handler.Ptr.Value.Dispose();

#if ENABLE_UNITY_COLLECTIONS_CHECKS
            PointCore.AssertThreadAffinity(in m_Owner);

            AtomicSafetyHandle.CheckExistsAndThrow(m_SafetyHandle);
            DisposeSentinel.Dispose(ref m_SafetyHandle, ref m_DisposeSentinel);
#endif
            m_HashMap.Dispose();
            m_Handler.Dispose();
        }

        public bool IsValid() => m_HashMap.IsCreated && Scene.IsValid();
    }
    public interface ITransformScene : IDisposable, IValidation
    {
        /// <summary>
        /// 목표 씬을 반환합니다.
        /// </summary>
        Scene Scene { get; }
        /// <summary>
        /// 이 구조체가 생성되었는지 반환합니다.
        /// </summary>
        bool IsCreated { get; }

        NativeTransform Add(Transformation tr);
        void Remove(NativeTransform transform);
    }
    internal interface IUnsafeTransformScene
    {
        UnsafeLinearHashMap<SceneID, UnsafeTransform> HashMap { get; }
        TypeInfo HandlerType { get; }
        UnsafeAllocator Handler { get; }
    }

    public interface ITransformSceneHandler : IDisposable
    {
        void OnInitialize();

        void OnTransformAdded(in NativeTransform transform);
        void OnTransformRemove(in NativeTransform transform);
    }
}
