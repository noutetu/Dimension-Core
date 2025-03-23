// ==========================
// Heart(味方と衝突時、味方を回復する)
// ==========================
public class Heart : Shape
{
    // 回復量
    int healValue = 10;

    // ==========================
    // 味方との衝突処理
    // ==========================
    protected override void OnFriendCollision(Shape other)
    {
        // otherの最大体力10%
        float heal = other.BaseStats.BaseHP * healValue / 100;
        // 回復処理
        other.CombatHandler.TakeHeal(heal);
    }
    // ==========================
    // 初期化
    // ==========================
    public override void Initialize()
    {
        // 通常の初期化処理
        base.Initialize();
        // オメガアップグレードされている場合
        if (baseStats.IsOmegaUpgrade && !IsEnemy)
        {
            // 999秒間毎秒30回復
            CombatHandler.TakeHealOverTime(30, 999, 5);
        }
    }
    // ==========================
    // Alphaスキル
    // ==========================
    public override void ActivateAlphaSkill()
    {
        // アルファアップグレードされている場合
        if (baseStats.IsAlphaUpgrade)
        {
            // 回復量を増加
            healValue = 30;
        }
    }
}
