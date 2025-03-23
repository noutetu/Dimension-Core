using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class PlayerUI : MonoBehaviour
{
    [Header("コンテンツパネル")]
    [SerializeField] private Transform contentPanel;

    [Header("図形ボタンプレハブ")]
    [SerializeField] private GameObject ShapeButtonPrefab;

    [Header("図形詳細パネル")]
    [SerializeField] private GameObject shapeUpgradePanel;

    private Player player; // プレイヤーオブジェクト
    ShapeBaseStats selectedShape; // 選択された図形

    List<ShapeButton> shapeButtons = new List<ShapeButton>(); // 図形ボタンリスト
    ButtonColorManager buttonColorManager; // ボタンの色を管理するクラス
    // ========================
    // 初期化
    // ========================
    private void OnEnable()
    {
        // 選択した図形を初期化
        selectedShape = null;
        // プレイヤーオブジェクトを取得
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            DebugUtility.LogError("Playerオブジェクトが見つかりません！");
            return;
        }
        // ボタンの色を管理するクラスを作成
        buttonColorManager = new ButtonColorManager(shapeButtons);
        // プレイヤーの図形を更新
        UpdatePlayerShapesUI();
        // 図形詳細パネルを非表示
        shapeUpgradePanel.SetActive(false);
    }

    // ========================
    // PlayerUIの更新
    // ========================
    public void UpdatePlayerShapesUI()
    {
        // コンテンツパネルをクリア
        ClearContentPanel();

        // 図形をグループ化してボタンを作成
        var groupedShapes = player.shapeManager.OwnedShapes.GroupBy(instance => instance);
        foreach (var group in groupedShapes)
        {   
            // 図形ボタンを生成
            ShapeButton shapeButton = CreateShapeButton();
            // 図形ボタンをリストに追加
            shapeButtons.Add(shapeButton);
            // 図形ボタンの詳細を設定
            SetShapeButtonDetails(shapeButton.gameObject, group.Key, group.Count());
        }
        // ボタンの色を更新
        buttonColorManager.UpdateButtons(shapeButtons);
    }

    // ========================
    // パネルのクリア
    // ========================
    private void ClearContentPanel()
    {
        // コンテンツパネル内の全ての子オブジェクトを削除
        foreach (Transform child in contentPanel)
        {
            Destroy(child.gameObject);
        }
        // shapeButtonsリストをクリア
        shapeButtons.Clear();
    }

    // ========================
    // 図形ボタンの作成
    // ========================
    private ShapeButton CreateShapeButton()
    {
        // プレハブから新しい図形ボタンを作成
        GameObject obj = Instantiate(ShapeButtonPrefab, contentPanel);
        ShapeButton shapeButton = obj.GetComponent<ShapeButton>();
        return shapeButton;
    }

    // ========================
    // 図形ボタンの詳細設定
    // ========================
    private void SetShapeButtonDetails(GameObject entryObject, ShapeBaseStats shapeStats, int shapeCount)
    {
        // 図形ボタンの詳細を設定
        ShapeButton shapeButton = entryObject.GetComponent<ShapeButton>();
        if (shapeButton != null)
        {
            shapeButton.SetShapeButton(shapeStats, ToggleShapeUpdatePanel, shapeCount);
            shapeButton.HidePrice();
        }
        else
        {
            DebugUtility.LogError("ShapeButtonコンポーネントが見つかりません！");
        }
    }

    // ========================
    // アップグレードパネルの切り替え
    // ========================
    private void ToggleShapeUpdatePanel(ShapeBaseStats shapeStats)
    {
        // 図形ボタンが押された時のイベント
        PanelManager.Instance.OnShapeButton?.Invoke();
        // 選択された図形が同じ場合はパネルを非表示
        if (shapeStats == selectedShape)
        {   
            // アップグレードパネルの非表示
            shapeUpgradePanel.SetActive(false);
            // 選択された図形をクリア
            selectedShape = null;
            // ボタンの色をリセット
            buttonColorManager.ResetButtonColors();
            return;
        }
        // 選択された図形を更新し、詳細パネルを表示
        selectedShape = shapeStats;
        shapeUpgradePanel.SetActive(true);
        // 選択された図形の詳細を表示
        shapeUpgradePanel.GetComponent<UpgradePanel>().SetShapeDetail(shapeStats, CloseShapeDetailPanel);
        // ボタンの色をリセットし、選択されたボタンをハイライト
        buttonColorManager.ResetButtonColors();
        buttonColorManager.HighlightSelectedButton(shapeStats);
    }

    // ========================
    // 図形詳細パネルの非表示
    // ========================
    private void CloseShapeDetailPanel()
    {
        // 選択された図形をクリアし、詳細パネルを非表示
        selectedShape = null;
        shapeUpgradePanel.SetActive(false);
        // ボタンの色をリセット
        buttonColorManager.ResetButtonColors();
    }
}
