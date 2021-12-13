// Copyright 2021 Ikina Games
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

using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Point.Collections.Editor
{
    public static class ScriptUtilities
    {
        public static bool IsDefinedSymbol(string constrains)
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out string[] defined);

            return defined.Contains(constrains);
        }
        public static void DefineSymbol(string constrains)
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out string[] defined);
            List<string> temp = defined.ToList();
            temp.Add(constrains);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, temp.ToArray());
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }
        public static void UndefSymbol(string constrains)
        {
            PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, out string[] defined);
            if (!defined.Contains(constrains)) return;

            List<string> temp = defined.ToList();
            temp.Remove(constrains);

            PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, temp.ToArray());
            UnityEditor.Compilation.CompilationPipeline.RequestScriptCompilation();
        }
    }
}
