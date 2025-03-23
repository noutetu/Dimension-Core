using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UniRx;
// ==========================
// メインメッセージクラス
// ==========================
public class MainMessage : MonoBehaviour
{
    [Header("テキスト要素")]
    [SerializeField] private Text primaryText; // メインメッセージのテキスト
    [SerializeField] private Text secondaryText; // サブメッセージのテキスト
    [SerializeField] private GameObject systemMessageHolder; // メッセージ表示用のホルダー
    [Header("リザルトテキスト")]
    [SerializeField] private Text resultText; // リザルトメッセージのテキスト
    [SerializeField] private Text leftText; // 左側のテキスト
    [SerializeField] private Text rightText; // 右側のテキスト

    [Header("アニメーション設定")]
    [SerializeField] private TextAnimator primaryAnimator; // メインメッセージのアニメーター
    [SerializeField] private TextAnimator secondaryAnimator; // サブメッセージのアニメーター
    [SerializeField] private TextAnimator resultAnimator; // リザルトメッセージのアニメーター
    [SerializeField] private TextAnimator.AnimationType TypeWriter; // アニメーションタイプ

    [Header("設定")]
    [SerializeField] private float displayDuration = 2f; // メッセージ表示時間
    [SerializeField] public Canvas uiCanvas; // UIキャンバス

    public static MainMessage Instance; // シングルトンインスタンス

    private void Awake()
    {
        Instance = this; // インスタンスを設定
        systemMessageHolder.SetActive(false); // メッセージホルダーを非表示に設定
    }

    // ==========================
    // メインメッセージ表示
    // ==========================
    public static void ShowMessage(string message, bool showSubText, string subMessage, bool isAutoHide)
    {
        if (Instance == null)
        {
            DebugUtility.LogWarning("SystemMessageインスタンスが初期化されていません。");
            return;
        }

        Instance.ActivateTextHolder(); // テキストホルダーをアクティブにする
        if (Instance.primaryText != null)
        {
            Instance.primaryText.gameObject.SetActive(true); // メインテキストを表示
            Instance.primaryText.text = message; // メインテキストにメッセージを設定
            Instance.primaryAnimator.StartAnimation(Instance.primaryAnimator.firstAnimationType); // アニメーションを開始
        }
        else
        {
            DebugUtility.LogWarning("primaryTextが設定されていません。");
        }
        Instance.secondaryText.gameObject.SetActive(false); // サブテキストを非表示に設定
        Observable.Timer(System.TimeSpan.FromSeconds(1f)).Subscribe(_ =>
        {
            if (showSubText && Instance.secondaryText != null)
            {
                Instance.secondaryText.gameObject.SetActive(true); // サブテキストを表示
                Instance.secondaryText.text = subMessage; // サブテキストにメッセージを設定
                Instance.secondaryAnimator.StartAnimation(Instance.secondaryAnimator.firstAnimationType); // アニメーションを開始
            }
            else if (showSubText)
            {
                DebugUtility.LogWarning("secondaryTextが設定されていません。");
            }

            if (isAutoHide)
            {
                Instance.StartCoroutine(Instance.HideAfterDelay(Instance.displayDuration)); // 一定時間後にメッセージを非表示にする
            }
        }
        ).AddTo(Instance);
    }

    private void ActivateTextHolder()
    {
        if (systemMessageHolder != null)
        {
            systemMessageHolder.SetActive(true); // メッセージホルダーを表示
        }
        else
        {
            DebugUtility.LogWarning("systemMessageHolderが破棄されています。");
        }
    }

    private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay); // 指定時間待機
        HideMessage(); // メッセージを非表示にする
    }

    // ==========================
    // メインメッセージ非表示
    // ==========================
    public static void HidePrimaryMessage()
    {
        if (Instance == null)
        {
            DebugUtility.LogWarning("SystemMessageインスタンスが初期化されていません。");
            return;
        }

        if (Instance.primaryText != null)
        {
            Instance.primaryText.gameObject.SetActive(false); // メインテキストを非表示に設定

            if (Instance.primaryAnimator != null)
            {
                Instance.primaryAnimator.ResetTransform(); // アニメーターのトランスフォームをリセット
            }
        }
    }

    // ==========================
    // サブメッセージ非表示
    // ==========================
    public static void HideSecondaryMessage()
    {
        if (Instance == null)
        {
            DebugUtility.LogWarning("SystemMessageインスタンスが初期化されていません。");
            return;
        }

        if (Instance.secondaryText != null)
        {
            Instance.secondaryText.gameObject.SetActive(false); // サブテキストを非表示に設定

            if (Instance.secondaryAnimator != null)
            {
                Instance.secondaryAnimator.ResetTransform(); // アニメーターのトランスフォームをリセット
            }
        }
    }

    // ==========================
    // 全メッセージ非表示
    // ==========================
    public static void HideMessage()
    {
        if (Instance == null)
        {
            DebugUtility.LogWarning("SystemMessageインスタンスが初期化されていません。");
            return;
        }

        HidePrimaryMessage(); // メインメッセージを非表示にする
        HideSecondaryMessage(); // サブメッセージを非表示にする
        if (Instance.systemMessageHolder != null)
        {
            Instance.systemMessageHolder.SetActive(false); // メッセージホルダーを非表示にする
        }
    }

    // ==========================
    // 現在のステージ表示
    // ==========================
    public static void ShowCurrentStage()
    {
        if (Instance == null)
        {
            DebugUtility.LogWarning("SystemMessageインスタンスが初期化されていません。");
            return;
        }

        Instance.ActivateTextHolder(); // テキストホルダーをアクティブにする
        ShowMessage($"Stage{StageManager.Instance.GetCurrentStage()}",
                                true, $"Phase{StageManager.Instance.GetCurrentPhase()}", false); // 現在のステージとフェーズを表示
    }

    // ==========================
    // 現在の次元表示
    // ==========================
    public static void ShowCurrentDimension()
    {
        if (Instance == null)
        {
            DebugUtility.LogWarning("SystemMessageインスタンスが初期化されていません。");
            return;
        }

        Instance.ActivateTextHolder(); // テキストホルダーをアクティブにする
        // Phase5の場合はボス次元を表示
        if (StageManager.Instance.GetCurrentPhase() == 5)
        {
            ShowMessage($"ボス次元", true, "", true);
            return;
        }
        // Stage5の場合は中心次元と表示
        if (StageManager.Instance.GetCurrentStage() == 5)
        {
            ShowMessage($"中心次元", true, "", true);
            return;
        }
        ShowMessage($"{DimensionManager.Instance.GetCurrentDimension().dimensionName}次元", true, "", true); // 現在の次元を表示
    }

    // ==========================
    // バトルフェーズメッセージ表示
    // ==========================
    public static void ShowBattlePhaseMessage()
    {
        if (Instance == null)
        {
            DebugUtility.LogWarning("SystemMessageインスタンスが初期化されていません。");
            return;
        }

        // 現在のステージを表示
        ShowCurrentStage();
        // 1.5秒後にステージを表示  
        Observable.Timer(System.TimeSpan.FromSeconds(2f)).
        Subscribe(_ => ShowCurrentDimension()
        ).AddTo(Instance);
    }

    // ==========================
    // ゲームオーバーメッセージ表示
    // ==========================
    public static void GameOverMessage()
    {
        Instance.primaryText.color = Color.red; // メインテキストの色を赤に設定
        Instance.secondaryText.color = Color.red; // サブテキストの色を赤に設定
        Instance.resultText.color = Color.red; // リザルトテキストの色を赤に設定
        Instance.resultText.text = "Retry?"; // リザルトテキストにメッセージを設定
        Instance.leftText.text = "NO"; // 左側のテキストを設定
        Instance.rightText.text = "YES"; // 右側のテキストを設定

        ShowMessage("次元が崩壊した", true, "全てが虚無へと飲み込まれる", false); // ゲームオーバーメッセージを表示
    }

    // ==========================
    // ゲームクリアメッセージ表示
    // ==========================
    public static void GameClearMessage()
    {
        Instance.primaryText.color = Color.green; // メインテキストの色を緑に設定
        Instance.secondaryText.color = Color.green; // サブテキストの色を緑に設定
        Instance.resultText.color = Color.green; // リザルトテキストの色を緑に設定
        Instance.resultText.text = "新たな次元へ"; // リザルトテキストにメッセージを設定
        Instance.leftText.text = "NO"; // 左側のテキストを設定
        Instance.rightText.text = "YES"; // 右側のテキストを設定
        ShowMessage("次元の崩壊を阻止した", true, "新たな秩序がここに生まれる", false); // ゲームクリアメッセージを表示
    }

    // ==========================
    // チュートリアルメッセージ表示
    // ==========================
    public static void ShowStartMessage()
    {
        ShowMessage("次元崩壊が始まった", true, "均衡を取り戻せ", false); // チュートリアルメッセージを表示
        Observable.Timer(System.TimeSpan.FromSeconds(3f)).Subscribe(_ =>
        {
            ShowMessage("共に戦う図形を選べ", false, "", false); // 次のメッセージを表示
        }).AddTo(Instance);
    }
}
