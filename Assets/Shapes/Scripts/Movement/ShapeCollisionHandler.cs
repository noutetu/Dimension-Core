using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// ==========================
// 衝突処理を行うクラス
// ==========================
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
    [Header("衝突時のエフェクトを再生するためのコンポーネント")]
    [SerializeField] ShapeEffectHandler effectControllerPrefab;

    // ==========================
    // 初期化メソッド
    // ==========================
    public void Initialize(UnityAction<Shape> onFriendCollision, UnityAction<Shape> onEnemyCollision, bool isEnemy)
    {
        // 敵味方を設定
        this.isEnemy = isEnemy;
        // 味方との衝突処理を設定
        OnFriendCollision = onFriendCollision;
        // 敵との衝突処理を設定
        OnEnemyCollision = onEnemyCollision;
    }

    // ==========================
    // Awakeメソッド
    // ==========================
    void Awake()
    {
        if (effectControllerPrefab == null)
        {
            // リソースフォルダからエフェクトのプレハブを読み込む
            effectControllerPrefab = Resources.Load<ShapeEffectHandler>("ShapeParticle");
        }
    }

    // ==========================
    // 2D衝突時に呼ばれるメソッド
    // ==========================
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 衝突処理を行う
        HandleCollision(collision.gameObject);
        // 衝突時にパーティクルを再生
        if (effectControllerPrefab != null)
        {
            // パーティクルを再生
            ShapeEffectHandler effect = Instantiate(effectControllerPrefab, transform.position, Quaternion.identity);
            effect.PlayEffect(transform.position);
        }
    }

    // ==========================
    // 衝突処理を行うメソッド
    // ==========================
    public void HandleCollision(GameObject other)
    {
        // オブジェクトが図形かどうかをチェック
        Shape otherShape = other.GetComponent<Shape>();
        if (otherShape == null || otherShape.IsDead) return;

        // クールダウン期間中かどうかをチェック
        if (collisionCooldowns.ContainsKey(other) && Time.time < collisionCooldowns[other])
        {
            return; // まだクールダウン期間中
        }

        // クールダウン時間を設定
        collisionCooldowns[other] = Time.time + COOLDOWN_TIME;

        // 味方との衝突処理
        if (isEnemy == otherShape.IsEnemy)
        {
            DebugUtility.Log("Friend Collision");
            OnFriendCollision?.Invoke(otherShape);
        }
        // 敵との衝突処理
        else
        {
            DebugUtility.Log("Enemy Collision");
            OnEnemyCollision?.Invoke(otherShape);
        }
    }
}
