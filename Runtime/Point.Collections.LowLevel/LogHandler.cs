using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Point.Collections.LowLevel
{
    // https://github.com/scissor/Hotter/blob/master/Hotter/Utilities/Debug.cs
    public sealed class LogHandler : ILogHandler
    {
        private const string c_Tag = "PointFramework";

        private readonly ILogHandler m_DefaultHandler;

        public LogHandler()
        {
            m_DefaultHandler = Debug.unityLogger.logHandler;

            Debug.unityLogger.logHandler = this;
        }

        public void LogException(Exception exception, UnityEngine.Object context)
        {
            m_DefaultHandler.LogException(exception, context);
        }

        public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
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

            m_DefaultHandler.LogFormat(logType, context, format, args);
        }
    }
}
