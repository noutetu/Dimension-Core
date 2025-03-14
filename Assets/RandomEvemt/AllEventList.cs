using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllEventList : MonoBehaviour
{
    public List<BaseEventData> allEvents = new List<BaseEventData>();

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
    // ランダムなイベントを取得
    // ==========================
    public BaseEventData GetRandomEvent()
    {
        if (allEvents.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, allEvents.Count);
        BaseEventData randomEvent = allEvents[randomIndex];
        return randomEvent;
    }
}
