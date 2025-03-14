using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;

public class TextAnimator : MonoBehaviour
{
    public enum AnimationType
    {
        Pulse,        // 拡縮アニメーション（タイトル向け）
        Shake,        // シェイク（次元の崩壊感）
        FadeLoop,     // フェードイン・アウト（ボタン向け）
        ScaleBounce,  // バウンドする拡縮（ボタン向け）
        Glitch,       // グリッチ（次元の歪み）
        Wave,         // 波のように揺れる
        Float,        // ふわふわと浮遊
        FadeInOut,    // ふわっと消えて現れる
        Typewriter,   // 1文字ずつ表示する
        None,
    }

    [Header("アニメーション設定")]
    public AnimationType firstAnimationType;
    public AnimationType secondAnimationType;
    public float duration = 1.5f; // アニメーションの長さ
    public float strength = 10f; // シェイク時の強さ
    public float scaleAmount = 1.1f; // 拡縮の倍率
    public float waveAmplitude = 10f; // 波の振れ幅
    public float glitchAmount = 5f; // グリッチの強さ
    public float typewriterSpeed = 0.05f; // 1文字ずつ表示する速度

    private Text textComponent;
    private RectTransform rectTransform;
    private Tween activeTween;
    private string fullText; // 元のテキストを保持
    private float originalAlpha;
    private Coroutine typewriterCoroutine; // タイプライター用コルーチン

    // ==========================
    // サウンド
    // ==========================
    [Header("サウンド")]
    public AudioSource audioSource;

    private void OnEnable()
    {
        KillActiveTween();
        textComponent = GetComponent<Text>();
        rectTransform = GetComponent<RectTransform>();

        if (textComponent == null || rectTransform == null)
        {
            DebugUtility.LogWarning("TextAnimator: Text or RectTransform component is missing.");
            return;
        }
        originalAlpha = textComponent.color.a;

        StartAnimation(firstAnimationType);
    }

    private void OnDisable() {
        textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, originalAlpha);
        KillActiveTween();

    }

    // ==========================
    // アニメーション開始
    // ==========================
    public void StartAnimation(AnimationType type)
    {
        KillActiveTween();

        switch (type)
        {
            case AnimationType.Pulse:
                activeTween = AnimatePulse();
                break;
            case AnimationType.Shake:
                activeTween = AnimateShake();
                break;
            case AnimationType.FadeLoop:
                activeTween = AnimateFadeLoop();
                break;
            case AnimationType.ScaleBounce:
                activeTween = AnimateScaleBounce();
                break;
            case AnimationType.Glitch:
                activeTween = AnimateGlitch();
                break;
            case AnimationType.Wave:
                activeTween = AnimateWave();
                break;
            case AnimationType.Float:
                activeTween = AnimateFloat();
                break;
            case AnimationType.FadeInOut:
                activeTween = AnimateFadeInOut();
                break;
            case AnimationType.Typewriter:
                if (typewriterCoroutine != null)
                {
                    StopCoroutine(typewriterCoroutine);
                }
                typewriterCoroutine = StartCoroutine(AnimateTypewriter());
                break;
            case AnimationType.None:
                break;
        }
    }

    // ==========================
    // アニメーションメソッド
    // ==========================
    private Tween AnimatePulse()
    {
        return rectTransform.DOScale(scaleAmount, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private Tween AnimateShake()
    {
        return rectTransform.DOShakePosition(duration, strength, 10, 90, false, true)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.InOutSine);
    }

    private Tween AnimateFadeLoop()
    {
        return textComponent.DOFade(0.2f, duration / 2)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private Tween AnimateScaleBounce()
    {
        return rectTransform.DOScale(scaleAmount, duration / 2)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.OutElastic);
    }

    private Tween AnimateGlitch()
    {
        return rectTransform.DOShakePosition(duration, glitchAmount, 30, 90, false, true)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(Ease.Linear);
    }

    private Tween AnimateWave()
    {
        return rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + waveAmplitude, duration / 2)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutSine);
    }

    private Tween AnimateFloat()
    {
        return rectTransform.DOAnchorPosY(rectTransform.anchoredPosition.y + 20f, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }

    private Tween AnimateFadeInOut()
    {
        return textComponent.DOFade(0.3f, duration / 2)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(Ease.InOutQuad);
    }

    // ==========================
    // タイプライターアニメーション
    // ==========================
    private IEnumerator AnimateTypewriter()
    {
        fullText = textComponent.text; // 元のテキストを保持
        textComponent.text = ""; // 初期状態は空にする

        for (int i = 0; i < fullText.Length; i++)
        {
            audioSource.Stop();
            PlaySound(); // タイプ音を再生
            textComponent.text += fullText[i]; // 1文字ずつ追加
            yield return new WaitForSeconds(typewriterSpeed); // 指定した速度で待つ
        }
        if (secondAnimationType != AnimationType.Typewriter)
        {
            StartAnimation(secondAnimationType);
        }
    }

    // ==========================
    // タイプライター終了
    // ==========================
    public void FinishTypewriterImmediately()
    {
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }

        // タイプライター用に保持していた全文があれば、それを反映
        if (!string.IsNullOrEmpty(fullText))
        {
            textComponent.text = fullText;
        }

        // 通常、タイプライター終了後に secondAnimationType を開始しているため、
        // スキップした場合も同じ挙動を再現する
        if (secondAnimationType != AnimationType.Typewriter)
        {
            StartAnimation(secondAnimationType);
        }
    }

    // 何らかのUIボタン押下時にスキップする例
    public void OnSkipButtonPressed()
    {
        this.FinishTypewriterImmediately();
    }

    // ==========================
    // Tween管理
    // ==========================
    private void KillActiveTween()
    {
        if (activeTween != null && activeTween.IsActive())
        {
            activeTween.Kill();
            activeTween = null;
        }

        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
        }
    }

    public void ResetTransform()
    {
        KillActiveTween(); // すべてのTweenを停止
        if (rectTransform != null)
        {
            rectTransform.localScale = Vector3.one; // スケールを元に戻す
        }
        if (textComponent != null)
        {
            textComponent.DOKill();
            textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1f); // アルファ値をリセット
        }
    }

    // ==========================
    // サウンド再生
    // ==========================
    public void PlaySound()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            DebugUtility.LogWarning("TextAnimator: AudioSource component is missing. Added automatically.");
        }
        audioSource.Play();
    }
}
