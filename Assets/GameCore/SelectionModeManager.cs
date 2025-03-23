using UnityEngine;

// =====================
/// 図形の出現方法を決めるクラス
// =====================
public class SelectionModeManager : MonoBehaviour
{
    // =====================
    /// 図形の出現方法を返すメソッド
    // =====================
    public static SelectionMode GetSelectionMode()
    {
        int currentDifficulty = GameDifficultyManager.Instance.GetCurrentDifficulty();
        return currentDifficulty switch
        {
            <= 2 => SelectionMode.ManyCommon, // コモン多め
            <= 4 => SelectionMode.ManyCommon, // コモン多め
            <= 6 => SelectionMode.ManyRare, // レア多め
            <= 8 => SelectionMode.OnlyRare, // レアのみ
            _ => SelectionMode.ManyLegendary // レジェンダリー多め
        };
    }
}
// ==========================
// 図形選択のモード
// ==========================
public enum SelectionMode
{
    OnlyCommon,
    OnlyRare,
    OnlyLegendary,
    ManyCommon,
    ManyRare,
    ManyLegendary
}

