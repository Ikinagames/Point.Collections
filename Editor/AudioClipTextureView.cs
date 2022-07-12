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
                m_OverlayBox.RemoveFromHierarchy();

                m_AudioClip = value;
                m_Texture = m_AudioClip.PaintWaveformSpectrum(.5f, 600, 100, Color.gray);

                style.backgroundImage = new StyleBackground(m_Texture);

                if (m_AudioClip == null)
                {
                    Add(m_OverlayBox);
                }
            }
        }
        public Texture2D texture => m_Texture;

        VisualElement m_OverlayBox;
        Label m_OverlayLabel;

        public string emptyString
        {
            get => m_OverlayLabel.text;
            set => m_OverlayLabel.text = value;
        }

        public AudioClipTextureView()
        {
            style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            style.flexGrow = 1;
            style.minHeight = new StyleLength(new Length(50, LengthUnit.Pixel));
            style.maxHeight = new StyleLength(new Length(100, LengthUnit.Pixel));
            style.SetBorderColor(Color.gray);
            style.SetBorderRadius(.15f);
            style.SetBorderWidth(.1f);

            m_OverlayBox = new VisualElement();
            m_OverlayBox.style.position = Position.Absolute;
            m_OverlayBox.style.flexGrow = 1;
            m_OverlayBox.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            m_OverlayBox.style.height = style.height;
            m_OverlayBox.style.backgroundColor = new Color(.25f, .25f, .25f, .5f);

            m_OverlayLabel = new Label();
            m_OverlayLabel.style.flexGrow = 1;
            m_OverlayLabel.style.alignContent = Align.Center;
            m_OverlayLabel.style.justifyContent = Justify.SpaceAround;
            m_OverlayLabel.style.unityTextAlign = TextAnchor.MiddleCenter;
            m_OverlayLabel.style.fontSize = 15;
            m_OverlayBox.Add(m_OverlayLabel);

            emptyString = "Empty";

            audioClip = null;
        }
    }
}

#endif