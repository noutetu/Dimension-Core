using UnityEngine;

// =====================
/// 戦闘報酬クラス
// =====================
public class BattleCoinReward : MonoBehaviour
{
    // =====================
    // 難易度に応じた報酬
    // =====================
    public int GetReward()
    {
        int difficulty = GameDifficultyManager.Instance.GetCurrentDifficulty();
        return difficulty switch
        {
            1 => 40,
            2 => 40,
            3 => 50,
            4 => 50,
            5 => 60,
            6 => 60,
            7 => 70,
            8 => 70,
            9 => 80,
            10 => 80,
            _ => 0,
        };
    }

    // =====================
    /// 報酬テキストを生成
    // =====================
    public void ShowReward(string text, bool isCoinIncrease)
    {   
        // UIキャンバスを取得
        Canvas uiCanvas = MainMessage.Instance.uiCanvas;
        if (uiCanvas == null)
        {
            DebugUtility.LogError("BattleCoinReward: uiCanvasが設定されていません！");
            return;
        }

        // オブジェクトプールから EffectText を取得
        EffectText effectText = EffectTextPool.Instance.GetFromPool();

        if (effectText != null)
        {
            // キャンバスの子要素に設定
            effectText.transform.position = transform.position;
            // 報酬の表示
            effectText.SetRewardText(text, isCoinIncrease);
        }
        else
        {
            DebugUtility.LogError("BattleCoinReward: EffectTextをプールから取得できませんでした！");
        }
    }
}
