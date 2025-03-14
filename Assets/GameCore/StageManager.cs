using UnityEngine;
using System.Collections.Generic;

public class StageManager : MonoBehaviour
{
    public static StageManager Instance { get; private set; }

    [Header("ステージ設定")]
    private int currentStage = 1; // 現在のステージ (1〜5)
    private int currentPhase = 1; // 現在のフェーズ (1〜5)

    private const int MaxStages = 5;
    private const int MaxPhases = 5;

    public bool isClear;

    // ==========================
    // 初期化
    // ==========================
    private void Awake()
    {
        // シングルトンの適用
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンを跨いでも維持
        }
        else
        {
            Destroy(gameObject);
        }
        isClear = false;
    }

    // ==========================
    // 現在のステージを取得
    // ==========================
    public int GetCurrentStage()
    {
        return currentStage;
    }

    // ==========================
    // 現在のフェーズを取得
    // ==========================
    public int GetCurrentPhase()
    {
        return currentPhase;
    }

    // ==========================
    // 次のステージへ進む
    // ==========================
    public void NextStage()
    {
        if (currentStage < MaxStages)
        {
            currentStage++;
            currentPhase = 1; // フェーズをリセット
            DebugUtility.Log($"次のステージへ進みました: ステージ {currentStage} - Phase {currentPhase}");        
        }
        else
        {
            PanelManager.Instance.ToggleResultPanel();
            SystemMessage.GameClearMessage();
            isClear = true;
        }
    }

    // ==========================
    // 次のフェーズへ進む
    // ==========================
    public void NextPhase()
    {
        if (currentPhase < MaxPhases)
        {
            currentPhase++;
        }
        else
        {
            DebugUtility.Log("ステージクリア！ すべての次元を完了しました。");            
            NextStage();
        }
    }
    // ==========================
    // ステージとフェーズをリセット
    // ==========================
    public void ResetStageAndPhase()
    {
        DebugUtility.Log("ステージとフェーズをリセットしました。");        
        currentStage = 1;
        currentPhase = 1;
    }
}