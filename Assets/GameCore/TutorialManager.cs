using UnityEngine;
using UniRx;
// ===========================================
// チュートリアルを管理するクラス
// ===========================================
public class TutorialManager : MonoBehaviour
{
    [Header("オプションボタン")]
    [SerializeField] private GameObject optionButton; // オプションボタン

    [Header("矢印")]
    [SerializeField] public GameObject upArrowObject; // 上矢印オブジェクト
    [Header("矢印を出す位置")]
    [SerializeField] GameObject optionButtonPoint; // オプションボタンの位置
    [SerializeField] GameObject playerUIButtonPoint; // プレイヤーUIボタンの位置
    [SerializeField] GameObject shapeButtonPoint; // 図形ボタンの位置
    // 矢印を出すまでのクールタイム
    float waitTime = 0.8f;
    // シングルトン
    private static TutorialManager Instance;
    // チュートリアル終了フラグ
    public bool isFinishedButtonPanelTutorial;

    // ==========================
    // 初期化
    // ==========================
    private void Start()
    {   // シングルトンの適用
        Instance = this;
        // チュートリアル終了フラグを初期化
        isFinishedButtonPanelTutorial = false;
        // パネル開閉時のイベント登録
        PanelManager.Instance.OnDimensionSelectorPanel += ShowOptionButtonTutorial;
        PanelManager.Instance.OnButtonPanel += ShowPlayerUIButtonTutorial;
        PanelManager.Instance.OnPlayerUIPanel += ShowShapeButtonTutorial;
        PanelManager.Instance.OnShapeButton += () => 
        {
            if (isFinishedButtonPanelTutorial) return;
            HideUpArrow();
            isFinishedButtonPanelTutorial = true;
        };
    }
    // ==========================
    // オプションボタンのチュートリアル
    // ==========================
    public static void ShowOptionButtonTutorial()
    {
        if (Instance.isFinishedButtonPanelTutorial) return;
        // オプションボタンを表示
        Instance.optionButton.SetActive(true); 
        HideUpArrow();
        // upArrowをArrowPointの位置に移動
        Instance.upArrowObject.transform.position = Instance.optionButtonPoint.transform.position;
        // 上矢印を表示
        Instance.upArrowObject.SetActive(true); 
    }
    // ==========================
    // プレイヤーUIボタンのチュートリアル
    // ==========================
    public static void ShowPlayerUIButtonTutorial()
    {
        if (Instance.isFinishedButtonPanelTutorial) return;
        HideUpArrow();
        // waitTime秒後にプレイヤーUIボタンのチュートリアルを表示
        Observable.Timer(System.TimeSpan.FromSeconds(Instance.waitTime)).Subscribe(_ =>
        {
            // upArrowをArrowPointの位置に移動
            Instance.upArrowObject.transform.position = Instance.playerUIButtonPoint.transform.position;
            // 上矢印を表示
            Instance.upArrowObject.SetActive(true); 
        });
    }
    // ==========================
    // 図形ボタンのチュートリアル
    // ==========================
    public static void ShowShapeButtonTutorial()
    {
        if (Instance.isFinishedButtonPanelTutorial) return;
        HideUpArrow();
        // waitTime秒後に図形ボタンのチュートリアルを表示
        Observable.Timer(System.TimeSpan.FromSeconds(Instance.waitTime)).Subscribe(_ =>
        {
            // upArrowをArrowPointの位置に移動
            Instance.upArrowObject.transform.position = Instance.shapeButtonPoint.transform.position;
            // 上矢印を表示
            Instance.upArrowObject.SetActive(true); 
        });
    }

    // ==========================
    // 上矢印非表示
    // ==========================
    public static void HideUpArrow()
    {
        Instance.upArrowObject.SetActive(false); // 上矢印を非表示にする
    }
}
