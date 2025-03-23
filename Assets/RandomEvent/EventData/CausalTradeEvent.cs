using UnityEngine;
// ================================
// 次の戦闘で敵の攻撃力が増える代わりに通貨が増加するイベント 
// ================================
[CreateAssetMenu(fileName = "CausalTradeEvent", menuName = "Game/Event/CausalTrade")]
public class CausalTradeEvent : BaseEventData
{
    [Header("通貨の増加量")]
    [SerializeField] private int price;
    [Header("敵の攻撃力の増加量")]
    [SerializeField] private int attackPower;
    // ================================
    // 受け入れた時の処理
    // ================================
    public override bool OnAccept()
    {
        // 敵ジェネレーターを取得
        EnemyGenerator enemyGenerator = FindObjectOfType<EnemyGenerator>();
        // 敵の攻撃力を増加
        enemyGenerator.SetEnemyBuff(
            0f, // 体力
            200f, // 攻撃力
            0f, // 速度
            0f,  // 回避
            0f    // クリティカル
        );

        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("Playerオブジェクトが見つかりません！");
            return false;
        }

        // 通貨が増加
        player.EarnCurrency(price);
        DebugUtility.Log("[CausalTradeEvent] 通貨が100増加しました。");
        return true;
    }
    // ================================
    // 拒否した時の処理
    // ================================
    public override void OnDecline()
    {
        // 拒否した場合は特に何もしない
        DebugUtility.Log("[CausalTradeEvent] イベントを拒否しました。");
    }
}
