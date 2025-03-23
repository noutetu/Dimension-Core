using UnityEngine;
using UnityEngine.UI;
// ========================
// 図形の詳細情報を表示するパネル
// ========================
public class ShapeDetailPanel : MonoBehaviour
{
    [Header("ステータステキスト")]
    [SerializeField] private Text shapeName;                // 図形の名前
    [SerializeField] private Text hpText;                   // HP
    [SerializeField] private Text attackText;               // 攻撃力
    [SerializeField] private Text speedText;                // 速度
    [SerializeField] private Text evasionText;              // 回避率
    [SerializeField] private Text criticalText;             // クリティカル率
    [SerializeField] private Text abilityText;              // 特性
    [SerializeField] private Text skillText;                // スキル
    [SerializeField] private ShapeRarityText shapeRarity;   // レアリティ

    // ========================
    // 図形の詳細情報を設定
    // ========================
    public void SetShapeDetail(ShapeBaseStats shape)
    {
        // 名前を設定
        shapeName.text = shape.ShapeName;
        // ステータスを設定
        hpText.text = "HP: " + shape.BaseHP;
        attackText.text = "攻撃: " + shape.BaseAttackPower;
        speedText.text = "速度: " + shape.BaseSpeed;
        evasionText.text = "回避: " + shape.BaseEvasion + "%";
        criticalText.text = "クリティカル: " + shape.BaseCritical + "%";
        abilityText.text = "特性: " + shape.AbilityDescription;
        skillText.text = "スキル: " + shape.SkillDescription;
        shapeRarity.ShowRarity((int)shape.rarity);
    }
}
