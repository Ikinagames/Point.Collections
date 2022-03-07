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
