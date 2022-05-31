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

using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEditor.AssetImporters;
using UnityEngine;

namespace Point.Collections.Editor
{
    [ScriptedImporter(1, "txml", AllowCaching = false)]
    public sealed class T4AssetImporter : ScriptedImporter
    {
        private const string c_Identifier = "T4CodeGen";

        private static Regex 
            NameSpaceRegex = new Regex(@"<import\snamespace\s=\s[\W](.+)[\W]>");

        public List<string> m_Usings = new List<string>();

        public override void OnImportAsset(AssetImportContext ctx)
        {
            string txt = File.ReadAllText(ctx.assetPath);

            ConstructUsings(txt, m_Usings);

            Build(ctx);
        }
        private static void ConstructUsings(string txt, List<string> output)
        {
            MatchCollection usings = NameSpaceRegex.Matches(txt);
            for (int i = 0; i < usings.Count; i++)
            {
                string usingTxt = usings[i].Groups[1].Value;
                if (output.Contains(usingTxt)) continue;
                else if (usingTxt.Contains(","))
                {
                    usingTxt = usingTxt.Replace("\"", string.Empty).Trim();
                    string[] vs = usingTxt.Split(',');

                    foreach (var item in vs)
                    {
                        string temp = item.Trim();
                        if (output.Contains(temp)) continue;

                        output.Add(temp);
                    }

                    continue;
                }

                output.Add(usingTxt);
            }
        }
        private static void Build(AssetImportContext ctx)
        {
            //string fileName = Path.GetFileNameWithoutExtension(ctx.assetPath).Replace(".", string.Empty);
            //Type type = s_GeneratedTypes.Where(t => t.Name.Equals(fileName)).First();

            //var methodInfo = type.GetMethod("TransformText", BindingFlags.Public | BindingFlags.Instance);
            //object instance = Activator.CreateInstance(type);
            //string result = (string)methodInfo.Invoke(instance, new object[] { });

            //$"{result}".ToLog();
        }
    }
}

#endif