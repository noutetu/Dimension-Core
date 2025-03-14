using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Square : Shape
{
    // ================================
    // 初期化
    // ================================
    protected override void OnEnemyCollision(Shape other)
    {
        base.OnEnemyCollision(other);
        ReflectDamage(other);
    }

    // ダメージを減少させるメソッド
    private float ReduceDamage(float damage)
    {
        if (IsEnemy || !baseStats.isLambdaUpgrade) return damage; // 敵はダメージ減少を受けない
        float reductionRate = 0.1f; // ダメージ減少率
        return damage * (1 - reductionRate);
    }

    // ダメージを反射するメソッド
    private void ReflectDamage(Shape shape)
    {
        if (IsEnemy || !baseStats.isAlphaUpgrade) return;
        float reflectedDamage = shape.BaseStats.BaseAttackPower * 0.5f;
        shape.CombatHandler.TakeDamage(reflectedDamage, 0,shape.IsDead);
    }

    // ================================
    // Omegaスキル
    // ================================
    public override void ActivateOmegaSkill()
    {
        // 敵は強化を受けない
        if (IsEnemy)
        {
            DebugUtility.Log("敵は強化を受けない");            return;
        }

        if (!baseStats.IsOmegaUpgrade) { return; }
        Stats.ApplyStatusModifierByUpgrade(500, 0, 0, 0, 0);
    }
    // ================================
    // Alphaスキル
    // ================================
    public override void ActivateLambdaSkill()
    {
        // 敵は強化を受けない
        if (IsEnemy) return;
        if (!baseStats.isLambdaUpgrade) { return; }

        Stats.ApplyStatusModifierByUpgrade(500, 0, 0, 0, 0);
    }
}
