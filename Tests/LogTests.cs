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
            Point.Log(Point.LogChannel.Default, "TEST NORMAL");
        }
        [Test]
        public void WarningTest()
        {
            Point.LogWarning(Point.LogChannel.Default, "TEST WARNING");
        }
        [Test]
        public void ErrorTest()
        {
            Point.LogError(Point.LogChannel.Default, "TEST ERROR");

            LogAssert.Expect(UnityEngine.LogType.Error, Point.LogErrorString(Point.LogChannel.Default, "TEST ERROR"));
        }

        //[UnityTearDown]
        //public IEnumerator TearDown()
        //{
        //    yield return new ExitPlayMode();
        //}
    }
}
