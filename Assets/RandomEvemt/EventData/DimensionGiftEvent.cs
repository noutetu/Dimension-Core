using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

[CreateAssetMenu(fileName = "DimensionGiftEvent", menuName = "Game/Event/DimensionGift")]
public class DimensionGiftEvent : BaseEventData
{
    [Tooltip("通貨を失う割合 (例: 0.2f = 20%)")]
    [SerializeField] private float costPercentage; 

    [Tooltip("入手する最低レアリティ")]
    [SerializeField] private Rarity minimumRarity = Rarity.Rare; // Rare以上

    // イベント名 (BaseEventDataで定義された)
    // public string eventTitle; // ScriptableObjectのインスペクタでセット

    public override bool OnAccept()
    {
        // 1. プレイヤーの所持通貨を % 消費
        var player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            DebugUtility.LogWarning("PlayerStatus.Instance が存在しません。");
            return false;
        }
        // 通貨が0より多いなら
        if (player.currencyManager.Currency <= 0) return false;

        float currentCurrency = player.currencyManager.Currency; 
        float cost = currentCurrency * costPercentage; 
        player.SpendCurrency(cost); // 20%分マイナス

        // 2. Rare 以上の図形をランダムに 1つ獲得
        // 例: ShapeDex や他の管理クラスから、指定レアリティ以上をピックアップ
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
        // 必要なら "拒否すると呪いが…" のようなデメリットをここに実装
        DebugUtility.Log("[DimensionGiftEvent] イベントを拒否しました。");    }

    /// <summary>
    /// minimumRarity以上のランダムなShapeBaseStatsを返す
    /// 例: ShapeDexや他の管理クラスを参照する想定
    /// </summary>
    private ShapeBaseStats GetRandomShapeOfRarity(Rarity minRarity)
    {
        // 全Shapeリストを仮定 (ShapeDex.Instance 等から取得)
        List<ShapeBaseStats> allShapes = ShapeDex.Instance.GetAllShapes();
        if (allShapes == null || allShapes.Count == 0) return null;

        // Rare以上のShapeだけを抽出
        List<ShapeBaseStats> filtered = allShapes.Where(shape => ((int)shape.Rarity) == ((int)minRarity)).ToList();
        if (filtered.Count == 0) return null;

        // ランダムに1つ選んで返す
        int index = Random.Range(0, filtered.Count);
        return filtered[index];
    }
}
