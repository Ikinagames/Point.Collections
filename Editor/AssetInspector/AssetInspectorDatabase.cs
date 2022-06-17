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


using Point.Collections.Threading;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Point.Collections.Editor
{
    internal sealed class AssetInspectorDatabase : EditorStaticScriptableObject<AssetInspectorDatabase>
    {
        private AssetInfo[] m_Assets = Array.Empty<AssetInfo>();

        AtomicSafeInteger m_CurrentIndex = 0;
        private AtomicSafeBoolen m_Builded = false;

        private AtomicOperator m_Op = new AtomicOperator();
        private ConcurrentDictionary<string, AssetInfo> m_Database = new ConcurrentDictionary<string, AssetInfo>();

        public AssetInfo this[string key]
        {
            get
            {
                AssetInfo result;
                m_Op.Enter();
                {
                    m_Database.TryGetValue(key, out result);
                }
                m_Op.Exit();

                return result;
            }
        }
        public static bool Builded => Instance.m_Builded;
        
        //private Task<Dictionary<string, AssetInfo>> BuildDatabaseAsync()
        //{
        //    "start task".ToLog();
        //    //SynchronizationContext.Current.pos

        //    Task<Dictionary<string, AssetInfo>> task 
        //        = new Task<Dictionary<string, AssetInfo>>(BuildDatabase);
        //    task.Start();

        //    return task;
        //}
        //private Task<Dictionary<string, AssetInfo>> RebuildDatabaseAsync()
        //{
        //    "start task".ToLog();
        //    //var result = await Task.Run(RebuildDatabase);

            

        //    Task<Dictionary<string, AssetInfo>> task
        //        = new Task<Dictionary<string, AssetInfo>>(RebuildDatabase);
        //    task.Start();

        //    return task;
        //}
        public IEnumerator RebuildDatabase(BackgroundTask task)
        {
            m_Builded.Value = false;

            m_Op.Enter();
            m_Assets = null;
            m_Database.Clear();
            m_Op.Exit();

            return BuildDatabase(task);
        }

        public IEnumerator BuildDatabase(BackgroundTask task)
        {
            m_Op.Enter();

            //"Build Start".ToLog();
            //"Gather All Assets".ToLog();
            Progress.Report(task, 0, "Gather All Assets");
            float totalProgress = 0;
            yield return null;
            {
                if (m_Assets == null || m_Assets.Length == 0)
                {
                    totalProgress = .25f;
                    string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
                    m_Assets = new AssetInfo[allAssetPaths.Length];

                    int gatherAllAssetID = Progress.Start("Gather All Assets", parentId: task);

                    for (int i = 0; i < allAssetPaths.Length; i++)
                    {
                        m_Assets[i] = new AssetInfo(allAssetPaths[i]);

                        float progress = i / (float)allAssetPaths.Length;
                        Progress.Report(gatherAllAssetID, progress, allAssetPaths[i]);
                        Progress.Report(task, totalProgress * progress, "Gather All Assets");

                        yield return null;
                    }
                    Progress.Remove(gatherAllAssetID);
                }
            }

            Progress.Report(task, .25f, "Update Hashmap");
            totalProgress = .25f;
            yield return null;
            {
                int updateHashmapID = Progress.Start("Update Hashmap", parentId: task);
                //$"Update Hashmap 0 / {m_Assets.Length}".ToLog();

                m_CurrentIndex = 0;
                var result = Parallel.For(0, m_Assets.Length, i =>
                {
                    m_Database[m_Assets[i].Asset.AssetPath] = m_Assets[i];
                    m_CurrentIndex.Increment();
                });
                while (!result.IsCompleted || m_CurrentIndex.Value != m_Assets.Length)
                {
                    float progress = m_CurrentIndex / (float)m_Assets.Length;
                    Progress.Report(updateHashmapID, progress, $"{m_CurrentIndex.Value} / {m_Assets.Length}");
                    Progress.Report(task, totalProgress + (.25f * progress), "Update Hashmap");

                    yield return null;
                }

                Progress.Remove(updateHashmapID);
                yield return null;
            }

            Progress.Report(task, .5f, "Build Hashmap");
            totalProgress = .5f;
            yield return null;
            {
                int buildHashmapID = Progress.Start("Build Hashmap", parentId: task);
                //$"Build Hashmap 0 / {m_Assets.Length}".ToLog();

                m_CurrentIndex = 0;
                var result = Parallel.For(0, m_Assets.Length, i =>
                {
                    m_Assets[i].BuildReferenceSet(m_Database);
                    m_CurrentIndex.Increment();
                });
                while (!result.IsCompleted || m_CurrentIndex.Value != m_Assets.Length)
                {
                    float progress = m_CurrentIndex / (float)m_Assets.Length;
                    Progress.Report(buildHashmapID, progress, $"{m_CurrentIndex.Value} / {m_Assets.Length}");
                    Progress.Report(task, totalProgress + (.5f * progress), "Build Hashmap");

                    yield return null;
                }

                Progress.Remove(buildHashmapID);
                yield return null;
            }

            m_Op.Exit();

            m_Builded.Value = true;

            "Build Finished".ToLog();
            //EditorUtility.SetDirty(this);
        }

        public static BackgroundTask Build()
        {
            IEnumerator task;
            var temp = new BackgroundTask("Build AssetDatabase");
            if (Builded)
            {
                //"rebuild btt".ToLog();
                task = Instance.RebuildDatabase(temp);
            }
            else
            {
                //"build btt".ToLog();
                task = Instance.BuildDatabase(temp);
            }

            temp.StartTask(task);
            return temp;
        }
        public static void Add(string path)
        {
            Instance.m_Op.Enter();

            Instance.m_Database[path] = new AssetInfo(path);
            Instance.m_Database[path].BuildReferenceSet(Instance.m_Database);

            Instance.m_Op.Exit();
        }
        public static void Remove(string path)
        {
            Instance.m_Op.Enter();

            if (Instance.m_Database.ContainsKey(path))
            {
                Instance.m_Database[path].RemoveReferenceSet(Instance.m_Database);
                Instance.m_Database.TryRemove(path, out _);
            }

            Instance.m_Op.Exit();
        }
    }
}

#endif