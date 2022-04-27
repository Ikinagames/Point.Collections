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
#if UNITY_COLLECTIONS
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System;
using System.Runtime.InteropServices;

namespace Point.Collections.Actions
{
    /// <summary>
    /// Code-level 에서 미리 정의된 런타임 값을 반환하는 액션입니다.
    /// </summary>
    /// <remarks>
    /// 모든 ConstAction 은 <see cref="GuidAttribute"/> 를 가지고 있어야 합니다. 
    /// 정의된 <see cref="ConstAction{TValue}"/> 는 <seealso cref="ConstActionReference{TValue}"/> 를 통해 
    /// 레퍼런스 될 수 있습니다.
    /// </remarks>
    /// <typeparam name="TValue">
    /// 의미있는 값을 반환하지 않는다면 int 를 사용하세요
    /// </typeparam>
    [Serializable]
    public abstract class ConstAction<TValue> : IConstAction
    {
        Type IConstAction.ReturnType => TypeHelper.TypeOf<TValue>.Type;

        public ConstAction()
        {
        }

        ConstActionUtilities.Info IConstAction.GetInfo() => ConstActionUtilities.HashMap[GetType()];
        void IConstAction.SetArguments(params object[] args) => InternalSetArguments(args);

        void IConstAction.Initialize() => OnInitialize();
        object IConstAction.Execute() => InternalExecute();
        void IConstAction.OnShutdown() => OnShutdown();
        void IDisposable.Dispose() => OnDispose();

        protected virtual void InternalSetArguments(params object[] args)
        {
            ConstActionUtilities.HashMap[GetType()].SetArguments(this, args);
        }
        protected virtual TValue InternalExecute() => Execute();

        protected virtual void OnInitialize() { }
        protected abstract TValue Execute();

        protected virtual void OnShutdown() { }
        protected virtual void OnDispose() { }
    }
}
