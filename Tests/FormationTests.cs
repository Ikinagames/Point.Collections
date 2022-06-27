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
using Point.Collections.Formations;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Point.Collections.Tests
{
    public sealed class FormationTests
    {
        private float3 randomPosition => new float3(
            UnityEngine.Random.Range(-100, 100),
            UnityEngine.Random.Range(-100, 100),
            UnityEngine.Random.Range(-100, 100));

        [Test]
        public void RowFormationTest()
        {
            ColumnFormationGroup group = new ColumnFormationGroup()
            {
                Offset = 5
            };
            float3 position = randomPosition;
            Formation leader = new Formation()
            {
                GroupProvider = group,
                DisplayName = "Column Leader",
                position = position,
            };

            leader.AddChild(new Formation()
            {
                DisplayName = "Child 0",
            });
            leader.AddChild(new Formation()
            {
                DisplayName = "Child 1",
            });
            leader.AddChild(new Formation()
            {
                DisplayName = "Child 2",
            });

            int index = 1;
            float3 offset = new float3(0, 0, -5);
            foreach (var item in leader)
            {
                Debug.Log($"{item.DisplayName}: {item.position}");

                Assert.AreEqual(position + (offset * index), item.position);
                index++;
            }
        }
        [Test]
        public void FormationChangeTest()
        {
            ColumnFormationGroup group = new ColumnFormationGroup()
            {
                Offset = 5
            };
            Formation leader = new Formation()
            {
                GroupProvider = group,
                DisplayName = "Column Leader"
            };

            leader.AddChild(new Formation()
            {
                DisplayName = "Child 0",
            });
            leader.AddChild(new Formation()
            {
                DisplayName = "Child 1",
            });
            leader.AddChild(new Formation()
            {
                DisplayName = "Child 2",
            });

            foreach (var item in leader)
            {
                Debug.Log($"{item.DisplayName}: {item.position}");
            }

            RowFormationGroup newGroup = new RowFormationGroup
            {
                Offset = 5
            };
            leader.GroupProvider = newGroup;

            foreach (var item in leader)
            {
                Debug.Log($"{item.DisplayName}: {item.position}");
            }
        }
    }
}

#endif