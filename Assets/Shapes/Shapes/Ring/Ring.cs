using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ring : Shape
{
    // 回復量の割合
    float recoverAmount = 0.4f;
    // 攻撃力バフの割合
    float atkBuff = 0.1f;
    // 攻撃力バフの持続時間
    float atkBuffDuration = 10f;

    // ==========================
    // 初期化メソッド
    // ==========================
    public override void Initialize()
    {
        base.Initialize();
        if (baseStats.isLambdaUpgrade && !IsEnemy)
        {
            // 999秒間５秒ごとに50回復
            CombatHandler.TakeHealOverTime(50, 999, 5);
        }
    }

    // ==========================
    // 敵との衝突時に呼ばれるメソッド
    // ==========================
    protected override void OnEnemyCollision(Shape enemy)
    {
        base.OnEnemyCollision(enemy);
        float enemyAtk = enemy.Stats.CurrentPower;
        // 敵の攻撃力を吸収してHPを回復
        AbsorbHP(enemyAtk);
        if (!IsEnemy && BaseStats.isAlphaUpgrade)
        {
            // 攻撃力バフを付与
            CombatHandler.TakeFlatBuffForBattle(StatType.Attack, enemyAtk * atkBuff, atkBuffDuration);
        }
    }

    // ==========================
    // 吸収するスキル
    // ==========================
    public void AbsorbHP(float value)
    {
        float recoverValue = value * recoverAmount;
        // 回復処理
        CombatHandler.TakeHeal(recoverValue);
    }

    // ==========================
    // オメガスキルを発動するメソッド
    // ==========================
    public override void ActivateOmegaSkill()
    {
        if (BaseStats.IsOmegaUpgrade)
        {
            // 回復量の割合を増加
            recoverAmount = 0.6f;
        }
    }
}
