using System;
using UnityEngine;

namespace VmodMonkeMapLoader.Helpers
{
    public static class Logger
    {
        public static void LogText(string text)
        {
            var logText = $"{DateTime.Now}: MML LOG: {text}";
            Debug.Log(logText);
        }

        public static void LogException(Exception ex)
        {
            var logText = $"{DateTime.Now}: MML LOG ERROR: {ex}";
            Debug.Log(logText);
        }
    }
}