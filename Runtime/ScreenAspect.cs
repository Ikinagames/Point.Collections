// Copyright 2021 Ikina Games
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
#else
#define POINT_COLLECTIONS_NATIVE
#endif


namespace Point.Collections
{
    public readonly struct ScreenAspect
    {
        public readonly float
            WidthRatio, HeightRatio;
        public readonly int
            Width, Height;

        public ScreenAspect(UnityEngine.Resolution resolution)
        {
            Width = resolution.width;
            Height = resolution.height;

            WidthRatio = resolution.width / 80f;
            HeightRatio = resolution.height / 80f;
        }

        public bool Is16p9()
        {
            return WidthRatio == 16 && HeightRatio == 9;
        }
        public bool Is16p10()
        {
            return WidthRatio == 16 && HeightRatio == 10;
        }

        /// <summary>
        /// is 19.5 / 9 ratio ?
        /// </summary>
        /// <remarks>
        /// iPhone XS
        /// </remarks>
        /// <returns></returns>
        public bool Is19d5p9()
        {
            return WidthRatio == 19.5f && HeightRatio == 9;
        }
        /// <summary>
        /// Is 13 / 5 ratio ?
        /// </summary>
        /// <remarks>
        /// iPhone X
        /// </remarks>
        /// <returns></returns>
        public bool Is13p5()
        {
            return WidthRatio == 13 && HeightRatio == 5;
        }
    }
}
