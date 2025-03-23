using UnityEngine;
using UnityEngine.UI;
// ==========================
// ランダムイベントパネル
// ==========================
public class RandomEventPanel : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] Text riskDescriptionText;
    [SerializeField] Text returnDescriptionText;
    [Header("Button")]
    [SerializeField] Button acceptButton;
    [SerializeField] Button declineButton;
    // イベントデータ
    BaseEventData baseEventData;

    // ==========================
    // 初期化
    // ==========================
    void OnEnable()
    {
        SetRandomEvent();
    }
    // ==========================
    // ランダムイベントをセット
    // ==========================   
    private void SetRandomEvent()
    {
        // ランダムなイベントを取得
        baseEventData = AllEventList.instance.GetRandomEvent();
        // イベント名を表示
        string eventName = baseEventData.eventTitle;
        MainMessage.ShowMessage(eventName, false, "", false);
        // テキストを表示
        riskDescriptionText.text = baseEventData.riskDescription;
        returnDescriptionText.text = baseEventData.returnDescription;

        // acceptButtonにメソッドをセット
        SetAcceptButton();
        // declineButtonにメソッドをセット
        SetDeclineButton();
    }

    private void SetAcceptButton()
    {
        // クリックイベントを設定
        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(() =>
        {
            // クリック可能か
            bool canClick = baseEventData.OnAccept();
            // クリック可能なら
            if (canClick)
            {
                // ランダムイベント終了
                FinishRandomEvent();
            }
        });
    }

    // ==========================
    // declineButtonにメソッドをセット
    // ==========================
    private void SetDeclineButton()
    {
        //　クリックイベントを設定
        declineButton.onClick.RemoveAllListeners();
        declineButton.onClick.AddListener(() =>
        {   
            // 拒否の処理をして
            baseEventData.OnDecline();
            // ランダムイベント終了
            FinishRandomEvent();
        });
    }

    private void FinishRandomEvent()
    {
        // 難易度に応じたSelectionModeを取得
        SelectionMode mode = SelectionModeManager.GetSelectionMode();
        // 次のフェーズへ
        GameFlowManager gameFlowManager = FindObjectOfType<GameFlowManager>();
        gameFlowManager.SelectPhase(mode, 1);
        // パネルを非表示
        gameObject.SetActive(false);
    }
}
