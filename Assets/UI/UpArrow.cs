using UnityEngine;
using DG.Tweening;

// ==========================
// チュートリアル用の矢印アニメーション
// ==========================
public class TutorialArrowFloat : MonoBehaviour
{
    [Header("浮き上がる距離")]
    [SerializeField] private float floatDistance = 20f; 
    [Header("アニメーション時間")]
    [SerializeField] private float duration = 1.5f;
    [Header("Easingの種類")]
    [SerializeField] private Ease easeType = Ease.InOutSine;

    private RectTransform arrowRect; // 矢印のRectTransform 
    private Tween floatTween;        // 浮き上がるアニメーション

    // ==========================
    // 初期化
    // ==========================
    private void OnEnable()
    {
        // RectTransformを取得
        arrowRect = GetComponent<RectTransform>();
        if (arrowRect == null)
        {
            DebugUtility.LogWarning("RectTransformが見つかりません。UI要素にアタッチしてください。");
        }
        // 最初のアニメーション開始
        StartFloatingAnimation();
    }

    // ==========================
    // アニメーション開始
    // ==========================
    /// <summary>
    /// 矢印を上下にループさせるアニメーションを開始
    /// </summary>
    public void StartFloatingAnimation()
    {
        if (arrowRect == null) return;

        // DOTweenのオブジェクトが既にある場合、先にKillしておく
        KillCurrentTween();

        // 現在の位置を取得
        Vector2 startPos = arrowRect.anchoredPosition;
        // 「現在位置 + floatDistance」を目標とし、Yoyoループで戻る
        floatTween = arrowRect
            .DOAnchorPosY(startPos.y + floatDistance, duration)
            .SetLoops(-1, LoopType.Yoyo)
            .SetEase(easeType);
    }

    // ==========================
    // アニメーション停止
    // ==========================
    /// <summary>
    /// アニメーションを停止し、矢印を元の位置に戻す
    /// </summary>
    public void StopAndReset()
    {
        KillCurrentTween();
        // 元の位置に戻したい場合
        //   arrowRect.anchoredPosition = initialPos; 
        // などの処理を入れるとよい
    }

    // ==========================
    // Tweenの管理
    // ==========================
    private void KillCurrentTween()
    {
        // Tweenがアクティブな場合、Killしてnullにする
        if (floatTween != null && floatTween.IsActive())
        {
            floatTween.Kill();
            floatTween = null;
        }
    }
    // ==========================
    // 破棄時の処理
    // ==========================
    private void OnDestroy()
    {
        floatTween.Kill();
    }
}
