using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Shape は図形オブジェクトの「存在管理とライフサイクル」および
/// IMovable（移動インターフェース）の実装に特化
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(ShapeMovementHandler))]
[RequireComponent(typeof(ShapeStats))]
[RequireComponent(typeof(ShapeTextHandler))]
[RequireComponent(typeof(ShapeCollisionHandler))]
[RequireComponent(typeof(ShapeCombatHandler))]
public abstract class Shape : MonoBehaviour
{
    // --------- フィールド ---------
    [Header("基本ステータス (ScriptableObject)")]
    [SerializeField] protected ShapeBaseStats baseStats;
    public ShapeBaseStats BaseStats => baseStats;

    // 各種依存コンポーネント(RequreComponentで自動取得)
    [HideInInspector] public ShapeStats Stats { get; private set; }
    [HideInInspector] public ShapeCombatHandler CombatHandler { get; protected set; }
    [HideInInspector] public ShapeCollisionHandler CollisionHandler { get; private set; }
    [HideInInspector] public ShapeMovementHandler FloatingController { get; private set; }

    // 図形としての状態
    public bool IsEnemy { get;  set; }
    public bool IsDead { get; private set; }

    // 破壊イベント 
    public event UnityAction OnDestroyedEvent;
    // ================================
    // 初期化
    // ================================
    public virtual void Awake()
    {
        // 必要コンポーネントを取得
        Stats = GetComponent<ShapeStats>();
        CombatHandler = GetComponent<ShapeCombatHandler>();
        CollisionHandler = GetComponent<ShapeCollisionHandler>();
        FloatingController = GetComponent<ShapeMovementHandler>();
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
        // 浮遊コントローラ初期化 (速度を参照するため最後)
        FloatingController.Initialize(this);
    }

    // ================================
    // 衝突処理
    // ================================
    protected virtual void OnFriendCollision(Shape friend) { }
    protected virtual void OnEnemyCollision(Shape enemy) 
    {
        if (enemy.IsDead) return;
        enemy.CombatHandler.TakeDamage(Stats.CurrentPower, Stats.CriticalRate,enemy.IsDead);
    }

    // ================================
    // 図形が破壊された時の処理
    // ================================
    public virtual void OnDestroyed()
    {
        if (IsDead) return;
        IsDead = true;

        OnDestroyedEvent?.Invoke();

        // 購読解除など
        CombatHandler.DisposeAll();
        Stats.Dispose();
    }
    void OnDestroy()
    {
        OnDestroyed();
    }
    public virtual void ActivateOmegaSkill() { }
    public virtual void ActivateAlphaSkill() { }
    public virtual void ActivateLambdaSkill() { }
}
