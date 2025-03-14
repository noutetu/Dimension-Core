using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShapeCollisionHandler : MonoBehaviour
{
    // 衝突クールダウンを管理する辞書
    private Dictionary<GameObject, float> collisionCooldowns = new Dictionary<GameObject, float>();
    // クールダウン時間（秒）
    private const float COOLDOWN_TIME = 0.2f;
    // 敵かどうかのフラグ
    bool isEnemy;

    // 衝突時のイベント
    UnityAction<Shape> OnFriendCollision;
    UnityAction<Shape> OnEnemyCollision;

    // ==========================
    // 初期化メソッド
    // ==========================
    public void Initialize(UnityAction<Shape> onFriendCollision, UnityAction<Shape> onEnemyCollision, bool isEnemy)
    {
        this.isEnemy = isEnemy;
        OnFriendCollision = onFriendCollision;
        OnEnemyCollision = onEnemyCollision;
    }

    // ==========================
    // 2D衝突時に呼ばれるメソッド
    // ==========================
    void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    // ==========================
    // 衝突処理を行うメソッド
    // ==========================
    public void HandleCollision(GameObject other)
    {
        Shape otherShape = other.GetComponent<Shape>();
        if (otherShape == null || otherShape.IsDead) return;

        // クールダウン期間中かどうかをチェック
        if (collisionCooldowns.ContainsKey(other) && Time.time < collisionCooldowns[other])
        {
            return; // まだクールダウン期間中
        }

        // クールダウン時間を設定
        collisionCooldowns[other] = Time.time + COOLDOWN_TIME;

        // 敵か味方かを判定してイベントを呼び出す
        if (isEnemy == otherShape.IsEnemy)
        {
            DebugUtility.Log("Friend Collision");
            OnFriendCollision?.Invoke(otherShape);
        }
        else
        {
            DebugUtility.Log("Enemy Collision");
            OnEnemyCollision?.Invoke(otherShape);
        }
    }
}
