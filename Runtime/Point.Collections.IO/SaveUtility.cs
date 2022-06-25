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

#if UNITY_2019_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
#if UNITY_COLLECTIONS
#endif


using Point.Collections;
using Point.Collections.Buffer.LowLevel;
using Point.Collections.IO;
using Point.Collections.IO.LowLevel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Assertions;

namespace Point.Collections.IO
{
    // https://devblogs.microsoft.com/oldnewthing/20060222-11/?p=32193
    // https://stackoverflow.com/questions/3929697/performance-of-reading-a-registry-key

    public unsafe static class SaveUtility
    {
        private static string DataPath => Path.Combine(PointPath.DataPath, new Hash(Application.identifier).Value.ToString());
        private static string GetPath(Identifier identifier)
        {
            const string c_NameFormat = "{0}.dat";

            return Path.Combine(DataPath, string.Format(c_NameFormat, identifier.ToString()));
        }

        public static void Save(this ISaveable t)
        {
            Assert.IsFalse(t.Identifier.IsEmpty());

            Bucket bucket = new Bucket(DataState.Save | DataState.Calculate);
            t.SaveValues(ref bucket);

            int size = bucket.totalSize;
            UnsafeAllocator<byte> data = new UnsafeAllocator<byte>(size, Allocator.Temp);
            bucket = new Bucket(DataState.Save | DataState.Allocate, data);
            t.SaveValues(ref bucket);

            string path = GetPath(t.Identifier);
            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }
            using (var st = new FileStream(path, FileMode.Create, FileAccess.Write))
            using (var wr = new BinaryWriter(st, Encoding.UTF8))
            {
                for (int i = 0; i < data.Length; i++)
                {
                    wr.Write(data[i]);
                }
            }

            data.Dispose();
        }
        public static void Load(this ISaveable t)
        {
            Assert.IsFalse(t.Identifier.IsEmpty());
            string path = GetPath(t.Identifier);
            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }

            if (!File.Exists(path))
            {
                return;
            }

            Bucket bucket;
            //bucket = new Bucket(DataState.Load | DataState.Calculate);
            //t.LoadValues(ref bucket);

            int totalSize = (int)new FileInfo(path).Length;
            UnsafeAllocator<byte> data 
                = new UnsafeAllocator<byte>(totalSize, Allocator.Temp);
            
            using (var st = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var rdr = new BinaryReader(st, Encoding.UTF8))
            {
                fixed (byte* p = rdr.ReadBytes(totalSize))
                {
                    UnsafeUtility.MemCpy(data.Ptr, p, totalSize);
                }
       
                bucket = new Bucket(DataState.Load | DataState.Allocate, data);
                t.LoadValues(ref bucket);
            }

            data.Dispose();
        }
    }
    public interface ISaveable
    {
        Identifier Identifier { get; }

        void SaveValues(ref Bucket bucket);
        void LoadValues(ref Bucket bucket);
    }
}

#endif