using UnityEngine;
using System.Collections.Generic;

public class ShapeDex : MonoBehaviour
{
    public static ShapeDex Instance { get; private set; }

    [SerializeField] private List<ShapeBaseStats> allShapes = new List<ShapeBaseStats>();  // 全図形リスト

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
            shape.IsDiscovered = false; // 図鑑リセット
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
        //========================
        // 大文字小文字の違いや空白を除去
        //========================
        ShapeBaseStats entry = allShapes.Find(shape => shape.ShapeName.Trim().ToLower() == shapeName.Trim().ToLower());

        if (entry == null)
        {
            DebugUtility.LogError($"図形 {shapeName} が allShapes に見つかりませんでした！");
            return;
        }

        if (!entry.IsDiscovered)
        {
            entry.IsDiscovered = true;
        }
        else
        {
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
    // 図鑑の全データを取得（UI表示用）
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
