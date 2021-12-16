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
            }
        }

        //[Test, Order(1)]
        //public void a1_AssetBundleUnloadTest()
        //{
        //    var names = ResourceAddresses.Instance.TrackedAssetBundles;
        //    for (int i = 0; i < names.Count; i++)
        //    {
        //        AssetBundleInfo info = ResourceManager.GetAssetBundleInfo(names[i].Name);
        //        info.Unload(true);
        //        $"{info.Name} : isloaded?{info.IsLoaded} : bundleNull?{info.AssetBundle == null}".ToLog();
        //    }

        //    for (int i = 0; i < names.Count; i++)
        //    {
        //        AssetBundleInfo info = ResourceManager.GetAssetBundleInfo(names[i].Name);
        //        Assert.IsFalse(info.IsLoaded, $"bundle({info.Name}) is not unloaded.");
        //    }
        //}

        //[UnityTest, Order(2)]
        //public IEnumerator b0_AssetBundleLoadAsyncTest()
        //{
        //    var names = ResourceAddresses.Instance.TrackedAssetBundles;
        //    for (int i = 0; i < names.Count; i++)
        //    {
        //        AssetBundleInfo info = ResourceManager.GetAssetBundleInfo(names[i].Name);
        //        Assert.False(info.IsLoaded, "bundle is already loaded.");

        //        AssetBundleHandler handle = info.LoadAsync();

        //        yield return new WaitUntil(() => handle.IsDone);
        //        $"{info.Name} : {handle.AssetBundle != null}".ToLog();
        //    }
        //}

        //[Test]
        //public void b1_()
        //{
        //    var names = ResourceAddresses.Instance.TrackedAssetBundles;
        //    for (int i = 0; i < names.Count; i++)
        //    {
        //        AssetBundleInfo info = ResourceManager.GetAssetBundleInfo(names[i].Name);
        //        Assert.IsTrue(info.IsLoaded, $"bundle(has {info.AssetBundle != null}) is not loaded.");

        //        foreach (var item in info.AssetBundle.GetAllAssetNames())
        //        {
        //            Debug.Log(item);
        //        };
        //    }
        //}

        //[UnityTearDown]
        //public IEnumerator TearDown()
        //{
        //    yield return new ExitPlayMode();
        //}
    }
}
