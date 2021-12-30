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

    internal sealed class LayerNTagSetups
    {
        SerializedObject m_TagManagerObject;
        SerializedProperty m_TagProperty, m_LayerProperty;

        static string[] c_RequireTags = new string[] { };
        static string[] c_RequireLayers = new string[] { "Terrain", "FloorProjection" };

        List<string> m_MissingTags, m_MissingLayers;

        private bool m_OpenTagManager = false;

        public LayerNTagSetups()
        {
            UnityEngine.Object tagManagerObject = AssetDatabase.LoadMainAssetAtPath("ProjectSettings/TagManager.asset");
            m_TagManagerObject = new SerializedObject(tagManagerObject);
            m_TagProperty = m_TagManagerObject.FindProperty("tags");
            m_LayerProperty = m_TagManagerObject.FindProperty("layers");

            m_MissingTags = new List<string>(c_RequireTags);
            for (int i = 0; i < m_TagProperty.arraySize; i++)
            {
                string value = m_TagProperty.GetArrayElementAtIndex(i).stringValue;
                if (string.IsNullOrEmpty(value)) continue;

                m_MissingTags.Remove(value);
            }

            m_MissingLayers = new List<string>(c_RequireLayers);
            for (int i = 0; i < m_LayerProperty.arraySize; i++)
            {
                string value = m_LayerProperty.GetArrayElementAtIndex(i).stringValue;
                if (string.IsNullOrEmpty(value)) continue;

                m_MissingLayers.Remove(value);
            }

            if (m_MissingTags.Count > 0 || m_MissingLayers.Count > 0)
            {
                m_OpenTagManager = true;
            }
        }

        private bool TagManagerPredicate()
        {
            if (m_MissingTags.Count > 0 || m_MissingLayers.Count > 0) return false;
            return true;
        }
        private void DrawTagManager()
        {
            m_OpenTagManager = EditorUtilities.Foldout(m_OpenTagManager, "Tag Manager");
            if (!m_OpenTagManager) return;

            EditorGUI.indentLevel++;

            EditorUtilities.StringRich("Tags", 13);
            if (m_MissingTags.Count > 0)
            {
                EditorGUILayout.HelpBox($"Number({m_MissingTags.Count}) of Tags are missing", MessageType.Error);

                for (int i = m_MissingTags.Count - 1; i >= 0; i--)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.TextField(m_MissingTags[i]);
                    if (GUILayout.Button("Add", GUILayout.Width(100)))
                    {
                        InsertTag(m_MissingTags[i]);
                        m_MissingTags.RemoveAt(i);
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            else EditorGUILayout.HelpBox("Nominal", MessageType.Info);

            EditorUtilities.Line();

            EditorUtilities.StringRich("Layers", 13);
            if (m_MissingLayers.Count > 0)
            {
                EditorGUILayout.HelpBox($"Number({m_MissingLayers.Count}) of Layers are missing", MessageType.Error);

                for (int i = m_MissingLayers.Count - 1; i >= 0; i--)
                {
                    EditorGUILayout.BeginHorizontal();

                    EditorGUILayout.TextField(m_MissingLayers[i]);
                    if (GUILayout.Button("Add", GUILayout.Width(100)))
                    {
                        if (!InsertLayer(m_MissingLayers[i]))
                        {
                            PointCore.LogError(PointCore.LogChannel.Editor,
                                $"Could not add layer {m_MissingLayers[i]} because layer is full.");
                        }
                        else
                        {
                            m_MissingLayers.RemoveAt(i);
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }
            }
            else EditorGUILayout.HelpBox("Nominal", MessageType.Info);

            EditorGUI.indentLevel--;
        }

        private void InsertTag(string tag)
        {
            for (int i = 0; i < m_TagProperty.arraySize; i++)
            {
                string value = m_TagProperty.GetArrayElementAtIndex(i).stringValue;
                if (string.IsNullOrEmpty(value))
                {
                    m_TagProperty.GetArrayElementAtIndex(i).stringValue = tag;
                    m_TagManagerObject.ApplyModifiedProperties();
                    return;
                }
            }

            m_TagProperty.InsertArrayElementAtIndex(m_TagProperty.arraySize);
            m_TagProperty.GetArrayElementAtIndex(m_TagProperty.arraySize - 1).stringValue = tag;
            m_TagManagerObject.ApplyModifiedProperties();
        }
        private bool InsertLayer(string layer)
        {
            for (int i = 0; i < m_LayerProperty.arraySize; i++)
            {
                string value = m_LayerProperty.GetArrayElementAtIndex(i).stringValue;
                if (string.IsNullOrEmpty(value))
                {
                    m_LayerProperty.GetArrayElementAtIndex(i).stringValue = layer;
                    m_TagManagerObject.ApplyModifiedProperties();
                    return true;
                }
            }

            return false;
        }
    }
    internal sealed class UnityAudioSetups
    {
        SerializedObject m_UnityAudioManager;
        SerializedProperty
            m_UnityAudioDisableAudio,

            m_UnityAudioGlobalVolume,
            m_UnityAudioRolloffScale,
            m_UnityAudioDopplerFactor,

            m_UnityAudioRealVoiceCount,
            m_UnityAudioVirtualVoiceCount,
            m_UnityAudioDefaultSpeakerMode;

        private bool
                m_OpenUnityAudio = false, m_IsUnityAudioModified = false;

        public UnityAudioSetups()
        {
            var audioManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/AudioManager.asset")[0];
            m_UnityAudioManager = new SerializedObject(audioManager);

            m_UnityAudioDisableAudio = m_UnityAudioManager.FindProperty("m_DisableAudio");

            m_UnityAudioGlobalVolume = m_UnityAudioManager.FindProperty("m_Volume");
            m_UnityAudioRolloffScale = m_UnityAudioManager.FindProperty("Rolloff Scale");
            m_UnityAudioDopplerFactor = m_UnityAudioManager.FindProperty("Doppler Factor");

            m_UnityAudioRealVoiceCount = m_UnityAudioManager.FindProperty("m_RealVoiceCount");
            m_UnityAudioVirtualVoiceCount = m_UnityAudioManager.FindProperty("m_VirtualVoiceCount");
            m_UnityAudioDefaultSpeakerMode = m_UnityAudioManager.FindProperty("Default Speaker Mode");
        }

        private void DrawUnityAudio()
        {
            m_OpenUnityAudio = EditorUtilities.Foldout(m_OpenUnityAudio, "Unity Audio");
            if (!m_OpenUnityAudio) return;

            EditorGUI.indentLevel++;
            using (new EditorUtilities.BoxBlock(Color.white))
            {
                using (var check = new EditorGUI.ChangeCheckScope())
                {
                    EditorGUILayout.PropertyField(m_UnityAudioDisableAudio);

                    if (check.changed)
                    {
                        m_UnityAudioManager.ApplyModifiedProperties();
                        m_UnityAudioManager.Update();
                    }
                }

                EditorUtilities.Line();

                if (m_UnityAudioDisableAudio.boolValue)
                {
                    EditorGUILayout.HelpBox("Unity Audio has been disabled", MessageType.Info);
                    return;
                }

                using (new EditorGUI.DisabledGroupScope(!m_IsUnityAudioModified))
                using (new EditorGUILayout.HorizontalScope())
                {
                    if (m_IsUnityAudioModified)
                    {
                        EditorGUILayout.LabelField("Modified");
                    }

                    GUILayout.FlexibleSpace();
                    if (GUILayout.Button("Apply", GUILayout.Width(100)))
                    {
                        m_UnityAudioManager.ApplyModifiedProperties();
                        m_UnityAudioManager.Update();

                        m_IsUnityAudioModified = false;
                    }
                }

                EditorGUI.BeginChangeCheck();

                m_UnityAudioGlobalVolume.floatValue
                    = EditorGUILayout.Slider("Global Volume", m_UnityAudioGlobalVolume.floatValue, 0, 1);
                m_UnityAudioRolloffScale.floatValue
                    = EditorGUILayout.Slider("Volume Rolloff Scale", m_UnityAudioRolloffScale.floatValue, 0, 1);
                m_UnityAudioDopplerFactor.floatValue
                    = EditorGUILayout.Slider("Doppler Factor", m_UnityAudioDopplerFactor.floatValue, 0, 1);

                EditorUtilities.Line();

                m_UnityAudioRealVoiceCount.intValue
                    = EditorGUILayout.IntField("Max Real Voices", m_UnityAudioRealVoiceCount.intValue);

                m_UnityAudioVirtualVoiceCount.intValue
                    = EditorGUILayout.IntField("Max Virtual Voices", m_UnityAudioVirtualVoiceCount.intValue);

                m_UnityAudioDefaultSpeakerMode.intValue =
                    (int)(AudioSpeakerMode)EditorGUILayout.EnumPopup("Default Speaker Mode", (AudioSpeakerMode)m_UnityAudioDefaultSpeakerMode.intValue);

                if (EditorGUI.EndChangeCheck())
                {
                    m_IsUnityAudioModified = true;
                }
            }
            EditorGUI.indentLevel--;
        }
    }
}
