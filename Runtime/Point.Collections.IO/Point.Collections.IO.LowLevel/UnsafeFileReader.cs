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
using Unity.Collections.LowLevel.Unsafe;
using Unity.IO.LowLevel.Unsafe;
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Point.Collections.IO.LowLevel
{
    /// <summary>
    /// <see cref="NativeFileStream"/> 에서 수행한 바이너리 정보를 읽는 리더입니다.
    /// </summary>
    public struct UnsafeFileReader : INativeReader, IValidation
    {
        private ReadHandle handle;
        [NativeDisableUnsafePtrRestriction] private unsafe byte* data;
        private long size;

        public bool IsReadable
        {
            get
            {
                return handle.Status != ReadStatus.InProgress;
            }
        }

        void INativeReader.Initialize(ReadHandle handle, ReadCommand cmd)
        {
            this.handle = handle;
            unsafe
            {
                data = (byte*)cmd.Buffer;
            }
            size = cmd.Size;
        }

        public bool IsValid()
        {
            unsafe
            {
                if (data == null) return false;
            }

            return true;
        }

        public unsafe byte* UnsafeGetData() => data;
        public byte[] GetData()
        {
            byte[] bytes = new byte[size];
            unsafe
            {
                fixed (byte* buffer = bytes)
                {
                    UnsafeUtility.MemCpy(buffer, data, size);
                }
            }

            return bytes;
        }

        public string ReadString()
        {
            string data;
            unsafe
            {
                data = Encoding.Default.GetString(this.data, (int)size);
            }

            return data;
        }
        public T ReadData<T>() where T : unmanaged
        {
#if DEBUG_MODE
            if (UnsafeUtility.SizeOf<T>() != size)
            {
                throw new InvalidCastException($"binary size is not matched. " +
                    $"expected {size} but {UnsafeUtility.SizeOf<T>()}({TypeHelper.TypeOf<T>.ToString()})");
            }
#endif
            unsafe
            {
                UnsafeUtility.CopyPtrToStructure<T>(this.data, out T output);
                return output;
            }
        }
    }
}
