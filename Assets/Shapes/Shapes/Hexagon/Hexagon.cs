// ==========================
// Hexagon(敵との衝突時にデバフを与える)
// ==========================
public class Hexagon : Shape
{
    // デバフ量(%)
    float debuffValue = 40f;
    // デバフの持続時間
    float debuffDuration = 10f;
    // スピードデバフ量
    float speDebuffValue = -1;
        

    // ==========================
    // 敵との衝突処理
    // ==========================
    protected override void OnEnemyCollision(Shape other)
    {
        // 通常のダメージ処理
        base.OnEnemyCollision(other);
        // デバフ処理
        Debuff(other);
        // スピードデバフ処理
        SpeedDebuff(other);
    }
    // ==========================
    // デバフ
    // ==========================
    private void Debuff(Shape other)
    {
        // 敵の攻撃力の指定割合分をデバフ
        float debuff = other.Stats.CurrentPower * (debuffValue / 100);
        // デバフを与える
        other.CombatHandler.TakeFlatDebuffForBattle(StatType.Attack, debuff, debuffDuration);
    }
    // ==========================
    // スピードデバフ
    // ==========================
    private void SpeedDebuff(Shape shape)
    {
        // 敵か、オメガアップグレードでない場合は処理を終了
        if (IsEnemy || !baseStats.IsOmegaUpgrade) { return; }
        shape.CombatHandler.TakeFlatBuffForBattle(StatType.Speed, speDebuffValue, debuffDuration);
    }
    // ==========================
    // Alphaスキル
    // ==========================
    public override void ActivateAlphaSkill()
    {
        // アルファアップグレードを取得している場合
        if (baseStats.IsAlphaUpgrade)
        {
            // デバフ量を増加
            debuffValue = 60f;
        }
    }
    // ==========================
    // Lambdaスキル
    // ==========================
    public override void ActivateLambdaSkill()
    {
        // ラムダアップグレードを取得している場合
        if (baseStats.IsLambdaUpgrade)
        {
            // デバフの持続時間を増加
            debuffDuration = 20f;
        }
    }
}
