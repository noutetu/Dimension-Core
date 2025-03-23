using UnityEngine;
using UnityEngine.UI;
// ==========================
// 図形のレアリティを表示するクラス
// ==========================
public class ShapeRarityText : MonoBehaviour
{
    Text rarityText; // レアリティを表示するテキスト
    // ==========================
    // 初期化
    // ==========================
    public void ShowRarity(int rarity)
    {
        // テキストコンポーネントを取得
        rarityText = GetComponentInChildren<Text>();
        // レアリティを表示
        rarityText.text = $"{rarity}";
    }
}
