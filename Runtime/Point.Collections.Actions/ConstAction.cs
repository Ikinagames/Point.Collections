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
    /// <typeparam name="TResult">
    /// </typeparam>
    [Serializable]
    public abstract class ConstAction<TResult> : ConstActionBase<TResult>
    {
        protected internal override sealed object InternalExecute(params object[] args)
        {
            return Execute();
        }
        protected abstract TResult Execute();
    }
    [Serializable]
    public abstract class ConstAction : ConstAction<Void>
    {
    }
    [Serializable]
    public abstract class ConstAction<TResult, T> : ConstActionBase<TResult>
    {
        protected internal override object InternalExecute(params object[] args)
        {
            if (args == null || args.Length == 0)
            {
                return Execute(default(T));
            }

            T arg0 = (T)args[0];

            return Execute(arg0);
        }
        protected abstract TResult Execute(T arg0);
    }

    public abstract class ConstActionBase : IConstAction
    {
        protected abstract Type ReturnType { get; }
        Type IConstAction.ReturnType => ReturnType;

        ConstActionUtilities.Info IConstAction.GetInfo() => ConstActionUtilities.HashMap[GetType()];
        void IConstAction.SetArguments(params object[] args) => InternalSetArguments(args);

        void IConstAction.Initialize() => OnInitialize();
        object IConstAction.Execute(params object[] args) => InternalExecute();
        void IConstAction.OnShutdown() => OnShutdown();
        void IDisposable.Dispose() => OnDispose();

        internal protected virtual void InternalSetArguments(params object[] args)
        {
            ConstActionUtilities.HashMap[GetType()].SetArguments(this, args);
        }
        internal protected abstract object InternalExecute(params object[] args);

        protected virtual void OnInitialize() { }
        protected virtual void OnShutdown() { }
        protected virtual void OnDispose() { }
    }
    public abstract class ConstActionBase<TResult> : ConstActionBase
    {
        protected override sealed Type ReturnType => TypeHelper.TypeOf<TResult>.Type;
    }

    public struct Void
    {
        public static implicit operator Void(int _) => new Void();
        public static implicit operator Void(bool _) => new Void();
        public static implicit operator Void(float _) => new Void();
        public static implicit operator Void(double _) => new Void();
        public static implicit operator Void(long _) => new Void();
        public static implicit operator Void(string _) => new Void();
    }
}
