using UnityEngine;

[CreateAssetMenu(fileName = "CausalTradeEvent", menuName = "Game/Event/CausalTrade")]
public class CausalTradeEvent : BaseEventData
{
    // イベント名 (BaseEventDataで定義された)
    // public string eventTitle; // ScriptableObjectのインスペクタでセット

    [Header("金額")]
    [SerializeField] private int price;
    public override bool OnAccept()
    {
        EnemyGenerator enemyGenerator = FindObjectOfType<EnemyGenerator>();
        enemyGenerator.SetEnemyBuff(
            0f, // 体力
            200f, // 攻撃力
            0f, // 速度
            0f,  // 回避
            0f    // クリティカル
        );

        // 通貨が100増加
        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("Playerオブジェクトが見つかりません！");
            return false;
        }

        player.EarnCurrency(price);
        DebugUtility.Log("[CausalTradeEvent] 通貨が100増加しました。");
        return true;
    }

    public override void OnDecline()
    {
        // 拒否した場合は特に何もしない
        DebugUtility.Log("[CausalTradeEvent] イベントを拒否しました。");    }
}
