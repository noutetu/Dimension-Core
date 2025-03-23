using UnityEngine;
using UnityEngine.Events;
// ================================
// 図形の基底クラス
// ================================
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(ShapeMovementHandler))]
[RequireComponent(typeof(ShapeBattleStats))]
[RequireComponent(typeof(ShapeTextHandler))]
[RequireComponent(typeof(ShapeCollisionHandler))]
[RequireComponent(typeof(ShapeCombatHandler))]

public abstract class Shape : MonoBehaviour
{
    // --------- フィールド ---------
    [Header("基本ステータス (ScriptableObject)")]
    [SerializeField] protected ShapeBaseStats baseStats;
    public ShapeBaseStats BaseStats => baseStats;

    // 各種依存コンポーネント(RequireComponentで自動取得)
    // 戦闘中のステータス
    [HideInInspector] public ShapeBattleStats Stats { get; private set; }
    // 戦闘中の処理ハンドラ
    [HideInInspector] public ShapeCombatHandler CombatHandler { get; protected set; }
    // 衝突処理ハンドラ
    [HideInInspector] public ShapeCollisionHandler CollisionHandler { get; private set; }
    // 移動処理ハンドラ
    [HideInInspector] public ShapeMovementHandler MovementHandler { get; private set; }

    // 図形としての状態
    public bool IsEnemy { get; set; }
    public bool IsDead { get; private set; }

    // 破壊イベント 
    public event UnityAction OnDestroyedEvent;

    // ================================
    // 初期化
    // ================================
    public virtual void Awake()
    {
        // 必要コンポーネントを取得
        Stats = GetComponent<ShapeBattleStats>();
        CombatHandler = GetComponent<ShapeCombatHandler>();
        CollisionHandler = GetComponent<ShapeCollisionHandler>();
        MovementHandler = GetComponent<ShapeMovementHandler>();
    }

    // ================================
    // 初期化
    // ================================
    public virtual void Initialize()
    {
        // 衝突ハンドラ初期設定
        CollisionHandler.Initialize(OnFriendCollision, OnEnemyCollision, IsEnemy);

        // エフェクトアプライヤ初期化
        CombatHandler.Initialize(this);

        // ステータス初期化
        Stats.Initialize(this, baseStats);
        // 移動ハンドラ初期化 (速度を参照するため最後)
        MovementHandler.Initialize(this);
    }

    // ================================
    // 衝突処理
    // ================================
    protected virtual void OnFriendCollision(Shape friend) { }
    protected virtual void OnEnemyCollision(Shape enemy)
    {
        if (enemy.IsDead) return;
        // ダメージ計算
        float damage = Stats.CurrentPower;
        // 敵オブジェクトがIEaseDamageを実装している場合
        if(enemy.GetComponent<IEaseDamage>() != null)
        {
            // ダメージを減少させる
            damage = enemy.GetComponent<IEaseDamage>().ReduceDamage(damage);
        }
        // ダメージを与える
        enemy.CombatHandler.TakeDamage(damage, Stats.CriticalRate, enemy.IsDead);
    }

    // ================================
    // 図形が破壊された時の処理
    // ================================
    public virtual void OnDestroyed()
    {
        if (IsDead) return;
        // 死亡フラグを立てる
        IsDead = true;
        // 破壊処理
        OnDestroyedEvent?.Invoke();

        // 購読解除
        CombatHandler.DisposeAll();
        Stats.Dispose();
    }
    // ================================
    // 破棄時の処理
    // ================================
    void OnDestroy()
    {
        OnDestroyed();
    }
    // ================================
    // 各種アップグレード
    // ================================
    public virtual void ActivateOmegaSkill() { }
    public virtual void ActivateAlphaSkill() { }
    public virtual void ActivateLambdaSkill() { }
}
public interface IEaseDamage
{
    float ReduceDamage(float damage);
}
