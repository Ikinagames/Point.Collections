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

using UnityEngine;
using UnityEditor;

namespace Point.Collections.Editor
{
    internal sealed class AssetBundleMenu : SetupWizardMenuItem
    {
        public override string Name => "AssetBundle";
        public override int Order => 0;

        public override bool Predicate()
        {
            return true;
        }
        public override void OnGUI()
        {
            if (GUILayout.Button("Build"))
            {
                Build();
            }
        }
        public void Build()
        {
            string[] bundleNames = AssetDatabase.GetAllAssetBundleNames();
            AssetBundleBuild[] infos = new AssetBundleBuild[bundleNames.Length];
            for (int i = 0; i < bundleNames.Length; i++)
            {
                string[] assetPaths = AssetDatabase.GetAssetPathsFromAssetBundle(bundleNames[i]);
                AssetBundleBuild bundleBuild = new AssetBundleBuild
                {
                    assetBundleName = bundleNames[i],
                    assetNames = assetPaths,
                };
                infos[i] = bundleBuild;
            }

            BuildPipeline.BuildAssetBundles(
                Application.streamingAssetsPath, 
                infos, 
                BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);


        }
    }
}

#endif