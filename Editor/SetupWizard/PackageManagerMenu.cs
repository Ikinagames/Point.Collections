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

using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEditor.PackageManager.UI;
using UnityEditorInternal;
using UnityEngine;

namespace Point.Collections.Editor
{
    internal sealed class PackageManagerMenu : SetupWizardMenuItem
    {
        public override string Name => "Package Manager";
        public override int Order => 0;

        private PackageCollection m_Packages;
        private ListRequest m_Request;
        private bool m_OpenAllPackages = false;

        private Vector2 m_Scroll;

        public PackageManagerMenu()
        {
            m_Request = UnityEditor.PackageManager.Client.List();
        }

        public override bool Predicate() => true;
        public override void OnGUI()
        {
            if (!m_Request.IsCompleted)
            {
                EditorGUILayout.LabelField($"Retriving package infomations .. {m_Request.Status}");
                return;
            }
            if (m_Packages == null)
            {
                m_Packages = m_Request.Result;
                OnPackageLoaded();
            }

            DrawGUI();

            m_OpenAllPackages = EditorGUILayout.Foldout(m_OpenAllPackages, "Open All Packages", true);
            if (m_OpenAllPackages)
            {
                using (var scroll = new EditorGUILayout.ScrollViewScope(m_Scroll))
                {
                    foreach (var item in m_Packages)
                    {
                        EditorGUILayout.LabelField(item.packageId);
                    }

                    m_Scroll = scroll.scrollPosition;
                }
            }
        }

        const string
            c_Json = "com.unity.nuget.newtonsoft-json",
            c_UI = "com.unity.ui",
#if !UNITY_2021_1_OR_NEWER
            c_UIBuilder = "com.unity.ui.builder",
#endif
            c_InputSystem = "com.unity.inputsystem",
            c_Burst = "com.unity.burst",
            c_Collections = "com.unity.collections",
            c_Mathematics = "com.unity.mathematics";
        private bool
            m_JsonInstalled,
            m_UIInstalled,
            m_UIBuilderInstalled,
            m_InputSystemInstalled,
            m_BurstInstalled,
            m_CollectionsInstalled,
            m_MathematicsInstalled;

        private void OnPackageLoaded()
        {
            m_JsonInstalled = HasPackage(c_Json);
            m_UIInstalled = HasPackage(c_UI);
#if !UNITY_2021_1_OR_NEWER
            m_UIBuilderInstalled = HasPackage(c_UIBuilder);
#endif
            m_InputSystemInstalled = HasPackage(c_InputSystem);
            m_BurstInstalled = HasPackage(c_Burst);
            m_CollectionsInstalled = HasPackage(c_Collections);
            m_MathematicsInstalled = HasPackage(c_Mathematics);
        }
        private void DrawGUI()
        {
            DrawPackageField(ref m_JsonInstalled, c_Json);
            DrawPackageField(ref m_UIInstalled, c_UI);
#if !UNITY_2021_1_OR_NEWER
            DrawPackageField(ref m_UIBuilderInstalled, c_UIBuilder);
#endif
            DrawPackageField(ref m_InputSystemInstalled, c_InputSystem);
            DrawPackageField(ref m_BurstInstalled, c_Burst);
            DrawPackageField(ref m_CollectionsInstalled, c_Collections);
            DrawPackageField(ref m_MathematicsInstalled, c_Mathematics);
        }

        private AddRequest m_AddRequest;
        private string m_AddPackageID;

        private void DrawPackageField(ref bool installed, in string id)
        {
            string text;
            if (installed)
            {
                text = $"Installed {id}";
            }
            else
            {
                text = $"Install {id}";
            }

            using (new EditorGUI.DisabledGroupScope(installed))
            using (var changed = new EditorGUI.ChangeCheckScope())
            {
                installed = EditorGUILayout.ToggleLeft(text, installed);

                if (changed.changed)
                {
                    if (installed)
                    {
                        m_AddPackageID = id;
                        m_AddRequest = AddPackage(m_AddPackageID);

                        EditorUtility.DisplayProgressBar($"Add package {m_AddPackageID}", "Downloading package data ...", 0);
                    }
                }
            }
        }
        public override void OnUpdate()
        {
            if (m_AddRequest != null)
            {
                if (m_AddRequest.Status == StatusCode.InProgress)
                {
                    EditorUtility.DisplayProgressBar($"Add package {m_AddPackageID}", "Downloading package data ...", 50);
                }
                else
                {
                    EditorUtility.ClearProgressBar();

                    if (m_AddRequest.Status == StatusCode.Failure)
                    {
                        ShowNotification(
                            new GUIContent($"Add package {m_AddPackageID} request has been failed."));
                    }

                    m_AddPackageID = null;
                    m_AddRequest = null;
                }
            }
        }

        // https://forum.unity.com/threads/is-there-a-scripting-api-to-view-installed-projects.536908/
        private bool HasPackage(string id)
        {
            foreach (var item in m_Packages)
            {
                if (item.packageId.Contains(id))
                {
                    return true;
                }
            }
            return false;
        }
        private static AddRequest AddPackage(string id)
        {
            var request = UnityEditor.PackageManager.Client.Add(id);

            return request;
        }
    }
}

#endif