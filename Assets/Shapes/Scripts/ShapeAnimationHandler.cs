using UnityEngine;
using DG.Tweening;

/// <summary>
/// ShapeAnimation は図形のアニメーション処理を管理するクラス。
/// 点滅や破壊アニメーションなどを実行する。
/// </summary>
public class ShapeAnimationHandler : MonoBehaviour
{
    private SpriteRenderer spr;
    private Tween blinkTween;
    private Tween destroyTween;
    protected const int BLINK_COUNT = 2;
    protected const float BLINK_INTERVAL = 0.1f;
    private bool isDestroyed = false;

    // ================================
    // 初期化
    // ================================
    public void Initialize(Shape shape)
    {
        InitializeSpriteRenderer(shape.IsEnemy);
        shape.OnDestroyedEvent += PlayDestroyAnimation;
    }

    public void InitializeSpriteRenderer(bool isEnemy)
    {
        spr = GetComponentInChildren<SpriteRenderer>();
        if (spr == null)
        {
            DebugUtility.LogError($"{gameObject.name} の SpriteRenderer が見つかりません！", gameObject);
            return;
        }
        if (isEnemy)
        {
            spr.color = Color.red;
        }
    }

    // ================================
    // 破壊アニメーション
    // ================================
    public void PlayDestroyAnimation()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        if (spr == null || transform == null) return;

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
        Color originalColor = spr.color;
        originalColor.a = 1f;
        spr.color = originalColor;

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
        if (spr == null) return;

        if (blinkTween != null)
        {
            blinkTween.Kill();
        }

        // 点滅処理
        blinkTween = spr.DOFade(0f, BLINK_INTERVAL)
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
        if (spr == null) return;

        float targetAlpha = Mathf.Lerp(0.2f, 1.0f, hpRatio);

        Color newColor = spr.color;
        newColor.a = targetAlpha;
        spr.color = newColor;
    }

    public void SetColor(Color color)
    {
        if (spr == null) return;

        spr.color = color;
    }

    // ================================
    // 終了処理
    // ================================
    void OnDestroy()
    {
        KillTweens();
    }

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

