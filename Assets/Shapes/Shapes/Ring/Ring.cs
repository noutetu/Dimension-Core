// ==========================
// Ring(敵と衝突時に受けたダメージの一定割合回復する)
// ==========================
public class Ring : Shape
{
    float recoverAmount = 0.4f;     // 回復する割合
    float atkBuff = 0.1f;           // 攻撃力バフの割合
    float atkBuffDuration = 10f;    // 攻撃力バフの持続時間

    // ==========================
    // 初期化メソッド
    // ==========================
    public override void Initialize()
    {
        // 通常の初期化処理
        base.Initialize();
        // ラムダスキルが適用可能なら
        if (baseStats.IsLambdaUpgrade && !IsEnemy)
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
        // 通常の衝突処理
        base.OnEnemyCollision(enemy);
        // 敵の攻撃力を取得
        float enemyAtk = enemy.Stats.CurrentPower;
        // 敵の攻撃力を吸収してHPを回復
        AbsorbHP(enemyAtk);
        // アルファスキルが適用可能なら
        if (!IsEnemy && BaseStats.IsAlphaUpgrade)
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
        // 回復量を計算
        float recoverValue = value * recoverAmount;
        // 回復処理
        CombatHandler.TakeHeal(recoverValue);
    }

    // ==========================
    // オメガスキルを発動するメソッド
    // ==========================
    public override void ActivateOmegaSkill()
    {
        // オメガスキルが適用されている場合
        if (BaseStats.IsOmegaUpgrade)
        {
            // 回復量の割合を増加
            recoverAmount = 0.6f;
        }
    }
}
