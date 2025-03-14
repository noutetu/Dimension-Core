using UnityEngine;
using UniRx;
using Unity.VisualScripting;
using UnityEngine.Events;

/// <summary>
/// ステータス（HP/Attack/Speed等）の保持と、
/// 敵強化・バフ等の戦闘開始時の「最終ステータス計算」を責務とする。
/// </summary>
public class ShapeStats : MonoBehaviour
{
    // 現在ステータス
    public float MaxHP { get; private set; }
    public ReactiveProperty<float> CurrentHP { get; private set; } = new ReactiveProperty<float>();
    private float currentPower;
    public float CurrentPower {
        get => currentPower;
        private set => currentPower = Mathf.Max(0, value);
    }
    public ReactiveProperty<float> Speed { get; private set; } = new ReactiveProperty<float>();
    public float CriticalRate { get; private set; }
    public float EvasionRate { get; private set; }

    // バフ蓄積（イベントバフなど）
    private float EnhancedHP;
    private float EnhancedAttackPower;
    private float EnhancedSpeed;
    private float EnhancedCriticalRate;
    private float EnhancedEvasionRate;

    private ShapeBaseStats baseStats;
    private CompositeDisposable disposables = new CompositeDisposable();

    UnityAction OnOmega;
    UnityAction OnAlpha;
    UnityAction OnLambda;

    // ================================
    // 初期化
    // ================================
    public void Initialize(Shape shape, ShapeBaseStats baseStats)
    {
        OnOmega = shape.ActivateOmegaSkill;
        OnAlpha = shape.ActivateAlphaSkill;
        OnLambda = shape.ActivateLambdaSkill;
        this.baseStats = baseStats;
        ResetStats();
        ApplyUpgrade();
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
        MaxHP += EnhancedHP;
        CurrentHP.Value = Mathf.Min(CurrentHP.Value, MaxHP);

        CurrentPower += EnhancedAttackPower;
        Speed.Value += EnhancedSpeed;
        CriticalRate += EnhancedCriticalRate;
        EvasionRate += EnhancedEvasionRate;

        // ログに表示
        DebugUtility.Log($"最終ステータス: HP={MaxHP}, Attack={CurrentPower}, Speed={Speed}, Critical={CriticalRate}, Evasion={EvasionRate}");    }

    // ================================
    // HPや攻撃力の加減
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
    /// <summary> 敵専用の強化（固定値） </summary>
    public void ApplyEnemyEnhance(float hpBuff, float atkBuff, float spdBuff, float evaBuff, float critBuff)
    {
        EnhancedHP += MaxHP * hpBuff;
        EnhancedAttackPower += CurrentPower * atkBuff;
        EnhancedSpeed += Speed.Value * spdBuff;
        EnhancedEvasionRate += evaBuff;
        EnhancedCriticalRate += critBuff;
    }

    /// <summary> バフを%で加算（イベ用） </summary>
    public void ApplyStatusModifierByEvent(float hpBuff, float atkBuff, float spdBuff, float evaBuff, float critBuff)
    {
        EnhancedHP += MaxHP * hpBuff;
        EnhancedAttackPower += CurrentPower * atkBuff;
        EnhancedSpeed += Speed.Value * spdBuff;
        EnhancedEvasionRate += evaBuff;
        EnhancedCriticalRate += critBuff;
    }
    // バフを固定値で加算（スキルアップグレード）  
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
