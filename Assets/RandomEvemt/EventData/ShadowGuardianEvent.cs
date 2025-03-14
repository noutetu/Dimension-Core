using UnityEngine;

[CreateAssetMenu(fileName = "ShadowGuardianEvent", menuName = "Game/Event/ShadowGuardian")]
public class ShadowGuardianEvent : BaseEventData
{
    // イベント名 (BaseEventDataで定義された)
    // public string eventTitle; // ScriptableObjectのインスペクタでセット

    public override bool OnAccept()
    {
        // 全ての図形の速度が20%減少し、回避率が10%増加
        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("Playerオブジェクトが見つかりません！");
            return false;
        }

        player.ReduceSpeedByPercentage(20);
        player.EnhanceEvasionByPercentage(10);

        DebugUtility.Log("[ShadowGuardianEvent] 全ての図形の速度が20%減少し、回避率が10%増加しました。");
        return true;
    }

    public override void OnDecline()
    {
        // 拒否した場合は特に何もしない
        DebugUtility.Log("[ShadowGuardianEvent] イベントを拒否しました。");    }
}
