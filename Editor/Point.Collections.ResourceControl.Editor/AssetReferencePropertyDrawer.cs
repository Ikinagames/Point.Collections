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

using Point.Collections.Editor;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Point.Collections.ResourceControl.Editor
{
    [CustomPropertyDrawer(typeof(AssetReference))]
    internal sealed class AssetReferencePropertyDrawer : PropertyDrawerUXML<AssetReference>
    {
        private sealed class AssetReferenceVE : VisualElement
        {
            private UnityEngine.Object m_Object;
            private string m_KeyPath, m_SubAssetNamePath;
            private ObjectField m_ObjectField;

            public AssetReferenceVE(SerializedProperty property)
            {
                m_Object = property.serializedObject.targetObject;
                SerializedProperty
                    keyProp = property.FindPropertyRelative("m_Key"),
                    subProp = property.FindPropertyRelative("m_SubAssetName");

                m_ObjectField = new ObjectField();
                {
                    m_ObjectField.label = property.displayName;
                    m_ObjectField.objectType = TypeHelper.TypeOf<UnityEngine.Object>.Type;

                    string key = SerializedPropertyHelper.ReadFixedString128Bytes(keyProp).ToString();
                    string subAssetName = SerializedPropertyHelper.ReadFixedString128Bytes(subProp).ToString();

                    if (subAssetName.IsNullOrEmpty())
                    {
                        m_ObjectField.value = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(key);
                    }
                    else
                    {
                        var found = AssetDatabase.LoadAllAssetsAtPath(key).Where(t => t.name.Equals(subAssetName));
                        if (found.Any())
                        {
                            m_ObjectField.value = found.First();
                        }
                    }

                    m_ObjectField.RegisterValueChangedCallback(OnValueChanged);
                }
                Add(m_ObjectField);

                m_KeyPath = keyProp.propertyPath;
                m_SubAssetNamePath = subProp.propertyPath;
            }
            private void OnValueChanged(ChangeEvent<UnityEngine.Object> ev)
            {
                UnityEngine.Object obj = ev.newValue;
                if (obj != null && AssetDatabase.IsSubAsset(obj))
                {
                    string mainPath = AssetDatabase.GetAssetPath(obj);
                    string subAssetName = obj.name;

                    Update(mainPath, subAssetName);
                }
                else
                {
                    Update(AssetDatabase.GetAssetPath(obj), string.Empty);
                }
            }

            private void Update(string key, string subAssetName)
            {
                if (m_Object == null) return;

                using (SerializedObject obj = new SerializedObject(m_Object))
                {
                    SerializedPropertyHelper.SetFixedString128Bytes(obj.FindProperty(m_KeyPath), key);
                    SerializedPropertyHelper.SetFixedString128Bytes(obj.FindProperty(m_SubAssetNamePath), subAssetName);

                    obj.ApplyModifiedProperties();
                }
            }
        }

        protected override VisualElement CreateVisualElement(SerializedProperty property)
        {
            AssetReferenceVE root = new AssetReferenceVE(property);

            

            //root.Add(CoreGUI.VisualElement.PropertyField(property.FindPropertyRelative("m_Key")));
            //root.Add(CoreGUI.VisualElement.PropertyField(property.FindPropertyRelative("m_SubAssetName")));

            return root;
        }
    }
}

#endif