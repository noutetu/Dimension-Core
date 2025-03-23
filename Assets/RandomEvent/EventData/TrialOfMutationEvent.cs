using UnityEngine;
using System.Collections.Generic;
// ================================
// プレイヤーのランダムな図形を一つ別のレア図形に変化させるイベント
// ================================
[CreateAssetMenu(fileName = "TrialOfMutationEvent", menuName = "Game/Event/TrialOfMutation")]
public class TrialOfMutationEvent : BaseEventData
{
    // ==========================
    // イベントを受け入れた時の処理
    // ==========================
    public override bool OnAccept()
    {
        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("Playerオブジェクトが見つかりません。");
            return false;
        }

        if (player.shapeManager.OwnedShapes.Count > 0)
        {
            // プレイヤーの所持図形からランダムで1つ取得
            int randomIndex = Random.Range(0, player.shapeManager.OwnedShapes.Count);
            ShapeBaseStats shapeToMutate = player.shapeManager.OwnedShapes[randomIndex];

            // 別のレア図形に変化
            ShapeBaseStats newShapeStats = GetRandomRareShape();
            if (newShapeStats != null)
            {
                // 元の図形を削除
                player.RemoveShape(shapeToMutate);
                // 新しい図形を追加
                player.AddShape(newShapeStats);
                DebugUtility.Log($"[TrialOfMutationEvent] 図形 {shapeToMutate.ShapeName} が {newShapeStats.ShapeName} に変化しました。"); return true;
            }
            else
            {
                DebugUtility.LogWarning("[TrialOfMutationEvent] 新しい図形を取得できませんでした。");
            }
        }
        else
        {
            DebugUtility.LogWarning("[TrialOfMutationEvent] 変化させる図形がありません。");
            return false;
        }

        return true;
    }

    public override void OnDecline()
    {
        // 拒否した場合は特に何もしない
        DebugUtility.Log("[TrialOfMutationEvent] イベントを拒否しました。");
    }

    // ======================================================================
    // ランダムなレア図形を取得
    // ======================================================================
    private ShapeBaseStats GetRandomRareShape()
    {
        // 例: ShapeDex.Instance の全図形リストからRare以上だけ抽出
        List<ShapeBaseStats> allShapes = ShapeDex.Instance.GetAllShapes();
        if (allShapes == null || allShapes.Count == 0)
        {
            return null;
        }

        // RarityがRareのものだけを抽出
        List<ShapeBaseStats> rareList = allShapes.FindAll(s => s.rarity == Rarity.星2);
        if (rareList.Count == 0)
        {
            return null;
        }
        // ランダムに1つ選んで返す
        int randomIndex = Random.Range(0, rareList.Count);
        return rareList[randomIndex];
    }
}
