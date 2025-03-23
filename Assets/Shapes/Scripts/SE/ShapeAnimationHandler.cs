using UnityEngine;
using DG.Tweening;

// ==========================================
// バトル中のアニメーション表示を管理するクラス
// ==========================================
public class ShapeAnimationHandler : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;              // スプライトレンダラー
    private Tween blinkTween;                           // 点滅アニメーション用Tween
    private Tween destroyTween;                         // 破壊アニメーション用Tween
    protected const int BLINK_COUNT = 1;                // 点滅回数
    protected const float BLINK_INTERVAL = 0.15f;       // 点滅間隔
    private bool isDestroyed = false;                   // 破壊済みかどうか

    // ================================
    // 初期化
    // ================================
    public void Initialize(Shape shape)
    {
        // スプライトレンダラーの初期化
        InitializeSpriteRenderer(shape.IsEnemy);
        // 破壊イベントの登録
        shape.OnDestroyedEvent += PlayDestroyAnimation;
    }
    // ================================
    // SpriteRendererの初期化
    // ================================
    public void InitializeSpriteRenderer(bool isEnemy)
    {
        // SpriteRendererの取得
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            DebugUtility.LogError($"{gameObject.name} の SpriteRenderer が見つかりません！", gameObject);
            return;
        }
        // 敵の場合は赤色にする
        if (isEnemy)
        {
            spriteRenderer.color = Color.red;
        }
    }

    // ================================
    // 破壊アニメーション
    // ================================
    public void PlayDestroyAnimation()
    {
        if (isDestroyed) return;
        if (spriteRenderer == null || transform == null) return;
        // 破壊済みフラグを立てる
        isDestroyed = true;
        // 既存のアニメーションをkill
        if (destroyTween != null && destroyTween.IsActive())
        {
            destroyTween.Kill();
        }

        float explosionScale = 2f; // 膨張倍率
        float shrinkScale = 0.5f; // 一瞬縮小する倍率
        float explosionDuration = 0.3f; // 膨張時間
        float pauseDuration = 0.5f; // 最後の停止時間

        // 透明度を1に戻す
        Color originalColor = spriteRenderer.color;
        originalColor.a = 1f;
        // 色を元に戻す
        spriteRenderer.color = originalColor;

        // 1回目の膨張
        destroyTween = transform.DOScale(transform.localScale * explosionScale, explosionDuration)
            .SetEase(Ease.OutExpo)
            .OnComplete(() =>
            {
                // 縮小
                destroyTween = transform.DOScale(transform.localScale * shrinkScale, explosionDuration)
                    .SetEase(Ease.InExpo)
                    .OnComplete(() =>
                    {
                        // 2回目の膨張
                        destroyTween = transform.DOScale(transform.localScale * explosionScale, explosionDuration)
                            .SetEase(Ease.OutExpo)
                            .OnComplete(() =>
                            {
                                // 少しの間静止してから削除
                                destroyTween = DOVirtual.DelayedCall(pauseDuration, () =>
                                {
                                    Destroy(gameObject);
                                });
                            });
                    });
            });
    }

    // ================================
    // 点滅アニメーション
    // ================================
    public void PlayBlinkAnimation(float hpRatio)
    {
        if (spriteRenderer == null) return;

        if (blinkTween != null)
        {
            // 既存のアニメーションをkill
            blinkTween.Kill();
        }

        // 点滅処理
        blinkTween = spriteRenderer.DOFade(0f, BLINK_INTERVAL)
            .SetLoops(BLINK_COUNT * 2, LoopType.Yoyo)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                FadeTo(hpRatio);
            });
    }

    // ================================
    // 透明度変更
    // ================================
    public void FadeTo(float hpRatio)
    {
        if (spriteRenderer == null) return;
        // 透明度の目標値を計算
        float targetAlpha = Mathf.Lerp(0.2f, 1.0f, hpRatio);
        // カラーを設定
        Color newColor = spriteRenderer.color;
        // 透明度を設定
        newColor.a = targetAlpha;
        // spriteRendererに設定
        spriteRenderer.color = newColor;
    }
    // ================================
    // 色変更
    // ================================
    public void SetColor(Color color)
    {
        if (spriteRenderer == null) return;
        // spriteRendererに設定
        spriteRenderer.color = color;
    }

    // ================================
    // 終了処理
    // ================================
    void OnDestroy()
    {
        // 既存のアニメーションをkill
        KillTweens();
    }
    // ================================
    // 既存のアニメーションをkill
    // ================================
    private void KillTweens()
    {
        if (blinkTween != null && blinkTween.IsActive())
        {
            blinkTween.Kill();
        }
        if (destroyTween != null && destroyTween.IsActive())
        {
            destroyTween.Kill();
        }
    }
}

