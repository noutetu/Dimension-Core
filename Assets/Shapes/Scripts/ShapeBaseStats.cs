using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "ShapeBaseStats", menuName = "Shape/BaseStats")]
public class ShapeBaseStats : ScriptableObject
{
    // 基本ステータス
    public float BaseHP => baseHP;
    public float BaseAttackPower => baseAttackPower;
    public float BaseSpeed => baseSpeed;
    public float BaseCritical => criticalRate;
    public float BaseEvasion => evasionRate;
    public Rarity Rarity;

    // アップグレードフラグ
    public bool IsOmegaUpgrade { get; private set; }
    public bool isAlphaUpgrade { get; private set; }
    public bool isLambdaUpgrade { get; private set; }
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
    [SerializeField] private float baseHP;
    [SerializeField] private float baseAttackPower;
    [SerializeField] private float baseSpeed;
    [SerializeField] private float criticalRate;
    [SerializeField] private float evasionRate;

    public int Price
    {
        get
        {
            switch (Rarity)
            {
                case Rarity.Common:
                    return 200;
                case Rarity.Rare:
                    return 250;
                case Rarity.Legendary:
                    return 300;
                default:
                    return 0;
            }
        }
    }

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
        isAlphaUpgrade = true;
    }
    public void ActivateLambdaSkill()
    {
        isLambdaUpgrade = true;
    }
    public void ResetUpgrades()
    {
        IsOmegaUpgrade = false;
        isAlphaUpgrade = false;
        isLambdaUpgrade = false;
    }
}

public enum Rarity
{
    Common = 1,
    Rare = 2,
    Legendary = 3,
}