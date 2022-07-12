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
#if !UNITY_2020_1_OR_NEWER
#define UNITYENGINE_OLD
#endif

using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public class AudioClipTextureView : VisualElement
    {
        private AudioClip m_AudioClip;
        private Texture2D m_Texture;

        public AudioClip audioClip
        {
            get => m_AudioClip;
            set
            {
                m_AudioClip = value;
                m_Texture = m_AudioClip.PaintWaveformSpectrum(.5f, 600, 100, Color.gray);

                style.backgroundImage = m_Texture;
            }
        }

        public AudioClipTextureView()
        {
            style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            style.height = new StyleLength(new Length(100, LengthUnit.Pixel));
        }
    }
}

#endif