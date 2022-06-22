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
#if !UNITY_2020_1_OR_NEWER
#define UNITYENGINE_OLD
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE && !UNITYENGINE_OLD

//using Mono.Cecil;
//using Point.Collections.LowLevel.IL;
//using System;

namespace Point.Collections.Editor
{
    //public abstract class IL2CppPostProcessor : IL2CppPostProcessorBase
    //{
    //    internal override bool InternalOnProcess(ModuleDefinition module, TypeDefinition type, MethodDefinition method, CustomAttribute attribute)
    //    {
    //        Attribute att = GetCustomAttribute(method, TargetAttributeType);

    //        return OnProcess(module, type, method, attribute, att);
    //    }
    //    internal override bool InternalOnProcess(ModuleDefinition module, TypeDefinition type, CustomAttribute attribute)
    //    {
    //        Attribute att = GetCustomAttribute(type, TargetAttributeType);

    //        return OnProcess(module, type, attribute, att);
    //    }

    //    public virtual bool OnProcess(ModuleDefinition module, TypeDefinition type, MethodDefinition method, CustomAttribute attribute, Attribute attributeInstance) { return false; }
    //    public virtual bool OnProcess(ModuleDefinition module, TypeDefinition type, CustomAttribute attribute, Attribute attributeInstance) { return false; }
    //}
    //public abstract class IL2CppPostProcessor<T> : IL2CppPostProcessorBase
    //    where T : ILProcessorAttribute
    //{
    //    public override sealed Type TargetAttributeType => TypeHelper.TypeOf<T>.Type;

    //    internal override bool InternalOnProcess(ModuleDefinition module, TypeDefinition type, MethodDefinition method, CustomAttribute attribute)
    //    {
    //        T att = (T)GetCustomAttribute(method, TargetAttributeType);

    //        return OnProcess(module, type, method, attribute, att);
    //    }
    //    internal override bool InternalOnProcess(ModuleDefinition module, TypeDefinition type, CustomAttribute attribute)
    //    {
    //        T att = (T)GetCustomAttribute(type, TargetAttributeType);

    //        return OnProcess(module, type, attribute, att);
    //    }

    //    public virtual bool OnProcess(ModuleDefinition module, TypeDefinition type, CustomAttribute attribute, T attributeInstance) { return false; }
    //    public virtual bool OnProcess(ModuleDefinition module, TypeDefinition type, MethodDefinition method, CustomAttribute attribute, T attributeInstance) { return false; }
    //}
}

#endif