using Point.Collections.IO;
using Point.Collections.IO.LowLevel;
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

            string txt = rdr.ReadData<string>();

            Debug.Log($"rdr: {txt}");
        }
    }
}
