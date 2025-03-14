using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NoCostChoiceEvent", menuName = "Game/Event/NoCostChoice")]
public class NoCostChoiceEvent : BaseEventData
{
    [Tooltip("入手する最低レアリティ")]
    [SerializeField] private Rarity minimumRarity = Rarity.Rare; // Rare以上

    // イベント名 (BaseEventDataで定義された)
    // public string eventTitle; // ScriptableObjectのインスペクタでセット

    public override bool OnAccept()
    {
        // 1. 次の戦闘の敵が1段階強化される
        GameDifficultyManager.Instance.strongNext = true;

        // 2. プレイヤーが好きなレア図形を1つ選んで獲得
        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("Playerオブジェクトが見つかりません。");
            return false;
        }

        ShapeBaseStats randomShape = GetRandomShapeOfRarity(minimumRarity);
        if (randomShape != null)
        {
            // 例: インベントリに追加する、または即時装備するなど
            player.AddShape(randomShape);
            return true;
        }
        else
        {
            DebugUtility.LogWarning("該当するレアリティ以上の図形が見つかりません。");
            return false;
        }
    }

    public override void OnDecline()
    {
        // 拒否した場合は特に何もしない
        DebugUtility.Log("[NoCostChoiceEvent] イベントを拒否しました。");
    }

    /// <summary>
    /// minimumRarity以上のランダムなShapeBaseStatsを返す
    /// 例: ShapeDexや他の管理クラスを参照する想定
    /// </summary>
    private ShapeBaseStats GetRandomShapeOfRarity(Rarity minRarity)
    {
        // 全Shapeリストを仮定 (ShapeDex.Instance 等から取得)
        List<ShapeBaseStats> allShapes = ShapeDex.Instance.GetAllShapes();
        if (allShapes == null || allShapes.Count == 0) return null;

        // コモンかレアのShapeだけを抽出
        List<ShapeBaseStats> filtered = allShapes.FindAll(s => s.Rarity == Rarity.Common || s.Rarity == Rarity.Rare);
        if (filtered.Count == 0) return null;

        // ランダムに1つ選んで返す
        int index = Random.Range(0, filtered.Count);
        return filtered[index];
    }
}
