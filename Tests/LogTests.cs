using NUnit.Framework;
using System.Collections;
using UnityEngine.TestTools;

namespace Point.Collections.Tests
{
    public sealed class LogTests
    {
        //[UnitySetUp]
        //public IEnumerator SetUp()
        //{
        //    yield return new EnterPlayMode();
        //}

        [Test]
        public void NormalTest()
        {
            PointCore.Log(PointCore.LogChannel.Default, "TEST NORMAL");
        }
        [Test]
        public void WarningTest()
        {
            PointCore.LogWarning(PointCore.LogChannel.Default, "TEST WARNING");
        }
        [Test]
        public void ErrorTest()
        {
            PointCore.LogError(PointCore.LogChannel.Default, "TEST ERROR");

            LogAssert.Expect(UnityEngine.LogType.Error, PointCore.LogErrorString(PointCore.LogChannel.Default, "TEST ERROR"));
        }

        //[UnityTearDown]
        //public IEnumerator TearDown()
        //{
        //    yield return new ExitPlayMode();
        //}
    }
}
