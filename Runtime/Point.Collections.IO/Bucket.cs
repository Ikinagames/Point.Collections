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


using Point.Collections.Buffer.LowLevel;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.Assertions;

namespace Point.Collections.IO
{
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
            Assert.IsTrue(UnsafeUtility.IsUnmanaged(type),
                $"Type({TypeHelper.ToString(type)}) is not unmananged type.");
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
}

#endif