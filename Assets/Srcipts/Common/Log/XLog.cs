using System;
using UnityEngine;

public class XLog
    {
        enum LogLevel
        {
            None = 0,
            Exception = 1,
            Error = 2,
            Warning = 3,
            Normal = 4,
            Max = 5,
        }

        private static LogLevel logLevel = LogLevel.Normal;

        [System.Diagnostics.Conditional("LOG_ON")]
        public static void I(object msg)
        {
            if (logLevel < LogLevel.Normal)
            {
                return;
            }
            Debug.Log(msg);
        }
        
        [System.Diagnostics.Conditional("LOG_ON")]
        public static void I(string msg)
        {
            if (logLevel < LogLevel.Normal)
            {
                return;
            }
            Debug.Log(msg);
        }

        [System.Diagnostics.Conditional("LOG_ON")]
        public static void I(string msg, params object[] args)
        {
            if (logLevel < LogLevel.Normal)
            {
                return;
            }
            Debug.LogFormat(msg, args);
        }

        [System.Diagnostics.Conditional("LOG_ON")]
        public static void E(object msg)
        {
            if (logLevel < LogLevel.Error)
            {
                return;
            }
            Debug.LogError(msg);
        }

        [System.Diagnostics.Conditional("LOG_ON")]
        public static void E(Exception e)
        {
            if (logLevel < LogLevel.Exception)
            {
                return;
            }
            Debug.LogException(e);
        }
        
        [System.Diagnostics.Conditional("LOG_ON")]
        public static void E(string msg, params object[] args)
        {
            if (logLevel < LogLevel.Error)
            {
                return;
            }
            Debug.LogErrorFormat(msg, args);
        }

        [System.Diagnostics.Conditional("LOG_ON")]
        public static void W(object msg)
        {
            if (logLevel < LogLevel.Warning)
            {
                return;
            }
            Debug.LogWarning(msg);
        }

        [System.Diagnostics.Conditional("LOG_ON")]
        public static void W(string msg, params object[] args)
        {
            if (logLevel < LogLevel.Warning)
            {
                return;
            }
            Debug.LogWarningFormat(msg, args);
        }
    }

