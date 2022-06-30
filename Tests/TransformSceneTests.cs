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

using NUnit.Framework;
using Point.Collections.Buffer.LowLevel;
using Point.Collections.SceneManagement.LowLevel;
using System.Collections.Generic;
using UnityEngine;

namespace Point.Collections.Tests
{
    public sealed class TransformSceneTests
    {
        [Test]
        public void PtrTest()
        {
            UnsafeAllocator<UnsafeTransform> temp1 = new UnsafeAllocator<UnsafeTransform>(1, Unity.Collections.Allocator.Temp);

            UnsafeTransformScene scene = new UnsafeTransformScene(Unity.Collections.Allocator.Temp);

            scene.RemoveTransform(temp1.Ptr);

            GameObject obj = new GameObject();

            var temp = scene.AddTransform(obj.transform);
            scene.RemoveTransform(temp);

            //Unmanaged<UnsafeTransformScene>();

            scene.Dispose();
        }
        public void Unmanaged<T>() where T : unmanaged { }
        [Test]
        public void ResizeTest()
        {
            List<Transform> list = new List<Transform>();
            UnsafeTransformScene scene = new UnsafeTransformScene(Unity.Collections.Allocator.Temp);
            scene.Resize(list);

            scene.Dispose();
        }
    }
}

#endif