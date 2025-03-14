using UniRx;
using UnityEngine;

/// <summary>
/// ダメージ計算や回復など戦闘中の「時間依存型の効果」や「ダメージ計算ロジック」を管理。
/// 実際のステータス変更は ShapeStats に委譲する。
/// </summary>
[RequireComponent(typeof(ShapeStats))]
[RequireComponent(typeof(ShapeTextHandler))]
[RequireComponent(typeof(ShapeAnimationHandler))]
public class ShapeCombatHandler : MonoBehaviour
{
    private ShapeStats stats;
    private ShapeTextHandler textManager;
    private ShapeAnimationHandler shapeAnimation;
    private CompositeDisposable disposables = new CompositeDisposable();

    bool isEnemy;
    // ================================
    // 初期化
    // ================================
    public void Initialize(Shape shape)
    {
        this.isEnemy = shape.IsEnemy;
        stats = GetComponent<ShapeStats>();
        textManager = GetComponent<ShapeTextHandler>();
        shapeAnimation = GetComponent<ShapeAnimationHandler>();
        shapeAnimation.Initialize(shape);
    }

    // ================================
    // 通常ダメージ処理(死んだかどうかを返す)
    // ================================
    public bool TakeDamage(float atk, float CriticalRate, bool isDead, bool canEva = true)
    {
        if (isDead) return true;  // すでに死んでる場合
        if (stats == null) return true; // 保険

        // 回避判定
        bool isDodge = Random.value < (stats.EvasionRate / 100f);
        if (isDodge && canEva)
        {
            textManager.ShowDodge();
            return false;
        }

        // クリティカル判定
        float damage = atk;
        bool isCritical = Random.value < (CriticalRate / 100f);
        // ダメージ表示
        if (isCritical)
        {
            damage *= 1.5f;
            textManager.ShowCritDamage(damage,isEnemy);
        }
        else
        {
            textManager.ShowNormalDamage(damage, isEnemy);
        }

        // ダメージ処理
        // 死亡判定
        bool willDie = stats.ReduceHP(damage);
        if (willDie)
        {
            // 死亡演出（実際の破壊処理はアニメーション後）
            shapeAnimation.PlayDestroyAnimation();
            return true;
        }

        // ダメージを受けてHPが残った場合の演出
        float hpRatio = Mathf.Clamp01(stats.CurrentHP.Value / stats.MaxHP);
        shapeAnimation.PlayBlinkAnimation(hpRatio);

        return false;
    }

    // ================================
    // 継続ダメージ
    // ================================
    public void TakeContinuousDamage(float damage, float duration)
    {
        if (stats == null) return;
        // 1秒ごとにダメージ
        Observable.Interval(System.TimeSpan.FromSeconds(1))
            .TakeWhile(_ => duration > 0)
            .Subscribe(_ =>
            {
                // 毎秒ダメージ
                TakeDamage(damage, 0, false);
                duration -= 1;
            })
            .AddTo(this);
    }

    // ================================
    // 回復
    // ================================
    public void TakeHeal(float healValue)
    {
        if (stats == null) return;
        stats.IncreaseHP(healValue);
        textManager.ShowHeal(healValue,isEnemy);

        float hpRatio = Mathf.Clamp01(stats.CurrentHP.Value / stats.MaxHP);
        shapeAnimation.FadeTo(hpRatio);
    }

    public void TakeHealOverTime(float healValue, float duration, float interval)
    {
        if (stats == null) return;

        Observable.Interval(System.TimeSpan.FromSeconds(interval))
            .TakeWhile(_ => duration > 0)
            .Subscribe(_ =>
            {
                if (stats.CurrentHP.Value < stats.MaxHP)
                {
                    TakeHeal(healValue);
                }
                duration -= interval;
            })
            .AddTo(this);
    }

    // ================================
    // 一定時間固定バフ
    // ================================
    public void TakeFlatBuffForBattle(StatType statType, float value, float duration)
    {
        if (stats == null) return;

        ApplyFlatBuff(statType, value);
        textManager.ShowBuff(isEnemy);

        // duration後に元に戻す
        Observable.Timer(System.TimeSpan.FromSeconds(duration))
            .Subscribe(_ => { ApplyFlatDebuff(statType, value); })
            .AddTo(this);
    }

    public void TakeFlatDebuffForBattle(StatType statType, float value, float duration)
    {
        if (stats == null) return;

        ApplyFlatDebuff(statType, value);
        textManager.ShowDebuff(isEnemy);

        Observable.Timer(System.TimeSpan.FromSeconds(duration))
            .Subscribe(_ => { ApplyFlatBuff(statType, value); })
            .AddTo(this);
    }

    // ================================
    // 実際のバフ・デバフ実行
    // ================================
    private void ApplyFlatBuff(StatType statType, float value)
    {
        switch (statType)
        {
            case StatType.HP:       stats.ApplyFlatHPBuff(value); break;
            case StatType.Attack:   stats.ApplyFlatAttackBuff(value); break;
            case StatType.Speed:    stats.ApplyFlatSpeedBuff(value); break;
            case StatType.Critical: stats.ApplyFlatCriticalBuff(value); break;
            case StatType.Evasion:  stats.ApplyFlatEvasionBuff(value); break;
        }
    }
    private void ApplyFlatDebuff(StatType statType, float value)
    {
        switch (statType)
        {
            case StatType.HP:       stats.ApplyFlatHPDebuff(value); break;
            case StatType.Attack:   stats.ApplyFlatAttackDebuff(value); break;
            case StatType.Speed:    stats.ApplyFlatSpeedDebuff(value); break;
            case StatType.Critical: stats.ApplyFlatCriticalDebuff(value); break;
            case StatType.Evasion:  stats.ApplyFlatEvasionDebuff(value); break;
        }
    }

    // ================================
    // Singularityのモードチェンジ
    // ================================
    public void ChangeMode(bool isInDefenceMode)
    {
        textManager.ShowCurrentMode(isInDefenceMode);
    }
    // ================================
    // Sun用のバフ表示
    // ================================
    public void ShowBuff()
    {
        textManager.ShowBuff(isEnemy);
    }
    // ================================
    // 終了処理
    // ================================
    public void DisposeAll()
    {
        disposables.Dispose();
    }
}
