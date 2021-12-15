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
        public void AssetBundleLoadTest()
        {
            var names = ResourceAddresses.Instance.TrackedAssetBundleNames;
            for (int i = 0; i < names.Count; i++)
            {
                AssetBundleInfo info = ResourceAddresses.GetAssetBundleInfo(names[i]);
                AssetBundle bundle = info.Load();
                $"{info.Name} : {bundle != null}".ToLog();
            }
        }

        [UnityTest]
        public IEnumerator AssetBundleLoadAsyncTest()
        {
            var names = ResourceAddresses.Instance.TrackedAssetBundleNames;
            for (int i = 0; i < names.Count; i++)
            {
                AssetBundleInfo info = ResourceAddresses.GetAssetBundleInfo(names[i]);
                AssetBundleHandler handle = info.LoadAsync();

                yield return new WaitUntil(() => handle.IsLoaded);
                $"{info.Name} : {handle.AssetBundle != null}".ToLog();
            }
        }

        //[UnityTearDown]
        //public IEnumerator TearDown()
        //{
        //    yield return new ExitPlayMode();
        //}
    }
}
