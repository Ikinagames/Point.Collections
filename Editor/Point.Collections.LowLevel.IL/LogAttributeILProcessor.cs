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
//using Mono.Cecil.Cil;
//using System;
//using System.Linq;
//using System.Runtime.InteropServices;
//using UnityEngine;

namespace Point.Collections.Editor
{
    //internal sealed class LogAttributeILProcessor : IL2CppPostProcessor<LogAttribute>
    //{
    //    public override bool OnProcess(ModuleDefinition module, TypeDefinition type, MethodDefinition method, CustomAttribute attribute, LogAttribute attributeInstance)
    //    {
    //        MethodReference logMethodReference = module.ImportReference(typeof(Debug).GetMethod("Log", new Type[] { typeof(object) }));

    //        ILProcessor ilProcessor = method.Body.GetILProcessor();
    //        Instruction first = method.Body.Instructions.First();
    //        Instruction last = method.Body.Instructions.Last();

    //        //Insertion function
    //        ilProcessor.InsertBefore(first, ilProcessor.Create(OpCodes.Ldstr, attributeInstance.Text));
    //        //string log = string.Copy(attributeInstance.Text);
    //        //ilProcessor.InsertBefore(first, ilProcessor.Create(OpCodes.Ldstr, log));
    //        ilProcessor.InsertBefore(first, ilProcessor.Create(OpCodes.Call, logMethodReference));

    //        return true;
    //    }
    //}
}

#endif