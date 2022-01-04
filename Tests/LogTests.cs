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
            PointCore.Log(LogChannel.Core, "TEST NORMAL");
        }
        [Test]
        public void WarningTest()
        {
            PointCore.LogWarning(LogChannel.Core, "TEST WARNING");
        }
        [Test]
        public void ErrorTest()
        {
            PointCore.LogError(LogChannel.Core, "TEST ERROR");

            LogAssert.Expect(UnityEngine.LogType.Error, PointCore.LogErrorString(LogChannel.Core, "TEST ERROR"));
        }

        //[UnityTearDown]
        //public IEnumerator TearDown()
        //{
        //    yield return new ExitPlayMode();
        //}
    }
}
