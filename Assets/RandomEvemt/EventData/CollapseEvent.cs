using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using UniRx;

[CreateAssetMenu(fileName = "CollapseRelicEvent", menuName = "Game/Event/CollapseRelic")]
public class CollapseRelicEvent : BaseEventData
{
    public override bool OnAccept()
    {
        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("[CollapseRelicEvent] PlayerStatus.Instance が見つかりません。");
            return false;
        }

        // 1. プレイヤーの所持図形からランダムで2つ失う
        LoseRandomTwoShapes(player);

        // 2. レジェンダリー図形をランダムで1つ獲得
        ShapeBaseStats legendaryShape = GetRandomLegendaryShape();
        if (legendaryShape != null)
        {
            player.AddShape(legendaryShape); // 例: 図形をインベントリに追加
            DebugUtility.Log($"[CollapseRelicEvent] {legendaryShape.ShapeName} (Legendary) を獲得しました。");            return true;
        }
        else
        {
            DebugUtility.LogWarning("[CollapseRelicEvent] レジェンダリー図形を取得できませんでした。");
        }
        return true;
    }

    private static void LoseRandomTwoShapes(Player player)
    {
        ReactiveCollection<ShapeBaseStats> playerShapes = player.shapeManager.OwnedShapes; // 例: プレイヤーの図形リスト取得メソッド
        if (playerShapes != null && playerShapes.Count > 1)
        {
            for (int i = 0; i < 2; i++)
            {
                int randomIndex = Random.Range(0, playerShapes.Count);
                ShapeBaseStats shapeToRemove = playerShapes[randomIndex];

                player.RemoveShape(shapeToRemove);  // 例: プレイヤーの図形インベントリから削除するメソッド
                DebugUtility.Log($"[CollapseRelicEvent] {shapeToRemove.ShapeName} を失いました。");            }
        }
        else
        {
            DebugUtility.Log("[CollapseRelicEvent] プレイヤーが図形を所持していません。");        }
    }

    public override void OnDecline()
    {
        // 拒否した場合の処理。必要があればデメリット等を入れる
        DebugUtility.Log("[CollapseRelicEvent] イベントを拒否しました。何も起こりません。");    }

    /// <summary>
    /// レジェンダリー(Rarity.Legendary)の図形をランダムに取得するメソッド
    /// </summary>
    private ShapeBaseStats GetRandomLegendaryShape()
    {
        // 例: ShapeDex.Instance の全図形リストからLegendaryだけ抽出
        List<ShapeBaseStats> allShapes = ShapeDex.Instance.GetAllShapes();
        if (allShapes == null || allShapes.Count == 0)
        {
            return null;
        }

        // RarityがLegendaryのものだけを抽出
        List<ShapeBaseStats> legendaryList = allShapes.FindAll(s => s.Rarity == Rarity.Legendary);
        if (legendaryList.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, legendaryList.Count);
        return legendaryList[randomIndex];
    }
}
