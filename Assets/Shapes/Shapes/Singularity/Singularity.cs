using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using DG.Tweening;

public class Singularity : Shape
{
    [SerializeField] private SpriteRenderer defaultSpriteRenderer;
    [SerializeField] private SpriteRenderer attackSpriteRenderer;

    private float defenseThreshold = 0.3f; // 防御モードに移行する体力の割合
    private float healAmount = 7; // 1秒ごとに回復する量(%)
    private CompositeDisposable healDisposables = new CompositeDisposable();
    private CompositeDisposable damageDisposables = new CompositeDisposable();
    private float originalPower; // 元の攻撃力を保存
    private float originalSpeed; // 元の速度を保存
    private bool isInDefenceMode = false; // 回復モードの状態を管理するフラグ

    Color originalColor;

    // ==========================
    // 初期化
    // ==========================
    public override void Initialize()
    {
        originalPower = baseStats.BaseAttackPower; // 元の攻撃力を保存
        originalSpeed = baseStats.BaseSpeed; // 元の速度を保存
        base.Initialize();
        InitializeSpriteRenderer(); // スプライトレンダラーの初期化
        ObserveHPChanges();

        // CombatHandlerがnullでないことを確認
        if (CombatHandler == null)
        {
            CombatHandler = GetComponent<ShapeCombatHandler>();
            if (CombatHandler == null)
            {
                DebugUtility.LogError($"{gameObject.name} の CombatHandler が初期化されていません！", gameObject);
            }
        }
    }

    // ==========================
    // スプライトレンダラーの初期化
    // ==========================
    public void InitializeSpriteRenderer()
    {
        // 親クラスのspriteRendererは使用しない
        if (defaultSpriteRenderer == null)
        {
            defaultSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (attackSpriteRenderer == null)
        {
            attackSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        if (defaultSpriteRenderer == attackSpriteRenderer)
        {
            DebugUtility.LogError($"{gameObject.name} の defaultSpriteRenderer と attackSpriteRenderer が同じです！", gameObject);
            return;
        }

        if (defaultSpriteRenderer == null || attackSpriteRenderer == null)
        {
            DebugUtility.LogError($"{gameObject.name} の SpriteRenderer が見つかりません！", gameObject);
            return;
        }

        originalColor = defaultSpriteRenderer.color;
    }

    // ==========================
    // HPの変化を監視
    // ==========================
    private void ObserveHPChanges()
    {
        Stats.CurrentHP
            .Subscribe(currentHP =>
            {
                float hpRatio = currentHP / baseStats.BaseHP;
                if (hpRatio <= defenseThreshold)
                {
                    ChangeToDefenseMode();
                }
                else if (hpRatio == 1f)
                {
                    ChangeToAttackMode();
                }
            })
            .AddTo(this);
    }

    // ==========================
    // 攻撃モードに変更
    // ==========================
    public void ChangeToAttackMode()
    {
        if (!isInDefenceMode) return; // すでに攻撃モードの場合は何もしない

        if (CombatHandler != null)
        {
            CombatHandler.ChangeMode(isInDefenceMode);
        }
        else
        {
            DebugUtility.LogError($"{gameObject.name} の CombatHandler が null です！", gameObject);
            return;
        }
        defaultSpriteRenderer.gameObject.SetActive(true);
        attackSpriteRenderer.gameObject.SetActive(true);
        Stats.SetAttackPower(originalPower); // 元の攻撃力に戻す
        Stats.SetSpeed(originalSpeed); // 元の速度に戻す

        // Lambdaスキルがアップグレードされている場合、攻撃力と速度を増加させ、体力を減少させる
        if (baseStats.isLambdaUpgrade)
        {
            float atkbuffValue = originalPower * 1.3f;
            float spdBuffValue = originalSpeed * 1.3f;
            Stats.SetAttackPower(atkbuffValue);
            Stats.SetSpeed(spdBuffValue);

            Observable.Interval(System.TimeSpan.FromSeconds(2))
                .Subscribe(_ =>
                {
                    CombatHandler.TakeDamage(60, 0, false); // 体力を徐々に減少させる
                })
                .AddTo(damageDisposables);
        }

        healDisposables.Clear(); // 回復処理を停止
        isInDefenceMode = false; // 回復モードフラグをリセット
    }

    // ==========================
    // 防御モードに変更
    // ==========================
    public void ChangeToDefenseMode()
    {
        if (isInDefenceMode) return; // すでに防御モードの場合は何もしない

        if (CombatHandler != null)
        {
            CombatHandler.ChangeMode(isInDefenceMode);
        }
        else
        {
            DebugUtility.LogError($"{gameObject.name} の CombatHandler が null です！", gameObject);
            return;
        }

        defaultSpriteRenderer.gameObject.SetActive(true);
        attackSpriteRenderer.gameObject.SetActive(false);

        float healValue = Stats.MaxHP * healAmount / 100; // MaxHPの5%を回復量に設定
        // 1秒ごとに体力を回復する処理を追加
        Observable.Interval(System.TimeSpan.FromSeconds(1))
            .Subscribe(_ =>
            {
                CombatHandler.TakeHeal(healValue);
            })
            .AddTo(healDisposables);
        damageDisposables.Clear(); // 体力減少処理を停止
        isInDefenceMode = true; // 回復モードフラグを設定
        // モード変更ステータスを適用
        Stats.SetAttackPower(0);
        Stats.SetSpeed(0);
    }

    // ==========================
    // 破壊時の処理
    // ==========================
    public override void OnDestroyed()
    {
        base.OnDestroyed();
        healDisposables.Dispose(); // オブジェクトが破棄される際に回復処理を停止
        damageDisposables.Dispose(); // オブジェクトが破棄される際に体力減少処理を停止
    }

    // ==========================
    // Omegaスキル
    // ==========================
    public override void ActivateOmegaSkill()
    {
        if (baseStats.IsOmegaUpgrade)
        {
            float atkbuffValue = originalPower * 0.2f;
            Stats.ApplyStatusModifierByUpgrade(0, atkbuffValue, 0, 0, 0);
        }
    }

    // ==========================
    // Alphaスキル
    // ==========================
    public override void ActivateAlphaSkill()
    {
        if (baseStats.isAlphaUpgrade)
        {
            healAmount = 10f;
        }
    }

    // ==========================
    // Lambdaスキル
    // ==========================
    public override void ActivateLambdaSkill()
    {
        if (baseStats.isLambdaUpgrade)
        {
            float atkbuffValue = originalPower * 0.3f;
            float spdBuffValue = originalSpeed * 0.3f;
            Stats.ApplyStatusModifierByUpgrade(0, atkbuffValue, spdBuffValue, 0, 0);
        }
    }
}