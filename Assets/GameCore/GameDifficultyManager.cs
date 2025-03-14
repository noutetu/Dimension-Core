using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// ゲーム全体の難易度を管理するシングルトンクラス
/// </summary>
public class GameDifficultyManager : MonoBehaviour
{
    public static GameDifficultyManager Instance { get; private set; }

    private int currentDifficulty = 1; // 現在の難易度 (1~10)
    private int maxDifficulty = 10;

    public bool strongNext;

    // 生成できるレアリティの合計
    public int TotalRarity
    {
        get
        {
            return currentDifficulty switch
            {
                1 => 2,
                2 => 2,
                3 => 4,
                4 => 4,
                5 => 5,
                6 => 5,
                7 => 6,
                8 => 6,
                9 => 7,
                10 => 8,
                _ => 0,
            };
        }
    }

    private void Awake()
    {
        // シングルトンの作成
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも維持
        }
        else
        {
            Destroy(gameObject);
        }
        strongNext = false;
    }

    // ===========================================
    // 現在の難易度の取得
    // ===========================================
    public int GetCurrentDifficulty()
    {
        return currentDifficulty;
    }
    // ===========================================
    // 難易度の設定
    // ===========================================
    public bool SetDifficulty()
    {
        int currentPhase = StageManager.Instance.GetCurrentPhase();
        
        if (strongNext || currentPhase == 5)
        {
            SetStrongDifficulty();
            strongNext = false;
            return true;
        }
        int difficulty = StageManager.Instance.GetCurrentStage();

        // 20%の確率で強い難易度に設定
        if (Random.value < 0.2f)
        {
            difficulty = GetStrongDifficulty();
            currentDifficulty = Mathf.Clamp(difficulty, 1, maxDifficulty);
            return true;
        }

        int randomAdjustment = Random.value < 0.5f ? 1 : 0;

        currentDifficulty = Mathf.Clamp(difficulty * 2 - randomAdjustment, 1, maxDifficulty);
        return false;
    }
    // ===========================================
    // 強い難易度の設定
    // ===========================================
    public void SetStrongDifficulty()
    {
        currentDifficulty = GetStrongDifficulty();
    }
    // ===========================================
    // 強い難易度の取得
    // ===========================================
    public int GetStrongDifficulty()
    {
        return Mathf.Clamp(StageManager.Instance.GetCurrentStage() * 2 + 1, 1, maxDifficulty);
    }
}
