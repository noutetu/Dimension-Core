using UnityEngine;

[CreateAssetMenu(fileName = "FateCoinEvent", menuName = "Game/Event/FateCoin")]
public class FateCoinEvent : BaseEventData
{
    // イベント名 (BaseEventDataで定義された)
    // public string eventTitle; // ScriptableObjectのインスペクタでセット

    [Header("通貨の増減")]
    [SerializeField] private int currencyChangeAmount; // 通貨の増減量

    public override bool OnAccept()
    {
        // 50%の確率で通貨が200増加または200減少
        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("Playerオブジェクトが見つかりません！");
            return false;
        }

        if (Random.value < 0.5f)
        {
            player.EarnCurrency(currencyChangeAmount);
            DebugUtility.Log("[FateCoinEvent] 通貨が200増加しました。");        }
        else
        {
            player.SpendCurrency(currencyChangeAmount);
            DebugUtility.Log("[FateCoinEvent] 通貨が200減少しました。");        }

        return true;
    }

    public override void OnDecline()
    {
        // 拒否した場合は特に何もしない
        DebugUtility.Log("[FateCoinEvent] イベントを拒否しました。");    }
}
