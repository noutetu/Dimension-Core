using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;

public class DimensionSelector : MonoBehaviour
{
    [SerializeField] GameObject firstDimensionOption;
    [SerializeField] GameObject secondDimensionOption;

    [SerializeField] Button firstButton;
    [SerializeField] Button secondButton;

    private DimensionData firstDimension;
    private DimensionData secondDimension;

    // ==========================
    // 初期化
    // ==========================
    void OnEnable()
    {   
        // ステージ5かフェーズ5の時はボス次元を選択
        if(StageManager.Instance.GetCurrentStage() == 5 || StageManager.Instance.GetCurrentPhase() == 5)
            SetBossDimension();
        else
            SetNormalDimension();
    }

    // ==========================
    // ボス次元の設定
    // ==========================
    private void SetBossDimension()
    {
        // ===== ボタンのイベントを削除 =====
        firstButton.onClick.RemoveAllListeners();
        secondButton.onClick.RemoveAllListeners();
        // =================================
        // ===== 2つ目のオプションを非表示 =====
        secondDimensionOption.SetActive(false);
        // =====================================
        // ===== テキストを取得 =====
        Text firstText = firstDimensionOption.GetComponentInChildren<Text>();
        // =================================
        // ===== テキストを設定 =====
        if(StageManager.Instance.GetCurrentStage() == 5)
            firstText.text = "中心次元";
        else
            firstText.text = "ボス次元";
        // ==========================
        // ===== ボタンにイベントを追加 =====
        firstButton.onClick.AddListener(() => OnDimensionSelected(firstDimension));
        // ================================
        // ===== プレイヤーにメッセージを表示 =====
        SystemMessage.ShowMessage("次元選択", false, "", false);
        // =====================================
    }

    // ==========================
    // 通常次元の設定
    // ==========================
    private void SetNormalDimension()
    {
        // ===== オプションを表示 =====
        secondDimensionOption.SetActive(true);
        // ===== ボタンのイベントを削除 =====
        firstButton.onClick.RemoveAllListeners();
        secondButton.onClick.RemoveAllListeners();
        // =================================
        // ===== テキストを取得 =====
        Text firstText = firstDimensionOption.GetComponentInChildren<Text>();
        Text secondText = secondDimensionOption.GetComponentInChildren<Text>();
        // =================================
        // ===== 次元を二つ取得 =====   
        firstDimension = DimensionManager.Instance.ReturnRandomDimension(StageManager.Instance.GetCurrentStage());
        secondDimension = DimensionManager.Instance.ReturnRandomDimension(StageManager.Instance.GetCurrentStage(), firstDimension);

        // ==========================
        // ===== テキストを設定 =====
        firstText.text = firstDimension.dimensionName + "次元";
        secondText.text = secondDimension.dimensionName + "次元";
        // ==========================
        // ===== ボタンにイベントを追加 =====
        firstButton.onClick.AddListener(() => OnDimensionSelected(firstDimension));
        secondButton.onClick.AddListener(() => OnDimensionSelected(secondDimension));
        // ================================
        // ===== プレイヤーにメッセージを表示 =====
        SystemMessage.ShowMessage("次元選択", false, "", false);
        // =====================================
    }

    // ==========================
    // 次元を選択した時の処理
    // ==========================
    void OnDimensionSelected(DimensionData dimension)
    {
        // ボタンを無効化して連続クリックを防止
        firstButton.interactable = false;
        secondButton.interactable = false;

        PanelManager.Instance.ToggleDimensionSelectorPanel();
        DimensionManager.Instance.SelectRandomDimension(dimension);
        GameFlowManager gameFlowManager = FindObjectOfType<GameFlowManager>();
        // 0.8秒後に次のフェーズに進む
        Observable.Timer(System.TimeSpan.FromSeconds(0.8f))
        .Subscribe(_ => 
        {
            gameFlowManager.StartBattlePhase();
            // ボタンを再度有効化
            firstButton.interactable = true;
            secondButton.interactable = true;
        })
        .AddTo(this);
    }
}
