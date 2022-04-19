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

#if UNITY_2020_1_OR_NEWER
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using Point.Collections.Graphs;
using System;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Point.Collections.Editor
{
    public static class VisualGraphAssetCallbacks
    {
        [MenuItem("Assets/Create/Point/Graphs/Visual Graph", false, 0)]
        public static void CreateGraphProcessor()
        {
            CreateGraphAsset<VisualGraph>(true);
        }
        [MenuItem("Assets/Create/Point/Graphs/Visual Logic Graph", false, 1)]
        public static void CreateLogicGraphProcessor()
        {
            CreateGraphAsset<VisualLogicGraph>(true);
        }
        
        public static VisualGraph CreateGraphAsset(Type type, bool save)
        {
            if (type.IsAbstract || type.IsInterface)
            {
                PointHelper.LogError(Channel.Editor,
                    $"Cannot create asset({TypeHelper.ToString(type)}) is abstract.");
                return null;
            }

            VisualGraph graph = (VisualGraph)ScriptableObject.CreateInstance(type);
            if (save)
            {
                ProjectWindowUtil.CreateAsset(graph, $"{TypeHelper.ToString(type)}.asset");
            }

            if (graph is VisualLogicGraph logicGraph)
            {
                EntryNode entryNode = new EntryNode();
                entryNode.OnNodeCreated();

                logicGraph.AddNode(entryNode);
                logicGraph.AddExposedParameter("This", TypeHelper.TypeOf<ObjectExposedParameter>.Type);
            }

            return graph;
        }
        public static bool OpenGraphAsset(VisualGraph graph)
        {
            if (graph == null) return false;

            else if (graph is VisualLogicGraph)
            {
                EditorWindow.GetWindow<VisualLogicGraphWindow>().InitializeGraph(graph);
                return true;
            }

            else if (graph is VisualGraph)
            {
                EditorWindow.GetWindow<VisualGraphWindow>().InitializeGraph(graph);
                return true;
            }

            return false;
        }

        public static T CreateGraphAsset<T>(bool save) where T : VisualGraph => (T)CreateGraphAsset(TypeHelper.TypeOf<T>.Type, save);

        [OnOpenAsset(0)]
        public static bool OnBaseGraphOpened(int instanceID, int line)
        {
            var asset = EditorUtility.InstanceIDToObject(instanceID) as VisualGraph;
            if (asset == null) return false;

            return OpenGraphAsset(asset);
        }
    }
}

#endif