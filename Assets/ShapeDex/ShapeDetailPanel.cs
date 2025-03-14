using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeDetailPanel : MonoBehaviour
{
    [Header("ステータステキスト")]
    [SerializeField] private Text shapeName;
    [SerializeField] private Text hpText;
    [SerializeField] private Text attackText;
    [SerializeField] private Text speedText;
    [SerializeField] private Text evasionText;
    [SerializeField] private Text criticalText;
    [SerializeField] private Text abilityText;
    [SerializeField] private Text skillText;

    public void SetShapeDetail(ShapeBaseStats shape)
    {
        shapeName.text = shape.ShapeName;
        hpText.text = "HP: " + shape.BaseHP;
        attackText.text = "攻撃: " + shape.BaseAttackPower;
        speedText.text = "速度: " + shape.BaseSpeed;
        evasionText.text = "回避: " + shape.BaseEvasion + "%";
        criticalText.text = "クリティカル: " + shape.BaseCritical + "%";
        abilityText.text = "特性: " + shape.AbilityDescription;
        skillText.text = "スキル: " + shape.SkillDescription;
    }
}
