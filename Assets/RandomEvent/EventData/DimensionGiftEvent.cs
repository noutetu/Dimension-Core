using UnityEngine;
using System.Collections.Generic;
using System.Linq;
// ==================================================
// 通貨を指定割合消費して、指定レアリティ以上の図形をランダムに1つ獲得するイベント
// ==================================================
[CreateAssetMenu(fileName = "DimensionGiftEvent", menuName = "Game/Event/DimensionGift")]
public class DimensionGiftEvent : BaseEventData
{
    [Tooltip("通貨を失う割合 (例: 0.2f = 20%)")]
    [SerializeField] private float costPercentage;

    [Tooltip("入手する最低レアリティ")]
    [SerializeField] private Rarity minimumRarity = Rarity.星2;
    // ==================================================
    // イベントを受け入れた場合の処理
    // ==================================================
    public override bool OnAccept()
    {
        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("PlayerStatus.Instance が存在しません。");
            return false;
        }
        if (player.currencyManager.Currency <= 0) return false;
        // 
        float currentCurrency = player.currencyManager.Currency;
        float cost = currentCurrency * costPercentage;
        // 1. 通貨を消費
        player.SpendCurrency(cost); 

        // 2. Rare 以上の図形をランダムに 1つ獲得
        ShapeBaseStats randomShape = GetRandomShapeOfRarity(minimumRarity);
        if (randomShape != null)
        {
            // 図形をプレイヤーのインベントリに追加
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
    // イベントを拒否した場合の処理
    // ==================================================
    public override void OnDecline()
    {
        // 拒否した場合は特に何もしない
        DebugUtility.Log("[DimensionGiftEvent] イベントを拒否しました。");
    }
    // ==================================================
    // レアリティ以上のランダムなShapeBaseStatsを返す
    // ==================================================
    private ShapeBaseStats GetRandomShapeOfRarity(Rarity minRarity)
    {
        // 全Shapeリストを仮定 (ShapeDex.Instance 等から取得)
        List<ShapeBaseStats> allShapes = ShapeDex.Instance.GetAllShapes();
        if (allShapes == null || allShapes.Count == 0) return null;

        // Rare以上のShapeだけを抽出
        List<ShapeBaseStats> filtered = allShapes.Where(shape => ((int)shape.rarity) == ((int)minRarity)).ToList();
        if (filtered.Count == 0) return null;

        // ランダムに1つ選んで返す
        int index = Random.Range(0, filtered.Count);
        return filtered[index];
    }
}
