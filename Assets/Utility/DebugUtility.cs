using UnityEngine;
using System.Diagnostics;
// ==========================
// ログを出力するためのユーティリティ
// ==========================
public static class DebugUtility
{
    // ログレベル
    public enum LogLevel { None, Info, Warning, Error }
    // 現在のログレベル
    public static LogLevel CurrentLogLevel = LogLevel.Info; 

    // ==========================
    // ログ
    // ==========================
    [Conditional("UNITY_EDITOR")]
    public static void Log(string message)
    {
        // デバッグビルドかつログレベルがInfo以上の場合のみログを出力
        if (UnityEngine.Debug.isDebugBuild && CurrentLogLevel <= LogLevel.Info)
            UnityEngine.Debug.Log(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void Log(string message, Object context)
    {
        // デバッグビルドかつログレベルがInfo以上の場合のみログを出力
        if (UnityEngine.Debug.isDebugBuild && CurrentLogLevel <= LogLevel.Info)
            UnityEngine.Debug.Log(message, context);
    }

    // ==========================
    // 警告
    // ==========================
    [Conditional("UNITY_EDITOR")]
    public static void LogWarning(string message)
    {
        // デバッグビルドかつログレベルがWarning以上の場合のみログを出力
        if (UnityEngine.Debug.isDebugBuild && CurrentLogLevel <= LogLevel.Warning)
            UnityEngine.Debug.LogWarning(message);
    }

    [Conditional("UNITY_EDITOR")]
    public static void LogWarning(string message, Object context)
    {
        // デバッグビルドかつログレベルがWarning以上の場合のみログを出力
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
