// ================================
// 四角形
// ================================
public class Square : Shape, IEaseDamage
{
    // ================================
    // 初期化
    // ================================
    protected override void OnEnemyCollision(Shape other)
    {
        base.OnEnemyCollision(other);
        ReflectDamage(other);
    }
    // ================================
    // ダメージを減少させるメソッド
    // ================================
    public float ReduceDamage(float damage)
    {
        // ラムダスキルが適用可能なら
        if (IsEnemy || !baseStats.IsLambdaUpgrade) return damage;
        // ダメージを減少させる割合
        float reductionRate = 0.1f;
        // 減少後のダメージを返す
        return damage * (1 - reductionRate);
    }
    // ================================
    // ダメージを反射するメソッド
    // ================================
    private void ReflectDamage(Shape shape)
    {
        // アルファアップグレードが適用可能なら
        if (IsEnemy || !baseStats.IsAlphaUpgrade) return;
        // 反射ダメージを計算
        float reflectedDamage = shape.BaseStats.BaseAttackPower * 0.5f;
        // ダメージを与える
        shape.CombatHandler.TakeDamage(reflectedDamage, 0,shape.IsDead);
    }

    // ================================
    // Omegaスキル
    // ================================
    public override void ActivateOmegaSkill()
    {
        // 敵は強化を受けない
        if (IsEnemy) return;
        // オメガアップグレードが適用されているなら
        if (!baseStats.IsOmegaUpgrade) { return; }
        // ステータスを強化
        Stats.ApplyStatusModifierByUpgrade(500, 0, 0, 0, 0);
    }
    // ================================
    // Alphaスキル
    // ================================
    public override void ActivateLambdaSkill()
    {
        // 敵は強化を受けない
        if (IsEnemy) return;
        // ラムダアップグレードが適用されているなら
        if (!baseStats.IsLambdaUpgrade) { return; }
        // ステータスを強化
        Stats.ApplyStatusModifierByUpgrade(500, 0, 0, 0, 0);
    }
}
