using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Events;

public class ShapeDexUI : MonoBehaviour
{
    [SerializeField] private Transform contentPanel;  // 図鑑の表示エリア
    [SerializeField] private GameObject shapeEntryPrefab;  // UIのプレハブ
    [SerializeField] private Sprite nullSprite;  // 未発見の図形のアイコン

    [SerializeField] private GameObject shapeDetailsPanel;  // 図形の詳細情報を表示するパネル(破壊対象から除外)
    [SerializeField] private Button closedDetailButton;  // ボタンを押した時の処理を確認するためのボタン
    [SerializeField] private GameObject simpleText;  // 図形の説明を表示するテキスト(破壊対象から除外)

    ShapeBaseStats selectedShape;

    private void OnEnable()
    {
        UpdateDexUI();
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
            if (child.gameObject != shapeDetailsPanel && child.gameObject != simpleText && child.gameObject != closedDetailButton.gameObject)
            {
                Destroy(child.gameObject);  // 対象外以外のオブジェクトを破壊
            }
        }

        // ShapeDexから全ての図形情報を取得
        List<ShapeBaseStats> shapes = ShapeDex.Instance.GetAllShapes();
        foreach (var shape in shapes)
        {
            // shapeEntryPrefabをインスタンス化してcontentPanelに配置
            GameObject entry = Instantiate(shapeEntryPrefab, contentPanel);
            // 図形が発見されているかどうかで表示するテキストとアイコンを変更
            entry.GetComponentInChildren<Text>().text = shape.IsDiscovered ? shape.ShapeName : "？？？";
            entry.GetComponentInChildren<Image>().sprite = shape.IsDiscovered ? shape.ShapeIcon : nullSprite;
            if (shape.IsDiscovered)
            {
                // ボタンのリスナーを設定
                entry.GetComponentInChildren<Button>().onClick.RemoveAllListeners();
                entry.GetComponentInChildren<Button>().onClick.AddListener(() => SetButton(shape));
            }
        }
        // shapeDetailsPanelを最後に表示するように設定
        shapeDetailsPanel.transform.SetAsLastSibling();
        // 閉じるボタンのリスナーを設定
        closedDetailButton.onClick.RemoveAllListeners();
        closedDetailButton.onClick.AddListener(() => shapeDetailsPanel.SetActive(false));
    }

    // ==========================
    // 図形の詳細情報を表示
    // ==========================
    public void ShowShapeDetailsPanel(ShapeBaseStats shape)
    {
        if(shape == selectedShape)
        {
            shapeDetailsPanel.SetActive(false);
            selectedShape = null;
            return;
        }
        selectedShape = shape;
        
        shapeDetailsPanel.SetActive(true);
        // ゲームオブジェクト名を指定してテキストを取得
        shapeDetailsPanel.transform.Find("ShapeName").GetComponent<Text>().text = shape.ShapeName;        
        shapeDetailsPanel.transform.SetAsLastSibling();
        // 図形の詳細情報を表示
        shapeDetailsPanel.GetComponent<ShapeDetailPanel>().SetShapeDetail(shape);
    }
    // ==========================
    // ボタンが押されたときに図形の詳細情報を表示
    // ==========================
    private void SetButton(ShapeBaseStats shape)
    {
        ShowShapeDetailsPanel(shape);
    }
}
