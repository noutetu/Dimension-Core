using UnityEngine;

[CreateAssetMenu(fileName = "DesireDarknessEvent", menuName = "Game/Event/DesireDarkness")]
public class DesireDarknessEvent : BaseEventData
{
    public override bool OnAccept()
    {
        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("Playerオブジェクトが見つかりません。");
            return false;
        }

        // 全ての図形のクリティカル率を15%増加し、回避率を15%減少
        player.EnhanceCriticalByPercentage(15f);
        player.ReduceEvasionByPercentage(15f);

        DebugUtility.Log("[DesireDarknessEvent] 全ての図形のクリティカル率が15%増加し、回避率が15%減少しました。");        return true;
    }

    public override void OnDecline()
    {
        // 拒否した場合は特に何もしない
        DebugUtility.Log("[DesireDarknessEvent] イベントを拒否しました。");    }
}
