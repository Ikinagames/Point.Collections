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

#if UNITY_EDITOR || DEVELOPMENT_BUILD
#define DEBUG_MODE
#endif

#if UNITY_2020
#define UNITYENGINE
#endif

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITYENGINE
using UnityEngine;

namespace Point.Collections.LowLevel
{
    // https://github.com/scissor/Hotter/blob/master/Hotter/Utilities/Debug.cs
    public sealed class LogHandler : ILogHandler
    {
#line hidden
        private const string c_Tag = "PointFramework";

        private readonly ILogHandler m_DefaultHandler;

        private bool m_EnableLogFile;
        private FileStream m_FileStream;
        private StreamWriter m_Writer;

        public LogHandler()
        {
            m_DefaultHandler = Debug.unityLogger.logHandler;

            m_EnableLogFile = false;
            //SetLogFile("logtest.txt");

            Debug.unityLogger.logHandler = this;
        }

        /// <summary>
        /// �α� ��θ� �����մϴ�.
        /// </summary>
        /// <param name="path">��δ� <see cref="Application.persistentDataPath"/> �������� ���۵˴ϴ�. (ex. SetLogFile("test.txt") => Application.persistentDataPath/test.txt</param>
        public void SetLogFile(string path)
        {
            if (m_EnableLogFile)
            {
                CloseLogFile();
            }

            m_EnableLogFile = true;

            m_FileStream = new FileStream(
                Path.Combine(Application.persistentDataPath, path),
                FileMode.OpenOrCreate, FileAccess.Write);
            m_Writer = new StreamWriter(m_FileStream);
        }
        /// <summary>
        /// <see cref="SetLogFile(string)"/> �޼ҵ带 ȣ���� ��, ���� �α� ������ �ݴ� �޼ҵ��Դϴ�.
        /// </summary>
        public void CloseLogFile()
        {
            if (!m_EnableLogFile) return;

            m_FileStream.Dispose();

            m_FileStream = null;
            m_Writer = null;
            m_EnableLogFile = false;
        }

        void ILogHandler.LogException(Exception exception, UnityEngine.Object context)
        {
            m_DefaultHandler.LogException(exception, context);
        }
        void ILogHandler.LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
        {
            string msg = string.Format(format, args);
            switch (logType)
            {
                case LogType.Log:
                default:
                    break;
                case LogType.Warning:
                    break;
                case LogType.Error:
                    break;
                case LogType.Assert:
                    break;
                case LogType.Exception:
                    break;
            }

            if (m_EnableLogFile)
            {
                m_Writer.WriteLine(msg);
                m_Writer.Flush();
            }

            m_DefaultHandler.LogFormat(logType, context, format, args);
        }
#line default
    }
}

#endif