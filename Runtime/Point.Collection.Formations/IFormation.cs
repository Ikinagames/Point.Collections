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
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System.Collections.Generic;
using Unity.Mathematics;

namespace Point.Collections.Formations
{
    public interface IFormation : ITransformation, IEnumerable<IFormation>
    {
        string DisplayName { get; }
        IFormationGroupProvider GroupProvider { get; }
        ITransformation TransformationProvider { get; }

#pragma warning disable IDE1006 // Naming Styles
        new IFormation parent { get; set; }
        IReadOnlyList<IFormation> children { get; }

        bool updateRotation { get; set; }

        float currentSpeed { get; set; }
        float targetSpeed { get; set; }
        float3 currentVelocity { get; set; }
#pragma warning restore IDE1006 // Naming Styles

        int AddChild(IFormation child);
        bool RemoveChild(IFormation child);
        int AddChildWithoutNotification(IFormation child);
        void ClearChildren();
        void SetParent(IFormation parent);
        void RemoveFromHierarchy();

        void UpdateCurrentSpeed(float accel);
        bool Refresh();
    }

    public static class IFormationExtensions
    {
        public static bool TryGetUnityTransformProvider(this IFormation t, out UnityTransformProvider provider)
        {
            if (t.TransformationProvider != null &&
                t.TransformationProvider is UnityTransformProvider prv)
            {
                provider = prv;
                return true;
            }

            provider = null;
            return false;
        }
    }
}
