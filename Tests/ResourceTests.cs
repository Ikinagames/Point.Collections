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

#if UNITY_2020
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

using NUnit.Framework;
using Point.Collections.ResourceControl;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.TestTools;

namespace Point.Collections.Tests
{
    public class ResourceTests
    {

        //[UnitySetUp]
        //public IEnumerator SetUp()
        //{
        //    yield return new EnterPlayMode();
        //}

        [Test]
        public void AssetBundleRegisterWithLoadedTest()
        {
            var names = ResourceAddresses.Instance.StreamingAssetBundles;
            $"loading bundles {names.Count}".ToLog();
            for (int i = 0; i < names.Count; i++)
            {
                AssetBundle bundle = AssetBundle.LoadFromFile(names[i].AssetPath);
                Assert.NotNull(bundle);

                AssetBundleInfo info = ResourceManager.RegisterAssetBundle(bundle);

                Assert.IsTrue(info.IsLoaded);
                Assert.NotNull(info.AssetBundle);

                info.Unload(true);

                Assert.IsFalse(info.IsLoaded);
                Assert.Null(info.AssetBundle);

                ResourceManager.UnregisterAssetBundle(info);
            }
        }
        [Test]
        public void AssetBundleRegisterWithoutLoadedTestA()
        {
            var names = ResourceAddresses.Instance.StreamingAssetBundles;
            $"loading bundles {names.Count}".ToLog();
            for (int i = 0; i < names.Count; i++)
            {
                AssetBundleInfo info = ResourceManager.RegisterAssetBundlePath(names[i].AssetPath);
                info.Load();

                Assert.IsTrue(info.IsLoaded);
                Assert.NotNull(info.AssetBundle);

                info.Unload(true);

                Assert.IsFalse(info.IsLoaded);
                Assert.Null(info.AssetBundle);

                ResourceManager.UnregisterAssetBundle(info);
            }
        }
        [UnityTest]
        public IEnumerator AssetBundleLoadAsyncTest()
        {
            var names = ResourceAddresses.Instance.StreamingAssetBundles;
            $"loading bundles {names.Count}".ToLog();
            for (int i = 0; i < names.Count; i++)
            {
                AssetBundleInfo info = ResourceManager.RegisterAssetBundlePath(names[i].AssetPath);
                info.LoadAsync();

                yield return new WaitUntil(() => info.IsLoaded);

                Assert.IsTrue(info.IsLoaded);
                Assert.NotNull(info.AssetBundle);

                info.Unload(true);

                Assert.IsFalse(info.IsLoaded);
                Assert.Null(info.AssetBundle);

                ResourceManager.UnregisterAssetBundle(info);
            }

            yield return null;
        }

        [Test]
        public void AssetBundleLoadAssetsTest()
        {
            var names = ResourceAddresses.Instance.StreamingAssetBundles;
            for (int i = 0; i < names.Count; i++)
            {
                AssetBundleInfo info = ResourceManager.RegisterAssetBundlePath(names[i].AssetPath);
                info.Load();

                string[] assetNames = info.GetAllAssetNames();
                foreach (var item in assetNames)
                {
                    Debug.Log(item);

                    var obj = info.LoadAsset(item);
                    Assert.NotNull(obj.Asset);

                    if (obj.Asset is AudioClip audioClip)
                    {
                        $"{audioClip.loadState}".ToLog();
                        audioClip.LoadAudioData();
                        $"{audioClip.loadState}".ToLog();
                    }

                    obj.Reserve();
                }

                Assert.IsTrue(info.IsLoaded);
                Assert.NotNull(info.AssetBundle);

                info.Unload(true);

                Assert.IsFalse(info.IsLoaded);
                Assert.Null(info.AssetBundle);

                ResourceManager.UnregisterAssetBundle(info);
            }
        }

        [Test]
        public void AssetBundleCheckSumTest()
        {
            var names = ResourceAddresses.Instance.StreamingAssetBundles;
            for (int i = 0; i < names.Count; i++)
            {
                AssetBundleInfo info = ResourceManager.RegisterAssetBundlePath(names[i].AssetPath);
                info.Load();

                string[] assetNames = info.GetAllAssetNames();
                foreach (var item in assetNames)
                {
                    Debug.Log(item);

                    var obj = info.LoadAsset(item);
                    Assert.NotNull(obj.Asset);
                }

                Assert.IsTrue(info.IsLoaded);
                Assert.NotNull(info.AssetBundle);

                info.Unload(true);
                LogAssert.Expect(LogType.Error, new Regex(@""));

                ResourceManager.UnregisterAssetBundle(info);
            }
        }

        [Test]
        public void AssetBundleUnregisterErrorTest()
        {
            var names = ResourceAddresses.Instance.StreamingAssetBundles;
            for (int i = 0; i < names.Count; i++)
            {
                AssetBundleInfo info = ResourceManager.RegisterAssetBundlePath(names[i].AssetPath);
                info.Load();

                string[] assetNames = info.GetAllAssetNames();
                foreach (var item in assetNames)
                {
                    Debug.Log(item);

                    var obj = info.LoadAsset(item);
                    Assert.NotNull(obj.Asset);

                    obj.Reserve();
                }

                Assert.IsTrue(info.IsLoaded);
                Assert.NotNull(info.AssetBundle);

                Assert.Throws(typeof(InvalidDataException), () => ResourceManager.UnregisterAssetBundle(info));

                info.Unload(true);

                Assert.IsFalse(info.IsLoaded);
                Assert.Null(info.AssetBundle);

                ResourceManager.UnregisterAssetBundle(info);
            }
        }

        //[UnityTearDown]
        //public IEnumerator TearDown()
        //{
        //    yield return new ExitPlayMode();
        //}
    }
}

#endif