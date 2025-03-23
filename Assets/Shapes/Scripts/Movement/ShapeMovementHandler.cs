using UniRx;
using UnityEngine;

// ================================
// 図形の移動を制御するクラス
// ================================ 
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(ShapeCollisionHandler))]
public class ShapeMovementHandler : MonoBehaviour
{
    private Rigidbody2D rb;                 // Rigidbody2Dコンポーネント
    private float speed;                    // 移動速度
    private float MIN_SPEED = 1f;           // 最小速度
    private float lastReflectionTime = 0f; // 最後に反射処理を行った時間
    private const float REFLECTION_COOLDOWN = 0.1f; // 反射処理のクールダウン時間（秒）

    // ================================
    // 初期化処理
    // ================================
    public void Initialize(Shape shape)
    {
        // 初期速度を設定
        speed = shape.Stats.Speed.Value;
        // Shapeのstatsのスピードの変化を購読
        shape.Stats.Speed
        .Subscribe(newSpeed => speed = newSpeed)
        .AddTo(this);
        // Rigidbody2Dコンポーネントを取得
        rb = GetComponent<Rigidbody2D>();
        // 初期速度を設定
        ApplyRandomVelocity();
    }

    // ================================
    // ShootingStarの初期化処理 
    // ================================
    public void Initialize(ShootingStar shootingStar)
    {
        // 初期速度を設定
        speed = shootingStar.speed;
        // Rigidbody2Dコンポーネントを取得
        rb = GetComponent<Rigidbody2D>();
        // 初期速度を設定
        ApplyRandomVelocity();
    }

    // ================================
    // フレームごとの物理更新処理
    // ================================
    void FixedUpdate()
    {
        // 速度のチェックとリセット
        CheckAndResetVelocity();
        // 図形の動きを適用
        PerformMovement();
    }

    // ================================
    // 移動
    // ================================
    public void PerformMovement()
    {
        // Shapeは移動速度のみ管理、実際の移動はFloatingShapeControllerが行う想定
        rb.velocity = rb.velocity.normalized * speed;
    }

    // ================================
    // 速度のチェックとリセット
    // ================================
    void CheckAndResetVelocity()
    {
        // 最小速度を下回る場合は速度をリセット
        if (rb.velocity.magnitude < MIN_SPEED)
        {
            ApplyRandomVelocity();
        }
    }

    // ================================
    // ランダムな方向に初期速度を設定
    // ================================
    void ApplyRandomVelocity()
    {
        // ランダムな方向を計算
        Vector2 randomDirection = Random.insideUnitCircle.normalized;
        // 速度を設定
        rb.velocity = randomDirection * speed;
    }

    // ================================
    // 衝突時の処理
    // ================================
    void OnCollisionEnter2D(Collision2D collision)
    {
        // 壁との衝突時の反射処理
        if (collision.gameObject.CompareTag("Wall"))
        {
            ReflectVelocity(collision.contacts[0].normal);
            return;
        }
    }

    // ================================
    // 速度の反射処理
    // ================================
    void ReflectVelocity(Vector2 collisionNormal)
    {
        // クールダウン中の場合は反射処理をスキップ
        if (Time.time - lastReflectionTime < REFLECTION_COOLDOWN)
        {
            return;
        }

        // 現在の速度
        Vector2 incomingVelocity = rb.velocity;

        // 最小速度チェック
        if (incomingVelocity.magnitude < MIN_SPEED)
        {
            ApplyRandomVelocity();
            return;
        }

        // 反射ベクトルを計算
        Vector2 reflectedVelocity =
        Vector2.Reflect(incomingVelocity, collisionNormal)
        .normalized * speed;

        // 壁に再度衝突しないように少しずらす
        reflectedVelocity += collisionNormal * 0.1f;

        // 新しい速度を設定
        rb.velocity = reflectedVelocity;

        // 最後に反射した時間を記録
        lastReflectionTime = Time.time;
    }
}
