﻿// Copyright 2021 Ikina Games
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

using Point.Collections.ResourceControl;
using Point.Collections.ResourceControl.Editor;
using System.IO;
using System.Linq;
using UnityEditor;

namespace Point.Collections.Editor
{
    public sealed class PointAssetPostProcessor : AssetPostprocessor
    {
        private static string[]
            // https://docs.unity3d.com/Manual/class-AudioClip.html
            s_AudioExtensions = new[] { ".aif", ".wav", ".ogg", ".mp3" };

        //public static void OnPostprocessAllAssets(
        //    string[] importedAssets, 
        //    string[] deletedAssets, 
        //    string[] movedAssets, 
        //    string[] movedFromAssetPaths)
        //{
        //    foreach (string str in importedAssets)
        //    {
        //        if (IsAudioAsset(in str))
        //        {
        //            PointProjectSettings.Instance.RegisterAsset(in str);
        //        }
        //    }
        //    foreach (string str in deletedAssets)
        //    {
        //        HandleDeletedAsset(in str);
        //    }

        //    for (int i = 0; i < movedAssets.Length; i++)
        //    {
        //        HandleMovedAsset(in movedFromAssetPaths[i], in movedAssets[i]);
        //    }
        //}
        //private static bool IsAudioAsset(in string assetPath)
        //{
        //    if ((AssetAddressesSetting.Instance.ImportHandles & AssetAddressesSetting.AssetImportHandles.Audio)
        //        != AssetAddressesSetting.AssetImportHandles.Audio) return false;

        //    string extension = Path.GetExtension(assetPath);

        //    return s_AudioExtensions.Contains(extension);
        //}

        //private static void HandleMovedAsset(in string from, in string to)
        //{
        //    if (!PointProjectSettings.Instance.IsTrackedAsset(in from)) return;

        //    PointProjectSettings.Instance.UpdateAsset(in from, in to);
        //}
        //private static void HandleDeletedAsset(in string assetPath)
        //{
        //    if (!PointProjectSettings.Instance.IsTrackedAsset(in assetPath)) return;

        //    PointProjectSettings.Instance.RemoveAsset(in assetPath);
        //}

        public void OnPreprocessAsset()
        {
            //if (assetImporter.importSettingsMissing)
            //{
            //    ModelImporter modelImporter = assetImporter as ModelImporter;
            //    if (modelImporter != null)
            //    {
            //        if (!assetPath.Contains("@"))
            //            modelImporter.importAnimation = false;
            //        modelImporter.materialImportMode = ModelImporterMaterialImportMode.None;
            //    }
            //}

            

            //$"{assetPath} imported".ToLog();
        }
        public void OnPreprocessAudio()
        {
            //if (assetPath.Contains("mono"))
            //{
            //    AudioImporter audioImporter = (AudioImporter)assetImporter;
            //    audioImporter.forceToMono = true;
            //}
        }
    }

    internal sealed class PointAssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        // https://docs.unity3d.com/ScriptReference/AssetModificationProcessor.html

        //static string[] OnWillSaveAssets(string[] paths)
        //{
        //    UnityEngine.Debug.Log("OnWillSaveAssets");
        //    foreach (string path in paths)
        //        UnityEngine.Debug.Log(path);
        //    return paths;
        //}
    }
}
