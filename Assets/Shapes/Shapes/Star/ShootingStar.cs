using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

[RequireComponent(typeof(ShapeCollisionHandler))]
[RequireComponent(typeof(ShapeMovementHandler))]
public class ShootingStar : MonoBehaviour
{
    bool isEnemy;
    float damage;
    public float speed = 2f;
    bool isInitialized = false;
    private IDisposable timerSubscription;

    private ShapeMovementHandler floatingController;
    private ShapeCollisionHandler collisionHandler;

    Star star;

    // ==========================
    // 初期化
    // ==========================
    void Awake()
    {
        collisionHandler = GetComponent<ShapeCollisionHandler>();
        floatingController = GetComponent<ShapeMovementHandler>();
        if (collisionHandler == null || floatingController == null)
        {
            Debug.LogError("Required components are missing.");
        }
    }

    // ==========================
    // 初期化
    // ==========================
    public void Initialize(float damage,bool isEnemy,Star star,bool isUpgraded = false)
    {
        this.damage = damage;
        this.isEnemy = isEnemy;
        SetColor(isEnemy);
        this.star = star;
        isInitialized = true;
        if (isUpgraded){return;}
        // 三秒後に消える
        timerSubscription = Observable.Timer(System.TimeSpan.FromSeconds(3))
            .Subscribe(_ =>
            {
                if (this != null) DestroyShootingStar();
            })
            .AddTo(this);

        if (collisionHandler != null && floatingController != null)
        {
            collisionHandler.Initialize(OnFriendCollision, OnEnemyCollision, isEnemy);
            floatingController.Initialize(this);
        }
        else
        {
            Debug.LogError("Initialization failed due to missing components.");
        }
    }

    // ==========================
    // 友軍との衝突時の処理
    // ==========================
    private void OnFriendCollision(Shape shape){}

    // ==========================
    // 敵との衝突時の処理
    // ==========================
    private void OnEnemyCollision(Shape shape)
    {
        if (!isInitialized) return;
        if (shape.IsEnemy == isEnemy) return;
        shape.CombatHandler.TakeDamage(damage,0,shape.IsDead);
        DestroyShootingStar();
    }

    // ==========================
    // ShootingStarの破壊
    // ==========================
    public void DestroyShootingStar()
    {
        if (this == null) return;
        Destroy(gameObject);
        star.shootingStars.Remove(this);
    }

    // ==========================
    // オブジェクトの破壊時の処理
    // ==========================
    private void OnDestroy()
    {
        if (timerSubscription != null)
        {
            timerSubscription.Dispose();
        }
    }

    // ==========================
    // 色の設定
    // ==========================
    public void SetColor(bool isEnemy)
    {
        Color color = isEnemy ? Color.red : Color.white;
        GetComponent<SpriteRenderer>().color = color;
    }
}
