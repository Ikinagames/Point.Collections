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
                AssetBundleInfo info = ResourceManager.RegisterAssetBundle(names[i].AssetPath, false);
                info.Load();

                Assert.IsTrue(info.IsLoaded);
                Assert.NotNull(info.AssetBundle);

                info.Unload(true);

                Assert.IsFalse(info.IsLoaded);
                Assert.Null(info.AssetBundle);

                ResourceManager.UnregisterAssetBundle(info);
            }
        }

        [Test]
        public void AssetBundleLoadAssetsTest()
        {
            var names = ResourceAddresses.Instance.StreamingAssetBundles;
            for (int i = 0; i < names.Count; i++)
            {
                AssetBundleInfo info = ResourceManager.RegisterAssetBundle(names[i].AssetPath, false);
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
