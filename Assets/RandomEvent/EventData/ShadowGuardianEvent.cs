using UnityEngine;
// ===================================
// 速度が減少する代わりに回避率が増加するイベント
// ===================================
[CreateAssetMenu(fileName = "ShadowGuardianEvent", menuName = "Game/Event/ShadowGuardian")]
public class ShadowGuardianEvent : BaseEventData
{
    [Header("速度減少率")]
    [SerializeField] private float speedReductionRate = 20f;
    [Header("回避率増加率")]
    [SerializeField] private float evasionIncreaseRate = 10f;
    // ===================================
    // イベントを受け入れた場合の処理
    // ===================================
    public override bool OnAccept()
    {
        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("Playerオブジェクトが見つかりません！");
            return false;
        }
        // 速度が減少
        player.ReduceSpeedByPercentage(speedReductionRate);
        // 回避率が増加
        player.EnhanceEvasionByPercentage(evasionIncreaseRate);

        DebugUtility.Log("[ShadowGuardianEvent] 全ての図形の速度が20%減少し、回避率が10%増加しました。");
        return true;
    }

    public override void OnDecline()
    {
        // 拒否した場合は特に何もしない
        DebugUtility.Log("[ShadowGuardianEvent] イベントを拒否しました。");
    }
}
