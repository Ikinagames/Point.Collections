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
using Point.Collections.Buffer.LowLevel;
using Point.Collections.IO;
using Point.Collections.IO.LowLevel;
using Point.Collections.Native;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.TestTools;

namespace Point.Collections.Tests
{
    public class NativeReaderTests
    {
        [UnityTest]
        public IEnumerator LoadDatabase()
        {
            const string c_Data = "ad.audiolink";
            string dbPath = Path.Combine(Application.streamingAssetsPath, c_Data);

            NativeFileStream stream = new NativeFileStream();
            UnsafeFileReader rdr = stream.ReadAsync<UnsafeFileReader>(dbPath);

            yield return new WaitUntil(() => rdr.IsReadable);

            string txt = rdr.ReadString();

            Debug.Log($"rdr: {txt}");
        }
    }

    public sealed class NativeCodeTests
    {
        [Test]
        public void MathTest()
        {
            TestStruct
                a = new TestStruct(),
                b = a;

            bool result;
            unsafe
            {
                void*
                    x = NativeUtility.AddressOf(ref a),
                    y = NativeUtility.AddressOf(ref b);

                NativeMath.binaryComparer(x, y, TypeHelper.SizeOf<TestStruct>(), &result);
            }

            Assert.IsTrue(result);
        }
    }

    public struct TestStruct
    {
        public int x;
        public float y;
        public bool z;
    }
}

#endif