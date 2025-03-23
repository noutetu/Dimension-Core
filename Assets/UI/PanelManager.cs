using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
// ==========================
// パネル管理クラス
// ==========================
public class PanelManager : MonoBehaviour
{
    [Header("ShapeDexパネル")]
    [SerializeField] private GameObject shapeDexPanel;  // 図鑑パネル

    [Header("イベントパネル")]
    [SerializeField] public GameObject selectShapePanel;  // イベントパネル（独立）

    [Header("リザルトパネル")]
    [SerializeField] private GameObject resultPanel;  // リザルトパネル

    [Header("プレイヤーUIパネル")]
    [SerializeField] private GameObject playerUIPanel;  // プレイヤーUIパネル

    [Header("ボタンパネル")]
    [SerializeField] private GameObject buttonPanel; // ボタンパネル（独立）

    [Header("次元選択パネル")]
    [SerializeField] public GameObject dimensionSelectorPanel; // 次元選択パネル(独立)

    [Header("ランダムイベントパネル")]
    [SerializeField] private GameObject randomEventPanel; // ランダムイベントパネル(独立)

    [Header("リスタート警告パネル")]
    [SerializeField] private GameObject RestartWarningPanel; // リスタート警告パネル
    [Header("ホームに戻る警告のパネル")]
    [SerializeField] private GameObject HomeWarningPanel; // ホームに戻る警告のパネル

    private Dictionary<GameObject, Vector2> panelOriginalPositions = new Dictionary<GameObject, Vector2>();
    private GameObject activePanel = null; // 現在開いているパネル（影響を受けないパネルは除外）

    // **ボタンパネルが閉じた際に閉じるべきパネルのリスト**
    private List<GameObject> buttonPanelRelatedPanels = new List<GameObject>();
    // **影響を受けないパネルのリスト**
    private HashSet<GameObject> independentPanels = new HashSet<GameObject>();

    //　イベント
    public UnityAction OnDimensionSelectorPanel;    // 次元選択パネル開閉イベント
    public UnityAction OnButtonPanel;               // ボタンパネル開閉イベント
    public UnityAction OnPlayerUIPanel;             // プレイヤーUIパネル開閉イベント
    public UnityAction OnShapeButton;               // 図鑑ボタン開閉イベント

    // パネル開閉アニメーション
    private Tween slideTween;
    //　インスタンス
    public static PanelManager Instance { get; private set; }

    // ==========================
    // 初期化
    // ==========================
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SaveOriginalPositions(); // 各パネルの元の位置を保存
            RegisterIndependentPanels(); // **独立パネルを登録**
            RegisterButtonPanelRelatedPanels(); // **ボタンパネル関連のパネルを登録**
        }
        else
        {
            Destroy(gameObject); // インスタンスが既に存在する場合は破棄
        }
    }

    // ==========================
    /// 各パネルの元の位置を保存
    // ==========================
    private void SaveOriginalPositions()
    {
        // 各パネルの位置を保存
        SavePanelPosition(shapeDexPanel);
        SavePanelPosition(selectShapePanel);
        SavePanelPosition(resultPanel);
        SavePanelPosition(playerUIPanel);
        SavePanelPosition(buttonPanel);
        SavePanelPosition(dimensionSelectorPanel);
        SavePanelPosition(randomEventPanel);
        SavePanelPosition(RestartWarningPanel);
        SavePanelPosition(HomeWarningPanel);
    }

    // ==========================
    /// **影響を受けないパネルを登録**
    // ==========================
    private void RegisterIndependentPanels()
    {
        // 影響を受けないパネルをリストに追加
        independentPanels.Add(selectShapePanel);
        independentPanels.Add(buttonPanel);
        independentPanels.Add(dimensionSelectorPanel);
        independentPanels.Add(randomEventPanel);
    }

    // ==========================
    /// **ボタンパネルが閉じたときに同時に閉じるべきパネルを登録**
    // ==========================
    private void RegisterButtonPanelRelatedPanels()
    {
        // ボタンパネル関連のパネルをリストに追加
        buttonPanelRelatedPanels.Add(shapeDexPanel);
        buttonPanelRelatedPanels.Add(playerUIPanel);
    }

    // ==========================
    /// 指定パネルの位置を保存（null チェック付き）
    // ==========================
    private void SavePanelPosition(GameObject panel)
    {
        if (panel != null)
        {
            RectTransform rectTransform = panel.GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                // パネルの位置を辞書に保存
                panelOriginalPositions[panel] = rectTransform.anchoredPosition;
            }
        }
    }

    // ==========================
    /// 指定のパネルを左からスライドして表示
    // ==========================
    public void ShowPanel(GameObject panel, float duration = 0.5f, float offset = 500f)
    {
        if (panel == null || !panelOriginalPositions.ContainsKey(panel)) return;

        // **影響を受けないパネル以外は、開く前に他のパネルを閉じる**
        if (!independentPanels.Contains(panel) && activePanel != null && activePanel != panel)
        {
            HidePanel(activePanel, duration, offset);
        }
        //　開くパネルの位置を取得
        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null) return;

        // パネルを左からスライドして表示
        Vector2 originalPosition = panelOriginalPositions[panel];
        rectTransform.anchoredPosition = new Vector2(originalPosition.x - offset, originalPosition.y);
        panel.SetActive(true);
        slideTween = rectTransform.DOAnchorPos(originalPosition, duration).SetEase(Ease.OutQuad);

        // **影響を受けないパネルは `activePanel` にしない**
        if (!independentPanels.Contains(panel))
        {
            activePanel = panel;
        }
    }

    // ==========================
    /// 指定のパネルを左にスライドして非表示
    // ==========================
    private void HidePanel(GameObject panel, float duration = 0.3f, float offset = 500f)
    {
        if (panel == null || !panelOriginalPositions.ContainsKey(panel)) return;
        // 閉じるパネルの位置を取得
        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null) return;

        // DOTweenのアニメーションを停止
        rectTransform.DOKill();

        // パネルを左にスライドして非表示
        Vector2 originalPosition = panelOriginalPositions[panel];
        rectTransform.DOAnchorPos(new Vector2(originalPosition.x - offset, originalPosition.y), duration)
            .SetEase(Ease.InBack) // InBackいい
            .OnComplete(() => panel.SetActive(false));

        // **ボタンパネルを閉じたら、関連するパネルもすべて閉じる**
        if (panel == buttonPanel)
        {
            foreach (var relatedPanel in buttonPanelRelatedPanels)
            {
                HidePanel(relatedPanel, duration, offset);
            }
        }

        // **開いているパネルが閉じられたら、activePanel をリセット**
        if (activePanel == panel)
        {
            activePanel = null;
        }
    }

    // ==========================
    /// 指定のパネルの表示・非表示をトグル（スライドアニメーション付き）
    // ==========================
    public void TogglePanel(GameObject panel, float duration = 0.5f, float offset = 500f)
    {
        // パネルが存在しないか、元の位置が保存されていない場合は処理しない
        if (panel == null || !panelOriginalPositions.ContainsKey(panel)) return;

        // パネルの表示・非表示をトグル
        if (panel.activeSelf)
        {
            HidePanel(panel, duration, offset);
        }
        else
        {
            ShowPanel(panel, duration, offset);
        }
    }
    // ==========================
    // 各パネルをトグルするメソッド（ボタン用）
    // ==========================
    // ShapeDexパネルをトグル
    public void ToggleShapeDexPanel() => TogglePanel(shapeDexPanel);
    // イベントパネルをトグル
    public void ToggleSelectShapePanel() => TogglePanel(selectShapePanel); // **影響を受けない**
    // リザルトパネルをトグル
    public void ToggleResultPanel() => TogglePanel(resultPanel);
    // ランダムイベントパネルをトグル
    public void ToggleRandomEventPanel() => TogglePanel(randomEventPanel); // **影響を受けない**
    // リスタート警告パネルをトグル
    public void ToggleRestartWarningPanel() => TogglePanel(RestartWarningPanel);
    // ホームに戻る警告のパネルをトグル
    public void ToggleHomeWarningPanel() => TogglePanel(HomeWarningPanel);
    // 次元選択パネルをトグル
    public void ToggleDimensionSelectorPanel()
    {
        OnDimensionSelectorPanel?.Invoke();
        TogglePanel(dimensionSelectorPanel); // **影響を受けない**
    }
    // ボタンパネルをトグル
    public void ToggleButtonPanel()
    {
        OnButtonPanel?.Invoke();
        TogglePanel(buttonPanel); // **影響を受けない**
    }
    // プレイヤーUIパネルをトグル
    public void TogglePlayerUIPanel()
    {
        OnPlayerUIPanel?.Invoke();
        TogglePanel(playerUIPanel);
    }
    // ==========================
    // 破壊時処理
    // ==========================
    void OnDestroy()
    {
        slideTween?.Kill();
    }
}
