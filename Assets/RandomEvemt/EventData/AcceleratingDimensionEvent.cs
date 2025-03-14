using UnityEngine;

[CreateAssetMenu(fileName = "AcceleratingDimensionEvent", menuName = "Game/Event/AcceleratingDimension")]
public class AcceleratingDimensionEvent : BaseEventData
{

    [Header("回避減少量")]
    public float evasionReduction = 10.0f;
    [Header("速度増加量")]
    public float speedIncrease = 20.0f;
    // イベント名 (BaseEventDataで定義された)
    // public string eventTitle; // ScriptableObjectのインスペクタでセット

    public override bool OnAccept()
    {
        // 全ての図形の回避率が10%減少し、速度が20%増加
        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("Playerオブジェクトが見つかりません！");
            return false;
        }
            player.ReduceEvasionByPercentage(evasionReduction);
            player.EnhanceSpeedByPercentage(speedIncrease);
        

        DebugUtility.Log("[AcceleratingDimensionEvent] 全ての図形の回避率が10%減少し、速度が20%増加しました。");
        return true;
    }

    public override void OnDecline()
    {
        // 拒否した場合は特に何もしない
        DebugUtility.Log("[AcceleratingDimensionEvent] イベントを拒否しました。");    }
}
