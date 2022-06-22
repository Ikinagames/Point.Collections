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

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Point.Collections
{
    public static class ExtensionMethods
    {
        public static TypeInfo ToTypeInfo(this Type type) => TypeHelper.ToTypeInfo(type);

        #region IConvertible

        public static int ToInt32<T>(this T t) where T : struct, IConvertible => t.ToInt32(System.Globalization.CultureInfo.InvariantCulture);
        public static double ToDouble<T>(this T t) where T : struct, IConvertible => t.ToDouble(System.Globalization.CultureInfo.InvariantCulture);
        public static float ToSingle<T>(this T t) where T : struct, IConvertible => t.ToSingle(System.Globalization.CultureInfo.InvariantCulture);
        public static long ToInt64<T>(this T t) where T : struct, IConvertible => t.ToInt64(System.Globalization.CultureInfo.InvariantCulture);
        public static string ToString<T>(this T t) where T : struct, IConvertible => t.ToString(System.Globalization.CultureInfo.InvariantCulture);

        #endregion

        #region String

        private const char c_StringLineSeperator = '\n';
        private const string c_Space = " ";
        private static string[] s_StringLineSpliter = new[] { "\r\n", "\r", "\n" };
        private static char[] s_StringLineSpliterChar = new[] { '\r', '\n' };

        public static string AddSpace(this string t)
        {
            return t + c_Space;
        }
        /// <summary>
        /// 이 스트링이 null 혹은 비었는지 반환합니다.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static bool IsNullOrEmpty(this string t)
        {
            return string.IsNullOrEmpty(t);
        }
        /// <summary>
        /// 문자열 마지막에 Return iteral 을 추가하여 반환합니다.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ReturnAtLast(this string t)
        {
            return t + c_StringLineSeperator;
        }
        /// <summary>
        /// 문자열 처음에 Return iteral 을 추가하여 반환합니다.
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string ReturnAtFirst(this string t)
        {
            return c_StringLineSeperator + t;
        }

        public static int GetLineCount(this string t)
        {
            return t.GetLines().Length;
        }
        public static string[] GetLines(this string t)
        {
            return t.Split(s_StringLineSpliter, StringSplitOptions.None);
        }
        public static string RemoveLines(this string t, int lineIndex, int count)
        {
            var lines = t.GetLines().ToList();
            lines.RemoveRange(lineIndex, count);

            string sum = string.Empty;
            for (int i = 0; i < lines.Count; i++)
            {
                sum += lines[i];

                if (i + 1 < lines.Count) sum += c_StringLineSeperator;
            }

            return sum;
        }

        #endregion

        #region Monobehaviour

        public static T GetOrAddComponent<T>(this GameObject t)
            where T : UnityEngine.Component
        {
            T p = t.GetComponent<T>();
            if (p == null) p = t.AddComponent<T>();

            return p;
        }
        public static T GetOrAddComponent<T>(this Transform t) where T : UnityEngine.Component => t.gameObject.GetOrAddComponent<T>();

        public static TComponent[] GetComponentsInChildrenOnly<TComponent>(this UnityEngine.Component t) 
        {
            Transform tr = t.transform;
            int count = tr.childCount;
            List<TComponent> components = new List<TComponent>();
            for (int i = 0; i < count; i++)
            {
                components.AddRange(tr.GetChild(i).GetComponentsInChildren<TComponent>());
            }

            return components.ToArray();
        }

        #endregion

        // https://answers.unity.com/questions/1187767/how-to-draw-a-gizmo-on-a-canvas.html
        public static Matrix4x4 GetCanvasMatrix(this Canvas t)
        {
            RectTransform rectTr = t.transform as RectTransform;
            Matrix4x4 canvasMatrix = rectTr.localToWorldMatrix;

            canvasMatrix *= Matrix4x4.Translate(-rectTr.sizeDelta * .5f);

            Vector3 curScale = canvasMatrix.lossyScale;
            canvasMatrix *= Matrix4x4.Scale(new Vector3(1 / curScale.x, 1 / curScale.y, 1 / curScale.z));

            return canvasMatrix;
        }

        public static Texture2D ToTexture2D(this RenderTexture rt)
        {
            Texture2D tex = new Texture2D(rt.width, rt.height, TextureFormat.RGB24, false);
            var old_rt = RenderTexture.active;
            RenderTexture.active = rt;

            tex.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);
            tex.Apply();

            RenderTexture.active = old_rt;
            return tex;
        }
        public static void Reverse(this Texture2D texture)
        {
            Color[] pixels = texture.GetPixels();
            Array.Reverse(pixels);
            texture.SetPixels(pixels);
        }
        public static void Flip(this Texture2D texture)
        {
            int textureWidth = texture.width;
            int textureHeight = texture.height;

            Color32[] pixels = texture.GetPixels32();

            for (int y = 0; y < textureHeight; y++)
            {
                int yo = y * textureWidth;
                for (int il = yo, ir = yo + textureWidth - 1; il < ir; il++, ir--)
                {
                    Color32 col = pixels[il];
                    pixels[il] = pixels[ir];
                    pixels[ir] = col;
                }
            }
            texture.SetPixels32(pixels);
            texture.Apply();
        }
        public static void SaveTextureAsPNG(this Texture2D t, string fullPath)
        {
            byte[] bytes = t.EncodeToPNG();
            System.IO.File.WriteAllBytes(fullPath, bytes);
            //Debug.Log(_bytes.Length / 1024 + "Kb was saved as: " + _fullPath);
        }
        public static byte[] CompressToBytes(this Texture2D source, bool highQuality = false)
        {
            source.Compress(highQuality);

            RenderTexture renderTex = RenderTexture.GetTemporary(
                        source.width,
                        source.height,
                        0,
                        RenderTextureFormat.Default,
                        RenderTextureReadWrite.Linear);

            Graphics.Blit(source, renderTex);
            RenderTexture previous = RenderTexture.active;
            RenderTexture.active = renderTex;
            Texture2D readableText = new Texture2D(source.width, source.height);
            readableText.ReadPixels(new Rect(0, 0, renderTex.width, renderTex.height), 0, 0);
            readableText.Apply();
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(renderTex);

            return readableText.EncodeToPNG();
        }
    }
}
