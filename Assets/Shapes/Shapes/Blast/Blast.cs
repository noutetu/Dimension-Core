using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;

public class Blast : Shape
{
    // 継続ダメージの値
    float continuousDamageRate = 0.3f;
    // 継続ダメージの効果時間
    float continuousDamageDuration = 3f;

    // ==========================
    // 敵との衝突時の処理
    // ==========================
    protected override void OnEnemyCollision(Shape other)
    {
        base.OnEnemyCollision(other);
        // 敵に継続ダメージを与える
        ApplyContinuousDamage(other);
        // Lambdaスキルがアップグレードされている場合、敵の速度を低下させる
        if (BaseStats.isLambdaUpgrade && !IsEnemy)
        {
            other.CombatHandler.TakeFlatDebuffForBattle(StatType.Speed, 3f,continuousDamageDuration);
        }
    }
    // ==========================
    // 継続ダメージを与えるメソッド
    // ==========================
    public void ApplyContinuousDamage(Shape enemy)
    {
        // 継続ダメージを計算
        float continuousDamage = Stats.CurrentPower * continuousDamageRate;
        // 敵に継続ダメージを与える
        enemy.CombatHandler.TakeContinuousDamage(continuousDamage, continuousDamageDuration);
    }
    // ==========================
    // Omegaスキルのアクティベート
    // ==========================
    public override void ActivateOmegaSkill()
    {
        // Omegaスキルがアップグレードされている場合、継続ダメージの割合を増加
        if (BaseStats.IsOmegaUpgrade)
        {
            continuousDamageRate = 0.5f;
        }
    }
    // ==========================
    // Alphaスキルのアクティベート
    // ==========================
    public override void ActivateAlphaSkill()
    {
        // Alphaスキルがアップグレードされている場合、継続ダメージの効果時間を延長
        if (BaseStats.isAlphaUpgrade)
        {
            continuousDamageDuration = 6f;
        }
    }
}
