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

#if UNITY_2020_1_OR_NEWER
#if (UNITY_EDITOR || DEVELOPMENT_BUILD) && !POINT_DISABLE_CHECKS
#define DEBUG_MODE
#endif
#define UNITYENGINE
using Point.Collections.Buffer.LowLevel;
using System;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Assertions;
#else
#define POINT_COLLECTIONS_NATIVE
#endif

namespace Point.Collections.SceneManagement.LowLevel
{
#if UNITYENGINE
    [BurstCompatible]
#endif
    public struct UnsafeTransform : IEquatable<UnsafeTransform>
    {
        public static UnsafeTransform Invalid => new UnsafeTransform(default(Transformation), -1);
        public int hashCode;

        public int parentIndex;
        public Transformation transformation;

        public UnsafeTransform(Transformation tr)
        {
            hashCode = CollectionUtility.CreateHashCode();
            parentIndex = -1;

            transformation = tr;
        }
        public UnsafeTransform(Transformation tr, int parent) : this(tr)
        {
            parentIndex = parent;
        }

        public override int GetHashCode() => hashCode;

        public bool Equals(UnsafeTransform other) => hashCode == other.hashCode;
    }

    public struct UnsafeTransformScene
    {
        public const int INIT_COUNT = 512;

        private UnsafeAllocator<UnsafeTransform> buffer;
        private UnsafeFixedListWrapper<UnsafeTransform> transforms;

        public UnsafeTransformScene(Allocator allocator, int initCount = INIT_COUNT)
        {
            buffer = new UnsafeAllocator<UnsafeTransform>(
                initCount,
                allocator);
            transforms = new UnsafeFixedListWrapper<UnsafeTransform>(buffer, 0);
        }

        public bool RequireResize()
        {
            if (transforms.Length >= buffer.Length ||
                transforms.Length + 1 >= buffer.Length)
            {
                return true;
            }
            return false;
        }

        public UnsafeReference<UnsafeTransform> AddTransform(Transformation tr = default(Transformation))
        {
            Assert.IsFalse(RequireResize(), "This Scene require resize but you didn\'t.");

            int count = transforms.Length;
            transforms.AddNoResize(new UnsafeTransform(tr));

            UnsafeReference<UnsafeTransform> ptr = buffer.ElementAt(count);
            return ptr;
        }
        public void RemoveTransform(UnsafeReference<UnsafeTransform> ptr)
        {
            if (!buffer.Contains(ptr))
            {
                $"?? not in the buffer {buffer.Ptr} ? {ptr}".ToLog();
                return;
            }

            int index = buffer.IndexOf(ptr);
            if (index < 0)
            {
                $"?? not in the buffer {buffer.Ptr} ? {ptr}".ToLog();
                return;
            }

            UnsafeTransform tr = buffer[index];
            transforms.RemoveSwapback(tr);
            "success".ToLog();
        }
    }
}
