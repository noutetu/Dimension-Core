using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class Sun : Shape
{
    // 太陽の特性: 時間と共に攻撃力が上昇する
    // 上限は基礎攻撃力の5倍（powerMultiplierで設定可能）
    // 上限まで到達するまでの時間はpowerIncreaseDurationで設定可能

    [SerializeField] private float powerIncreaseDuration = 120f; // 上限まで到達するまでの時間（秒）
    [SerializeField] private float powerMultiplier = 5f; // 基礎攻撃力の上限倍率
    [SerializeField] private GameObject[] sunrays; // 太陽光のエフェクト（8個）
    [SerializeField] private float healAmount = 25f; // 敵を倒した時の回復量（%）

    private float originalPower; // 元の攻撃力を保存
    private int currentSunrayIndex = 0; // 現在のsunrayインデックス

    // ==========================
    // 初期化
    // ==========================
    public override void Initialize()
    {
        base.Initialize();
        StartPowerIncrease();
    }

    // ==========================
    // 攻撃力の上昇を開始
    // ==========================
    private void StartPowerIncrease()
    {
        // 上限攻撃力
        float targetPower = originalPower * powerMultiplier;
        // 1段階ごとの攻撃力増加量
        float powerIncreaseStep = (targetPower - originalPower) / sunrays.Length;

        // 例: 基礎攻撃力が400の場合
        // targetPower = 400 * 3 = 1200
        // powerIncreaseStep = (1200 - 400) / 8 = 100

        Observable.Interval(System.TimeSpan.FromSeconds(powerIncreaseDuration / sunrays.Length))
            .Take(sunrays.Length)
            .Subscribe(_ =>
            {
                float currentPower = Mathf.Max(Stats.CurrentPower + powerIncreaseStep, targetPower);
                Stats.SetAttackPower(currentPower);
                DebugUtility.Log($"Sunの攻撃力が上昇: {currentPower}");
                ActivateSunray(currentSunrayIndex);
                currentSunrayIndex++;
            })
            .AddTo(this);
    }

    // ==========================
    // Sunrayをアクティベート
    // ==========================
    private void ActivateSunray(int index)
    {
        if (index >= 0 && index < sunrays.Length)
        {
            SpriteRenderer sunray = sunrays[index].GetComponent<SpriteRenderer>();
            sunray.color = Color.yellow;
            CombatHandler.ShowBuff();
        }
    }

    // ==========================
    // 敵との衝突時の処理
    // ==========================
    protected override void OnEnemyCollision(Shape other)
    {
        if (other.IsDead) return;
        if (IsDead) return;
        bool enemyIsDead = other.CombatHandler.TakeDamage(Stats.CurrentPower, Stats.CriticalRate, other.IsDead);
        ApplyContinuousDamage(other);
        if (enemyIsDead)
        {
            float healValue = baseStats.BaseHP * (healAmount / 100f); // 最大体力のhealAmount%分を回復
            CombatHandler.TakeHeal(healValue);
        }
    }

    // ==========================
    // 継続ダメージを与えるメソッド
    // ==========================
    public void ApplyContinuousDamage(Shape enemy)
    {
        if (!IsEnemy && baseStats.isAlphaUpgrade)
        {
            // 継続ダメージを計算
            float continuousDamage = Stats.CurrentPower * 0.2f;
            // 敵に継続ダメージを与える
            enemy.CombatHandler.TakeContinuousDamage(continuousDamage, 3f);
        }
    }

    // ==========================
    // Omegaスキル
    // ==========================
    public override void ActivateOmegaSkill()
    {
        // Omegaスキルがアップグレードされている場合、回復量を増加
        if (BaseStats.IsOmegaUpgrade)
        {
            healAmount = 50f;
        }
    }

    // ==========================
    // Lambdaスキル
    // ==========================
    public override void ActivateLambdaSkill()
    {
        // Lambdaスキルがアップグレードされている場合、攻撃力の上昇時間を短縮
        if (BaseStats.isLambdaUpgrade)
        {
            powerIncreaseDuration = 80f;
        }
    }
}