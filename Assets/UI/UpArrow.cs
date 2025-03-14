using UnityEngine;
using DG.Tweening;

/// <summary>
/// チュートリアル用の矢印アニメーションを行うコンポーネント
/// UI上に配置された矢印が上下にふわふわと動く演出
/// </summary>
public class TutorialArrowFloat : MonoBehaviour
{
    [Header("アニメーション設定")]
    [SerializeField] private float floatDistance = 20f; // 浮き上がる距離
    [SerializeField] private float duration = 1.5f;    // 往復にかかる時間
    [SerializeField] private Ease easeType = Ease.InOutSine; // アニメのEasing

    private RectTransform arrowRect; 
    private Tween floatTween;

    // ==========================
    // 初期化
    // ==========================
    private void OnEnable()
    {
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
    // ツイーンの管理
    // ==========================
    private void KillCurrentTween()
    {
        if (floatTween != null && floatTween.IsActive())
        {
            floatTween.Kill();
            floatTween = null;
        }
    }
}
