using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PanelManager : MonoBehaviour
{
    [Header("ShapeDexパネル")]
    [SerializeField] private GameObject shapeDexPanel;  // 図鑑パネル

    [Header("イベントパネル")]
    [SerializeField] public GameObject selectShapePanel;  // イベントパネル（独立）

    [Header("リザルトパネル")]
    [SerializeField] private GameObject resultPanel;  // リザルトパネル

    [Header("プレイヤーUIパネル")]
    [SerializeField] private GameObject playerUIPanel;

    [Header("ボタンパネル")]
    [SerializeField] private GameObject buttonPanel; // ボタンパネル（独立）

    [Header("次元選択パネル")]
    [SerializeField] private GameObject dimensionSelectorPanel; // 次元選択パネル(独立)

    [Header("ランダムイベントパネル")]
    [SerializeField] private GameObject randomEventPanel; // ランダムイベントパネル(独立)

    [Header("リスタート警告パネル")]
    [SerializeField] private GameObject RestartWarningPanel; // リスタート警告パネル
    [Header("ホームに戻る警告のパネル")]
    [SerializeField] private GameObject HomeWarningPanel; // ホームに戻る警告のパネル

    private Dictionary<GameObject, Vector2> panelOriginalPositions = new Dictionary<GameObject, Vector2>();
    private GameObject activePanel = null; // 現在開いているパネル（影響を受けないパネルは除外）

    // **影響を受けないパネルのリスト**
    private HashSet<GameObject> independentPanels = new HashSet<GameObject>();

    // **ボタンパネルが閉じた際に閉じるべきパネルのリスト**
    private List<GameObject> buttonPanelRelatedPanels = new List<GameObject>();

    public static PanelManager Instance { get; private set; }

    // ==========================
    // 初期化
    // ==========================
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            SaveOriginalPositions();
            RegisterIndependentPanels(); // **独立パネルを登録**
            RegisterButtonPanelRelatedPanels(); // **ボタンパネル関連のパネルを登録**
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ==========================
    /// 各パネルの元の位置を保存
    // ==========================
    private void SaveOriginalPositions()
    {
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
        independentPanels.Add(selectShapePanel);  // イベントパネル
        independentPanels.Add(buttonPanel); // ボタンパネル
        independentPanels.Add(dimensionSelectorPanel); // 次元選択パネル
        independentPanels.Add(randomEventPanel); // ランダムイベントパネル
    }

    // ==========================
    /// **ボタンパネルが閉じたときに同時に閉じるべきパネルを登録**
    // ==========================
    private void RegisterButtonPanelRelatedPanels()
    {
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

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null) return;

        Vector2 originalPosition = panelOriginalPositions[panel];
        rectTransform.anchoredPosition = new Vector2(originalPosition.x - offset, originalPosition.y);
        panel.SetActive(true);
        rectTransform.DOAnchorPos(originalPosition, duration).SetEase(Ease.OutQuad);

        // **影響を受けないパネルは `activePanel` にしない**
        if (!independentPanels.Contains(panel))
        {
            activePanel = panel;
        }

        SystemMessage.HideUpArrow(); // **パネルが開いたら、矢印を非表示にする**
    }

    // ==========================
    /// 指定のパネルを左にスライドして非表示
    // ==========================
    private void HidePanel(GameObject panel, float duration = 0.3f, float offset = 500f)
    {
        if (panel == null || !panelOriginalPositions.ContainsKey(panel)) return;

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        if (rectTransform == null) return;

        // DOTweenのアニメーションを停止
        rectTransform.DOKill();

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
        if (panel == null || !panelOriginalPositions.ContainsKey(panel)) return;

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
    public void ToggleShapeDexPanel() => TogglePanel(shapeDexPanel);
    public void ToggleSelectShapePanel() => TogglePanel(selectShapePanel); // **影響を受けない**
    public void ToggleResultPanel() => TogglePanel(resultPanel);
    public void TogglePlayerUIPanel() => TogglePanel(playerUIPanel);
    public void ToggleButtonPanel() => TogglePanel(buttonPanel); // **影響を受けない**
    public void ToggleDimensionSelectorPanel() => TogglePanel(dimensionSelectorPanel); // **影響を受けない**
    public void ToggleRandomEventPanel() => TogglePanel(randomEventPanel); // **影響を受けない**
    public void ToggleRestartWarningPanel() => TogglePanel(RestartWarningPanel);
    public void ToggleHomeWarningPanel() => TogglePanel(HomeWarningPanel);
}
