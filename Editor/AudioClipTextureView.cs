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
        protected AudioClip m_AudioClip;
        private Texture2D m_Texture;

        public float maxHeight { get; set; } = -1;
        public virtual AudioClip audioClip
        {
            get => m_AudioClip;
            set
            {
                m_AudioClip = value;
                //this.MarkDirtyRepaint();
            }
        }
        public Texture2D texture => m_Texture;
        public string emptyString
        {
            get => m_OverlayLabel.text;
            set => m_OverlayLabel.text = value;
        }

        public float width => m_TextureView.resolvedStyle.width;
        public float height => m_TextureView.resolvedStyle.height;

        ScrollView m_ScrollView;
        VisualElement m_TextureView;
        VisualElement m_OverlayBox;
        Label m_OverlayLabel;

        private bool scrolled => this.resolvedStyle.width != parent.resolvedStyle.width;
        public override VisualElement contentContainer => m_TextureView;

        public AudioClipTextureView()
        {
            m_ScrollView = new ScrollView(
                //ScrollViewMode.Horizontal
                );
            //m_ScrollView.elasticity = 100;
            //m_ScrollView.touchScrollBehavior = ScrollView.TouchScrollBehavior.Elastic;
            m_ScrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            hierarchy.Add(m_ScrollView);

            m_TextureView = new VisualElement();
            m_TextureView.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
            m_TextureView.style.flexGrow = 1;
            m_TextureView.style.minHeight = new StyleLength(new Length(80, LengthUnit.Pixel));
            m_TextureView.style.maxHeight = new StyleLength(new Length(100, LengthUnit.Pixel));
            m_TextureView.style.SetBorderColor(Color.gray);
            m_TextureView.style.SetBorderRadius(.15f);
            m_TextureView.style.SetBorderWidth(.1f);

            m_ScrollView.Add(m_TextureView);

            m_OverlayBox = new VisualElement();
            m_OverlayBox.style.position = Position.Absolute;
            m_OverlayBox.style.flexGrow = 1;
            m_OverlayBox.style.width = new StyleLength(new Length(100, LengthUnit.Percent));
            m_OverlayBox.style.height = new StyleLength(new Length(100, LengthUnit.Percent));
            //m_OverlayBox.style.height = style.height;
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

            m_TextureView.generateVisualContent += OnGenerateVisualContent;
            hierarchy.Add(m_OverlayBox);

            RegisterCallback<WheelEvent>(WheelEventHandler);
        }

        private void WheelEventHandler(WheelEvent ev)
        {
            if (!ev.actionKey) return;

            ev.StopPropagation();

            var scale = m_TextureView.resolvedStyle.width;
            scale = Mathf.Clamp(scale - (ev.delta.y), parent.resolvedStyle.width, float.MaxValue);
            m_TextureView.style.width = scale;
        }
        private void RepaintTexture()
        {
            //"repaint".ToLog();

            m_OverlayBox.RemoveFromHierarchy();
            int
               width = Mathf.RoundToInt(m_TextureView.resolvedStyle.width),
               height = Mathf.RoundToInt(m_TextureView.resolvedStyle.height);

            m_Texture = m_AudioClip.PaintWaveformSpectrum(.5f, width, height, Color.gray, .6f, maxHeight);
            m_TextureView.style.backgroundImage = new StyleBackground(m_Texture);

            if (m_AudioClip == null)
            {
                m_OverlayBox.style.width = width;
                m_OverlayBox.style.height = height;

                hierarchy.Add(m_OverlayBox);
            }

            OnRepaintTexture();
        }

        //public float GetSamplePosition(Vector3 pos)
        //{
        //    float scale = resolvedStyle.width / parent.resolvedStyle.width;
        //}

        protected virtual void OnGenerateVisualContent(MeshGenerationContext ctx)
        {
            this.schedule.Execute(RepaintTexture);
        }
        protected virtual void OnRepaintTexture() { }
    }
}

#endif