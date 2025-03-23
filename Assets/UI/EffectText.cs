using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
// ==========================
// 演出用のテキスト
// ==========================
public class EffectText : MonoBehaviour
{
    [SerializeField] private Text textComponent;
    private RectTransform rectTransform;
    private Tween moveTween;
    private Tween fadeTween;

    // ==========================
    // 初期化
    // ==========================
    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (textComponent == null)
        {
            textComponent = GetComponentInChildren<Text>();
            if (textComponent == null)
            {
                DebugUtility.LogError("DamageText: Text コンポーネントが見つかりません！");
            }
        }
    }

    // ==========================
    // 戦闘テキストの設定
    // ==========================
    public void SetBattleText(string text, Color color, bool isEnemy, bool isDamageTex = false)
    {
        if (textComponent == null) return;

        // 味方なら左方向、敵なら右方向に座標をシフト
        Vector2 startOffset = isEnemy ? new Vector2(100f, -500) : new Vector2(-100f, -500);
        rectTransform.anchoredPosition += startOffset;
        // ダメージテキストかどうか(バフや回復もあるため)
        bool isDamageText = isDamageTex;
        if (!isDamageText)
        {
            // ダメージ以外のテキストを上にずらす
            rectTransform.anchoredPosition += new Vector2(0, 200);
        }
        // テキストと色を設定
        textComponent.text = text;
        textComponent.color = color;

        // 固定の上方向（UI座標系で上に）に浮かぶように修正
        moveTween = rectTransform.DOAnchorPos(rectTransform.anchoredPosition + new Vector2(0, 50), 0.75f).SetEase(Ease.OutQuad);
        fadeTween = textComponent.DOFade(0f, 1.7f).OnComplete(() => EffectTextPool.Instance.ReturnToPool(this));
    }

    // ==========================
    // 報酬テキストの設定
    // ==========================
    public void SetRewardText(string reward, bool isCoinIncrease)
    {
        if (textComponent == null) return;
        // 数字を表示
        textComponent.text = reward;

        // コインの増減に応じて色を変更
        textComponent.color = isCoinIncrease ? Color.green : Color.red;

        // 固定の上方向（UI座標系で上に）に浮かぶように修正
        moveTween = rectTransform.DOAnchorPos(rectTransform.anchoredPosition + new Vector2(0, 50), 0.75f).SetEase(Ease.OutQuad);
        fadeTween = textComponent.DOFade(0f, 1.7f).OnComplete(() => EffectTextPool.Instance.ReturnToPool(this));
    }

    // ==========================
    // テキストのリセット
    // ==========================
    public void ResetEffectText()
    {
        rectTransform.anchoredPosition = Vector2.zero;  // 位置をリセット
        textComponent.color = Color.white;              // 色をリセット
        textComponent.text = string.Empty;              // テキストをリセット
        textComponent.DOFade(1f, 0f);                   // 透明度をリセット
    }

    // ==========================
    // オブジェクト破棄時の処理
    // ==========================
    void OnDestroy()
    {
        moveTween?.Kill();
        fadeTween?.Kill();
    }
}
