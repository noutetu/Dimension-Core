using UnityEngine;
using System.Collections.Generic;
// ==================================================
// 次の戦闘の敵が1段階強化される代わりに、レア図形を一つ入手するイベント
// ==================================================
[CreateAssetMenu(fileName = "NoCostChoiceEvent", menuName = "Game/Event/NoCostChoice")]
public class NoCostChoiceEvent : BaseEventData
{
    [Tooltip("入手する最低レアリティ")]
    [SerializeField] private Rarity minimumRarity = Rarity.星2; // Rare以上
    // ==================================================
    // イベントを受け入れた時の処理
    // ==================================================
    public override bool OnAccept()
    {
        // 次の戦闘の敵が1段階強化される
        GameDifficultyManager.Instance.isStrongEnemy = true;

        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("Playerオブジェクトが見つかりません。");
            return false;
        }
        // 指定されたレアリティ以上の図形をランダムに 1つ取得
        ShapeBaseStats randomShape = GetRandomShapeOfRarity(minimumRarity);
        if (randomShape != null)
        {
            // 図形をインベントリに追加
            player.AddShape(randomShape);
            return true;
        }
        else
        {
            DebugUtility.LogWarning("該当するレアリティ以上の図形が見つかりません。");
            return false;
        }
    }
    // ==================================================
    // イベントを拒否した時の処理
    // ==================================================
    public override void OnDecline()
    {
        // 拒否した場合は特に何もしない
        DebugUtility.Log("[NoCostChoiceEvent] イベントを拒否しました。");
    }

    // ==================================================
    // レアリティ以上のランダムな図形を取得するメソッド
    // ==================================================
    private ShapeBaseStats GetRandomShapeOfRarity(Rarity minRarity)
    {
        // 全Shapeリストを仮定 (ShapeDex.Instance 等から取得)
        List<ShapeBaseStats> allShapes = ShapeDex.Instance.GetAllShapes();
        if (allShapes == null || allShapes.Count == 0) return null;

        // コモンかレアのShapeだけを抽出
        List<ShapeBaseStats> filtered = allShapes.FindAll(s => s.rarity == Rarity.星1 || s.rarity == Rarity.星2);
        if (filtered.Count == 0) return null;

        // ランダムに1つ選んで返す
        int index = Random.Range(0, filtered.Count);
        return filtered[index];
    }
}
