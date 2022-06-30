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
using Unity.Collections;
using Unity.Mathematics;
#else
#define POINT_COLLECTIONS_NATIVE
#endif

namespace Point.Collections.SceneManagement.LowLevel
{
#if UNITYENGINE
    [BurstCompatible]
#endif
    public struct UnsafeTransform
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
    }

    public struct UnsafeTransformScene
    {

    }
}
