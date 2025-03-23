using UnityEngine;
// ================================
// 図形の基本ステータス
// ================================
[CreateAssetMenu(fileName = "ShapeBaseStats", menuName = "Shape/BaseStats")]
public class ShapeBaseStats : ScriptableObject
{
    // 基本ステータスプロパティ
    public float BaseHP => baseHP;                      // HP
    public float BaseAttackPower => baseAttackPower;    // 攻撃力
    public float BaseSpeed => baseSpeed;                // スピード
    public float BaseCritical => criticalRate;          // クリティカル率
    public float BaseEvasion => evasionRate;            // 回避率

    // アップグレードフラグ
    public bool IsOmegaUpgrade { get; private set; }
    public bool IsAlphaUpgrade { get; private set; }
    public bool IsLambdaUpgrade { get; private set; }
    // アップグレード内容
    [TextArea]
    public string omegaSkillDescription;
    [TextArea]
    public string alphaSkillDescription;
    [TextArea]
    public string lambdaSkillDescription;
    // アップグレード価格
    public float omegaSkillCost;
    public float alphaSkillCost;
    public float lambdaSkillCost;

    [Header("図形の基本ステータス")]
    // プライベートフィールド
    [SerializeField] private float baseHP;          // HP
    [SerializeField] private float baseAttackPower; // 攻撃力
    [SerializeField] private float baseSpeed;       // スピード
    [SerializeField] private float criticalRate;    // クリティカル率
    [SerializeField] private float evasionRate;     // 回避率
    public Rarity rarity;                           // レアリティ

    public int Price => rarity switch
    {
        Rarity.星1 => 200,
        Rarity.星2 => 250,
        Rarity.星3 => 300,
        _ => 0
    };

    // 図形情報
    [TextArea] public string ShapeName;  // 図形の名前
    [TextArea] public string AbilityDescription;  // 図形の能力説明
    [TextArea] public string SkillDescription;  // 図形のスキル説明
    public Sprite ShapeIcon;  // 図形のアイコン

    public GameObject ShapePrefab;  // 図形のプレハブ

    // 図鑑関連
    [SerializeField] public bool IsDiscovered = false;  // 発見済みかどうか（Inspectorには表示しない）

    // 強化スキル
    public void ActivateOmegaSkill()
    {
        IsOmegaUpgrade = true;
    }
    public void ActivateAlphaSkill()
    {
        IsAlphaUpgrade = true;
    }
    public void ActivateLambdaSkill()
    {
        IsLambdaUpgrade = true;
    }
    public void ResetUpgrades()
    {
        IsOmegaUpgrade = false;
        IsAlphaUpgrade = false;
        IsLambdaUpgrade = false;
    }
}

public enum Rarity
{
    星1 = 1,
    星2 = 2,
    星3 = 3,
}