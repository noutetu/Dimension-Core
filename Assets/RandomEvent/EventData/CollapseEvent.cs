using UnityEngine;
using System.Collections.Generic;
using UniRx;
// ================================
// ランダムに図形を二つ失う代わりに星3の図形を一つ獲得するイベント
// ================================
[CreateAssetMenu(fileName = "CollapseRelicEvent", menuName = "Game/Event/CollapseRelic")]
public class CollapseRelicEvent : BaseEventData
{
    // ================================
    // 受け入れた場合の処理
    // ================================
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
            //　図形をインベントリに追加
            player.AddShape(legendaryShape); 
            DebugUtility.Log($"[CollapseRelicEvent] {legendaryShape.ShapeName} (Legendary) を獲得しました。"); return true;
        }
        else
        {
            DebugUtility.LogWarning("[CollapseRelicEvent] レジェンダリー図形を取得できませんでした。");
        }
        return true;
    }
    // ================================
    // 図形をランダムで2つ失うメソッド
    // ================================
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
                DebugUtility.Log($"[CollapseRelicEvent] {shapeToRemove.ShapeName} を失いました。");
            }
        }
        else
        {
            DebugUtility.Log("[CollapseRelicEvent] プレイヤーが図形を所持していません。");
        }
    }
    // ================================
    // レジェンダリー図形をランダムで取得するメソッド
    // ================================
    private ShapeBaseStats GetRandomLegendaryShape()
    {
        // 例: ShapeDex.Instance の全図形リストからLegendaryだけ抽出
        List<ShapeBaseStats> allShapes = ShapeDex.Instance.GetAllShapes();
        if (allShapes == null || allShapes.Count == 0)
        {
            return null;
        }

        // RarityがLegendaryのものだけを抽出
        List<ShapeBaseStats> legendaryList = allShapes.FindAll(s => s.rarity == Rarity.星3);
        if (legendaryList.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, legendaryList.Count);
        return legendaryList[randomIndex];
    }
    // ================================
    // 拒否した場合の処理
    // ================================
    public override void OnDecline()
    {
        // 拒否した場合の処理。必要があればデメリット等を入れる
        DebugUtility.Log("[CollapseRelicEvent] イベントを拒否しました。何も起こりません。");
    }
}
