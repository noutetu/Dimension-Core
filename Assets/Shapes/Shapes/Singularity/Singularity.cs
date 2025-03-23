using UnityEngine;
using UniRx;
// ==========================
// Singularityクラス
// 回復モードと攻撃モードを切り替える
// ==========================
public class Singularity : Shape
{
    [Header("デフォルトのスプライトレンダラー")]
    [SerializeField] private SpriteRenderer defaultSpriteRenderer;
    [Header("攻撃モードのスプライトレンダラー")]
    [SerializeField] private SpriteRenderer attackSpriteRenderer;

    private float defenseThreshold = 0.3f;  // 防御モードに移行する体力の割合
    private float healAmount = 7;           // 1秒ごとに回復する量(%)
    private float originalPower;            // 元の攻撃力を保存
    private float originalSpeed;            // 元の速度を保存
    private bool isInDefenseMode = false;   // 回復モードの状態を管理するフラグ

    // 回復処理用のDisposable
    private CompositeDisposable healDisposables = new CompositeDisposable();
    private CompositeDisposable damageDisposables = new CompositeDisposable();

    // ==========================
    // 初期化
    // ==========================
    public override void Initialize()
    {
        // 元の攻撃力を保存
        originalPower = baseStats.BaseAttackPower; 
        // 元の速度を保存
        originalSpeed = baseStats.BaseSpeed; 
        // 通常の初期化処理
        base.Initialize();
        // HPの変化を監視
        ObserveHPChanges();
    }

    // ==========================
    // HPの変化を監視
    // ==========================
    private void ObserveHPChanges()
    {
        Stats.CurrentHP
            .Subscribe(currentHP =>
            {
                // 体力がの割合
                float hpRatio = currentHP / baseStats.BaseHP;
                // 体力が防御モードに移行する割合を下回った場合、防御モードに移行
                if (hpRatio <= defenseThreshold)
                {
                    ChangeToDefenseMode();
                }
                // 体力が100%になった場合、攻撃モードに移行
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
        if (!isInDefenseMode) return; // すでに攻撃モードの場合は何もしない

        if (CombatHandler != null)
        {
            // モード変更
            CombatHandler.ChangeMode(isInDefenseMode);
        }
        else
        {
            DebugUtility.LogError($"{gameObject.name} の CombatHandler が null です！", gameObject);
            return;
        }
        // スプライトレンダラーアクティブに
        defaultSpriteRenderer.gameObject.SetActive(true);
        attackSpriteRenderer.gameObject.SetActive(true);
        // 元の攻撃力と速度に戻す
        Stats.SetAttackPower(originalPower);
        Stats.SetSpeed(originalSpeed); 

        // Lambdaスキルがアップグレードされている場合
        if (baseStats.IsLambdaUpgrade)
        {
            // 攻撃と速度をそれぞれ30%アップ
            float attackBuffValue = originalPower * 1.3f;
            float speedBuffValue = originalSpeed * 1.3f;

            // 攻撃力を計算
            float attack = Stats.CurrentPower + attackBuffValue;
            // 速度を計算
            float speed = Stats.Speed.Value + speedBuffValue;

            // ステータスを適用
            Stats.SetAttackPower(attack);
            Stats.SetSpeed(speed);
            // 2秒ごとに体力を減少させる処理を追加
            Observable.Interval(System.TimeSpan.FromSeconds(2))
                .Subscribe(_ =>
                {
                    CombatHandler.TakeDamage(60, 0, false); // 体力を徐々に減少させる
                })
                .AddTo(damageDisposables);
        }

        healDisposables.Clear(); // 回復処理を停止
        isInDefenseMode = false; // defenseModeフラグを設定
    }

    // ==========================
    // 防御モードに変更
    // ==========================
    public void ChangeToDefenseMode()
    {
        if (isInDefenseMode) return; // すでに防御モードの場合は何もしない

        if (CombatHandler != null)
        {
            // モード変更
            CombatHandler.ChangeMode(isInDefenseMode);
        }
        else
        {
            DebugUtility.LogError($"{gameObject.name} の CombatHandler が null です！", gameObject);
            return;
        }
        // スプライトレンダラーを設定
        defaultSpriteRenderer.gameObject.SetActive(true);
        attackSpriteRenderer.gameObject.SetActive(false);
        // MaxHPの一定割合を回復量に設定
        float healValue = Stats.MaxHP * healAmount / 100; 
        // 1秒ごとに体力を回復する処理を追加
        Observable.Interval(System.TimeSpan.FromSeconds(1))
            .Subscribe(_ =>
            {
                CombatHandler.TakeHeal(healValue);
            })
            .AddTo(healDisposables);
        // 体力減少処理を停止
        damageDisposables.Clear(); 
        // 回復モードフラグを設定
        isInDefenseMode = true; 
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
        // オメガアップグレードされている場合
        if (baseStats.IsOmegaUpgrade)
        {
            // 攻撃力を20%アップ
            float atkbuffValue = originalPower * 0.2f;
            // ステータスを適用
            Stats.ApplyStatusModifierByUpgrade(0, atkbuffValue, 0, 0, 0);
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
            // 回復量を10%に設定
            healAmount = 10f;
        }
    }

    // ==========================
    // Lambdaスキル
    // ==========================
    public override void ActivateLambdaSkill()
    {
        // ラムダアップグレードされている場合
        if (baseStats.IsLambdaUpgrade)
        {
            // 攻撃力と速度をそれぞれ30%アップ
            float atkbuffValue = originalPower * 0.3f;
            float spdBuffValue = originalSpeed * 0.3f;
            // ステータスを適用
            Stats.ApplyStatusModifierByUpgrade(0, atkbuffValue, spdBuffValue, 0, 0);
        }
    }
}