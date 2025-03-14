using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public abstract bool OnAccept();
    public abstract void OnDecline();
}