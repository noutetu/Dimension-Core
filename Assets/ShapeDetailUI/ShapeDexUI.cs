using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
// ==========================
// 図鑑図鑑のUIを管理するクラス
// ==========================
public class ShapeDexUI : MonoBehaviour
{
    [Header("図鑑の表示エリア")]
    [SerializeField] private Transform contentPanel;
    [Header("図形のエントリープレハブ")]
    [SerializeField] private GameObject shapeEntryPrefab;
    [Header("未発見の図形のアイコン")]
    [SerializeField] private Sprite nullSprite;  
    [Header("図形の詳細情報を表示するパネル")]
    [SerializeField] private GameObject shapeDetailsPanel;
    [Header("詳細パネルを閉じるボタン")]
    [SerializeField] private Button closedDetailButton;
    [Header("図形の説明を表示するテキスト")]
    [SerializeField] private GameObject simpleText;

    // 選択された図形
    ShapeBaseStats selectedShape;
    // 各図駅のボタンを格納するリスト
    List<ShapeButton> shapeButtons = new List<ShapeButton>(); 
    // ボタンの色を管理するクラス
    ButtonColorManager buttonColorManager;
    // ==========================
    // 初期化
    // ==========================
    private void OnEnable()
    {
        // 選択された図形をnullに設定
        selectedShape = null;
        // ボタンの色を管理するクラスを初期化
        buttonColorManager = new ButtonColorManager(shapeButtons);
        // 図鑑のUIを更新
        UpdateDexUI();
        // 図形の詳細情報を非表示
        shapeDetailsPanel.SetActive(false);
    }

    // ==========================
    // 図鑑のUIを更新
    // ==========================
    // 図鑑のUIを更新するメソッド
    public void UpdateDexUI()
    {
        // contentPanel内の全ての子オブジェクトをチェック
        foreach (Transform child in contentPanel)
        {
            // shapeDetailsPanel、simpleText、closedDetailButtonは対象外にする
            if (child.gameObject != shapeDetailsPanel &&
                child.gameObject != simpleText &&
                child.gameObject != closedDetailButton.gameObject)
            {
                Destroy(child.gameObject);  // 対象外以外のオブジェクトを破壊
            }
        }
        // 以前のリストをクリア
        shapeButtons.Clear();
        // ShapeDexから全ての図形情報を取得
        List<ShapeBaseStats> shapes = ShapeDex.Instance.GetAllShapes();
        // 全ての図形情報をUIに表示
        foreach (var shape in shapes)
        {
            // shapeEntryPrefabをインスタンス化してcontentPanelに配置
            GameObject entry = Instantiate(shapeEntryPrefab, contentPanel);
            // ShapeButtonコンポーネントを取得
            ShapeButton shapeButton = entry.GetComponent<ShapeButton>();
            shapeButtons.Add(shapeButton);

            // 図形が発見されているかどうかで表示するテキストとアイコンを変更
            if (shape.IsDiscovered)
            {
                shapeButton.SetShapeButton(shape, SetButton);
            }
            else
            {
                shapeButton.SetShapeButton("？？？", nullSprite);
            }
        }
        // ボタンの色を更新
        buttonColorManager.UpdateButtons(shapeButtons);
        // shapeDetailsPanelを最後に表示するように設定
        shapeDetailsPanel.transform.SetAsLastSibling();
        // 閉じるボタンのリスナーを設定
        closedDetailButton.onClick.RemoveAllListeners();
        closedDetailButton.onClick.AddListener(() =>
        {
            // 図形の詳細情報を非表示
            shapeDetailsPanel.SetActive(false);
            // 選択された図形をnullに設し、色をリセット
            buttonColorManager.ResetButtonColors();
            selectedShape = null;
        });
    }

    // ==========================
    // 図形の詳細情報を表示
    // ==========================
    public void ShowShapeDetailsPanel(ShapeBaseStats shape)
    {
        // 選択された図形が既に選択されている場合
        if (shape == selectedShape)
        {
            // 詳細表示を非表示にする
            shapeDetailsPanel.SetActive(false);
            // 選択された図形をnullに設定
            selectedShape = null;
            // ボタンの色をリセット
            buttonColorManager.ResetButtonColors();
            return;
        }

        // 選択された図形を設定
        selectedShape = shape;
        // 詳細表示を表示
        shapeDetailsPanel.SetActive(true);
        // ゲームオブジェクト名を指定してテキストを取得
        shapeDetailsPanel.transform.Find("ShapeName").GetComponent<Text>().text = shape.ShapeName;
        shapeDetailsPanel.transform.SetAsLastSibling();
        // 図形の詳細情報を表示
        shapeDetailsPanel.GetComponent<ShapeDetailPanel>().SetShapeDetail(shape);
        // ボタンの色をリセット
        buttonColorManager.ResetButtonColors();
        // 選択されたボタンをハイライト
        buttonColorManager.HighlightSelectedButton(shape);
    }
    // ==========================
    // ボタンが押されたときに図形の詳細情報を表示
    // ==========================
    private void SetButton(ShapeBaseStats shape)
    {
        ShowShapeDetailsPanel(shape);
    }
}
