// ==========================
// Triangle
// ==========================
public class Triangle : Shape
{
    // ==========================
    // Omegaスキル
    // ==========================
    public override void ActivateOmegaSkill()
    {
        // 敵は強化を受けない
        if (IsEnemy) return;
        // Omegaスキルが適用可能なら
        if (!baseStats.IsOmegaUpgrade) { return; }
        // ステータスを強化
        Stats.ApplyStatusModifierByUpgrade(0, 0, 0, 10, 10);
    }

    // ==========================
    // Alphaスキル
    // ==========================
    public override void ActivateAlphaSkill()
    {
        // 敵は強化を受けない
        if (IsEnemy) return;
        // Alphaスキルが適用可能なら
        if (!baseStats.IsAlphaUpgrade) { return; }
        // ステータスを強化
        Stats.ApplyStatusModifierByUpgrade(0, 100, 0, 20, 0);
    }

    // ==========================
    // Lambdaスキル
    // ==========================
    public override void ActivateLambdaSkill()
    {
        // 敵は強化を受けない
        if (IsEnemy) return;
        // Lambdaスキルが適用可能なら
        if (!baseStats.IsLambdaUpgrade) { return; }
        // ステータスを強化
        Stats.ApplyStatusModifierByUpgrade(0, 0, 4, 0, 20);
    }
}
