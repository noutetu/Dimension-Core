// ==========================
// Blast(衝突時に基本ダメージを与える)
// ==========================
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
        // 通常のダメージ処理
        base.OnEnemyCollision(other);
        // 敵に継続ダメージを与える
        ApplyContinuousDamage(other);
        // Lambdaスキルがアップグレードされている場合、敵の速度を低下させる
        if (BaseStats.IsLambdaUpgrade && !IsEnemy)
        {
            // 敵の速度を低下
            other.CombatHandler.TakeFlatDebuffForBattle(StatType.Speed, 3f, continuousDamageDuration);
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
        // Omegaスキルがアップグレードされている場合
        if (BaseStats.IsOmegaUpgrade)
        {
            // 継続ダメージの値を増加
            continuousDamageRate = 0.5f;
        }
    }
    // ==========================
    // Alphaスキルのアクティベート
    // ==========================
    public override void ActivateAlphaSkill()
    {
        // Alphaスキルがアップグレードされている場合
        if (BaseStats.IsAlphaUpgrade)
        {
            // 継続ダメージの効果時間を増加
            continuousDamageDuration = 6f;
        }
    }
}
