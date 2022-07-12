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

using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Point.Collections.Editor
{
    public sealed class RulerView : VisualElement
    {
        //public new class UxmlFactory : UxmlFactory<RulerView, UxmlTraits> { }
        //public new class UxmlTraits : VisualElement.UxmlTraits
        //{
        //    public override IEnumerable<UxmlChildElementDescription> uxmlChildElementsDescription
        //    {
        //        get { yield break; }
        //    }
        //    public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
        //    {
        //        base.Init(ve, bag, cc);
        //        RulerView ate = ve as RulerView;
        //    }
        //}

        internal static class Styles
        {
            public static GUIContent StartFrame = EditorGUIUtility.TrTextContent("Start", "Start frame of the clip.");
            public static GUIContent EndFrame = EditorGUIUtility.TrTextContent("End", "End frame of the clip.");
            public static string AverageVelocity = L10n.Tr("Average Velocity: {0}\nAverage Angular Y Speed: {1} deg/s");

            public static GUIContent HasAdditiveReferencePose = EditorGUIUtility.TrTextContent("Additive Reference Pose", "Enable to define the additive reference pose frame.");
            public static GUIContent AdditiveReferencePoseFrame = EditorGUIUtility.TrTextContent("Pose Frame", "Pose Frame.");
            public static GUIContent LoopTime = EditorGUIUtility.TrTextContent("Loop Time", "Enable to make the animation play through and then restart when the end is reached.");
            public static GUIContent LoopPose = EditorGUIUtility.TrTextContent("Loop Pose", "Enable to make the animation loop seamlessly.");
            public static GUIContent LoopCycleOffset = EditorGUIUtility.TrTextContent("Cycle Offset", "Offset to the cycle of a looping animation, if we want to start it at a different time.");
            public static GUIContent RootTransformRotation = EditorGUIUtility.TrTextContent("Root Transform Rotation");
            public static GUIContent RootTransformRotationY = EditorGUIUtility.TrTextContent("Root Transform Position (Y)");
            public static GUIContent RootTransformPositionXZ = EditorGUIUtility.TrTextContent("Root Transform Position (XZ)");

            public static GUIContent BakeIntoPoseOrientation = EditorGUIUtility.TrTextContent("Bake Into Pose", "Enable to make root rotation be baked into the movement of the bones. Disable to make root rotation be stored as root motion.");
            public static GUIContent OrientationOffsetY = EditorGUIUtility.TrTextContent("Offset", "Offset to the root rotation (in degrees).");

            public static GUIContent BasedUponOrientation = EditorGUIUtility.TrTextContent("Based Upon", "What the root rotation is based upon.");
            public static GUIContent BasedUponStartOrientation = EditorGUIUtility.TrTextContent("Based Upon (at Start)", "What the root rotation is based upon.");

            public static GUIContent[] BasedUponRotationHumanOpt =
            {
                EditorGUIUtility.TrTextContent("Original", "Keeps the rotation as it is authored in the source file."),
                EditorGUIUtility.TrTextContent("Body Orientation", "Keeps the upper body pointing forward.")
            };

            public static GUIContent[] BasedUponRotationOpt =
            {
                EditorGUIUtility.TrTextContent("Original", "Keeps the rotation as it is authored in the source file."),
                EditorGUIUtility.TrTextContent("Root Node Rotation", "Keeps the upper body pointing forward.")
            };

            public static GUIContent BakeIntoPosePositionY = EditorGUIUtility.TrTextContent("Bake Into Pose", "Enable to make vertical root motion be baked into the movement of the bones. Disable to make vertical root motion be stored as root motion.");
            public static GUIContent PositionOffsetY = EditorGUIUtility.TrTextContent("Offset", "Offset to the vertical root position.");

            public static GUIContent BasedUponPositionY = EditorGUIUtility.TrTextContent("Based Upon", "What the vertical root position is based upon.");
            public static GUIContent BasedUponStartPositionY = EditorGUIUtility.TrTextContent("Based Upon (at Start)", "What the vertical root position is based upon.");


            public static GUIContent[] BasedUponPositionYHumanOpt =
            {
                EditorGUIUtility.TrTextContent("Original", "Keeps the vertical position as it is authored in the source file."),
                EditorGUIUtility.TrTextContent("Center of Mass", "Keeps the center of mass aligned with root transform position."),
                EditorGUIUtility.TrTextContent("Feet", "Keeps the feet aligned with the root transform position.")
            };

            public static GUIContent[] BasedUponPositionYOpt =
            {
                EditorGUIUtility.TrTextContent("Original", "Keeps the vertical position as it is authored in the source file."),
                EditorGUIUtility.TrTextContent("Root Node Position")
            };

            public static GUIContent BakeIntoPosePositionXZ = EditorGUIUtility.TrTextContent("Bake Into Pose", "Enable to make horizontal root motion be baked into the movement of the bones. Disable to make horizontal root motion be stored as root motion.");

            public static GUIContent BasedUponPositionXZ = EditorGUIUtility.TrTextContent("Based Upon", "What the horizontal root position is based upon.");
            public static GUIContent BasedUponStartPositionXZ = EditorGUIUtility.TrTextContent("Based Upon (at Start)", "What the horizontal root position is based upon.");

            public static GUIContent[] BasedUponPositionXZHumanOpt =
            {
                EditorGUIUtility.TrTextContent("Original", "Keeps the horizontal position as it is authored in the source file."),
                EditorGUIUtility.TrTextContent("Center of Mass", "Keeps the center of mass aligned with root transform position.")
            };

            public static GUIContent[] BasedUponPositionXZOpt =
            {
                EditorGUIUtility.TrTextContent("Original", "Keeps the horizontal position as it is authored in the source file."),
                EditorGUIUtility.TrTextContent("Root Node Position")
            };

            public static GUIContent Mirror = EditorGUIUtility.TrTextContent("Mirror", "Mirror left and right in this clip.");

            public static GUIContent Curves = EditorGUIUtility.TrTextContent("Curves", "Parameter-related curves.");
            public static GUIContent Length = EditorGUIUtility.TrTextContent("Length");
            public static GUIContent AddLoopFrame = EditorGUIUtility.TrTextContent("Add Loop Frame");
            public static GUIContent WrapMode = EditorGUIUtility.TrTextContent("Wrap Mode");
            public static GUIContent Events = EditorGUIUtility.TrTextContent("Events");
            public static GUIContent LoopMatch = EditorGUIUtility.TrTextContent("loop match");

            public static string InvalidMultiSelection = L10n.Tr("Both legacy and non legacy Animation Clips have been selected. This combination cannot be edited together. Select either legacy or non legacy Animation Clips.");

            public static GUIContent AddEventContent = EditorGUIUtility.TrIconContent("Animation.AddEvent", "Add Event.");

            public static GUIContent GreenLightIcon = EditorGUIUtility.IconContent("lightMeter/greenLight");
            public static GUIContent LightRimIcon = EditorGUIUtility.IconContent("lightMeter/lightRim");
            public static GUIContent OrangeLightIcon = EditorGUIUtility.IconContent("lightMeter/orangeLight");
            public static GUIContent RedLightIcon = EditorGUIUtility.IconContent("lightMeter/redLight");

            public static GUIContent PrevKeyContent = EditorGUIUtility.TrIconContent("Animation.PrevKey", "Go to previous key frame.");
            public static GUIContent NextKeyContent = EditorGUIUtility.TrIconContent("Animation.NextKey", "Go to next key frame.");
            public static GUIContent AddKeyframeContent = EditorGUIUtility.TrIconContent("Animation.AddKeyframe", "Add Keyframe.");

            public static GUIContent AddEvent = EditorGUIUtility.TrTextContent("Add Animation Event");
            public static GUIContent DeleteEvents = EditorGUIUtility.TrTextContent("Delete Animation Events");
            public static GUIContent DeleteEvent = EditorGUIUtility.TrTextContent("Delete Animation Event");
            public static GUIContent CopyEvents = EditorGUIUtility.TrTextContent("Copy Animation Events");
            public static GUIContent PasteEvents = EditorGUIUtility.TrTextContent("Paste Animation Events");
        }

        private IMGUIContainer m_Ruler;
        private ScrollView m_ContentContainer;
        private TimeArea m_TimeArea;

        public float startTime { get; set; }
        public float stopTime { get; set; } = 100;
        public float frameRate { get; set; } = 60;
        public override VisualElement contentContainer => m_ContentContainer;

        public RulerView()
        {
            styleSheets.Add(CoreGUI.VisualElement.DefaultStyleSheet);

            m_TimeArea = new TimeArea(true)
            {
                hRangeLocked = false,
                vRangeLocked = true,
                hSlider = true,
                vSlider = false,
                hRangeMin = startTime,
                hRangeMax = stopTime,
                margin = 10,
                scaleWithWindow = true,
                minWidth = 1.0f / frameRate,
                ignoreScrollWheelUntilClicked = true,
            };
            m_TimeArea.SetShownHRangeInsideMargins(startTime, stopTime);
            m_TimeArea.hTicks.SetTickModulosForFrameRate(frameRate);

            m_Ruler = new IMGUIContainer(OnGUI);
            m_ContentContainer = new ScrollView();
            m_ContentContainer.horizontalScrollerVisibility = ScrollerVisibility.Hidden;
            m_ContentContainer.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            
            hierarchy.Add(m_Ruler);
            hierarchy.Add(m_ContentContainer);
        }
        private void OnGUI()
        {
            bool changedStart = false;
            bool changedStop = false;
            bool changedAdditivePoseFrame = false;

            ClipRangeGUI(
                ref m_DraggingStartFrame, ref m_DraggingStopFrame, 
                out changedStart, out changedStop, false, 
                ref m_AdditivePoseFrame, out changedAdditivePoseFrame);
        }

        private bool
            m_DraggingRange, m_DraggingRangeBegin, m_DraggingRangeEnd;
        private float m_DraggingStartFrame = 0;
        private float m_DraggingStopFrame = 0;
        private float m_DraggingAdditivePoseFrame = 0;

        private bool m_LoopTime = false;
        private bool m_LoopBlend = false;
        private bool m_LoopBlendOrientation = false;
        private bool m_LoopBlendPositionY = false;
        private bool m_LoopBlendPositionXZ = false;
        private float m_StartFrame = 0;
        private float m_StopFrame = 1;
        private float m_AdditivePoseFrame = 0;
        private float m_InitialClipLength = 0;

        public float m_PlayPoint = 50;

        public void ClipRangeGUI(ref float startFrame, ref float stopFrame, out bool changedStart, out bool changedStop, bool showAdditivePoseFrame, ref float additivePoseframe, out bool changedAdditivePoseframe)
        {
            changedStart = false;
            changedStop = false;
            changedAdditivePoseframe = false;

            m_DraggingRangeBegin = false;
            m_DraggingRangeEnd = false;

            bool invalidRange = (
                startFrame + 0.01f < startTime * frameRate ||
                startFrame - 0.01f > stopTime * frameRate ||
                stopFrame + 0.01f < startTime * frameRate ||
                stopFrame - 0.01f > stopTime * frameRate);
            bool fixRange = false;
            if (invalidRange)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                GUILayout.Label("The clip range is outside of the range of the source take.", EditorStyles.wordWrappedMiniLabel);
                GUILayout.FlexibleSpace();
                GUILayout.BeginVertical();
                GUILayout.Space(5);
                if (GUILayout.Button("Clamp Range"))
                    fixRange = true;
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
            }

            // Time line
            Rect timeRect = GUILayoutUtility.GetRect(10, 18 + 15);
            GUI.Label(timeRect, "", "TE Toolbar");
            if (Event.current.type == EventType.Repaint)
                m_TimeArea.rect = timeRect;
            m_TimeArea.BeginViewGUI();
            m_TimeArea.EndViewGUI();
            timeRect.height -= 15;

            // Start and stop markers
            int startHandleId = GUIUtility.GetControlID(3126789, FocusType.Passive);
            int stopHandleId = GUIUtility.GetControlID(3126789, FocusType.Passive);
            int additiveHandleId = GUIUtility.GetControlID(3126789, FocusType.Passive);
            GUI.BeginGroup(new Rect(timeRect.x + 1, timeRect.y + 1, timeRect.width - 2, timeRect.height - 2));
            {
                timeRect.x = timeRect.y = -1;

                // Draw selected range as blue tint
                float startPixel = m_TimeArea.FrameToPixel(startFrame, frameRate, timeRect);
                float stopPixel = m_TimeArea.FrameToPixel(stopFrame, frameRate, timeRect);
                GUI.Label(new Rect(startPixel, timeRect.y, stopPixel - startPixel, timeRect.height), "", EditorStyles.selectionRect);

                // Draw time ruler
                m_TimeArea.TimeRuler(timeRect, frameRate);
                // Current time indicator
                // TODO: TEST
                TimeArea.DrawPlayhead(m_PlayPoint, timeRect.yMin, timeRect.yMax, 2f, 1f);

                using (new EditorGUI.DisabledScope(invalidRange))
                {
                    // Range handles
                    float startTime = startFrame / frameRate;
                    TimeArea.TimeRulerDragMode inPoint = m_TimeArea.BrowseRuler(timeRect, startHandleId, ref startTime, 0, false, "TL InPoint");
                    if (inPoint == TimeArea.TimeRulerDragMode.Cancel)
                    {
                        startFrame = m_DraggingStartFrame;
                    }
                    else if (inPoint != TimeArea.TimeRulerDragMode.None)
                    {
                        startFrame = startTime * frameRate;
                        // Snapping bias. Snap to whole frames when zoomed out.
                        startFrame = CoreGUI.MathUtilsExt.RoundBasedOnMinimumDifference(startFrame, m_TimeArea.PixelDeltaToTime(timeRect) * frameRate * 10);
                        changedStart = true;
                    }
                    float stopTime = stopFrame / frameRate;

                    TimeArea.TimeRulerDragMode outPoint = m_TimeArea.BrowseRuler(timeRect, stopHandleId, ref stopTime, 0, false, "TL OutPoint");
                    if (outPoint == TimeArea.TimeRulerDragMode.Cancel)
                    {
                        stopFrame = m_DraggingStopFrame;
                    }
                    else if (outPoint != TimeArea.TimeRulerDragMode.None)
                    {
                        stopFrame = stopTime * frameRate;
                        // Snapping bias. Snap to whole frames when zoomed out.
                        stopFrame = CoreGUI.MathUtilsExt.RoundBasedOnMinimumDifference(stopFrame, m_TimeArea.PixelDeltaToTime(timeRect) * frameRate * 10);
                        changedStop = true;
                    }

                    // Additive pose frame Handle
                    if (showAdditivePoseFrame)
                    {
                        float additivePoseTime = additivePoseframe / frameRate;
                        TimeArea.TimeRulerDragMode additivePoint = m_TimeArea.BrowseRuler(timeRect, additiveHandleId, ref additivePoseTime, 0, false, "TL playhead");
                        if (additivePoint == TimeArea.TimeRulerDragMode.Cancel)
                        {
                            additivePoseframe = m_DraggingAdditivePoseFrame;
                        }
                        else if (additivePoint != TimeArea.TimeRulerDragMode.None)
                        {
                            additivePoseframe = additivePoseTime * frameRate;
                            // Snapping bias. Snap to whole frames when zoomed out.
                            additivePoseframe = CoreGUI.MathUtilsExt.RoundBasedOnMinimumDifference(additivePoseframe, m_TimeArea.PixelDeltaToTime(timeRect) * frameRate * 10);
                            changedAdditivePoseframe = true;
                        }
                    }
                }

                if (EditorGUIUtility.hotControl == startHandleId)
                    changedStart = true;
                if (EditorGUIUtility.hotControl == stopHandleId)
                    changedStop = true;
                if (EditorGUIUtility.hotControl == additiveHandleId)
                    changedAdditivePoseframe = true;
            }
            GUI.EndGroup();

            // Start and stop time float fields
            using (new EditorGUI.DisabledScope(invalidRange))
            {
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUI.BeginChangeCheck();
                    startFrame = EditorGUILayout.FloatField(Styles.StartFrame, Mathf.Round(startFrame * 1000) / 1000);
                    if (EditorGUI.EndChangeCheck())
                        changedStart = true;

                    GUILayout.FlexibleSpace();

                    EditorGUI.BeginChangeCheck();
                    stopFrame = EditorGUILayout.FloatField(Styles.EndFrame, Mathf.Round(stopFrame * 1000) / 1000);
                    if (EditorGUI.EndChangeCheck())
                        changedStop = true;
                }
                EditorGUILayout.EndHorizontal();
            }

            changedStart |= fixRange;
            changedStop |= fixRange;

            // Start and stop time value clamping
            if (changedStart)
                startFrame = Mathf.Clamp(startFrame, startTime * frameRate, Mathf.Clamp(stopFrame, startTime * frameRate, stopFrame));

            if (changedStop)
                stopFrame = Mathf.Clamp(stopFrame, startFrame, stopTime * frameRate);

            if (changedAdditivePoseframe)
                additivePoseframe = Mathf.Clamp(additivePoseframe, startTime * frameRate, stopTime * frameRate);

            // Keep track of whether we're currently dragging the range or not
            if (changedStart || changedStop || changedAdditivePoseframe)
            {
                if (!m_DraggingRange)
                    m_DraggingRangeBegin = true;
                m_DraggingRange = true;
            }
            else if (m_DraggingRange && EditorGUIUtility.hotControl == 0 && Event.current.type == EventType.Repaint)
            {
                m_DraggingRangeEnd = true;
                m_DraggingRange = false;
                //m_DirtyQualityCurves = true;
                //Repaint();
            }

            GUILayout.Space(10);
        }
    }
}

#endif