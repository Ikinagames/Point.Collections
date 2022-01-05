using NUnit.Framework;
using Point.Collections.LowLevel;
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
        public void ChannelTest()
        {
            Channel channel = LogChannel.Core | LogChannel.Collections;

            UnityEngine.Debug.Log($"{(int)(LogChannel)channel} : {channel}");

            Channel temp = channel & LogChannel.Collections;

            UnityEngine.Debug.Log($"{(int)(LogChannel)temp} : {temp} : {temp == LogChannel.Collections}");
        }

        [Test]
        public void NormalTest()
        {
            PointHelper.Log("core", "TEST NORMAL");
            PointHelper.Log(LogChannel.Core, "TEST NORMAL");
            PointHelper.Log(Channel.Core, "TEST NORMAL");
        }
        [Test]
        public void WarningTest()
        {
            PointHelper.LogWarning(LogChannel.Core, "TEST WARNING");
        }
        [Test]
        public void ErrorTest()
        {
            PointHelper.LogError(LogChannel.Core, "TEST ERROR");

            LogAssert.Expect(UnityEngine.LogType.Error, PointHelper.LogErrorString(LogChannel.Core, "TEST ERROR"));
        }

        [Test]
        public void LogTest()
        {
        }

        //[UnityTearDown]
        //public IEnumerator TearDown()
        //{
        //    yield return new ExitPlayMode();
        //}
    }
}
