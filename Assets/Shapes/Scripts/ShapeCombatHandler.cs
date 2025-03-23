using UniRx;
using UnityEngine;

// ================================
// 戦闘処理
// ================================
[RequireComponent(typeof(ShapeBattleStats))]
[RequireComponent(typeof(ShapeTextHandler))]
[RequireComponent(typeof(ShapeAnimationHandler))]
public class ShapeCombatHandler : MonoBehaviour
{
    private ShapeBattleStats stats;                 // ステータス
    private ShapeTextHandler textManager;           // テキスト表示
    private ShapeAnimationHandler shapeAnimation;   // アニメーション

    // 購読管理
    private CompositeDisposable disposables = new CompositeDisposable();
    // 敵かどうか
    bool isEnemy;

    // ================================
    // 初期化
    // ================================
    public void Initialize(Shape shape)
    {
        // 敵かどうか
        this.isEnemy = shape.IsEnemy;
        // コンポーネント取得
        stats = GetComponent<ShapeBattleStats>();
        textManager = GetComponent<ShapeTextHandler>();
        shapeAnimation = GetComponent<ShapeAnimationHandler>();
        // animationの初期化
        shapeAnimation.Initialize(shape);
    }

    // ================================
    // 通常ダメージ処理(死んだかどうかを返す)
    // ================================
    public bool TakeDamage(float atk, float CriticalRate, bool isDead, bool canEva = true)
    {
        if (isDead) return true;  // すでに死んでる場合は処理しない

        // --------- 回避判定 ----------
        bool isDodge = Random.value < (stats.EvasionRate / 100f);
        if (isDodge && canEva)
        {
            textManager.ShowDodge();
            return false;
        }

        // --------- クリティカル判定 ----------
        float damage = atk;
        bool isCritical = Random.value < (CriticalRate / 100f);
        // --------- ダメージ計算 ----------
        if (isCritical)
        {
            damage *= 1.5f;
            // クリティカル演出
            textManager.ShowCritDamage(damage, isEnemy);
        }
        else
        {
            // 通常ダメージ演出
            textManager.ShowNormalDamage(damage, isEnemy);
        }

        // --------- ダメージ処理 ----------
        // ---------- 死亡判定 ----------
        bool willDie = stats.ReduceHP(damage);
        if (willDie)
        {
            // 死亡演出（実際の破壊処理はアニメーション後）
            shapeAnimation.PlayDestroyAnimation();
            return true;
        }

        // --------- ダメージアニメーション ----------
        float hpRatio = Mathf.Clamp01(stats.CurrentHP.Value / stats.MaxHP);
        shapeAnimation.PlayBlinkAnimation(hpRatio);

        return false;
    }

    // ================================
    // 継続ダメージ
    // ================================
    public void TakeContinuousDamage(float damage, float duration)
    {
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
        // HPを回復
        stats.IncreaseHP(healValue);
        // 回復テキスト表示
        textManager.ShowHeal(healValue, isEnemy);

        // HP割合に応じて透明度変更
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
        // バフ適用
        ApplyFlatBuff(statType, value);
        // バフ表示
        textManager.ShowBuff(isEnemy);

        // duration後に元に戻す
        Observable.Timer(System.TimeSpan.FromSeconds(duration))
            .Subscribe(_ => { ApplyFlatDebuff(statType, value); })
            .AddTo(this);
    }

    public void TakeFlatDebuffForBattle(StatType statType, float value, float duration)
    {
        if (stats == null) return;
        // デバフ適用
        ApplyFlatDebuff(statType, value);
        // デバフ表示
        textManager.ShowDebuff(isEnemy);
        // duration後に元に戻す
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
            case StatType.HP:       stats.ApplyFlatHPBuff(value);       break;
            case StatType.Attack:   stats.ApplyFlatAttackBuff(value);   break;
            case StatType.Speed:    stats.ApplyFlatSpeedBuff(value);    break;
            case StatType.Critical: stats.ApplyFlatCriticalBuff(value); break;
            case StatType.Evasion:  stats.ApplyFlatEvasionBuff(value);  break;
        }
    }
    private void ApplyFlatDebuff(StatType statType, float value)
    {   
        // 受け取ったステータスによって処理を分岐
        switch (statType)
        {
            case StatType.HP:       stats.ApplyFlatHPDebuff(value);         break;
            case StatType.Attack:   stats.ApplyFlatAttackDebuff(value);     break;
            case StatType.Speed:    stats.ApplyFlatSpeedDebuff(value);      break;
            case StatType.Critical: stats.ApplyFlatCriticalDebuff(value);   break;
            case StatType.Evasion:  stats.ApplyFlatEvasionDebuff(value);    break;
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
    // バフ表示
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
