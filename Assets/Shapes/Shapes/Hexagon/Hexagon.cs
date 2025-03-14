using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : Shape
{
    float debuffValue = 40f;
    float debuffDuration = 10f;

    // ==========================
    // 敵との衝突処理
    // ==========================
    protected override void OnEnemyCollision(Shape other)
    {
        base.OnEnemyCollision(other);
        Debuff(other);
        SpeedDebuff(other);
    }
    // ==========================
    // デバフ
    // ==========================
    private void Debuff(Shape other)
    {
        float debuff = other.Stats.CurrentPower * (debuffValue / 100);
        other.CombatHandler.TakeFlatDebuffForBattle(StatType.Attack,debuff, debuffDuration);
    }
    // ==========================
    // スピードデバフ
    // ==========================
    private void SpeedDebuff(Shape shape)
    {
        // 敵か、オメガアップグレードでない場合は処理を終了
        if(IsEnemy || !baseStats.IsOmegaUpgrade){return;}
        float speDebuffValue = -1;
        shape.CombatHandler.TakeFlatBuffForBattle(StatType.Speed,speDebuffValue, debuffDuration);
    }
    // ==========================
    // Alphaスキル
    // ==========================
    public override void ActivateAlphaSkill()
    {
        if(baseStats.isAlphaUpgrade)
        {
            debuffValue = 60f;
        }
    }
    // ==========================
    // Lambdaスキル
    // ==========================
    public override void ActivateLambdaSkill()
    {
        if(baseStats.isLambdaUpgrade)
        {
            debuffDuration = 20f;
        }
    }
}
