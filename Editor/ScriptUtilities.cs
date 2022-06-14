﻿// Copyright 2022 Ikina Games
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
        #region Symbols

#if UNITYENGINE_OLD
        private static char s_Spliter = ';';
#endif
        private static List<string> m_DefinedSymbols;
        private static List<string> DefinedSymbols
        {
            get
            {
                if (m_DefinedSymbols == null)
                {
#if UNITYENGINE_OLD
                    string[] defined = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup).Split(s_Spliter);
#else
                    PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out string[] defined);
#endif
                    m_DefinedSymbols = defined.ToList();
                }
                return m_DefinedSymbols;
            }
        }

        public static bool IsDefinedSymbol(string constrains)
        {
            return DefinedSymbols.Contains(constrains);
        }
        public static void DefineSymbol(string constrains)
        {
            DefinedSymbols.Add(constrains);

#if UNITYENGINE_OLD
            Apply(DefinedSymbols);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, DefinedSymbols.ToArray());
#endif
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }
        public static void UndefSymbol(string constrains)
        {
            if (!DefinedSymbols.Contains(constrains)) return;

            DefinedSymbols.Remove(constrains);

#if UNITYENGINE_OLD
            Apply(DefinedSymbols);
#else
            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, DefinedSymbols.ToArray());
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

        #endregion

        public static MonoScript FindScriptFromClassName(string className)
        {
            const string c_FindFormat = "t:script {0}";

            var scriptGUIDs = AssetDatabase.FindAssets(
                string.Format(c_FindFormat, className));

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
        public static MonoScript FindEditorScriptFromClassName(string className)
        {
            const string c_EditorNameFormat = "{0}Editor";

            string name = string.Format(c_EditorNameFormat, className);
            return FindScriptFromClassName(name);
        }
        public static MonoScript FindEditorScriptFromClassName<T>()
        {
            const string c_EditorNameFormat = "{0}Editor";

            string name = string.Format(c_EditorNameFormat, TypeHelper.TypeOf<T>.Name);
            return FindScriptFromClassName(name);
        }
        public static MonoScript FindPropertyDrawerScriptFromClassName<T>()
        {
            const string 
                c_PropertyNameFormat = "{0}PropertyDrawer";

            string name = string.Format(c_PropertyNameFormat, TypeHelper.TypeOf<T>.Name);
            return FindScriptFromClassName(name);
        }
    }
}

#endif