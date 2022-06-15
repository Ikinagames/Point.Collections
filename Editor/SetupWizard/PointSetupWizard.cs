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
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public sealed class PointSetupWizard : EditorWindowUXML, IStaticInitializer
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

        private SetupWizardMenuItem[] m_MenuItems;

        private SetupWizardMenuItem m_SelectedToolbar;
        public SetupWizardMenuItem SelectedToolbar => m_SelectedToolbar;

        private void OnEnable()
        {
            m_DisableTexture = AssetHelper.LoadAsset<Texture2D>("CrossYellow", "PointEditor");
            m_EnableTexture = AssetHelper.LoadAsset<Texture2D>("TickGreen", "PointEditor");

            Type[] menuItemTypes = TypeHelper.GetTypes(t => !t.IsAbstract && TypeHelper.TypeOf<SetupWizardMenuItem>.Type.IsAssignableFrom(t));
            m_MenuItems = new SetupWizardMenuItem[menuItemTypes.Length];
            for (int i = 0; i < menuItemTypes.Length; i++)
            {
                m_MenuItems[i] = (SetupWizardMenuItem)Activator.CreateInstance(menuItemTypes[i]);
                m_MenuItems[i].Initialize();
            }
            Array.Sort(m_MenuItems);
        }
        private void OnFocus()
        {
            if (m_SelectedToolbar != null)
            {
                m_SelectedToolbar.OnFocus();
            }
        }
        private void OnLostFocus()
        {
            if (m_SelectedToolbar != null)
            {
                m_SelectedToolbar.OnLostFocus();
            }
        }
        private void Update()
        {
            if (m_SelectedToolbar != null)
            {
                m_SelectedToolbar.OnUpdate();
            }
        }
        protected override VisualTreeAsset GetVisualTreeAsset()
        {
            VisualTreeAsset asset = AssetHelper.LoadAsset<VisualTreeAsset>("Uxml SetupWizard", "PointEditor");
            return asset;
        }
        protected override void SetupVisualElement(VisualElement root)
        {
            SetupMenuItems(root);

            SetupGUI(root);
        }
        private void SetupMenuItems(VisualElement root)
        {
            VisualElement container = root.Q("MenuItemContainer").Q("Container");
            Label menuItemLabel = root.Q("Bottom").Q<Label>("MenuItemLabel");
            IMGUIContainer gui = root.Q("Bottom").Q<IMGUIContainer>("MenuItemGUI");

            for (int i = 0; i < m_MenuItems.Length; i++)
            {
                SetupWizardMenuItem menuItem = m_MenuItems[i];
                Button button = new Button();
                button.text = menuItem.Name;
                button.AddToClassList("menuitem-button");

                button.clicked += delegate
                {
                    if (m_SelectedToolbar != null && m_SelectedToolbar.Root != null)
                    {
                        m_SelectedToolbar.Root.RemoveFromHierarchy();
                    }

                    m_SelectedToolbar = menuItem;
                    menuItemLabel.text = m_SelectedToolbar.Name;

                    m_SelectedToolbar.OnVisible();
                    if (m_SelectedToolbar.Root != null)
                    {
                        gui.Add(m_SelectedToolbar.Root);
                        gui.MarkDirtyRepaint();
                    }
                };

                container.Add(button);
            }

            if (m_MenuItems.Length > 0)
            {
                m_SelectedToolbar = m_MenuItems[0];
                menuItemLabel.text = m_SelectedToolbar.Name;

                m_SelectedToolbar.OnVisible();
                if (m_SelectedToolbar.Root != null)
                {
                    gui.Add(m_SelectedToolbar.Root);
                    gui.MarkDirtyRepaint();
                }
            }
        }
        private void SetupGUI(VisualElement root)
        {
            IMGUIContainer gui = root.Q("Bottom").Q<IMGUIContainer>("MenuItemGUI");

            gui.onGUIHandler += MenuItemGUI;
        }
        private void MenuItemGUI()
        {
            if (m_SelectedToolbar == null) return;

            if (m_SelectedToolbar.Root == null)
            {
                m_SelectedToolbar.OnGUI();
            }
        }
    }
}

#endif