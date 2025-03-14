using UnityEngine;

/// <summary>
/// 図形の出現方法を決めるクラス
/// </summary>
public class SelectionModeManager : MonoBehaviour
{
    public static SelectionMode GetSelectionMode()
    {
        int currentDifficulty = GameDifficultyManager.Instance.GetCurrentDifficulty();
        if (currentDifficulty <= 2)
        {
            return SelectionMode.ManyCommon; // コモン多め
        }
        else if (currentDifficulty <= 4)
        {
            return SelectionMode.ManyCommon; // コモン多め
        }
        else if (currentDifficulty <= 6)
        {
            return SelectionMode.ManyRare; // レア多め
        }
        else if (currentDifficulty <= 8)
        {
            return SelectionMode.OnlyRare; // レアのみ
        }
        else
        {
            return SelectionMode.ManyLegendary; // レジェンダリー多め
        }
    }
}
