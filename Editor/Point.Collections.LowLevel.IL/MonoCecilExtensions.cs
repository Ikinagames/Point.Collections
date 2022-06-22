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
//using System;
//using System.Reflection;

namespace Point.Collections.Editor
{
    //public static class MonoCecilExtensions
    //{
    //    public static string GetAssemblyQualifiedName(this TypeReference type)
    //    {
    //        string name = Assembly.CreateQualifiedName(type.Module.Assembly.FullName, type.FullName);

    //        return name;
    //    }
    //    public static Type GetActualType(this TypeReference type)
    //    {
    //        return Type.GetType(type.GetAssemblyQualifiedName());
    //    }
    //    public static MethodInfo GetMethodInfo(this MethodDefinition t)
    //    {
    //        Type declaringType = t.DeclaringType.GetActualType();

    //        BindingFlags bindingFlags = 0;
    //        if (t.IsPublic) bindingFlags |= BindingFlags.Public;
    //        else bindingFlags |= BindingFlags.NonPublic;

    //        if (t.IsStatic) bindingFlags |= BindingFlags.Static;
    //        else bindingFlags |= BindingFlags.Instance;

    //        MethodInfo method = declaringType.GetMethod(t.Name, bindingFlags | BindingFlags.FlattenHierarchy);

    //        //$"{t.Name}.{t.DeclaringType.Name} : {method != null}".ToLog();

    //        return method;
    //    }
    //}
}

#endif