using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
// ==========================
// 図形選択画面の処理を行うクラス
// ==========================
public class ShapeSelector : MonoBehaviour
{
    [Header("図形ボタン")]
    [SerializeField] private ShapeButton[] shapeButtons;
    [Header("図形詳細パネル")]
    [SerializeField] private GameObject shapeDetailPanel;
    [Header("確認ボタン")]
    [SerializeField] private Button confirmButton;
    [Header("スキップボタン")]
    [SerializeField] private Button skipButton;

    // 選択された図形のリスト
    private List<ShapeBaseStats> selectedShapes = new List<ShapeBaseStats>();
    // プレイヤーオブジェクト
    private Player player;
    // 選択された図形
    private ShapeBaseStats selectedShape = null;
    // 選択可能な図形の数
    private int canSelectCount = 0;
    private int selectedCount = 0;
    // 選択モード
    private SelectionMode currentSelectionMode;
    // 次のフェーズに進むためのイベント
    public UnityAction NextPhase;
    // ボタンの色を管理するクラス
    private ButtonColorManager buttonColorManager;

    // ==========================
    // 初期化
    // ==========================
    private void OnEnable()
    {
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            DebugUtility.LogError("Playerオブジェクトが見つかりません！");
            return;
        }
        // イベントボタンを非表示にする
        shapeDetailPanel.SetActive(false);
        // ボタンの色を管理するクラスを初期化
        buttonColorManager = new ButtonColorManager(new List<ShapeButton>(shapeButtons)); 
        // skipButtonのクリックイベントを設定
        skipButton.onClick.AddListener(SkipSelection); 
    }

    // ==========================
    // ボタンのクリックイベントをリセット
    // ==========================
    private void OnDisable()
    {
        confirmButton.onClick.RemoveAllListeners();
        skipButton.onClick.RemoveAllListeners();
    }

    // ==========================
    // 選択モード、選択可能な図形の数、および難易度に基づいてボタンに図形を割り当てる
    // ==========================
    public void AssignShapesToButtons(SelectionMode selectionMode, int canSelectCount)
    {
        // 図形詳細パネルを非表示にする
        shapeDetailPanel.SetActive(false);
        // 設定された選択可能な数と選択モードを保存
        this.canSelectCount = canSelectCount;
        currentSelectionMode = selectionMode;
        //　以前の図形リストをクリア
        selectedShapes.Clear();
        
        // 現在の次元を取得
        DimensionData currentDimension = DimensionManager.Instance.GetCurrentDimension();
        // 現在の次元の出やすい図形リストを取得
        List<ShapeBaseStats> dimensionShapes = currentDimension != null ? currentDimension.possibleShapes : new List<ShapeBaseStats>();

        // 同じ図形が選択肢に出るのを防ぐためのセット
        HashSet<ShapeBaseStats> usedShapes = new HashSet<ShapeBaseStats>();

        int difficulty = GameDifficultyManager.Instance.GetCurrentDifficulty();
        
        // 各ボタンに図形データを割り当てるループ
        for (int i = 0; i < shapeButtons.Length; i++)
        {
            // 選択モードと難易度に基づいてレアリティを決定
            Rarity chosenRarity = RarityManager.Instance.GetRarityBySelectionMode(selectionMode);

            // 重複しない図形を取得
            ShapeBaseStats shape = RarityManager.Instance.GetRandomShapeByRarity(chosenRarity);
            if(usedShapes.Contains(shape))
            {
                shape = RarityManager.Instance.GetRandomShapeByRarity(chosenRarity);
            }
            // 図形がnullでない場合
            if (shape != null)
            {
                DebugUtility.Log($"ボタン {i} に図形 {shape.ShapeName} を割り当てました。");
                selectedShapes.Add(shape);  // 選択された図形リストに追加   
                usedShapes.Add(shape);      // 重複しないようにセットに追加
            }
            else
            {
                DebugUtility.LogWarning($"ボタン {i} に割り当てる図形が見つかりませんでした。");
            }
        }
        DebugUtility.Log("選択された図形の数: " + selectedShapes.Count);
        // ボタンに図形の詳細を割り当てるループ
        for (int i = 0; i < shapeButtons.Length; i++)
        {
            // インデックスが範囲内で選択された図形がnullでない場合
            if (i < selectedShapes.Count && selectedShapes[i] != null)
            {
                // 図形データを取得
                ShapeBaseStats shapeData = selectedShapes[i];
                // ボタンの初期化
                shapeButtons[i].SetShapeButton(shapeData,SelectShape);
            }
        }
        buttonColorManager.UpdateButtons(new List<ShapeButton>(shapeButtons));
    }
    // ==========================
    // 図形を実際に購入するメソッド
    // ==========================
    public void BuyShape(ShapeBaseStats selectedShape)
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

        // 所持コインから図形の価格を引く
        player.SpendCurrency(selectedShape.Price);
        // プレイヤーに選択された図形を追加
        player.AddShape(selectedShape);
        // 選択カウントを増やす
        selectedCount++;
        // 図形を獲得したので選択した図形をリセット
        this.selectedShape = null;
        // 選択可能な図形の数に達した場合
        if (selectedCount >= canSelectCount)
        {
            // 選択終了
            FinishSelection();
            return;
        }
        // そうでない場合は再度選択可能な図形を表示
        // 図形詳細パネルを非表示にする
        shapeDetailPanel.SetActive(false);
        // イベントパネルを再表示
        GameObject eventpanel = PanelManager.Instance.selectShapePanel;
        PanelManager.Instance.ShowPanel(eventpanel);
        // ボタンに図形を再割り当て
        AssignShapesToButtons(currentSelectionMode, canSelectCount);
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
        // 選択可能な図形の数に達した場合
        selectedCount++;
        if (selectedCount >= canSelectCount)
        {   
            // 選択終了
            FinishSelection();
            return;
        }
        // そうでない場合は再度選択可能な図形を表示
        shapeDetailPanel.SetActive(false);
        // イベントパネルを再表示
        GameObject eventpanel = PanelManager.Instance.selectShapePanel;
        PanelManager.Instance.ShowPanel(eventpanel);
        // ボタンに図形を再割り当て
        AssignShapesToButtons(currentSelectionMode, canSelectCount);
    }

    // ==========================
    // 選択が完了したときの処理を共通化
    // ==========================
    private void FinishSelection()
    {
        // 選択可能な図形の数をリセット
        canSelectCount = 0;
        // 次元選択パネルを表示
        PanelManager.Instance.ToggleDimensionSelectorPanel();
        // イベントパネルを非表示
        GetComponentInParent<ShapeSelectPanel>().DisableEventPanel();
    }

    // ==========================
    // 図形を押した時の処理
    // ==========================
    public void SelectShape(ShapeBaseStats shapeData)
    {
        if (shapeData == null)
        {
            Debug.LogWarning("Shape data is null.");
            return;
        }

        if (selectedShape == shapeData)
        {
            BuyShape(shapeData);
            return;
        }

        if (selectedShape != null)
        {
            // 以前選択されていた図形の色をリセット
            buttonColorManager.ResetButtonColors();
        }
        // 選択された図形を保存
        selectedShape = shapeData;
        // 選択された図形の詳細を表示
        ShowShapeDetail(shapeData);
        // 選択された図形の色を変更
        buttonColorManager.HighlightSelectedButton(shapeData);
    }
    // ==========================
    // 図形の詳細を表示
    // ==========================
    private void ShowShapeDetail(ShapeBaseStats shapeData)
    {
        // 図形詳細パネルを表示
        shapeDetailPanel.SetActive(true);
        // 図形詳細パネルに図形データをセット
        ShapeDetailPanel shapeDeta = shapeDetailPanel.GetComponent<ShapeDetailPanel>();
        shapeDeta.SetShapeDetail(shapeData);
        // 確認ボタンのクリックイベントを設定
        confirmButton.onClick.RemoveAllListeners();
        confirmButton.onClick.AddListener(() => BuyShape(shapeData));
    }
}
