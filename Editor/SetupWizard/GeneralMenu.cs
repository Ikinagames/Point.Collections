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

using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Point.Collections.Editor
{
    internal sealed class GeneralMenu : SetupWizardMenuItem
    {
        public override string Name => "General";
        public override int Order => -9999;

        LayerNTagSetups layerNTagSetups = new LayerNTagSetups();
        UnityAudioSetups unityAudioSetups = new UnityAudioSetups();

        public override void OnGUI()
        {
            layerNTagSetups.DrawTagManager();

            EditorUtilities.Line();

            unityAudioSetups.DrawUnityAudio();
        }
        public override bool Predicate() 
            => layerNTagSetups.TagManagerPredicate();

        internal sealed class LayerNTagSetups
        {
            SerializedObject m_TagManagerObject;
            SerializedProperty m_TagProperty, m_LayerProperty;

            static string[] c_RequireTags = new string[] { };
            static string[] c_RequireLayers = new string[] {  };

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

            public bool TagManagerPredicate()
            {
                if (m_MissingTags.Count > 0 || m_MissingLayers.Count > 0) return false;
                return true;
            }
            public void DrawTagManager()
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
                                PointHelper.LogError(LogChannel.Editor,
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

            public void DrawUnityAudio()
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
}

#endif