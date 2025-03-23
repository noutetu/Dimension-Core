using UnityEngine;
// ==========================
// イベントデータの基底クラス
// ==========================
public abstract class BaseEventData : ScriptableObject
{   
    [Header("イベント名")]
    public string eventTitle;
    [Header("リスクテキスト")]
    [TextArea]
    public string riskDescription;
    [Header("リターンテキスト")]
    [TextArea]
    public string returnDescription;
    // ==========================
    // イベントを受け入れる時の処理
    // ==========================
    public abstract bool OnAccept();

    // ==========================
    // イベントを拒否する時の処理
    // ==========================
    public abstract void OnDecline();
}