using UnityEngine;
using UniRx;

// ==========================
// ゲームの進行を管理するクラス
// ==========================
public class GameFlowManager : MonoBehaviour
{
    [SerializeField] EnemyGenerator enemyGenerator;
    [SerializeField] ShapeSelector eventSelector;
    [SerializeField] FriendGenerator friendGenerator;
    [SerializeField] BattleCoinReward battleCoinReward;
    private Player player;

    // ==========================
    // 初期化処理
    // ==========================
    private void Start()
    {
        player = FindObjectOfType<Player>();
        if (player == null)
        {
            DebugUtility.LogError("Playerオブジェクトが見つかりません！");
            return;
        }
        // リセット処理
        TutorialManager.HideUpArrow();
        StageManager.Instance.ResetStageAndPhase();
        ShapeDex.Instance.ResetShapeDex();
        // ゲーム開始メッセージを表示
        MainMessage.ShowStartMessage();
        // 図形選択フェーズへ
        Observable.Timer(System.TimeSpan.FromSeconds(4.5f)).Subscribe(_ =>
        {
            SelectPhase(SelectionMode.OnlyCommon, 3,true); // フェーズを選択
        }).AddTo(this);
    }
    // ==========================
    // 図形獲得フェーズ
    // ==========================
    public void SelectPhase(SelectionMode selectionMode, int canSelectCount,bool isTutrial = false)
    {
        if(isTutrial)
        {
            PanelManager.Instance.ToggleSelectShapePanel();
            // 現在の難易度から選択肢を表示
            eventSelector.AssignShapesToButtons(selectionMode, canSelectCount);
            return;
        }
        // 70%の確率で図形獲得イベントを発生
        bool isShapeEventOccur = Random.value < 0.7f;
        if (isShapeEventOccur)
        {
            PanelManager.Instance.ToggleSelectShapePanel();
            // 現在の難易度から選択肢を表示
            eventSelector.AssignShapesToButtons(selectionMode, canSelectCount);
            return;
        }
        // 図形獲得イベントが発生しない場合
        // 次元選択へ
        PanelManager.Instance.ToggleDimensionSelectorPanel();
    }
    // ==========================
    // バトルフェーズ
    // ==========================
    public void StartBattlePhase()
    {
        // バトルフェーズ専用メッセージを表示
        MainMessage.ShowBattlePhaseMessage();

        // 3秒後にバトルスタート
        Observable.Timer(System.TimeSpan.FromSeconds(3.5f)).Subscribe(_ =>
        {
            // 難易度を設定
            bool isStrongDifficulty = GameDifficultyManager.Instance.SetDifficulty();
            // 通常の敵
            if (!isStrongDifficulty)
            {
                BGMManager.Instance.PlayBattleBGM();

                GenerateEnemyAndFriend();
                return;
            }

            // 強化された敵
            MainMessage.ShowMessage("次元が揺れる", true, "強敵出現", false);

            Observable.Timer(System.TimeSpan.FromSeconds(3f)).Subscribe(_ =>
            {
                MainMessage.HideMessage();
                BGMManager.Instance.PlayBossBGM();

                GenerateEnemyAndFriend();
                return;
            });
        }).AddTo(this);
    }
    // ==========================
    // 敵と味方を生成するメソッド
    // ==========================
    private void GenerateEnemyAndFriend()
    {
        // 敵の生成
        enemyGenerator.GenerateAllEnemies();
        // 味方の生成
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            friendGenerator.GenerateFriends(player);
        }
    }
    // ==========================
    // バトルフェーズを終了するメソッド
    // ==========================
    public void EndBattlePhase()
    {
        if (player == null)
        {
            DebugUtility.LogError("Playerオブジェクトが初期化されていません！");
            return;
        }

        DebugUtility.Log("Battle Phase Ended!");
        player.EndBattle();
        // BGMを再生
        BGMManager.Instance.PlayMainBGM();

        // 次のフェーズへ 
        StageManager.Instance.NextPhase();
        
        bool isClear = StageManager.Instance.isClear;
        if (isClear) { return; }
        // イベントが発生するかチェック
        CheckEventOccurrence();
    }
    // ==========================
    // イベントが発生するかチェックするメソッド
    // ==========================
    public void CheckEventOccurrence()
    {
        // イベントが発生するかどうかを判定(20%の確率)
        bool isEventOccur = Random.value < 1f;
        if (isEventOccur)
        {
            PanelManager.Instance.ToggleRandomEventPanel();
        }
        else
        {
            // 難易度に応じたSelectionModeを取得
            SelectionMode mode = SelectionModeManager.GetSelectionMode();
            SelectPhase(mode, 1);
        }
    }
    // ==========================
    // ゲームオーバー処理を行うメソッド
    // ==========================
    public void GameOver()
    {
        // ==== 初期化処理 ====
        ShapeDex.Instance.ResetShapeDex();
        StageManager.Instance.ResetStageAndPhase();
        // ====================
        // リザルト画面を表示
        PanelManager.Instance.ToggleResultPanel();
        // ゲームオーバーメッセージを表示
        MainMessage.GameOverMessage();
        BGMManager.Instance.PlayGameOverBGM();
    }
    // ==========================
    // ゲームクリア処理を行うメソッド
    // ==========================
    public void GameClear()
    {
        // ==== 初期化処理 ====
        ShapeDex.Instance.ResetShapeDex();
        StageManager.Instance.ResetStageAndPhase();
        // ====================
        // リザルト画面を表示
        PanelManager.Instance.ToggleResultPanel();
        // ゲームクリアメッセージを表示
        MainMessage.GameClearMessage();
        BGMManager.Instance.PlayGameClearBGM();
    }
}
