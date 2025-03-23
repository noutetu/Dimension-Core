using UnityEngine;
using UniRx;
using UnityEngine.Events;
// ================================
// 戦闘ステータス管理
// ================================
public class ShapeBattleStats : MonoBehaviour
{
    // HP
    public float MaxHP { get; private set; }
    public ReactiveProperty<float> CurrentHP { get; private set; } = new ReactiveProperty<float>();
    // 攻撃力
    private float currentPower;
    public float CurrentPower
    {
        get => currentPower;
        private set => currentPower = Mathf.Max(0, value);
    }
    // スピード
    public ReactiveProperty<float> Speed { get; private set; } = new ReactiveProperty<float>();
    // クリティカル率
    public float CriticalRate { get; private set; }
    // 回避率
    public float EvasionRate { get; private set; }

    // バフ蓄積（イベントバフなど）
    private float EnhancedHP;           // HP
    private float EnhancedAttackPower;  // 攻撃力
    private float EnhancedSpeed;        // スピード
    private float EnhancedCriticalRate; // クリティカル率
    private float EnhancedEvasionRate;  // 回避率
    // 基本ステータスの参照
    private ShapeBaseStats baseStats;
    // 購読解除用
    private CompositeDisposable disposables = new CompositeDisposable();
    // スキルをステータスに適用するためのアクション
    UnityAction OnOmega;
    UnityAction OnAlpha;
    UnityAction OnLambda;

    // ================================
    // 初期化
    // ================================
    public void Initialize(Shape shape, ShapeBaseStats baseStats)
    {
        // スキルのアクティベート
        OnOmega = shape.ActivateOmegaSkill;
        OnAlpha = shape.ActivateAlphaSkill;
        OnLambda = shape.ActivateLambdaSkill;
        // 基本ステータスの設定
        this.baseStats = baseStats;

        // ステータスをリセット
        ResetStats();
        // アップグレードの適用
        ApplyUpgrade();
        // 最終ステータスの設定
        SetFinalStats();
    }
    /// <summary> BaseStatsの値に戻す </summary>
    public void ResetStats()
    {
        MaxHP = baseStats.BaseHP;
        CurrentHP.Value = MaxHP;
        CurrentPower = baseStats.BaseAttackPower;
        Speed.Value = baseStats.BaseSpeed;
        CriticalRate = baseStats.BaseCritical;
        EvasionRate = baseStats.BaseEvasion;
    }

    // ================================
    // アップグレードの適用
    // ================================
    public void ApplyUpgrade()
    {
        OnOmega?.Invoke();
        OnAlpha?.Invoke();
        OnLambda?.Invoke();
    }

    // ================================
    // 最終ステータスの設定
    // ================================
    public void SetFinalStats()
    {
        // 各種ステータスにバフを適用
        MaxHP += EnhancedHP;
        CurrentHP.Value = Mathf.Min(CurrentHP.Value, MaxHP);

        CurrentPower += EnhancedAttackPower;
        Speed.Value += EnhancedSpeed;
        CriticalRate += EnhancedCriticalRate;
        EvasionRate += EnhancedEvasionRate;

        // ログに表示
        DebugUtility.Log($"最終ステータス: HP={MaxHP}, Attack={CurrentPower}, Speed={Speed}, Critical={CriticalRate}, Evasion={EvasionRate}");
    }

    // ================================
    // HPの増減
    // ================================
    public bool ReduceHP(float damageAmount)
    {
        CurrentHP.Value = Mathf.Max(CurrentHP.Value - damageAmount, 0);
        return CurrentHP.Value <= 0;
    }
    public void IncreaseHP(float healAmount)
    {
        CurrentHP.Value = Mathf.Min(CurrentHP.Value + healAmount, MaxHP);
    }
    // ================================
    // 敵の強化
    // ================================
    public void ApplyEnemyEnhance(float hpBuff, float atkBuff, float spdBuff, float evaBuff, float critBuff)
    {
        EnhancedHP += MaxHP * hpBuff;
        EnhancedAttackPower += CurrentPower * atkBuff;
        EnhancedSpeed += Speed.Value * spdBuff;
        EnhancedEvasionRate += evaBuff;
        EnhancedCriticalRate += critBuff;
    }
    // ================================
    // バフを加算（イベント）
    // ================================
    public void ApplyStatusModifierByEvent(float hpBuff, float atkBuff, float spdBuff, float evaBuff, float critBuff)
    {
        EnhancedHP += MaxHP * hpBuff;
        EnhancedAttackPower += CurrentPower * atkBuff;
        EnhancedSpeed += Speed.Value * spdBuff;
        EnhancedEvasionRate += evaBuff;
        EnhancedCriticalRate += critBuff;
    }
    // ================================
    // バフを加算（アップグレード）
    // ================================
    public void ApplyStatusModifierByUpgrade(float hpBuff, float atkBuff, float spdBuff, float evaBuff, float critBuff)
    {
        EnhancedHP += hpBuff;
        EnhancedAttackPower += atkBuff;
        EnhancedSpeed += spdBuff;
        EnhancedEvasionRate += evaBuff;
        EnhancedCriticalRate += critBuff;
    }
    // ================================
    // ステータス固定
    // ================================
    public void SetAttackPower(float power) => CurrentPower = power;
    public void SetSpeed(float speed) => Speed.Value = speed;

    // ================================
    // バフ/デバフ (固定値)
    // ================================
    public void ApplyFlatHPBuff(float hpBuff) => MaxHP += hpBuff;
    public void ApplyFlatHPDebuff(float hpDebuff) => MaxHP -= hpDebuff;

    public void ApplyFlatAttackBuff(float attackBuff) => CurrentPower += attackBuff;
    public void ApplyFlatAttackDebuff(float attackDebuff) => CurrentPower -= attackDebuff;

    public void ApplyFlatSpeedBuff(float speedBuff) => Speed.Value += speedBuff;
    public void ApplyFlatSpeedDebuff(float speedDebuff) => Speed.Value -= speedDebuff;

    public void ApplyFlatCriticalBuff(float criticalBuff) => CriticalRate += criticalBuff;
    public void ApplyFlatCriticalDebuff(float criticalDebuff) => CriticalRate -= criticalDebuff;

    public void ApplyFlatEvasionBuff(float evasionBuff) => EvasionRate += evasionBuff;
    public void ApplyFlatEvasionDebuff(float evasionDebuff) => EvasionRate -= evasionDebuff;

    // ================================
    // 終了処理
    // ================================
    public void Dispose()
    {
        disposables.Dispose();
    }
}

public enum StatType
{
    HP,
    Attack,
    Speed,
    Critical,
    Evasion
}
