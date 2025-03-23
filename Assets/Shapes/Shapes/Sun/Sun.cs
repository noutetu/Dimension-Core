using UnityEngine;
using UniRx;
// ==========================
// Sunクラス(徐々に攻撃力が増加し、敵を倒すと体力回復)
// ==========================
public class Sun : Shape
{
    private float powerIncreaseDuration = 120f; // 上限まで到達するまでの時間（秒）
    private float powerMultiplier = 5f;         // 基礎攻撃力の上限倍率
    private GameObject[] sunrays;               // 太陽光のエフェクト（8個）
    private float healAmount = 25f;             // 敵を倒した時の回復量（%）

    private float originalPower; // 元の攻撃力を保存
    private int currentSunrayIndex = 0; // 現在のsunrayインデックス

    // ==========================
    // 初期化
    // ==========================
    public override void Initialize()
    {
        // 通常の初期化
        base.Initialize();
        // 元の攻撃力を保存
        originalPower = Stats.CurrentPower;
        // 攻撃力増加開始
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
        // powerIncreaseStep =　(1200 - 400) / 8(=sunrays.Length) = 100

        // 一定時間ごとに攻撃力を上昇させる
        Observable.Interval(System.TimeSpan.FromSeconds(powerIncreaseDuration / sunrays.Length))
            .Take(sunrays.Length)
            .Subscribe(_ =>
            {
                // 攻撃力を上昇
                float currentPower = Mathf.Max(Stats.CurrentPower + powerIncreaseStep, targetPower);
                // 攻撃力を設定
                Stats.SetAttackPower(currentPower);
                DebugUtility.Log($"Sunの攻撃力が上昇: {currentPower}");
                // Sunrayをアクティベート
                ActivateSunray(currentSunrayIndex);
                // Sunrayのインデックスを更新
                currentSunrayIndex++;
            })
            .AddTo(this);
    }

    // ==========================
    // Sunrayをアクティベート
    // ==========================
    private void ActivateSunray(int index)
    {
        // インデックスが範囲内であればSunrayをアクティベート
        if (index >= 0 && index < sunrays.Length)
        {
            // SpriteRendererを取得
            SpriteRenderer sunray = sunrays[index].GetComponent<SpriteRenderer>();
            // サンライトの色を黄色に変更
            sunray.color = Color.yellow;
            // バフ表示
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
        // 敵にダメージを与える
        // 敵が倒れた場合、回復を行う
        bool enemyIsDead = other.CombatHandler.TakeDamage(Stats.CurrentPower, Stats.CriticalRate, other.IsDead);
        ApplyContinuousDamage(other);
        if (enemyIsDead)
        {
            // 最大体力のhealAmount%分を回復
            float healValue = baseStats.BaseHP * (healAmount / 100f); 
            CombatHandler.TakeHeal(healValue);
        }
    }

    // ==========================
    // 継続ダメージを与えるメソッド
    // ==========================
    public void ApplyContinuousDamage(Shape enemy)
    {
        if (!IsEnemy && baseStats.IsAlphaUpgrade)
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
        // Omegaスキルがアップグレードされている場合
        if (BaseStats.IsOmegaUpgrade)
        {
            // 回復量を増加
            healAmount = 50f;
        }
    }

    // ==========================
    // Lambdaスキル
    // ==========================
    public override void ActivateLambdaSkill()
    {
        // Lambdaスキルがアップグレードされている場合
        if (BaseStats.IsLambdaUpgrade)
        {
            // 攻撃力上昇時間を短縮
            powerIncreaseDuration = 80f;
        }
    }
}