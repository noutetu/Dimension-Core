using UnityEngine;
using System.Collections.Generic;
// ==========================
// 図形図鑑クラス
// ==========================
public class ShapeDex : MonoBehaviour
{
    // シングルトン
    public static ShapeDex Instance { get; private set; }
    [Tooltip("全図形のデータ")]
    [SerializeField] private List<ShapeBaseStats> allShapes = new List<ShapeBaseStats>();
    // ==========================
    // 初期化
    // ==========================
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        foreach (var shape in allShapes)
        {
            // 図形の発見状態をリセット
            shape.IsDiscovered = false;
        }
    }
    // ==========================
    // 図形を図鑑に登録
    // ==========================
    public void DiscoverShape(string shapeName)
    {

        if (allShapes == null || allShapes.Count == 0)
        {
            DebugUtility.LogError("allShapes リストが空または null です！");
            return;
        }
        // 文字の前後の空白を削除してから検索
        ShapeBaseStats button = allShapes.Find(shape => shape.ShapeName.Trim().ToLower() == shapeName.Trim().ToLower());

        if (button == null)
        {
            DebugUtility.LogError($"図形 {shapeName} が allShapes に見つかりませんでした！");
            return;
        }
        // 図形が未発見の場合
        if (!button.IsDiscovered)
        {
            // 図形を発見済みにする
            button.IsDiscovered = true;
        }
    }
    // ==========================
    // 図形の情報を取得
    // ==========================
    public ShapeBaseStats GetShapeInfo(string shapeName)
    {
        return allShapes.Find(shape => shape.ShapeName == shapeName);
    }
    // ==========================
    // 全図形のデータを取得（UI表示用）
    // ==========================
    public List<ShapeBaseStats> GetAllShapes()
    {
        return allShapes;
    }
    // ==========================
    // 図鑑をリセット
    // ==========================
    public void ResetShapeDex()
    {
        foreach (var shape in allShapes)
        {
            shape.IsDiscovered = false;
            shape.ResetUpgrades();
        }
    }
}
