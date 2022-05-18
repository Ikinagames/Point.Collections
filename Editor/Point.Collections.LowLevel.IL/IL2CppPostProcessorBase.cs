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

using Mono.Cecil;
using System;
using System.Reflection;

namespace Point.Collections.Editor
{
    public abstract class IL2CppPostProcessorBase
    {
        public abstract Type TargetAttributeType { get; }

        public virtual void OnInitialize() { }
        internal abstract bool InternalOnProcess(ModuleDefinition module, TypeDefinition type, MethodDefinition method, CustomAttribute attribute);

        internal protected Attribute GetCustomAttribute(TypeDefinition type, Type attributeType)
        {
            return type.GetActualType().GetCustomAttribute(attributeType);
        }
        internal protected Attribute GetCustomAttribute(MethodDefinition type, Type attributeType)
        {
            MethodInfo method = type.GetMethodInfo();
            Attribute attribute = null;
            foreach (var item in method.GetCustomAttributes(true))
            {
                if (TypeHelper.InheritsFrom(item.GetType(), attributeType))
                {
                    attribute = item as Attribute;
                    break;
                }
            }

            //Attribute attribute = method.GetCustomAttribute(attributeType, true);
            //$"{method.Name}.{attributeType.Name} :: {attribute != null}".ToLog();

            return attribute;
        }
    }
}

#endif