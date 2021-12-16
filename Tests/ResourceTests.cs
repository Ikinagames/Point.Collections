using NUnit.Framework;
using Point.Collections.ResourceControl;
using System.Collections;
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
        public void AssetBundleRegisterWithoutLoadedTest()
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

        //[UnityTearDown]
        //public IEnumerator TearDown()
        //{
        //    yield return new ExitPlayMode();
        //}
    }
}
