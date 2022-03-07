// Copyright 2022 Ikina Games
// Author : Seung Ha Kim (Syadeu)
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2020
#define UNITYENGINE
#else
#define POINT_COLLECTIONS_NATIVE
#endif

#if UNITYENGINE

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

#endif