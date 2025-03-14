using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UniRx;
using UnityEngine.Events;

public enum SelectionMode
{
    OnlyCommon,
    OnlyRare,
    OnlyLegendary,
    ManyCommon,
    ManyRare,
    ManyLegendary
}

public class ShapeSelector : MonoBehaviour
{
    [SerializeField] private Button[] eventButtons;
    [SerializeField] private GameObject shapeDetailPanel;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button skipButton;

    private List<ShapeBaseStats> selectedShapes = new List<ShapeBaseStats>();
    private Player player;
    private ShapeBaseStats selectedShape = null;

    private int canSelectCount = 0;
    private int selectedCount = 0;
    private ReactiveProperty<bool> isFinishSelection = new ReactiveProperty<bool>(false);
    private SelectionMode selectionMode;
    public UnityAction NextPhase;

    private void Start() { }

    // ==========================
    // プレイヤーオブジェクトを取得し、スキップボタンのクリックイベントを設定
    // ==========================
    private void OnEnable()
    {
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            DebugUtility.LogError("Playerオブジェクトが見つかりません！");
            return;
        }
        shapeDetailPanel.SetActive(false);

        skipButton.onClick.AddListener(SkipSelection); // skipButtonのクリックイベントを設定
    }

    // ==========================
    // ボタンのクリックイベントをリセット
    // ==========================
    private void OnDisable()
    {
        foreach (Button button in eventButtons)
        {
            button.onClick.RemoveAllListeners();
        }
        confirmButton.onClick.RemoveAllListeners();
        skipButton.onClick.RemoveAllListeners();
    }

    // ==========================
    // 選択モード、選択可能な図形の数、および難易度に基づいてボタンに図形を割り当てる
    // ==========================
    public void AssignShapesToButtons(SelectionMode selectionMode, int canSelectCount)
    {
        shapeDetailPanel.SetActive(false);
        // 設定された選択可能な数と選択モードを保存
        this.canSelectCount = canSelectCount;
        this.selectionMode = selectionMode;
        selectedShapes.Clear();
        
        // SelectionModeをログに出力
        DebugUtility.Log($"SelectionMode: {selectionMode}");
        // 各レアリティの図形リストを取得
        List<ShapeBaseStats> commonShapes = RarityManager.Instance.GetShapesByRarity(Rarity.Common);
        List<ShapeBaseStats> rareShapes = RarityManager.Instance.GetShapesByRarity(Rarity.Rare);
        List<ShapeBaseStats> legendaryShapes = RarityManager.Instance.GetShapesByRarity(Rarity.Legendary);

        // 現在の次元を取得
        DimensionData currentDimension = DimensionManager.Instance.GetCurrentDimension();
        List<ShapeBaseStats> dimensionShapes = currentDimension != null ? currentDimension.possibleShapes : new List<ShapeBaseStats>();

        // 使用済みの図形を追跡するためのセット
        HashSet<ShapeBaseStats> usedShapes = new HashSet<ShapeBaseStats>();

        int difficulty = GameDifficultyManager.Instance.GetCurrentDifficulty();
        
        // 各ボタンに図形を割り当てる
        for (int i = 0; i < eventButtons.Length; i++)
        {
            // 選択モードと難易度に基づいてレアリティを決定
            Rarity chosenRarity = DetermineRarity(selectionMode, difficulty);
            ShapeBaseStats shape;

            // 重複しない図形を取得
            do
            {
                shape = GetRandomShapeByRarity(chosenRarity, commonShapes, rareShapes, legendaryShapes, dimensionShapes);
            } while (shape != null && usedShapes.Contains(shape));

            // 図形がnullでない場合、選択された図形リストに追加
            if (shape != null)
            {
                selectedShapes.Add(shape);
                usedShapes.Add(shape);
            }
        }

        // ボタンに図形を割り当てるループ
        for (int i = 0; i < eventButtons.Length; i++)
        {
            // 選択された図形がnullでない場合
            if (selectedShapes[i] != null)
            {
                // 図形データを取得
                ShapeBaseStats shapeData = selectedShapes[i];
                // ボタンの画像を図形のアイコンに設定
                eventButtons[i].GetComponent<Image>().sprite = shapeData.ShapeIcon;
                // ボタン内のコインテキストを設定
                Text coinText = eventButtons[i].transform.Find("CoinText").GetComponent<Text>();
                coinText.text = shapeData.Price.ToString();
                // ボタンのクリックイベントをリセット
                eventButtons[i].onClick.RemoveAllListeners();
                // ボタンがクリックされたときに図形の詳細を表示するように設定
                eventButtons[i].onClick.AddListener(() => ShowShapeDetail(shapeData));
            }
        }
    }

    // ==========================
    // 選択モードと難易度に基づいてレアリティを決定
    // ==========================
    private Rarity DetermineRarity(SelectionMode mode, int difficulty = -1)
    {
        return RarityManager.Instance.DetermineRarityByMode(mode);
    }

    // ==========================
    // レアリティを受け取って、そのレアリティの中からランダムに1つのShapeBaseStatsを返す
    // ==========================
    private ShapeBaseStats GetRandomShapeByRarity(Rarity rarity, List<ShapeBaseStats> common, List<ShapeBaseStats> rare, List<ShapeBaseStats> legendary, List<ShapeBaseStats> dimensionShapes)
    {
        // 30%の確率で次元の出やすい図形を選択
        if (Random.value < 0.3f && dimensionShapes.Count > 0)
        {
            return dimensionShapes[Random.Range(0, dimensionShapes.Count)];
        }

        switch (rarity)
        {
            case Rarity.Legendary:
                if (legendary.Count > 0) return legendary[Random.Range(0, legendary.Count)];
                return GetRandomShapeByRarity(Rarity.Rare, common, rare, legendary, dimensionShapes);
            case Rarity.Rare:
                if (rare.Count > 0) return rare[Random.Range(0, rare.Count)];
                return GetRandomShapeByRarity(Rarity.Common, common, rare, legendary, dimensionShapes);
            case Rarity.Common:
            default:
                return common.Count > 0 ? common[Random.Range(0, common.Count)] : null;
        }
    }

    // ==========================
    // 図形を実際に選択するメソッド
    // ==========================
    public void SelectShape(ShapeBaseStats selectedShape)
    {
        if (selectedShape == null)
        {
            Debug.LogWarning("Selected shape is null.");
            return;
        }

        if(selectedShape.Price > player.currencyManager.Currency)
        {
            DebugUtility.Log("所持コインが足りません！"); 
            return;
        }
        else
        {
            DebugUtility.Log("所持コインが足ります！");
        }
        // 所持コインから図形の価格を引く
        player.SpendCurrency(selectedShape.Price);
        // プレイヤーに選択された図形を追加
        player.AddShape(selectedShape);
        // 選択された図形のカウントを増やす
        selectedCount++;
        // 図形を獲得したので選択した図形をリセット
        this.selectedShape = null;
        // 選択可能な図形の数に達した場合
        if (selectedCount >= canSelectCount)
        {
            FinishSelection();
            return;
        }
        // 図形詳細パネルを非表示にする
        shapeDetailPanel.SetActive(false);
        // イベントパネルを再表示
        GameObject eventpanel = PanelManager.Instance.selectShapePanel;
        PanelManager.Instance.ShowPanel(eventpanel);
        // ボタンに図形を再割り当て
        AssignShapesToButtons(selectionMode, canSelectCount);
    }

    // ==========================
    // 選択をスキップするメソッド
    // ==========================
    private void SkipSelection()
    {
        Player player = FindObjectOfType<Player>();
        if (player == null)
        {
            DebugUtility.LogError("Playerオブジェクトが初期化されていません！");
            return;
        }

        selectedCount++;
        if (selectedCount >= canSelectCount)
        {
            FinishSelection();
            return;
        }
        shapeDetailPanel.SetActive(false);
        GameObject eventpanel = PanelManager.Instance.selectShapePanel;
        PanelManager.Instance.ShowPanel(eventpanel);
        AssignShapesToButtons(selectionMode, canSelectCount);
    }

    // ==========================
    // 選択が完了したときの処理を共通化
    // ==========================
    private void FinishSelection()
    {
        canSelectCount = 0;
        isFinishSelection.Value = true;
        PanelManager.Instance.ToggleDimensionSelectorPanel();
        GetComponentInParent<ShapeSelectPanel>().DisableEventPanel();
        GameFlowManager gameFlowManager = FindObjectOfType<GameFlowManager>();
        gameFlowManager.ButtonPanelTutorial();
    }

    // ==========================
    // 図形の詳細を表示
    // ==========================
    public void ShowShapeDetail(ShapeBaseStats shapeData)
    {
        if (shapeData == null)
        {
            Debug.LogWarning("Shape data is null.");
            return;
        }

        if (selectedShape == shapeData)
        {
            SelectShape(shapeData);
            return;
        }

        selectedShape = shapeData;
        shapeDetailPanel.SetActive(true);
        ShapeDetailPanel shapeDeta = shapeDetailPanel.GetComponent<ShapeDetailPanel>();
        shapeDeta.SetShapeDetail(shapeData);

        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => SelectShape(shapeData));
    }
}
