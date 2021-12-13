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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Point.Collections.Editor
{
    public sealed class PointSetupWizard : EditorWindow, IStaticInitializer
    {
        static PointSetupWizard()
        {
            EditorApplication.delayCall -= Startup;
            EditorApplication.delayCall += Startup;
        }
        static void Startup()
        {
            if (Application.isPlaying || EditorApplication.isPlayingOrWillChangePlaymode) return;

            //if (!new GeneralMenu().Predicate() ||
            //    !new SceneMenu().Predicate() ||
            //    !new PrefabMenu().Predicate())
            //{
            //    CoreSystemMenuItems.CoreSystemSetupWizard();
            //    return;
            //}

            //if (!CoreSystemSettings.Instance.m_HideSetupWizard)
            {
                PointMenuItems.CoreSystemSetupWizard();
            }
        }

        private Rect m_CopyrightRect = new Rect(175, 475, 245, 20);

        private Texture2D m_EnableTexture;
        private Texture2D m_DisableTexture;

        private GUIStyle titleStyle;
        private GUIStyle iconStyle;

        private void OnEnable()
        {
            m_DisableTexture = AssetHelper.LoadAsset<Texture2D>("CrossYellow", "PointEditor");
            m_EnableTexture = AssetHelper.LoadAsset<Texture2D>("TickGreen", "PointEditor");

            titleStyle = new GUIStyle();
            titleStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.white : Color.black;
            titleStyle.wordWrap = true;
            titleStyle.fontStyle = FontStyle.Bold;
            titleStyle.alignment = TextAnchor.MiddleCenter;

            iconStyle = new GUIStyle();
            iconStyle.alignment = TextAnchor.MiddleCenter;
        }
        private void OnGUI()
        {
            GUILayout.Space(20);
            EditorUtilities.StringHeader("Setup", 30, true);
            GUILayout.Space(10);
            EditorUtilities.Line();
            GUILayout.Space(10);

            //DrawToolbar();

            EditorUtilities.Line();

            //using (new EditorUtilities.BoxBlock(Color.black))
            //{
            //    switch ((ToolbarNames)m_SelectedToolbar)
            //    {
            //        case ToolbarNames.General:
            //            m_GeneralMenu.OnGUI();
            //            break;
            //        case ToolbarNames.Scene:
            //            m_SceneMenu.OnGUI();
            //            break;
            //        case ToolbarNames.Prefab:
            //            m_PrefabMenu.OnGUI();
            //            break;
            //        default:
            //            break;
            //    }
            //}

            EditorGUI.LabelField(m_CopyrightRect, EditorUtilities.String("Copyright 2021 Ikina Games. All rights reserved.", 11), EditorStyleUtilities.CenterStyle);
        }

        #region Toolbar

        //private readonly Dictionary<ToolbarNames, Func<bool>> m_IsSetupDone = new Dictionary<ToolbarNames, Func<bool>>();
        //private void DrawToolbar()
        //{
        //    const float spacing = 50;

        //    EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        //    GUILayout.Space(spacing);

        //    string[] toolbarNames = Enum.GetNames(typeof(ToolbarNames));
        //    for (int i = 0; i < toolbarNames.Length; i++)
        //    {
        //        bool done;
        //        if (m_IsSetupDone.ContainsKey((ToolbarNames)i))
        //        {
        //            done = m_IsSetupDone[(ToolbarNames)i].Invoke();
        //        }
        //        else done = true;
        //        DrawToolbarButton(i, toolbarNames[i], done);
        //    }

        //    GUILayout.Space(spacing);
        //    EditorGUILayout.EndHorizontal();
        //}
        //private void DrawToolbarButton(int i, string name, bool enable)
        //{
        //    using (new EditorUtilities.BoxBlock(i.Equals(m_SelectedToolbar) ? Color.black : Color.gray))
        //    {
        //        EditorGUILayout.BeginHorizontal(GUILayout.Height(22));
        //        if (GUILayout.Button(name, titleStyle))
        //        {
        //            m_SelectedToolbar = i;
        //        }
        //        GUILayout.Label(enable ? m_EnableTexture : m_DisableTexture, iconStyle);
        //        EditorGUILayout.EndHorizontal();
        //    }
        //}

        #endregion
    }
}
