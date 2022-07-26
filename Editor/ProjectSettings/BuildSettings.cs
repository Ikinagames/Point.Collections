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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE

#if UNITY_MATHEMATICS
#endif

using UnityEditor;

namespace Point.Collections.Editor
{
    public static class BuildSettings
    {
        public static Platform CurrentPlatform
        {
            get => (Platform)EditorUserBuildSettings.activeBuildTarget;
            set
            {
                BuildTarget target = (BuildTarget)value;
                BuildTargetGroup group = Convert(target);
                EditorUserBuildSettings.SwitchActiveBuildTargetAsync(group, target);
            }
        }
        public static Archtect CurrentArchtect
        {
            get
            {
                BuildTarget target = CurrentBuildTarget;
                switch (target)
                {
                    default:
                    case BuildTarget.NoTarget:
                    case BuildTarget.StandaloneOSX:
                    case BuildTarget.Android:
                    case BuildTarget.iOS:
                    case BuildTarget.WebGL:
                        return Archtect.None;
                    case BuildTarget.StandaloneWindows:
                    case BuildTarget.StandaloneLinux:
                        return Archtect.x86;
                    case BuildTarget.StandaloneWindows64:
                    case BuildTarget.StandaloneLinux64:
                        return Archtect.x86_64;
                }
            }
        }

        public static BuildTarget CurrentBuildTarget => EditorUserBuildSettings.activeBuildTarget;

        // https://forum.unity.com/threads/buildtargetgroup-buildtarget-the-difference-between-the-two.473364/
        public static BuildTargetGroup Convert(this BuildTarget buildTarget)
        {
            switch (buildTarget)
            {
                default:
                case BuildTarget.NoTarget:
                case BuildTarget.StandaloneWindows:
                case BuildTarget.StandaloneLinux:
                case BuildTarget.StandaloneWindows64:
                case BuildTarget.StandaloneLinux64:
                case BuildTarget.StandaloneLinuxUniversal:
                    return BuildTargetGroup.Standalone;
                case BuildTarget.StandaloneOSX:
                case BuildTarget.iOS:
                    return BuildTargetGroup.iOS;
                case BuildTarget.Android:
                    return BuildTargetGroup.Android;
                case BuildTarget.WebGL:
                    return BuildTargetGroup.WebGL;
                case BuildTarget.WSAPlayer:
                    return BuildTargetGroup.WSA;
                case BuildTarget.PS4:
                    return BuildTargetGroup.PS4;
                case BuildTarget.XboxOne:
                    return BuildTargetGroup.XboxOne;
                case BuildTarget.tvOS:
                    return BuildTargetGroup.tvOS;
                case BuildTarget.Switch:
                    return BuildTargetGroup.Switch;
            }
        }

        public static string Convert(this Archtect t)
        {
            if (t == Archtect.None) return string.Empty;

            return t.ToString();
        }
    }

    public enum Platform
    {
        android = BuildTarget.Android,
        html5 = BuildTarget.WebGL,
        ios = BuildTarget.iOS,
        linux = BuildTarget.StandaloneLinux64,
        mac = BuildTarget.StandaloneOSX,
        tvos = BuildTarget.tvOS,
        uwp = BuildTarget.GameCoreXboxSeries,
        win = BuildTarget.StandaloneWindows64,
    }
    public enum Archtect
    {
        None,

        x86,
        x86_64
    }
}

#endif