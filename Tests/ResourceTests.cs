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

        [Test, Order(0)]
        public void a0_AssetBundleLoadTest()
        {
            var names = ResourceAddresses.Instance.TrackedAssetBundleNames;
            $"loading bundles {names.Count}".ToLog();
            for (int i = 0; i < names.Count; i++)
            {
                AssetBundleInfo info = ResourceManager.GetAssetBundleInfo(names[i]);
                AssetBundle bundle = info.Load();
                $"{info.Name} : {bundle != null}".ToLog();
            }
        }

        [Test, Order(1)]
        public void a1_AssetBundleUnloadTest()
        {
            var names = ResourceAddresses.Instance.TrackedAssetBundleNames;
            for (int i = 0; i < names.Count; i++)
            {
                AssetBundleInfo info = ResourceManager.GetAssetBundleInfo(names[i]);
                info.Unload();
                $"{info.Name} : isloaded?{info.IsLoaded} : bundleNull?{info.AssetBundle == null}".ToLog();
            }

            for (int i = 0; i < names.Count; i++)
            {
                AssetBundleInfo info = ResourceManager.GetAssetBundleInfo(names[i]);
                Assert.IsFalse(info.IsLoaded, $"bundle({info.Name}) is not unloaded.");
            }
        }

        [UnityTest, Order(2)]
        public IEnumerator b0_AssetBundleLoadAsyncTest()
        {
            var names = ResourceAddresses.Instance.TrackedAssetBundleNames;
            for (int i = 0; i < names.Count; i++)
            {
                AssetBundleInfo info = ResourceManager.GetAssetBundleInfo(names[i]);
                Assert.False(info.IsLoaded, "bundle is already loaded.");

                AssetBundleHandler handle = info.LoadAsync();

                yield return new WaitUntil(() => handle.IsDone);
                $"{info.Name} : {handle.AssetBundle != null}".ToLog();
            }
        }

        [Test]
        public void b1_()
        {
            var names = ResourceAddresses.Instance.TrackedAssetBundleNames;
            for (int i = 0; i < names.Count; i++)
            {
                AssetBundleInfo info = ResourceManager.GetAssetBundleInfo(names[i]);
                Assert.IsTrue(info.IsLoaded, "bundle is not loaded.");

                foreach (var item in info.AssetBundle.GetAllAssetNames())
                {
                    Debug.Log(item);
                };
            }
        }

        //[UnityTearDown]
        //public IEnumerator TearDown()
        //{
        //    yield return new ExitPlayMode();
        //}
    }
}
