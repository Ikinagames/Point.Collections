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

using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    /// <summary>
    /// Display a Scene Reference object in the editor.
    /// If scene is valid, provides basic buttons to interact with the scene's role in Build Settings.
    /// </summary>
    //[CustomPropertyDrawer(typeof(SceneReference))]
    public class SceneReferencePropertyDrawerUXML : PropertyDrawerUXML<SceneReference>
    {
        // The exact name of the asset Object variable in the SceneReference object
        private const string sceneAssetPropertyString = "sceneAsset";
        // The exact name of the scene Path variable in the SceneReference object
        private const string scenePathPropertyString = "scenePath";

        //private readonly RectOffset boxPadding = EditorStyles.helpBox.padding;

        // Made these two const btw
        //private const float PAD_SIZE = 2f;
        //private const float FOOTER_HEIGHT = 10f;

        //private static readonly float lineHeight = EditorGUIUtility.singleLineHeight;
        //private static readonly float paddedLine = lineHeight + PAD_SIZE;

        protected override VisualElement CreateVisualElement(SerializedProperty property)
        {
            var asset = AssetHelper.LoadAsset<VisualTreeAsset>("Uxml SceneReference", "PointEditor");

            return asset.CloneTree();
        }
        protected override void SetupVisualElement(SerializedProperty property, VisualElement root)
        {
            SerializedProperty sceneAssetProperty = GetSceneAssetProperty(property);

            ObjectField sceneField = root.Q<ObjectField>("SceneField");
            VisualElement detailContainer = root.Q("Details");

            sceneField.value = sceneAssetProperty.objectReferenceValue;
            sceneField.RegisterValueChangedCallback(t =>
            {
                sceneAssetProperty.objectReferenceValue = t.newValue;

                var newBuildScene = BuildUtils.GetBuildScene(sceneAssetProperty.objectReferenceValue);
                if (newBuildScene.scene == null)
                {
                    GetScenePathProperty(property).stringValue = string.Empty;
                }
                
                if (newBuildScene.assetGUID.Empty())
                {
                    detailContainer.style.Hide(true);
                }
                else
                {
                    detailContainer.style.Hide(false);
                    SetupLeftDetails(property, root);
                    SetupButtons(property, root);
                }

                property.serializedObject.ApplyModifiedProperties();
            });

            root.Q<Label>("PropertyName").text = property.displayName;

            var buildScene = BuildUtils.GetBuildScene(sceneAssetProperty.objectReferenceValue);
            if (buildScene.assetGUID.Empty())
            {
                detailContainer.style.Hide(true);
            }

            SetupLeftDetails(property, root);
            SetupButtons(property, root);
        }
        private void SetupLeftDetails(SerializedProperty property, VisualElement root)
        {
            SerializedProperty sceneAssetProperty = GetSceneAssetProperty(property);
            var buildScene = BuildUtils.GetBuildScene(sceneAssetProperty.objectReferenceValue);

            var readOnly = BuildUtils.IsReadOnly();
            var readOnlyWarning = readOnly ? "\n\nWARNING: Build Settings is not checked out and so cannot be modified." : "";
            VisualElement iconVe = root.Q("Icon");
            Texture2D iconTex;
            string text, tooltip;
            // Missing from build scenes
            if (buildScene.buildIndex == -1)
            {
                iconTex = EditorGUIUtility.FindTexture("d_winbtn_mac_close");
                text = "NOT In Build";
                tooltip = "This scene is NOT in build settings.\nIt will be NOT included in builds.";
            }
            // In build scenes and enabled
            else if (buildScene.scene.enabled)
            {
                iconTex = EditorGUIUtility.FindTexture("d_winbtn_mac_max");
                text = buildScene.buildIndex.ToString();
                tooltip = "This scene is in build settings and ENABLED.\nIt will be included in builds." + readOnlyWarning;
            }
            // In build scenes and disabled
            else
            {
                iconTex = EditorGUIUtility.FindTexture("d_winbtn_mac_min");
                text = buildScene.buildIndex.ToString();
                tooltip = "This scene is in build settings and DISABLED.\nIt will be NOT included in builds.";
            }
            root.Q<Label>("IndexLabel").text = text;
            root.Q("BuildIndexLabel").tooltip = tooltip;

            //iconVe.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            iconVe.style.backgroundImage = iconTex;
            iconVe.style.width = iconTex.width;
        }
        private void SetupButtons(SerializedProperty property, VisualElement root)
        {
            SerializedProperty sceneAssetProperty = GetSceneAssetProperty(property);
            var buildScene = BuildUtils.GetBuildScene(sceneAssetProperty.objectReferenceValue);

            bool readOnly = BuildUtils.IsReadOnly();
            var readOnlyWarning = readOnly ? "\n\nWARNING: Build Settings is not checked out and so cannot be modified." : "";

            Button
                addBtt = root.Q<Button>("AddButton"),
                removeBtt = root.Q<Button>("RemoveButton"),
                disableBtt = root.Q<Button>("DisableButton"),
                settingsBtt = root.Q<Button>("SettingsButton");

            if (buildScene.buildIndex < 0)
            {
                removeBtt.style.Hide(true);
                disableBtt.style.Hide(true);
                settingsBtt.style.Hide(true);
                addBtt.style.Hide(false);
            }
            bool isEnabled = buildScene.scene == null ? false : buildScene.scene.enabled;
            string stateString = isEnabled ? "Disable" : "Enable";
            disableBtt.text = stateString;

            addBtt.clicked += delegate
            {
                addBtt.style.Hide(true);
                removeBtt.style.Hide(false);
                disableBtt.style.Hide(false);
                settingsBtt.style.Hide(false);

                BuildUtils.AddBuildScene(buildScene);
                SetupLeftDetails(property, root);
            };
            removeBtt.clicked += delegate
            {
                removeBtt.style.Hide(true);
                disableBtt.style.Hide(true);
                settingsBtt.style.Hide(true);
                addBtt.style.Hide(false);

                BuildUtils.RemoveBuildScene(buildScene);
                SetupLeftDetails(property, root);
            };
            disableBtt.clicked += delegate
            {
                bool newValueEnabled = buildScene.scene.enabled;
                BuildUtils.SetBuildSceneState(buildScene, !newValueEnabled);
                SetupLeftDetails(property, root);
            };
            settingsBtt.clicked += delegate
            {
                BuildUtils.OpenBuildSettings();
            };
        }

        ///// <summary>
        ///// Drawing the 'SceneReference' property
        ///// </summary>
        //public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        //{
        //    AutoRect rect = new AutoRect(position);

        //    // Move this up
        //    using (new EditorGUI.PropertyScope(position, GUIContent.none, property))
        //    {
        //        // Here we add the foldout using a single line height, the label and change
        //        // the value of property.isExpanded
        //        //property.isExpanded = EditorGUI.Foldout(
        //        //    new Rect(position.x, position.y, position.width, lineHeight), 
        //        //    property.isExpanded, 
        //        //    label);
        //        property.isExpanded = true;
        //        //EditorGUI.LabelField(new Rect(position.x, position.y, position.width, lineHeight), label);
        //        // Now you want to draw the content only if you unfold this property
        //        //if (property.isExpanded)
        //        {
        //            // Optional: Indent the content
        //            //EditorGUI.indentLevel++;
        //            //{

        //            // reduce the height by one line and move the content one line below
        //            //position.height -= lineHeight;
        //            //position.y += lineHeight;

        //            var sceneAssetProperty = GetSceneAssetProperty(property);

        //            // Draw the Box Background
        //            position.height -= FOOTER_HEIGHT;
        //            GUI.Box(EditorGUI.IndentedRect(position), GUIContent.none, EditorStyles.helpBox);
        //            position = boxPadding.Remove(position);
        //            position.height = lineHeight;

        //            // Draw the main Object field
        //            label.tooltip = "The actual Scene Asset reference.\nOn serialize this is also stored as the asset's path.";


        //            var sceneControlID = GUIUtility.GetControlID(FocusType.Passive);
        //            EditorGUI.BeginChangeCheck();
        //            {
        //                // removed the label here since we already have it in the foldout before
        //                sceneAssetProperty.objectReferenceValue = EditorGUI.ObjectField(position, sceneAssetProperty.objectReferenceValue, typeof(SceneAsset), false);
        //            }
        //            var buildScene = BuildUtils.GetBuildScene(sceneAssetProperty.objectReferenceValue);
        //            if (EditorGUI.EndChangeCheck())
        //            {
        //                // If no valid scene asset was selected, reset the stored path accordingly
        //                if (buildScene.scene == null) GetScenePathProperty(property).stringValue = string.Empty;
        //            }

        //            position.y += paddedLine;

        //            if (!buildScene.assetGUID.Empty())
        //            {
        //                // Draw the Build Settings Info of the selected Scene
        //                DrawSceneInfoGUI(position, buildScene, sceneControlID + 1);
        //            }

        //            // Optional: If enabled before reset the indentlevel
        //            //}
        //            //EditorGUI.indentLevel--;
        //        }
        //    }
        //}

        ///// <summary>
        ///// Ensure that what we draw in OnGUI always has the room it needs
        ///// </summary>
        //public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        //{
        //    var sceneAssetProperty = GetSceneAssetProperty(property);
        //    // Add an additional line and check if property.isExpanded
        //    var lines = property.isExpanded ? sceneAssetProperty.objectReferenceValue != null ? 2 : 1 : 1;
        //    // If this oneliner is confusing you - it does the same as
        //    //var line = 3; // Fully expanded and with info
        //    //if(sceneAssetProperty.objectReferenceValue == null) line = 2;
        //    //if(!property.isExpanded) line = 1;

        //    return boxPadding.vertical + lineHeight * lines + PAD_SIZE * (lines - 1) + FOOTER_HEIGHT;
        //}

        ///// <summary>
        ///// Draws info box of the provided scene
        ///// </summary>
        //private static void DrawSceneInfoGUI(Rect position, BuildUtils.BuildScene buildScene, int sceneControlID)
        //{
        //    var readOnly = BuildUtils.IsReadOnly();
        //    var readOnlyWarning = readOnly ? "\n\nWARNING: Build Settings is not checked out and so cannot be modified." : "";

        //    // Label Prefix
        //    var iconContent = new GUIContent();
        //    var labelContent = new GUIContent();

        //    // Missing from build scenes
        //    if (buildScene.buildIndex == -1)
        //    {
        //        iconContent = EditorGUIUtility.IconContent("d_winbtn_mac_close");
        //        labelContent.text = "NOT In Build";
        //        labelContent.tooltip = "This scene is NOT in build settings.\nIt will be NOT included in builds.";
        //    }
        //    // In build scenes and enabled
        //    else if (buildScene.scene.enabled)
        //    {
        //        iconContent = EditorGUIUtility.IconContent("d_winbtn_mac_max");
        //        labelContent.text = "BuildIndex: " + buildScene.buildIndex;
        //        labelContent.tooltip = "This scene is in build settings and ENABLED.\nIt will be included in builds." + readOnlyWarning;
        //    }
        //    // In build scenes and disabled
        //    else
        //    {
        //        iconContent = EditorGUIUtility.IconContent("d_winbtn_mac_min");
        //        labelContent.text = "BuildIndex: " + buildScene.buildIndex;
        //        labelContent.tooltip = "This scene is in build settings and DISABLED.\nIt will be NOT included in builds.";
        //    }

        //    // Left status label
        //    using (new EditorGUI.DisabledScope(readOnly))
        //    {
        //        var labelRect = DrawUtils.GetLabelRect(position);
        //        var iconRect = labelRect;
        //        iconRect.width = iconContent.image.width + PAD_SIZE;
        //        labelRect.width -= iconRect.width;
        //        labelRect.x += iconRect.width;
        //        EditorGUI.PrefixLabel(iconRect, sceneControlID, iconContent);
        //        EditorGUI.PrefixLabel(labelRect, sceneControlID, labelContent);
        //    }

        //    // Right context buttons
        //    var buttonRect = DrawUtils.GetFieldRect(position);
        //    buttonRect.width = (buttonRect.width) / 3;

        //    var tooltipMsg = "";
        //    using (new EditorGUI.DisabledScope(readOnly))
        //    {
        //        // NOT in build settings
        //        if (buildScene.buildIndex == -1)
        //        {
        //            buttonRect.width *= 2;
        //            var addIndex = EditorBuildSettings.scenes.Length;
        //            tooltipMsg = "Add this scene to build settings. It will be appended to the end of the build scenes as buildIndex: " + addIndex + "." + readOnlyWarning;
        //            if (DrawUtils.ButtonHelper(buttonRect, "Add...", "Add (buildIndex " + addIndex + ")", EditorStyles.miniButtonLeft, tooltipMsg))
        //                BuildUtils.AddBuildScene(buildScene);
        //            buttonRect.width /= 2;
        //            buttonRect.x += buttonRect.width;
        //        }
        //        // In build settings
        //        else
        //        {
        //            var isEnabled = buildScene.scene.enabled;
        //            var stateString = isEnabled ? "Disable" : "Enable";
        //            tooltipMsg = stateString + " this scene in build settings.\n" + (isEnabled ? "It will no longer be included in builds" : "It will be included in builds") + "." + readOnlyWarning;

        //            if (DrawUtils.ButtonHelper(buttonRect, stateString, stateString + " In Build", EditorStyles.miniButtonLeft, tooltipMsg))
        //                BuildUtils.SetBuildSceneState(buildScene, !isEnabled);
        //            buttonRect.x += buttonRect.width;

        //            tooltipMsg = "Completely remove this scene from build settings.\nYou will need to add it again for it to be included in builds!" + readOnlyWarning;
        //            if (DrawUtils.ButtonHelper(buttonRect, "Remove...", "Remove from Build", EditorStyles.miniButtonMid, tooltipMsg))
        //                BuildUtils.RemoveBuildScene(buildScene);
        //        }
        //    }

        //    buttonRect.x += buttonRect.width;

        //    tooltipMsg = "Open the 'Build Settings' Window for managing scenes." + readOnlyWarning;
        //    if (DrawUtils.ButtonHelper(buttonRect, "Settings", "Build Settings", EditorStyles.miniButtonRight, tooltipMsg))
        //    {
        //        BuildUtils.OpenBuildSettings();
        //    }

        //}

        private static SerializedProperty GetSceneAssetProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative(sceneAssetPropertyString);
        }

        private static SerializedProperty GetScenePathProperty(SerializedProperty property)
        {
            return property.FindPropertyRelative(scenePathPropertyString);
        }

        private static class DrawUtils
        {
            /// <summary>
            /// Draw a GUI button, choosing between a short and a long button text based on if it fits
            /// </summary>
            public static bool ButtonHelper(Rect position, string msgShort, string msgLong, GUIStyle style, string tooltip = null)
            {
                var content = new GUIContent(msgLong) { tooltip = tooltip };

                var longWidth = style.CalcSize(content).x;
                if (longWidth > position.width) content.text = msgShort;

                return GUI.Button(position, content, style);
            }

            /// <summary>
            /// Given a position rect, get its field portion
            /// </summary>
            public static Rect GetFieldRect(Rect position)
            {
                position.width -= EditorGUIUtility.labelWidth;
                position.x += EditorGUIUtility.labelWidth;
                return position;
            }
            /// <summary>
            /// Given a position rect, get its label portion
            /// </summary>
            public static Rect GetLabelRect(Rect position)
            {
                //position.width = EditorGUIUtility.labelWidth - PAD_SIZE;
                return position;
            }
        }

        /// <summary>
        /// Various BuildSettings interactions
        /// </summary>
        private static class BuildUtils
        {
            // time in seconds that we have to wait before we query again when IsReadOnly() is called.
            public static float minCheckWait = 3;

            private static float lastTimeChecked;
            private static bool cachedReadonlyVal = true;

            /// <summary>
            /// A small container for tracking scene data BuildSettings
            /// </summary>
            public struct BuildScene
            {
                public int buildIndex;
                public GUID assetGUID;
                public string assetPath;
                public EditorBuildSettingsScene scene;
            }

            /// <summary>
            /// Check if the build settings asset is readonly.
            /// Caches value and only queries state a max of every 'minCheckWait' seconds.
            /// </summary>
            public static bool IsReadOnly()
            {
                var curTime = Time.realtimeSinceStartup;
                var timeSinceLastCheck = curTime - lastTimeChecked;

                if (!(timeSinceLastCheck > minCheckWait)) return cachedReadonlyVal;

                lastTimeChecked = curTime;
                cachedReadonlyVal = QueryBuildSettingsStatus();

                return cachedReadonlyVal;
            }

            /// <summary>
            /// A blocking call to the Version Control system to see if the build settings asset is readonly.
            /// Use BuildSettingsIsReadOnly for version that caches the value for better responsivenes.
            /// </summary>
            private static bool QueryBuildSettingsStatus()
            {
                // If no version control provider, assume not readonly
                if (!Provider.enabled) return false;

                // If we cannot checkout, then assume we are not readonly
                if (!Provider.hasCheckoutSupport) return false;

                //// If offline (and are using a version control provider that requires checkout) we cannot edit.
                //if (UnityEditor.VersionControl.Provider.onlineState == UnityEditor.VersionControl.OnlineState.Offline)
                //    return true;

                // Try to get status for file
                var status = Provider.Status("ProjectSettings/EditorBuildSettings.asset", false);
                status.Wait();

                // If no status listed we can edit
                if (status.assetList == null || status.assetList.Count != 1) return true;

                // If is checked out, we can edit
                return !status.assetList[0].IsState(Asset.States.CheckedOutLocal);
            }

            /// <summary>
            /// For a given Scene Asset object reference, extract its build settings data, including buildIndex.
            /// </summary>
            public static BuildScene GetBuildScene(Object sceneObject)
            {
                var entry = new BuildScene
                {
                    buildIndex = -1,
                    assetGUID = new GUID(string.Empty)
                };

                if (sceneObject as SceneAsset == null) return entry;

                entry.assetPath = AssetDatabase.GetAssetPath(sceneObject);
                entry.assetGUID = new GUID(AssetDatabase.AssetPathToGUID(entry.assetPath));

                var scenes = EditorBuildSettings.scenes;
                for (var index = 0; index < scenes.Length; ++index)
                {
                    if (!entry.assetGUID.Equals(scenes[index].guid)) continue;

                    entry.scene = scenes[index];
                    entry.buildIndex = index;
                    return entry;
                }

                return entry;
            }

            /// <summary>
            /// Enable/Disable a given scene in the buildSettings
            /// </summary>
            public static void SetBuildSceneState(BuildScene buildScene, bool enabled)
            {
                var modified = false;
                var scenesToModify = EditorBuildSettings.scenes;
                foreach (var curScene in scenesToModify.Where(curScene => curScene.guid.Equals(buildScene.assetGUID)))
                {
                    curScene.enabled = enabled;
                    modified = true;
                    break;
                }
                if (modified) EditorBuildSettings.scenes = scenesToModify;
            }

            /// <summary>
            /// Display Dialog to add a scene to build settings
            /// </summary>
            public static void AddBuildScene(BuildScene buildScene, bool force = false, bool enabled = true)
            {
                if (force == false)
                {
                    var selection = EditorUtility.DisplayDialogComplex(
                        "Add Scene To Build",
                        "You are about to add scene at " + buildScene.assetPath + " To the Build Settings.",
                        "Add as Enabled",       // option 0
                        "Add as Disabled",      // option 1
                        "Cancel (do nothing)"); // option 2

                    switch (selection)
                    {
                        case 0: // enabled
                            enabled = true;
                            break;
                        case 1: // disabled
                            enabled = false;
                            break;
                        default:
                            //case 2: // cancel
                            return;
                    }
                }

                var newScene = new EditorBuildSettingsScene(buildScene.assetGUID, enabled);
                var tempScenes = EditorBuildSettings.scenes.ToList();
                tempScenes.Add(newScene);
                EditorBuildSettings.scenes = tempScenes.ToArray();
            }

            /// <summary>
            /// Display Dialog to remove a scene from build settings (or just disable it)
            /// </summary>
            public static void RemoveBuildScene(BuildScene buildScene, bool force = false)
            {
                var onlyDisable = false;
                if (force == false)
                {
                    var selection = -1;

                    var title = "Remove Scene From Build";
                    var details = $"You are about to remove the following scene from build settings:\n    {buildScene.assetPath}\n    buildIndex: {buildScene.buildIndex}\n\nThis will modify build settings, but the scene asset will remain untouched.";
                    var confirm = "Remove From Build";
                    var alt = "Just Disable";
                    var cancel = "Cancel (do nothing)";

                    if (buildScene.scene.enabled)
                    {
                        details += "\n\nIf you want, you can also just disable it instead.";
                        selection = EditorUtility.DisplayDialogComplex(title, details, confirm, alt, cancel);
                    }
                    else
                    {
                        selection = EditorUtility.DisplayDialog(title, details, confirm, cancel) ? 0 : 2;
                    }

                    switch (selection)
                    {
                        case 0: // remove
                            break;
                        case 1: // disable
                            onlyDisable = true;
                            break;
                        default:
                            //case 2: // cancel
                            return;
                    }
                }

                // User chose to not remove, only disable the scene
                if (onlyDisable)
                {
                    SetBuildSceneState(buildScene, false);
                }
                // User chose to fully remove the scene from build settings
                else
                {
                    var tempScenes = EditorBuildSettings.scenes.ToList();
                    tempScenes.RemoveAll(scene => scene.guid.Equals(buildScene.assetGUID));
                    EditorBuildSettings.scenes = tempScenes.ToArray();
                }
            }

            /// <summary>
            /// Open the default Unity Build Settings window
            /// </summary>
            public static void OpenBuildSettings()
            {
                EditorWindow.GetWindow(typeof(BuildPlayerWindow));
            }
        }
    }
}

#endif