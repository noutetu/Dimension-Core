using UnityEngine;
using UniRx;
using System;
// ==========================
// Starの特性で呼び出されるオブジェクト
// ==========================
[RequireComponent(typeof(ShapeCollisionHandler))]
[RequireComponent(typeof(ShapeMovementHandler))]
public class ShootingStar : MonoBehaviour
{
    bool isEnemy;                           // 敵かどうか
    bool isInitialized = false;             // 初期化されたかどうか

    float damage;                           // ダメージ
    public float speed = 2f;                // 速度
    private IDisposable timerSubscription;  // 消滅までの時間

    private ShapeMovementHandler moveMentHandler;    // 移動制御
    private ShapeCollisionHandler collisionHandler;     // 衝突判定

    Star star;     // ShootingStarを発射したStar

    // ==========================
    // 初期化
    // ==========================
    void Awake()
    {
        // コンポーネントの取得
        collisionHandler = GetComponent<ShapeCollisionHandler>();
        moveMentHandler = GetComponent<ShapeMovementHandler>();
        if (collisionHandler == null || moveMentHandler == null)
        {
            Debug.LogError("Required components are missing.");
        }
    }

    // ==========================
    // 初期化
    // ==========================
    public void Initialize(float damage, bool isEnemy, Star star, bool isUpgraded = false)
    {
        this.damage = damage;   // ダメージ設定
        this.isEnemy = isEnemy; // 敵かどうか設定
        SetColor(isEnemy);      // 色設定
        this.star = star;       // 親Star設定
        isInitialized = true;   // 初期化完了
        // 強化されている場合は自動消滅しない
        if (isUpgraded) { return; }
        // 三秒後に消える
        timerSubscription = Observable.Timer(System.TimeSpan.FromSeconds(3))
            .Subscribe(_ =>
            {
                if (this != null) DestroyShootingStar();
            })
            .AddTo(this);
        
        if (collisionHandler != null && moveMentHandler != null)
        {
            // 衝突判定と移動制御の初期化
            collisionHandler.Initialize(OnFriendCollision, OnEnemyCollision, isEnemy);
            moveMentHandler.Initialize(this);
        }
        else
        {
            Debug.LogError("Initialization failed due to missing components.");
        }
    }

    // ==========================
    // 味方との衝突時の処理
    // ==========================
    private void OnFriendCollision(Shape shape) { }

    // ==========================
    // 敵との衝突時の処理
    // ==========================
    private void OnEnemyCollision(Shape shape)
    {
        if (!isInitialized) return;
        if (shape.IsEnemy == isEnemy) return;
        // ダメージを与える
        shape.CombatHandler.TakeDamage(damage, 0, shape.IsDead);
        // ShootingStarを消滅
        DestroyShootingStar();
    }

    // ==========================
    // ShootingStarの破壊
    // ==========================
    public void DestroyShootingStar()
    {
        if (this == null) return;
        // ShootingStarを消滅
        Destroy(gameObject);
        // Starのリストから削除
        star.shootingStars.Remove(this);
    }

    // ==========================
    // オブジェクトの破壊時の処理
    // ==========================
    private void OnDestroy()
    {
        if (timerSubscription != null)
        {
            // タイマーの解放
            timerSubscription.Dispose();
        }
    }

    // ==========================
    // 色の設定
    // ==========================
    public void SetColor(bool isEnemy)
    {
        // 敵の場合は赤、それ以外は白
        Color color = isEnemy ? Color.red : Color.white;
        GetComponent<SpriteRenderer>().color = color;
    }
}
