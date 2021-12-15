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

using Point.Collections.ResourceControl;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Point.Collections.ResourceControl.Editor
{
    public static class ResourceAddressesEditorUtils
    {
        private static SerializedObject s_Target;
        public static SerializedObject Target
        {
            get
            {
                if (s_Target == null)
                {
                    s_Target = new SerializedObject(ResourceAddresses.Instance);
                }

                return s_Target;
            }
        }

        #region AssetID

        private static FieldInfo s_AssetIDsField;
        private static FieldInfo AssetIDsField
        {
            get
            {
                if (s_AssetIDsField == null)
                {
                    s_AssetIDsField
                        = TypeHelper.TypeOf<ResourceAddresses>.GetFieldInfo("m_AssetIDs");
                }

                return s_AssetIDsField;
            }
        }
        private static AssetID[] AssetIDs
        {
            get
            {
                return (AssetID[])AssetIDsField.GetValue(ResourceAddresses.Instance);
            }
            set
            {
                AssetIDsField.SetValue(ResourceAddresses.Instance, value);
                EditorUtility.SetDirty(ResourceAddresses.Instance);
                AssetDatabase.SaveAssetIfDirty(ResourceAddresses.Instance);
            }
        }
        private static Dictionary<AssetID, int> s_RegisteredAssets;
        private static Dictionary<AssetID, int> RegisteredAssets
        {
            get
            {
                if (s_RegisteredAssets == null)
                {
                    s_RegisteredAssets = new Dictionary<AssetID, int>();
                    AssetID[] ids = AssetIDs;
                    for (int i = 0; i < ids.Length; i++)
                    {
                        s_RegisteredAssets.Add(ids[i], i);
                    }
                }

                return s_RegisteredAssets;
            }
        }
        public static void Reload()
        {
            s_RegisteredAssets = new Dictionary<AssetID, int>();
            AssetID[] ids = AssetIDs;
            for (int i = 0; i < ids.Length; i++)
            {
                s_RegisteredAssets.Add(ids[i], i);
            }
        }

        public static bool IsTrackedAsset(this ResourceAddresses _, in string assetPath)
        {
            AssetID id = new AssetID(new Hash(assetPath));
            return RegisteredAssets.ContainsKey(id);
        }
        public static AssetID UpdateAsset(this ResourceAddresses _, in string prevAssetPath, in string targetAssetPath)
        {
            AssetID 
                prev = new AssetID(new Hash(prevAssetPath)),
                target = new AssetID(new Hash(targetAssetPath));

            if (!RegisteredAssets.ContainsKey(prev))
            {
                throw new System.Exception("1");
            }
            else if (RegisteredAssets.ContainsKey(target))
            {
                throw new System.Exception("2");
            }

            int index = RegisteredAssets[prev];
            RegisteredAssets.Remove(prev);
            AssetID[] ids = AssetIDs;

            ids[index] = target;
            RegisteredAssets.Add(target, index);
            AssetIDs = ids;

            Point.Log(Point.LogChannel.Editor,
                $"Asset({target}) hash has been updated. (prev {prev})");

            return target;
        }
        public static AssetID RegisterAsset(this ResourceAddresses _, in string assetPath)
        {
            AssetID id = new AssetID(new Hash(assetPath));
            if (RegisteredAssets.ContainsKey(id))
            {
                throw new System.Exception();
            }

            List<AssetID> temp = AssetIDs.ToList();
            int index = temp.Count;
            temp.Add(id);
            AssetIDs = temp.ToArray();

            RegisteredAssets.Add(id, index);

            Point.Log(Point.LogChannel.Editor,
                $"Asset({id.Hash}) has been registered and tracked by resource manager.");

            return id;
        }
        public static void RemoveAssets(this ResourceAddresses _, in string[] assetPaths)
        {
            AssetID[] ids = new AssetID[assetPaths.Length];
            int[] indics = new int[assetPaths.Length];
            for (int i = 0; i < assetPaths.Length; i++)
            {
                ids[i] = new AssetID(new Hash(assetPaths[i]));
                if (!RegisteredAssets.ContainsKey(ids[i]))
                {
                    throw new System.Exception();
                }

                indics[i] = RegisteredAssets[ids[i]];
                RegisteredAssets.Remove(ids[i]);
            }

            List<AssetID> temp = AssetIDs.ToList();
            for (int i = 0; i < indics.Length; i++)
            {
                temp.RemoveAt(indics[i]);
            }
            AssetIDs = temp.ToArray();

            Point.Log(Point.LogChannel.Editor,
                $"Multiple assets({ids[0].Hash}, and {ids.Length - 1} more assets) has been removed and no longer tracked by resource manager.");
        }
        public static void RemoveAsset(this ResourceAddresses _, in string assetPath)
        {
            AssetID id = new AssetID(new Hash(assetPath));
            if (!RegisteredAssets.ContainsKey(id))
            {
                throw new System.Exception();
            }

            List<AssetID> temp = AssetIDs.ToList();
            int index = RegisteredAssets[id];

            temp.RemoveAt(index);
            AssetIDs = temp.ToArray();

            RegisteredAssets.Remove(id);

            Point.Log(Point.LogChannel.Editor,
                $"Asset({id.Hash}) has been removed and no longer tracked by resource manager.");
        }

        #endregion
    }
}
