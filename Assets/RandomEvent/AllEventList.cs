using System.Collections.Generic;
using UnityEngine;
// ==========================
// 全てのイベントのリスト
// ==========================
public class AllEventList : MonoBehaviour
{
    // 全てのイベントのリスト
    public List<BaseEventData> allEvents = new List<BaseEventData>();
    // シングルトン
    public static AllEventList instance;

    // ==========================
    // 初期化
    // ==========================
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // ==========================
    // ランダムなイベントを取得する
    // ==========================
    public BaseEventData GetRandomEvent()
    {
        if (allEvents.Count == 0)
        {
            return null;
        }
        // ランダムなインデックスを取得
        int randomIndex = Random.Range(0, allEvents.Count);
        // ランダムなイベントを取得
        BaseEventData randomEvent = allEvents[randomIndex];
        // イベントを返す
        return randomEvent;
    }
}
