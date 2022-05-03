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

using System.Collections;
using UnityEngine;
using UnityEditor;
using System;

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

        private SetupWizardMenuItem[] m_MenuItems;

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

            Type[] menuItemTypes = TypeHelper.GetTypes(t => !t.IsAbstract && TypeHelper.TypeOf<SetupWizardMenuItem>.Type.IsAssignableFrom(t));
            m_MenuItems = new SetupWizardMenuItem[menuItemTypes.Length];
            for (int i = 0; i < menuItemTypes.Length; i++)
            {
                m_MenuItems[i] = (SetupWizardMenuItem)Activator.CreateInstance(menuItemTypes[i]);
            }
            Array.Sort(m_MenuItems);

            if (m_MenuItems.Length > 0) m_SelectedToolbar = m_MenuItems[0];

            //CoreSystemSettings.Instance.m_HideSetupWizard = true;
            //EditorUtility.SetDirty(CoreSystemSettings.Instance);
        }
        private void OnGUI()
        {
            const string c_Copyrights = "Copyright 2021 Ikinagames. All rights reserved.";

            GUILayout.Space(20);
            EditorUtilities.StringHeader("Point Framework¢ç", 30, true);
            GUILayout.Space(10);
            EditorUtilities.Line();
            GUILayout.Space(10);

            DrawToolbar();

            EditorUtilities.Line();

            using (new EditorUtilities.BoxBlock(Color.black))
            {
                if (m_SelectedToolbar != null)
                {
                    m_SelectedToolbar.OnGUI();
                }
            }

            EditorGUI.LabelField(m_CopyrightRect, EditorUtilities.String(c_Copyrights, 11), EditorStyleUtilities.CenterStyle);
        }

        public SetupWizardMenuItem SelectedToolbar => m_SelectedToolbar;

        #region Toolbar

        private SetupWizardMenuItem m_SelectedToolbar;

        private void DrawToolbar()
        {
            const float spacing = 50;

            EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
            GUILayout.Space(spacing);

            for (int i = 0; i < m_MenuItems.Length; i++)
            {
                DrawToolbarButton(i, m_MenuItems[i].Name, m_MenuItems[i].Predicate());
            }

            GUILayout.Space(spacing);
            EditorGUILayout.EndHorizontal();
        }
        private void DrawToolbarButton(int i, string name, bool enable)
        {
            using (new EditorUtilities.BoxBlock(i.Equals(m_SelectedToolbar) ? Color.black : Color.gray))
            {
                EditorGUILayout.BeginHorizontal(GUILayout.Height(22));
                if (GUILayout.Button(name, titleStyle))
                {
                    m_SelectedToolbar = m_MenuItems[i];
                }
                GUILayout.Label(enable ? m_EnableTexture : m_DisableTexture, iconStyle);
                EditorGUILayout.EndHorizontal();
            }
        }

        #endregion
    }
}

#endif