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

using Point.Collections.Editor;
using UnityEditor;
using UnityEngine;
using UEditor = global::UnityEditor.Editor;

namespace Point.Collections.ResourceControl.Editor
{
    public sealed class ResourceAssetInspectorGUI
    {
        //[InitializeOnLoadMethod]
        //static void Intialize()
        //{
        //    UEditor.finishedDefaultHeaderGUI -= OnPostHeaderGUI;
        //    UEditor.finishedDefaultHeaderGUI += OnPostHeaderGUI;
        //}

        //private static void OnPostHeaderGUI(UEditor editor)
        //{
        //    bool isTrackedAsset = editor.target.IsTrackedAsset();
        //    string headerString = EditorUtilities.String("Asset", 13);
        //    if (isTrackedAsset)
        //    {
        //        headerString += EditorUtilities.String(": Tracked", 10);
        //    }
        //    else
        //    {
        //        headerString += EditorUtilities.String(": None", 10);
        //    }

        //    EditorUtilities.Line();
        //    using (new EditorGUILayout.HorizontalScope())
        //    {
        //        EditorUtilities.StringRich(headerString);

        //        if (GUILayout.Button(isTrackedAsset ? "Remove" : "Add", GUILayout.Width(60)))
        //        {
        //            if (isTrackedAsset) editor.target.RemoveAsset();
        //            else editor.target.RegisterAsset();
        //        }
        //    }

        //    EditorUtilities.Line();
        //}
    }
}
