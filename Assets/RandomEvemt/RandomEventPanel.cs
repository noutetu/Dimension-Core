using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class RandomEventPanel : MonoBehaviour
{
    [SerializeField] Text riskDescriptionText;
    [SerializeField] Text returnDescriptionText;
    [SerializeField] Button acceptButton;
    [SerializeField] Button declineButton;

    BaseEventData baseEventData;

    void OnEnable()
    {
        SetRandmEvent();
    }

    private void SetRandmEvent()
    {
        baseEventData = AllEventList.instance.GetRandomEvent();

        string eventName = baseEventData.eventTitle;
        SystemMessage.ShowMessage(eventName, false, "", false);
        riskDescriptionText.text = baseEventData.riskDescription;
        returnDescriptionText.text = baseEventData.returnDescription;

        // ==========================
        // SelectPhaseを呼び出す準備
        // ==========================
        GameFlowManager gameFlowManager = FindObjectOfType<GameFlowManager>();
        
        // ==========================
        // acceptButtonにメソッドをセット
        // ==========================
        acceptButton.onClick.RemoveAllListeners();
        acceptButton.onClick.AddListener(() =>
        {
            bool canClick = baseEventData.OnAccept();
            if (canClick)
            {
                // 難易度に応じたSelectionModeを取得
                SelectionMode mode = SelectionModeManager.GetSelectionMode();
                gameFlowManager.SelectPhase(mode, 1);
                gameObject.SetActive(false);
            }
        });

        // ==========================
        // declineButtonにメソッドをセット
        // ==========================
        declineButton.onClick.RemoveAllListeners();
        declineButton.onClick.AddListener(() =>
        {
            baseEventData.OnDecline();
            // 難易度に応じたSelectionModeを取得
            SelectionMode mode = SelectionModeManager.GetSelectionMode();
            gameFlowManager.SelectPhase(mode, 1);
            gameObject.SetActive(false);
        });
    }
}
