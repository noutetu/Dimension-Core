using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// バトルでのコイン報酬を管理するクラス
public class BattleCoinReward : MonoBehaviour
{
    // 数字の1~10を受け取って、それに応じた報酬を返すメソッドを作成してください。 
    // 1~3の場合は10、4~7の場合は20、8~10の場合は30を返してください。
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

    /// <summary>
    /// 報酬テキストを生成
    /// </summary>
    public void ShowReward(string text, bool isCoinIncrease)
    {
        Canvas uiCanvas = SystemMessage.Instance.uiCanvas; // Screen Space - Overlay 用 Canvas

        if (uiCanvas == null)
        {
            DebugUtility.LogError("BattleCoinReward: uiCanvas が設定されていません！");
            return;
        }

        // **オブジェクトプールから EffectText を取得**
        EffectText effectText = EffectTextPool.Instance.GetFromPool();

        if (effectText != null)
        {
            effectText.transform.position = transform.position;
            // **報酬の表示**
            effectText.SetRewardText(text, isCoinIncrease);
        }
        else
        {
            DebugUtility.LogError("BattleCoinReward: EffectText をプールから取得できませんでした！");
        }
    }
}
