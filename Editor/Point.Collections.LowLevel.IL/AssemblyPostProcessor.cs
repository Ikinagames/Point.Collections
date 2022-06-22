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
//using NUnit.Framework;
//using Point.Collections.LowLevel.IL;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using UnityEditor;
//using UnityEditor.Build;
//using UnityEditor.Build.Player;
//using UnityEditor.Build.Reporting;
//using UnityEditor.Callbacks;
//using UnityEditor.Compilation;
//using UnityEditor.Il2Cpp;
//using UnityEditorInternal;
//using UnityEngine;

namespace Point.Collections.Editor
{
    // https://www.codersblock.org/blog//2014/06/integrating-monocecil-with-unity.html
    // https://stackoverflow.com/questions/49475927/mono-cecil-and-unity-not-playing-nice-together

    //[InitializeOnLoad]
    //public static class AssemblyPostProcessor /*: IPostBuildPlayerScriptDLLs*/
    //{
    //    #region Initialize

    //    private static Dictionary<string, List<IL2CppPostProcessorBase>> postProcessors = new Dictionary<string, List<IL2CppPostProcessorBase>>();

    //    static AssemblyPostProcessor()
    //    {
    //        var processorIter = TypeHelper
    //            .GetTypesIter(t => !t.IsAbstract && !t.IsInterface)
    //            .Where(TypeHelper.InheritsFrom<IL2CppPostProcessorBase>);
    //        foreach (var item in processorIter)
    //        {
    //            IL2CppPostProcessorBase ins = (IL2CppPostProcessorBase)Activator.CreateInstance(item);

    //            if (!postProcessors.TryGetValue(ins.TargetAttributeType.Name, out var list))
    //            {
    //                list = new List<IL2CppPostProcessorBase>();
    //                postProcessors.Add(ins.TargetAttributeType.Name, list);
    //            }

    //            list.Add(ins);
    //            ins.OnInitialize();
    //        }

    //        //CompilationPipeline.assemblyCompilationFinished += CompilationPipeline_assemblyCompilationFinished;

    //        ProcessAllAsemblies();
    //    }

    //    private static void CompilationPipeline_assemblyCompilationFinished(string arg1, CompilerMessage[] arg2)
    //    {
    //        if (hasGen == true) return;

    //        ProcessAllAsemblies();
    //        hasGen = true;
    //    }

    //    private static bool hasGen = false;

    //    //[PostProcessBuild(1000)]
    //    private static void OnPostprocessBuildPlayer(BuildTarget buildTarget, string buildPath)
    //    {
    //        hasGen = false;

    //        string pathDirectory = buildPath.Replace(Path.GetFileName(buildPath), string.Empty);
    //        $"{pathDirectory}".ToLog();

    //        ScriptingImplementation scriptTarget = PlayerSettings.GetScriptingBackend(EditorUserBuildSettings.selectedBuildTargetGroup);
    //        if (scriptTarget == ScriptingImplementation.Mono2x)
    //        {
    //            const string c_MonoPath = "Ramsey_Data/Managed";
    //            pathDirectory = Path.Combine(pathDirectory, c_MonoPath);

    //            EditorApplication.LockReloadAssemblies();

    //            foreach (string dllPath in Directory.GetFiles(pathDirectory))
    //            {
    //                $"process {dllPath}, :: {Path.GetExtension(dllPath)}".ToLog();
    //                if (Path.GetExtension(dllPath) != ".dll")
    //                {
    //                    continue;
    //                }

    //                try
    //                {
    //                    using (var st = new FileStream(dllPath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
    //                    //using (var st = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
    //                    {
    //                        AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(st);
    //                        if (AssemblyPostProcessor.PostProcessAssembly(assemblyDefinition))
    //                        {
    //                            st.Position = 0;
    //                            assemblyDefinition.Write(st);
    //                        }
    //                    }
    //                }
    //                catch (Exception e)
    //                {
    //                    Debug.LogWarning(e);
    //                }
    //            }

    //            $"{pathDirectory}".ToLog();
    //            EditorApplication.UnlockReloadAssemblies();
    //        }
    //        else
    //        {
    //            "not handled".ToLogError();
    //        }
    //    }

    //    //[PostProcessScene]
    //    public static void TestInjectMothodOnPost()
    //    {
    //        if (hasGen == true) return;
    //        hasGen = true;

    //        ProcessAllAsemblies();
    //    }

    //    //[MenuItem("Point/Process Assemblies")]
    //    public static void ProcessAllAsemblies()
    //    {
    //        // Lock assemblies while they may be altered
    //        EditorApplication.LockReloadAssemblies();
    //        foreach (System.Reflection.Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
    //        {
    //            try
    //            {
    //                // Only process assemblies which are in the project
    //                if (assembly.Location.Replace('\\', '/').StartsWith(Application.dataPath.Substring(0, Application.dataPath.Length - 7)))
    //                {
    //                    //string tempFilePath = Application.dataPath + "/../Temp/" + Path.GetFileName(assembly.Location);

    //                    using (var st = new FileStream(assembly.Location, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
    //                    //using (var st = new FileStream(tempFilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite))
    //                    {
    //                        AssemblyDefinition assemblyDefinition = AssemblyDefinition.ReadAssembly(st);
    //                        if (AssemblyPostProcessor.PostProcessAssembly(assemblyDefinition))
    //                        {
    //                            st.Position = 0;
    //                            assemblyDefinition.Write(st);
    //                        }
    //                    }
    //                }
    //            }
    //            catch (Exception e)
    //            {
    //                Debug.LogWarning(e);
    //            }
    //        }
    //        // Unlock now that we're done
    //        EditorApplication.UnlockReloadAssemblies();
    //    }

    //    #endregion

    //    #region Utils

    //    public static TypeReference ToTypeReference(Type t)
    //    {
    //        AssemblyDefinition a = AssemblyDefinition.ReadAssembly(t.Assembly.Location);
    //        TypeReference temp = a.MainModule.ImportReference(t);

    //        return temp;
    //    }

    //    /// <summary>
    //    /// Pre-statement insertion Instruction, And return the current statement
    //    /// </summary>
    //    private static Instruction InsertBefore(ILProcessor ilProcessor, Instruction target, Instruction instruction)
    //    {
    //        ilProcessor.InsertBefore(target, instruction);
    //        return instruction;
    //    }

    //    /// <summary>
    //    /// Insert after statement Instruction, And return the current statement
    //    /// </summary>
    //    private static Instruction InsertAfter(ILProcessor ilProcessor, Instruction target, Instruction instruction)
    //    {
    //        ilProcessor.InsertAfter(target, instruction);
    //        return instruction;
    //    }
    //    //Calculating the offset of the injected function
    //    private static void ComputeOffsets(MethodBody body)
    //    {
    //        var offset = 0;
    //        foreach (var instruction in body.Instructions)
    //        {
    //            instruction.Offset = offset;
    //            offset += instruction.GetSize();
    //        }
    //    }

    //    #endregion

    //    private static bool PostProcessAssembly(AssemblyDefinition assemblyDefinition)
    //    {
    //        bool hasChanged = false;
    //        List<CustomAttribute> processedAtts = new List<CustomAttribute>();
    //        foreach (ModuleDefinition moduleDefinition in assemblyDefinition.Modules)
    //        {
    //            foreach (TypeDefinition typeDefinition in moduleDefinition.Types)
    //            {
    //                processedAtts.Clear();
    //                foreach (CustomAttribute customAttribute in typeDefinition.CustomAttributes)
    //                {
    //                    if (!postProcessors.TryGetValue(customAttribute.AttributeType.Name, out var processors))
    //                    {
    //                        continue;
    //                    }

    //                    for (int i = 0; i < processors.Count; i++)
    //                    {
    //                        processors[i].m_IsMethod = true;

    //                        hasChanged |= processors[i].InternalOnProcess(moduleDefinition, typeDefinition, customAttribute);
    //                    }

    //                    processedAtts.Add(customAttribute);
    //                }
    //                // Remove the attribute so it won't be processed again
    //                for (int i = 0; i < processedAtts.Count; i++)
    //                {
    //                    typeDefinition.CustomAttributes.Remove(processedAtts[i]);
    //                }

    //                // method

    //                foreach (MethodDefinition methodDefinition in typeDefinition.Methods)
    //                {
    //                    processedAtts.Clear();

    //                    foreach (CustomAttribute customAttribute in methodDefinition.CustomAttributes)
    //                    {
    //                        if (!postProcessors.TryGetValue(customAttribute.AttributeType.Name, out var processors))
    //                        {
    //                            continue;
    //                        }

    //                        for (int i = 0; i < processors.Count; i++)
    //                        {
    //                            processors[i].m_IsMethod = true;

    //                            hasChanged |= processors[i].InternalOnProcess(moduleDefinition, typeDefinition, methodDefinition, customAttribute);

    //                            ComputeOffsets(methodDefinition.Body);
    //                        }

    //                        processedAtts.Add(customAttribute);
    //                    }

    //                    // Remove the attribute so it won't be processed again
    //                    for (int i = 0; i < processedAtts.Count; i++)
    //                    {
    //                        methodDefinition.CustomAttributes.Remove(processedAtts[i]);
    //                    }
    //                }
    //            }
    //        }

    //        return hasChanged;
    //    }


    //    public static bool Process(ModuleDefinition module, TypeDefinition type, MethodDefinition method, CustomAttribute attribute)
    //    {
    //        MethodReference logMethodReference = module.ImportReference(typeof(Debug).GetMethod("Log", new Type[] { typeof(object) }));

    //        ILProcessor ilProcessor = method.Body.GetILProcessor();

    //        Instruction first = method.Body.Instructions.First();
    //        //ilProcessor.InsertBefore(first, Instruction.Create(OpCodes.Ldstr,
    //        //"Enter " + type.FullName + "." + method.Name));
    //        //ilProcessor.InsertBefore(first, Instruction.Create(OpCodes.Call, logMethodReference));

    //        Instruction last = method.Body.Instructions.Last();
    //        //ilProcessor.InsertBefore(last, Instruction.Create(OpCodes.Ldstr,
    //        //"Exit " + type.FullName + "." + method.Name));
    //        //ilProcessor.InsertBefore(last, Instruction.Create(OpCodes.Call, logMethodReference));

    //        //Insertion function
    //        var current = InsertBefore(ilProcessor, first, ilProcessor.Create(OpCodes.Ldstr, "Inject"));
    //        current = InsertBefore(ilProcessor, first, ilProcessor.Create(OpCodes.Call, logMethodReference));

    //        ComputeOffsets(method.Body);
    //        return true;
    //    }
    //}
}

#endif