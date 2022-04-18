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
#if UNITY_2019 || !UNITY_2020_OR_NEWER
#define UNITYENGINE_OLD
#endif
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Point.Collections.Editor
{
    public static class ScriptUtilities
    {
#if UNITYENGINE_OLD
        private static char s_Spliter = ';';
#endif
        public static bool IsDefinedSymbol(string constrains)
        {
#if UNITYENGINE_OLD
            string[] defined = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(s_Spliter);
#else
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out string[] defined);
#endif

            return defined.Contains(constrains);
        }
        public static void DefineSymbol(string constrains)
        {
#if UNITYENGINE_OLD
            string[] defined = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(s_Spliter);
#else
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out string[] defined);
#endif
            List<string> temp = defined.ToList();
            temp.Add(constrains);

#if UNITYENGINE_OLD
            Apply(temp);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, temp.ToArray());
#endif
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }
        public static void UndefSymbol(string constrains)
        {
#if UNITYENGINE_OLD
            string[] defined = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(s_Spliter);
#else
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out string[] defined);
#endif
            if (!defined.Contains(constrains)) return;

            List<string> temp = defined.ToList();
            temp.Remove(constrains);

#if UNITYENGINE_OLD
            Apply(temp);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, temp.ToArray());
#endif
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }

#if UNITYENGINE_OLD
        private static void Apply(List<string> defines)
        {
            string sum = string.Empty;
            for (int i = 0; i < defines.Count; i++)
            {
                sum += defines[i] + s_Spliter;
            }

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, sum);
        }
#endif

        public static MonoScript FindScriptFromClassName(string className)
        {
            var scriptGUIDs = AssetDatabase.FindAssets($"t:script {className}");

            if (scriptGUIDs.Length == 0)
                return null;

            foreach (var scriptGUID in scriptGUIDs)
            {
                var assetPath = AssetDatabase.GUIDToAssetPath(scriptGUID);
                var script = AssetDatabase.LoadAssetAtPath<MonoScript>(assetPath);

                if (script != null && string.Equals(className, Path.GetFileNameWithoutExtension(assetPath), StringComparison.OrdinalIgnoreCase))
                    return script;
            }

            return null;
        }
    }
}

#endif