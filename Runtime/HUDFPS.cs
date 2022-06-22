﻿// Copyright 2021 Ikina Games
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
using UnityEngine;
#else
#define POINT_COLLECTIONS_NATIVE
#endif

using System.Collections;

namespace Point.Collections
{
    [AddComponentMenu("")]
    internal sealed class HUDFPS : StaticMonobehaviour<HUDFPS>
    {
        protected override bool HideInInspector => true;
        protected override bool EnableLog => false;

        // Attach this to any object to make a frames/second indicator.
        //
        // It calculates frames/second over each updateInterval,
        // so the display does not keep changing wildly.
        //
        // It is also fairly accurate at very low FPS counts (<10).
        // We do this not by simply counting frames per interval, but
        // by accumulating FPS for each frame. This way we end up with
        // corstartRect overall FPS even if the interval renders something like
        // 5.5 frames.

        public Rect startRect = new Rect(10, 10, 75, 50); // The rect the window is initially displayed at.
        public bool updateColor = true; // Do you want the color to change if the FPS gets low
        public bool allowDrag = true; // Do you want to allow the dragging of the FPS window
        public float frequency = 0.5F; // The update frequency of the fps
        public int nbDecimal = 1; // How many decimal do you want to display

        private float accum = 0f; // FPS accumulated over the interval
        private int frames = 0; // Frames drawn over the interval
        private Color color = Color.white; // The color of the GUI, depending of the FPS ( R < 10, Y < 30, G >= 30 )
        private string sFPS = ""; // The fps formatted into a string.
        private GUIStyle style; // The style the text will be displayed at, based en defaultSkin.label.

        void Start()
        {
            StartCoroutine(FPS());
        }

        void Update()
        {
            accum += Time.timeScale / Time.deltaTime;
            ++frames;
        }

        IEnumerator FPS()
        {
            // Infinite loop executed every "frenquency" secondes.
            while (true)
            {
                // Update the FPS
                float fps = accum / frames;
                sFPS = fps.ToString("f" + Mathf.Clamp(nbDecimal, 0, 10));

                //Update the color
                color = (fps >= 30) ? Color.green : ((fps > 10) ? Color.red : Color.yellow);

                accum = 0.0F;
                frames = 0;

                yield return new WaitForSeconds(frequency);
            }
        }

        //public float DesignWidth = 1280.0f;
        //public float DesignHeight = 720.0f;

        void OnGUI()
        {
            var prevMat = GUI.matrix;

            //Calculate change aspects
            float resX = (float)(Screen.width) / 800;
            float resY = (float)(Screen.height) / 600;

            GUI.matrix = Matrix4x4.TRS(new Vector3(0, 0, 0), Quaternion.identity, new Vector3(resX, resY, 1));

            // Copy the default label skin, change the color and the alignement
            if (style == null)
            {
                style = new GUIStyle(GUI.skin.label);
                style.normal.textColor = Color.white;
                style.alignment = TextAnchor.MiddleCenter;
            }

            GUI.color = updateColor ? color : Color.white;
            startRect = GUI.Window(0, startRect, DoMyWindow, string.Empty);

            GUI.matrix = prevMat;
        }

        void DoMyWindow(int windowID)
        {
            GUI.Label(new Rect(0, 0, startRect.width, startRect.height), sFPS + " FPS", style);
            if (allowDrag) GUI.DragWindow(new Rect(0, 0, Screen.width, Screen.height));
        }
    }
}
