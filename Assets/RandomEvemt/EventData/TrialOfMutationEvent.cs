using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "TrialOfMutationEvent", menuName = "Game/Event/TrialOfMutation")]
public class TrialOfMutationEvent : BaseEventData
{
    public override bool OnAccept()
    {
        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("Playerオブジェクトが見つかりません。");
            return false;
        }

        // ランダムな図形を選択
        if (player.shapeManager.OwnedShapes.Count > 0)
        {
            int randomIndex = Random.Range(0, player.shapeManager.OwnedShapes.Count);
            ShapeBaseStats shapeToMutate = player.shapeManager.OwnedShapes[randomIndex];

            // 別のレア図形に変化
            ShapeBaseStats newShapeStats = GetRandomRareShape();
            if (newShapeStats != null)
            {
                player.RemoveShape(shapeToMutate);
                player.AddShape(newShapeStats);
                DebugUtility.Log($"[TrialOfMutationEvent] 図形 {shapeToMutate.ShapeName} が {newShapeStats.ShapeName} に変化しました。");                return true;
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
        DebugUtility.Log("[TrialOfMutationEvent] イベントを拒否しました。");    }

    /// <summary>
    /// レア(Rarity.Rare)以上の図形をランダムに取得するメソッド
    /// </summary>
    private ShapeBaseStats GetRandomRareShape()
    {
        // 例: ShapeDex.Instance の全図形リストからRare以上だけ抽出
        List<ShapeBaseStats> allShapes = ShapeDex.Instance.GetAllShapes();
        if (allShapes == null || allShapes.Count == 0)
        {
            return null;
        }

        // RarityがRare以上のものだけを抽出
        List<ShapeBaseStats> rareList = allShapes.FindAll(s => s.Rarity == Rarity.Rare);
        if (rareList.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, rareList.Count);
        return rareList[randomIndex];
    }
}
