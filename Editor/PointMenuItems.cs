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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2019_1_OR_NEWER
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using Point.Collections.Native;
using Point.Collections.ResourceControl;
using System;
using System.Collections;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;
using UnityEngine.Rendering;

namespace Point.Collections.Editor
{
    public sealed class PointMenuItems : EditorWindow
    {
        [MenuItem("Point/Setup Wizard", priority = -10000)]
        public static void CoreSystemSetupWizard()
        {
            const string title = "Point Framework Setup Wizard";

            CoreGUI.EditorWindow.OpenWindowAtCenterSafe<PointSetupWizard>(title, true, 
                new Vector2(600, 500));
        }

        [MenuItem("Point/Utils/Capture Current Game Screen", priority = 500)]
        public static void CaptureCurrentGameScreen()
        {
            if (Application.isPlaying)
            {
                PointApplication.Instance.StartCoroutine(CaptureUpdate());
                return;
            }

            string path = EditorUtility.SaveFilePanel("Save ScreenShot",
                Application.dataPath, "GameCapture", "png");
            if (!path.IsNullOrEmpty())
            {
                GameView.Repaint();

                var tex = GameView.RenderTexture.ToTexture2D();
                tex.Flip();
                tex.Reverse();

                tex.SaveTextureAsPNG(path);
            }
        }
        private static IEnumerator CaptureUpdate()
        {
            yield return new WaitForEndOfFrame();

            Texture2D tex
                    = ScreenCapture.CaptureScreenshotAsTexture(ScreenCapture.StereoScreenCaptureMode.BothEyes);
            string path = EditorUtility.SaveFilePanel("Save ScreenShot",
                Application.dataPath, "GameCapture", "png");
            if (!path.IsNullOrEmpty())
            {
                tex.SaveTextureAsPNG(path);
            }
        }

        [MenuItem("Point/Utils/Generate Guid", priority = 1000)]
        public static void GenerateGUID()
        {
            Guid guid = Guid.NewGuid();
            EditorGUIUtility.systemCopyBuffer = guid.ToString();

            PointHelper.Log(LogChannel.Editor, $"{guid} is copied to clipboard.");
        }
        [MenuItem("Point/Utils/Unlock Assemblies", priority = 1001)]
        public static void Unlock()
        {
            EditorApplication.UnlockReloadAssemblies();
        }
        [MenuItem("Point/Utils/Locate Resource Hash Map", priority = 1002)]
        public static void LocateResourceHashMap()
        {
            ResourceHashMap hashMap = ResourceHashMap.Instance;

            Selection.activeObject = hashMap;
            EditorGUIUtility.PingObject(hashMap);
        }

        [MenuItem("Assets/Create/Point/Create T4")]
        public static void CreateAsset()
        {
            string filePath;
            if (Selection.assetGUIDs.Length == 0)
                filePath = "Assets/New TMP Color Gradient.asset";
            else
                filePath = AssetDatabase.GUIDToAssetPath(Selection.assetGUIDs[0]);

            $"{filePath}".ToLog();
            filePath = Path.Combine(filePath, "t4Templete.txml");

            File.WriteAllText(filePath, string.Empty);
            AssetDatabase.ImportAsset(filePath);
        }
    }
}

#endif