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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_EDITOR
#endif

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine.UIElements;

namespace Point.Collections
{
    public class TextParser
    {
        private char m_Seperator;
        private string m_Text;

        private string[][] m_Lines;

        public string[][] Lines => m_Lines;

        public string[] this[int index]
        {
            get
            {
                return Lines[index];
            }
        }
        public string this[int index, int innerIndex]
        {
            get
            {
                return Lines[index][innerIndex];
            }
            set
            {
                Lines[index][innerIndex] = value;
            }
        }

        public TextParser(char seperator, string text
#if UNITY_2021_1_OR_NEWER
            , StringSplitOptions options = StringSplitOptions.RemoveEmptyEntries
#endif
            )
        {
            m_Seperator = seperator;
            m_Text = text;

            List<string[]> lines = new List<string[]>();
            using (StringReader st = new StringReader(text))
            {
                string line = null;
                int index = 0;
                while ((line = st.ReadLine()) != null)
                {
                    string[] row = line.Split(m_Seperator
#if UNITY_2021_1_OR_NEWER
                        , options
#endif
                        );

                    OnProcessLine(index, line, row);

                    lines.Add(row);
                    index++;
                }
            }

            m_Lines = lines.ToArray();
            OnLineConstructed(m_Lines);
        }

        public StringBuilder Build()
        {
            StringBuilder b = new StringBuilder();
            for (int i = 0; i < m_Lines.Length; i++)
            {
                string line = string.Empty;
                for (int j = 0; j < m_Lines[i].Length; j++)
                {
                    if (!line.IsNullOrEmpty()) line += m_Seperator;

                    line += m_Lines[i][j];
                }

                b.AppendLine(line);
            }

            return b;
        }

        protected virtual void OnProcessLine(int index, string raw, string[] elements) { }
        protected virtual void OnLineConstructed(string[][] lines) { }
    }
}
