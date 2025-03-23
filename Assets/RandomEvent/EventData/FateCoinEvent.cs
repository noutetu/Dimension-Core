using UnityEngine;
// ==================================================
// 通貨が50%の確率で200増加または200減少するイベント
// ==================================================
[CreateAssetMenu(fileName = "FateCoinEvent", menuName = "Game/Event/FateCoin")]
public class FateCoinEvent : BaseEventData
{
    [Header("通貨の増減")]
    [SerializeField] private int currencyChangeAmount; // 通貨の増減量
    // ==================================================
    // イベントを受け入れた場合の処理
    // ==================================================
    public override bool OnAccept()
    {
        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("Playerオブジェクトが見つかりません！");
            return false;
        }
        // 50%の確率で通貨が増加
        if (Random.value < 0.5f)
        {
            player.EarnCurrency(currencyChangeAmount);
            DebugUtility.Log("[FateCoinEvent] 通貨が200増加しました。");
        }
        // 50%の確率で通貨が減少
        else
        {
            player.SpendCurrency(currencyChangeAmount);
            DebugUtility.Log("[FateCoinEvent] 通貨が200減少しました。");
        }

        return true;
    }
    // ==================================================
    // イベントを拒否した場合の処理                         
    // ==================================================
    public override void OnDecline()
    {
        // 拒否した場合は特に何もしない
        DebugUtility.Log("[FateCoinEvent] イベントを拒否しました。");
    }
}
