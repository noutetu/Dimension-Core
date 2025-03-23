using UnityEngine;
// ==========================
// ステージ管理クラス
// ==========================
public class StageManager : MonoBehaviour
{   
    // シングルトン
    public static StageManager Instance { get; private set; }
    // クリアフラグ
    public bool isClear;
    // 現在のステージ (1〜5)
    private int currentStage = 1; // 現在のステージ (1〜5)
    private int currentPhase = 1; // 現在のフェーズ (1〜5)
    // 最大ステージ数
    private const int MAX_STAGE = 5;
    private const int MAX_PHASE = 5;

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
    #region ステージとフェーズの取得
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
    #endregion

    #region ステージとフェーズの進行
    // ==========================
    // 次のステージへ進む
    // ==========================
    public void NextStage()
    {
        // 最終ステージでない場合
        if (currentStage < MAX_STAGE)
        {   
            // ステージを進める
            currentStage++;
            // フェーズをリセット
            currentPhase = 1; 
            DebugUtility.Log($"次のステージへ進みました: ステージ {currentStage} - フェーズ {currentPhase}");        
        }
        // 最終ステージの場合
        else
        {
            // リザルトパネルを表示
            PanelManager.Instance.ToggleResultPanel();
            // ゲームクリアメッセージを表示
            MainMessage.GameClearMessage();
            // クリアフラグを立てる
            isClear = true;
        }
    }
    // ==========================
    // 次のフェーズへ進む
    // ==========================
    public void NextPhase()
    {
        if (currentPhase < MAX_PHASE)
        {
            currentPhase++;
        }
        else
        {
            DebugUtility.Log("ステージクリア！ すべてのフェーズを完了しました。");            
            NextStage();
        }
    }
    #endregion
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