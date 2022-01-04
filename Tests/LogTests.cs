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
            PointCore.Log("core", "TEST NORMAL");
            PointCore.Log(LogChannel.Core, "TEST NORMAL");
            PointCore.Log(Channel.Core, "TEST NORMAL");
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
