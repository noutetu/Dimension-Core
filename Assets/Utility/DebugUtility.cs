using UnityEngine;
using System.Diagnostics;

public static class DebugUtility
{
    public enum LogLevel { None, Info, Warning, Error }

    public static LogLevel CurrentLogLevel = LogLevel.Info; // デフォルトログレベル

    // ==========================
    // ログ
    // ==========================
    [Conditional("UNITY_EDITOR")]
    public static void Log(string message)
    {
        if (UnityEngine.Debug.isDebugBuild && CurrentLogLevel <= LogLevel.Info)
            UnityEngine.Debug.Log(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void Log(string message, Object context)
    {
        if (UnityEngine.Debug.isDebugBuild && CurrentLogLevel <= LogLevel.Info)
            UnityEngine.Debug.Log(message, context);
    }

    // ==========================
    // 警告
    // ==========================
    [Conditional("UNITY_EDITOR")]
    public static void LogWarning(string message)
    {
        if (UnityEngine.Debug.isDebugBuild && CurrentLogLevel <= LogLevel.Warning)
            UnityEngine.Debug.LogWarning(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogWarning(string message, Object context)
    {
        if (UnityEngine.Debug.isDebugBuild && CurrentLogLevel <= LogLevel.Warning)
            UnityEngine.Debug.LogWarning(message, context);
    }

    // ==========================
    // エラー
    // ==========================
    public static void LogError(string message)
    {
        UnityEngine.Debug.LogError(message); // エラーは常に表示
    }

    public static void LogError(string message, Object context)
    {
        UnityEngine.Debug.LogError(message, context);
    }
}
