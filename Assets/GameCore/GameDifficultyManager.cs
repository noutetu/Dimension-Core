using UnityEngine;

// ===========================================
// ゲームの難易度を管理するクラス
// ===========================================
public class GameDifficultyManager : MonoBehaviour
{
    public static GameDifficultyManager Instance { get; private set; } // シングルトン

    private int currentDifficulty = 1; // 現在の難易度 (1~10)
    private int maxDifficulty = 10;// 最大の難易度

    public bool isStrongEnemy; // 強い難易度かどうか
    // その時の難易度で生成できるレアリティの合計
    public int TotalRarity
    {
        get
        {
            return currentDifficulty switch
            {
                <= 2 => 2,
                <= 4 => 4,
                <= 6 => 5,
                <= 8 => 6,
                   9 => 7,
                  10 => 8,
                   _ => 0,
            };
        }
    }
    // ===========================================
    // 初期化
    // ===========================================
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
        isStrongEnemy = false;
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
        // フェーズ5の時は強い難易度に設定
        int currentPhase = StageManager.Instance.GetCurrentPhase();
        if (isStrongEnemy || currentPhase == 5)
        {
            SetStrongDifficulty();
            isStrongEnemy = false;
            return true;
        }
        // 難易度の設定
        int difficulty = StageManager.Instance.GetCurrentStage();
        // 20%の確率で強敵に設定
        if (Random.value < 0.2f)
        {
            // 強敵に設定
            difficulty = GetStrongDifficulty();
            currentDifficulty = Mathf.Clamp(difficulty, 1, maxDifficulty);
            return true;
        }
        // 弱い難易度か強い難易度かをランダムで設定
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
