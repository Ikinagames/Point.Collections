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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Assertions;

namespace Point.Collections.IO
{
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

            Bucket bucket = new Bucket(DataState.Load | DataState.Calculate);
            t.LoadValues(ref bucket);

            UnsafeAllocator<byte> data = new UnsafeAllocator<byte>(bucket.totalSize, Allocator.Temp);
            string path = GetPath(t.Identifier);
            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }
            using (var st = new FileStream(path, FileMode.Open, FileAccess.Read))
            using (var rdr = new BinaryReader(st, Encoding.UTF8))
            {
                fixed (byte* p = rdr.ReadBytes(bucket.totalSize))
                {
                    UnsafeUtility.MemCpy(data.Ptr, p, bucket.totalSize);
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
    public struct Identifier : IEmpty, IEquatable<Identifier>
    {
        private readonly Hash m_Hash;

        public Identifier(string name)
        {
            m_Hash = new Hash(name);
        }

        public bool IsEmpty() => m_Hash.IsEmpty();

        public bool Equals(Identifier other) => m_Hash.Equals(other.m_Hash);
        public override string ToString() => m_Hash.Value.ToString();

        public static implicit operator uint(Identifier identifier) => identifier.m_Hash.Value;
    }
    public unsafe struct Data
    {
        public Identifier identifier;
        public UnsafeAllocator<byte> data;
    }
    
    [System.Flags]
    public enum DataState
    {
        None = 0,

        Load = 0b0001,
        Save = 0b0010,

        Calculate = 0b0100,
        Allocate = 0b1000,
    }
    public unsafe struct Bucket
    {
        internal int totalSize;
        private int m_Position;

        private DataState m_State;
        private UnsafeAllocator<byte> m_Data;

        internal Bucket(DataState state)
        {
            this = default(Bucket);

            m_State = state;
        }
        internal Bucket(DataState state, UnsafeAllocator<byte> data)
        {
            this = default(Bucket);

            m_State = state;
            m_Data = data;
        }

        public void Save(object t)
        {
            System.Type type = t.GetType();
            Assert.IsTrue(UnsafeUtility.IsUnmanaged(type));
#if UNITY_EDITOR
            if ((m_State & DataState.Save) != DataState.Save)
            {
                "fatal err".ToLogError();
                Debug.Break();
            }
#endif
            int size = UnsafeUtility.SizeOf(type);

            if ((m_State & DataState.Calculate) == DataState.Calculate)
            {
                this.totalSize += size;
                return;
            }

            try
            {
                //T boxed = t;
                //UnsafeReference<byte> p = (byte*)UnsafeUtility.AddressOf<T>(ref boxed);
                //UnsafeUtility.MemCpy(m_Data.Ptr + m_Position, p, size);
                Marshal.StructureToPtr(t, m_Data.Ptr + m_Position, false);
            }
            catch (System.Exception e)
            {
                $"Failed to serialize. Reason: {e.Message}".ToLogError();
            }

            m_Position += size;
        }
        public void Save<T>(T t) where T : struct
        {
            Save((object)t);
        }
        public object Load(System.Type type)
        {
            Assert.IsTrue(UnsafeUtility.IsUnmanaged(type));
#if UNITY_EDITOR
            if ((m_State & DataState.Load) != DataState.Load)
            {
                "fatal err".ToLogError();
                Debug.Break();
            }
#endif
            object t = null;
            int size = UnsafeUtility.SizeOf(type);

            if ((m_State & DataState.Calculate) == DataState.Calculate)
            {
                this.totalSize += size;
                return t;
            }

            try
            {
                t = Marshal.PtrToStructure(m_Data.Ptr + m_Position, type);
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogException(e);
            }

            m_Position += size;
            return t;
        }
        public T Load<T>() where T : struct
        {
            object obj = Load(TypeHelper.TypeOf<T>.Type);
            if (obj == null) return default(T);
            return (T)obj;
        }
    }

    public struct SaveData : ISaveable
    {
        private Identifier m_Name;
        private KeyValuePair<Identifier, System.Type>[] m_Properties;
        private object[] m_Values;

        Identifier ISaveable.Identifier => m_Name;

        public SaveData(Identifier name, IEnumerable<KeyValuePair<Identifier, System.Type>> values)
        {
            m_Name = name;
            m_Properties = values.ToArray();
            m_Values = new object[m_Properties.Length];
        }

        void ISaveable.SaveValues(ref Bucket bucket)
        {
            for (int i = 0; i < m_Values.Length; i++)
            {
                bucket.Save(m_Values[i]);
            }
        }
        void ISaveable.LoadValues(ref Bucket bucket)
        {
            for (int i = 0; i < m_Values.Length; i++)
            {
                m_Values[i] = bucket.Load(m_Properties[i].Value);
            }
        }
    }
}

#endif