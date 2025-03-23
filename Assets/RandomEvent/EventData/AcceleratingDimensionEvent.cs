using UnityEngine;
// ================================
// 回避が減少する代わりに速度が増加するイベント
// ================================
[CreateAssetMenu(fileName = "AcceleratingDimensionEvent", menuName = "Game/Event/AcceleratingDimension")]
public class AcceleratingDimensionEvent : BaseEventData
{

    [Header("回避減少量")]
    public float evasionReduction = 10.0f;
    [Header("速度増加量")]
    public float speedIncrease = 20.0f;
    // ================================
    // イベントを受け入れた際の処理
    // ================================
    public override bool OnAccept()
    {
        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("Playerオブジェクトが見つかりません！");
            return false;
        }
        // 回避を減少
        player.ReduceEvasionByPercentage(evasionReduction);
        // 速度を増加
        player.EnhanceSpeedByPercentage(speedIncrease);

        DebugUtility.Log("[AcceleratingDimensionEvent] 全ての図形の回避率が10%減少し、速度が20%増加しました。");
        return true;
    }

    public override void OnDecline()
    {
        // 拒否した場合は特に何もしない
        DebugUtility.Log("[AcceleratingDimensionEvent] イベントを拒否しました。");
    }
}
