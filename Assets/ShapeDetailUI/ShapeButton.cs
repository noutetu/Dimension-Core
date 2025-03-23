using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using DG.Tweening;
// ========================
// 図形のボタンの処理を行うクラス
// ========================
public class ShapeButton : MonoBehaviour
{
    // ボタンに格納する図形のデータ
    [HideInInspector] public ShapeBaseStats shapeData;
    [Header("枠")]
    [SerializeField] private Image frame;
    [Header("アイコン")]
    [SerializeField] private Image icon;
    [Header("名前")]
    [SerializeField] private Text shapeName;
    [Header("価格")]
    [SerializeField] private Text coinText;
    [Header("価格アイコン")]
    [SerializeField] private Image coinIcon;
    [Header("カウント")]
    [SerializeField] private Text countText;
    [Header("ボタン")]
    [SerializeField] private Button button;

    // 選択時の点滅アニメーション
    Tween blink;

    // ========================
    // 初期化
    // ========================
    public void SetShapeButton(ShapeBaseStats data, UnityAction<ShapeBaseStats> action, float count = 0)
    {
        // データの設定
        this.shapeData = data;
        // 名前の表示
        shapeName.text = data.ShapeName;
        // アイコンの表示
        icon.sprite = data.ShapeIcon;
        // 価格テキストの設定
        coinText.text = data.Price.ToString();
        // コインを表示
        coinText.gameObject.SetActive(true);
        coinIcon.gameObject.SetActive(true);
        // カウントテキストの設定
        SetCountText(count);
        // ボタンの色設定
        ResetColor();
        // ボタンのクリックイベント設定
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => action(shapeData));
    }
    // ========================
    // 初期化(名前と画像だけの場合)
    // ========================
    public void SetShapeButton(string name, Sprite sprite)
    {
        // データの設定
        shapeName.text = name;
        // アイコンの表示
        icon.sprite = sprite;

        // 価格を非表示
        coinText.gameObject.SetActive(false);
        coinIcon.gameObject.SetActive(false);
        // カウントを非表示
        countText.gameObject.SetActive(false);
    }

    // ========================
    // カウントテキストの設定
    // ========================
    private void SetCountText(float count)
    {
        if (countText == null)
        {
            Debug.LogError("countTextがnullです。");
            return;
        }
        // カウントが0より大きい場合
        if (count > 0)
        {
            // カウントテキストを表示
            countText.text = $"x{count}";
            // カウントをアクティブにする
            countText.gameObject.SetActive(true);
        }
        // カウントが0以下の場合
        else
        {
            // カウントを非表示にする
            countText.gameObject.SetActive(false);
        }
    }

    // ========================
    // 価格を非表示
    // ========================
    public void HidePrice()
    {
        coinText.gameObject.SetActive(false);
        coinIcon.gameObject.SetActive(false);
    }

    // ========================
    // 選択時の色を変更
    // ========================
    public void SelectedColor()
    {
        blink = frame.DOColor(Color.gray, 0.8f).SetLoops(-1, LoopType.Yoyo);
    }
    // ========================
    // 色をリセット
    // ========================
    public void ResetColor()
    {
        if (blink != null)
        {
            blink.Kill();
        }
        frame.color = Color.white;
    }

    // ========================
    // 現在の色を取得
    // ========================
    public Color GetColor()
    {
        return frame.color;
    }

    // ========================
    // ハイライト色を取得
    // ========================
    public Color GetHighlightColor()
    {
        return Color.white; 
    }
    // ========================
    // 非アクティブ時の処理
    // ========================
    void OnDisable()
    {
        if (blink != null)
        {
            blink.Kill();
        }
    }
}
