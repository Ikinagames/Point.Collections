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
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Mathematics;
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

        private Dictionary<string, SearchRequest> m_SearchRequests = new Dictionary<string, SearchRequest>();
        private Dictionary<string, SearchRequest> m_ServerSearchRequests = new Dictionary<string, SearchRequest>();

        private PackageCollection m_Packages;
        private ListRequest m_Request;
        private bool m_OpenAllPackages = false;

        private Vector2 m_Scroll;

        public enum PackageStatus
        {
            Loading = 0,

            Installed,
            InstalledWithDependencies,
            NotInstalled,

            RequireUpdate
        }
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
#if !UNITY_2021_1_OR_NEWER
            c_UI = "com.unity.ui",
            c_UIBuilder = "com.unity.ui.builder",
            c_DeviceSim = "com.unity.device-simulator",
#else
            c_CodeCoverage = "com.unity.testtools.codecoverage",
            c_VectorGraphics = "com.unity.vectorgraphics",
            c_PlayableGroupVisualizer = "com.unity.playablegraph-visualizer",
#if !UNITY_2022_1_OR_NEWER
            c_Spline = "com.unity.splines",
#endif
#endif
            c_InputSystem = "com.unity.inputsystem",
            c_Burst = "com.unity.burst",
            c_Collections = "com.unity.collections",
            c_Mathematics = "com.unity.mathematics";
        private PackageStatus
            m_JsonInstalled,
#if !UNITY_2021_1_OR_NEWER
            m_UIInstalled,
            m_UIBuilderInstalled,
            m_DeviceSimInstalled,
#else
            m_CodeCoverageInstalled,
            m_VectorGraphicsInstalled,
            m_PlayableGroupVisualizerInstalled,
#if !UNITY_2022_1_OR_NEWER
            m_SplineInstalled,
#endif
#endif
            m_InputSystemInstalled,
            m_BurstInstalled,
            m_CollectionsInstalled,
            m_MathematicsInstalled;
        private static readonly PackageVersion
            s_BurstVersion = new PackageVersion(1, 6, 6),
            s_CollectionsVersion = new PackageVersion(1, 3, 1),
            s_MathematicsVersion = new PackageVersion(1, 2, 6);

        private void OnPackageLoaded()
        {
            m_JsonInstalled = HasPackage(c_Json);
#if !UNITY_2021_1_OR_NEWER
            m_UIInstalled = HasPackage(c_UI);
            m_UIBuilderInstalled = HasPackage(c_UIBuilder);
            m_DeviceSimInstalled = HasPackage(c_DeviceSim);
#else
            m_CodeCoverageInstalled = HasPackage(c_CodeCoverage);
            m_VectorGraphicsInstalled = HasPackage(c_VectorGraphics);
            m_PlayableGroupVisualizerInstalled = HasPackage(c_PlayableGroupVisualizer);
#if !UNITY_2022_1_OR_NEWER
            m_SplineInstalled = HasPackage(c_Spline);
#endif
#endif
            m_InputSystemInstalled = HasPackage(c_InputSystem);
            m_BurstInstalled = HasPackage(c_Burst, s_BurstVersion);
            m_CollectionsInstalled = HasPackage(c_Collections, s_CollectionsVersion);
            m_MathematicsInstalled = HasPackage(c_Mathematics, s_MathematicsVersion);
        }
        private void DrawGUI()
        {
            DrawPackageField(ref m_JsonInstalled, c_Json);
#if !UNITY_2021_1_OR_NEWER
            DrawPackageField(ref m_UIInstalled, c_UI);
            DrawPackageField(ref m_UIBuilderInstalled, c_UIBuilder);
            DrawPackageField(ref m_DeviceSimInstalled, c_DeviceSim);
#else
            DrawPackageField(ref m_CodeCoverageInstalled, c_CodeCoverage);
            DrawPackageField(ref m_VectorGraphicsInstalled, c_VectorGraphics);
            DrawPackageField(ref m_PlayableGroupVisualizerInstalled, c_PlayableGroupVisualizer);
#if !UNITY_2022_1_OR_NEWER
            DrawPackageField(ref m_SplineInstalled, c_Spline);
#endif
#endif
            DrawPackageField(ref m_InputSystemInstalled, c_InputSystem);
            DrawPackageField(ref m_BurstInstalled, c_Burst, s_BurstVersion);
            DrawPackageField(ref m_CollectionsInstalled, c_Collections, s_CollectionsVersion);
            DrawPackageField(ref m_MathematicsInstalled, c_Mathematics, s_MathematicsVersion);
        }

        private AddRequest m_AddRequest;
        private string m_AddPackageID;
        private Action m_RevertDelegate;

        private void DrawPackageField(ref PackageStatus status, in string id, in PackageVersion minimumVersion = default)
        {
            string text;
            if (status == PackageStatus.Loading)
            {
                text = $"Loading {id}";
            }
            else if (status == PackageStatus.NotInstalled)
            {
                text = $"Install {id}";
            }
            else
            {
                text = $"{ObjectNames.NicifyVariableName(status.ToString())} {id}";
            }
            bool isInstalled = status == PackageStatus.Installed || status == PackageStatus.InstalledWithDependencies;

            using (new EditorGUI.DisabledGroupScope(
                isInstalled || status == PackageStatus.Loading))
            using (var changed = new EditorGUI.ChangeCheckScope())
            {
                isInstalled = EditorGUILayout.ToggleLeft(text, isInstalled);

                if (changed.changed)
                {
                    if (status == PackageStatus.NotInstalled)
                    {
                        status = PackageStatus.Installed;
                        m_AddPackageID = id;
                        m_AddRequest = AddPackage(m_AddPackageID);

                        EditorUtility.DisplayProgressBar($"Add package {m_AddPackageID}", "Requesting package data ...", 0);
                    }
                    else if (status == PackageStatus.RequireUpdate)
                    {
                        status = PackageStatus.Installed;
                        m_AddPackageID = id;
                        m_AddRequest = AddPackage(m_AddPackageID);

                        EditorUtility.DisplayProgressBar($"Add package {m_AddPackageID}", "Requesting package data ...", 0);
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
                    EditorUtility.DisplayProgressBar($"Add package {m_AddPackageID}", "Downloading package data ...", .5f);
                }
                else
                {
                    EditorUtility.ClearProgressBar();

                    if (m_AddRequest.Status == StatusCode.Failure)
                    {
                        ShowNotification(
                            new GUIContent($"Add package {m_AddPackageID} request has been failed."));
                        OnPackageLoaded();
                    }

                    m_AddPackageID = null;
                    m_AddRequest = null;
                }
            }
        }

        // https://forum.unity.com/threads/is-there-a-scripting-api-to-view-installed-projects.536908/
        private PackageStatus HasPackage(string id, PackageVersion minimumVersion = default)
        {
            foreach (var item in m_Packages)
            {
                if (item.packageId.Contains(id))
                {
                    PackageVersion packageVersion = new PackageVersion(item.version);
                    if (minimumVersion.IsValid() && packageVersion < minimumVersion)
                    {
                        return PackageStatus.RequireUpdate;
                    }
                    return PackageStatus.Installed;
                }
            }

            foreach (var item in m_Packages)
            {
                foreach (var dep in item.resolvedDependencies)
                {
                    //PackageVersion packageVersion = new PackageVersion(dep.version);
                    //$"{dep.name} ({dep.version} : {packageVersion})".ToLog();

                    if (dep.name.Contains(id))
                    {
                        PackageVersion packageVersion = new PackageVersion(dep.version);
                        if (minimumVersion.IsValid() && packageVersion < minimumVersion)
                        {
                            return PackageStatus.RequireUpdate;
                        }

                        return PackageStatus.InstalledWithDependencies;
                    }
                }
            }

            return PackageStatus.NotInstalled;
        }
        private static AddRequest AddPackage(string id, PackageVersion version = default)
        {
            if (version.IsValid())
            {
                id += $"@{version.ToString()}";
                $"request id {id}".ToLog();
            }

            var request = UnityEditor.PackageManager.Client.Add(id);

            return request;
        }
        private static RemoveRequest RemovePackage(string id)
        {
            var request = UnityEditor.PackageManager.Client.Remove(id);
            return request;
        }

        private struct PackageVersion : IValidation
        {
            public int x, y, z;
            public string wString;
            public int w;

            public PackageVersion(string version)
            {
                var match 
                    = Regex.Match(version, @"^(\d+)" + Regex.Escape(".") + @"(\d+)" + Regex.Escape(".") + @"(\d+)(?:-?)(.+)?");
                if (!match.Success)
                {
                    this = default(PackageVersion);
                    return;
                }

                x = int.Parse(match.Groups[1].Value);
                y = int.Parse(match.Groups[2].Value);
                z = int.Parse(match.Groups[3].Value);

                if (!match.Groups[4].Value.IsNullOrEmpty())
                {
                    wString = match.Groups[4].Value;
                    var temp = Regex.Replace(wString, @"[^\d]", string.Empty);
                    w = int.Parse(temp);
                }
                else
                {
                    wString = string.Empty;
                    w = -1;
                }
            }
            public PackageVersion(int x, int y, int z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
                wString = string.Empty;
                w = -1;
            }
            public PackageVersion(int4 version)
            {
                x = version.x;
                y = version.y;
                z = version.z;
                wString = string.Empty;
                w = version.w;
            }

            public bool IsValid()
            {
                if (x == 0 && y == 0 && z == 0 && w == 0) return false;
                return true; ;
            }

            public override bool Equals(object obj)
            {
                if (obj == null) return false;
                if (!(obj is PackageVersion other)) return false;
                
                return x == other.x &&
                    y == other.y &&
                    z == other.z &&
                    w == other.w;
            }
            public override int GetHashCode()
            {
                return x ^ y ^ z ^ w;
            }
            public override string ToString()
            {
                if (wString.IsNullOrEmpty())
                {
                    return $"{x}.{y}.{z}";
                }
                return $"{x}.{y}.{z}-{wString}";
            }

            public static bool operator <(PackageVersion xx, PackageVersion yy)
            {
                if (xx.x < yy.x) return true;
                else if (xx.x == yy.x && xx.y < yy.y) return true;
                else if (xx.x == yy.x && xx.y == yy.y && xx.z < yy.z) return true;

                if (xx.w != -1 && yy.w != -1)
                {
                    if (xx.x == yy.x && xx.y == yy.y && xx.z == yy.z && xx.w < yy.w) return true;
                }

                return false;
            }
            public static bool operator >(PackageVersion yy, PackageVersion xx)
            {
                if (xx.x < yy.x) return true;
                else if (xx.x == yy.x && xx.y < yy.y) return true;
                else if (xx.x == yy.x && xx.y == yy.y && xx.z < yy.z) return true;

                if (xx.w != -1 && yy.w != -1)
                {
                    if (xx.x == yy.x && xx.y == yy.y && xx.z == yy.z && xx.w < yy.w) return true;
                }

                return false;
            }
            public static bool operator ==(PackageVersion xx, PackageVersion yy)
            {
                if (xx.x == yy.x && xx.y == yy.y && xx.z == yy.z && xx.w == yy.w) return true;
                return false;
            }
            public static bool operator !=(PackageVersion xx, PackageVersion yy)
            {
                if (xx.x != yy.x || xx.y != yy.y || xx.z != yy.z || xx.w != yy.w) return true;
                return false;
            }
        }
    }
}

#endif