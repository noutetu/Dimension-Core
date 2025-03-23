using UnityEngine;
// ================================
// クリティカル率が15%増加し、回避率が15%減少するイベント
// ================================
[CreateAssetMenu(fileName = "DesireDarknessEvent", menuName = "Game/Event/DesireDarkness")]
public class DesireDarknessEvent : BaseEventData
{
    [Header("クリティカル増加率")]
    [SerializeField] private float criticalIncreaseRate = 15f;
    [Header("回避減少率")]
    [SerializeField] private float evasionDecreaseRate = 15f;
    public override bool OnAccept()
    {
        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("Playerオブジェクトが見つかりません。");
            return false;
        }

        // 全ての図形のクリティカル率を15%増加し、回避率を15%減少
        player.EnhanceCriticalByPercentage(criticalIncreaseRate);
        player.ReduceEvasionByPercentage(evasionDecreaseRate);

        DebugUtility.Log("[DesireDarknessEvent] 全ての図形のクリティカル率が15%増加し、回避率が15%減少しました。"); return true;
    }
    // ================================
    // イベントを拒否した場合の処理
    // ================================
    public override void OnDecline()
    {
        // 拒否した場合は特に何もしない
        DebugUtility.Log("[DesireDarknessEvent] イベントを拒否しました。");
    }
}
