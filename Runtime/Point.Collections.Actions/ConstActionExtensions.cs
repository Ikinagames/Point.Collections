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
using System.Collections.Generic;

namespace Point.Collections.Actions
{
    public static class ConstActionExtensions
    {
        private static readonly Dictionary<Guid, IConstAction> s_ConstActions = new Dictionary<Guid, IConstAction>();
        static ConstActionExtensions()
        {
            foreach (var item in ConstActionUtilities.Types)
            {
                var ctor = TypeHelper.GetConstructorInfo(item);
                IConstAction constAction;
                if (ctor != null)
                {
                    constAction = (IConstAction)ctor.Invoke(null);
                }
                else
                {
                    constAction = (IConstAction)Activator.CreateInstance(item);
                }

                constAction.Initialize();
                s_ConstActions.Add(item.GUID, constAction);
            }
        }
        private static IConstAction GetConstAction(Type type)
        {
#if DEBUG_MODE
            if (type == null)
            {
                "??".ToLogError();
                return null;
            }
            else if (!s_ConstActions.ContainsKey(type.GUID))
            {
                "?? not found".ToLogError();
                return null;
            }
#endif
            return s_ConstActions[type.GUID];
        }

        public static object Execute(this IConstActionReference action)
        {
            if (!ConstActionUtilities.TryGetWithGuid(action.Guid, out var info))
            {
                "?".ToLogError();
                return null;
            }

            IConstAction constAction = GetConstAction(info.Type);
            constAction.SetArguments(action.Arguments);
            //info.SetArguments(constAction, action.Arguments);

            object result;
            try
            {
                result = constAction.Execute();
            }
            catch (Exception ex)
            {
                PointHelper.LogError(Channel.Core,
                    $"Unexpected error has been raised while executing ConstAction");

                UnityEngine.Debug.LogError(ex);

                return null;
            }
            return result;
        }
        public static void Execute(this IList<ConstActionReference> action)
        {
            for (int i = 0; i < action.Count; i++)
            {
                action[i].Execute();
            }
        }
        public static TValue Execute<TValue>(this ConstActionReference<TValue> action)
        {
            return (TValue)Execute((IConstActionReference)action);
        }

        ///// <summary>
        ///// 전부 참을 반환해야 <see langword="true"/> 를 반환합니다.
        ///// </summary>
        ///// <param name="action"></param>
        ///// <returns></returns>
        //public static bool True(this ConstActionReferenceArray<bool> action)
        //{
        //    for (int i = 0; i < action.Count; i++)
        //    {
        //        if (!action[i].Execute()) return false;
        //    }
        //    return true;
        //}
        ///// <summary>
        ///// 전부 거짓을 반환해야 <see langword="true"/> 를 반환합니다.
        ///// </summary>
        ///// <param name="action"></param>
        ///// <returns></returns>
        //public static bool False(this ConstActionReferenceArray<bool> action)
        //{
        //    for (int i = 0; i < action.Count; i++)
        //    {
        //        if (action[i].Execute()) return false;
        //    }
        //    return true;
        //}
    }
}
