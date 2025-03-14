using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class UpgradePanel : MonoBehaviour
{
    [Header("スキルボタン")]
    [SerializeField] private Button omegaButton;  // オメガ図形の詳細情報を表示するボタン
    [SerializeField] private Button alphaButton;  // アルファ図形の詳細情報を表示するボタン
    [SerializeField] private Button lambdaButton;  // ベータ図形の詳細情報を表示するボタン

    [Header("スキル説明")]
    [SerializeField] private GameObject skillDescription;  // スキルの説明

    [Header("確認ボタン")]
    [SerializeField] private Button confirmButton;  // ボタンを押した時の処理を確認するためのボタン

    [Header("図形名テキスト")]
    [SerializeField] private Text detailNameText;  // 図形の名前を表示するテキスト
    [Header("閉じるボタン")]
    [SerializeField] private Button closeButton;  // 図形の詳細画面を閉じるボタン

    Player player;

    // ==========================
    // 図形の詳細を表示
    // ==========================
    public void SetShapeDetail(ShapeBaseStats shapeData, UnityAction unityAction)
    {
        player = FindObjectOfType<Player>();

        if (closeButton != null)
        {
            closeButton.onClick.AddListener(unityAction);
        }
        CloseSkillDescription();

        DebugUtility.Log($"図形の詳細を表示: {shapeData.name}");
        // 詳細画面の名前を設定
        if (detailNameText != null)
        {
            detailNameText.text = shapeData.name;
        }
        else
        {
            DebugUtility.LogError("Textコンポーネント（NameText）が見つかりません！");
        }

        // ボタンのクリックイベントを設定
        omegaButton.onClick.RemoveAllListeners();
        omegaButton.onClick.AddListener(() => ShowOmegaSkill(shapeData));

        alphaButton.onClick.RemoveAllListeners();
        alphaButton.onClick.AddListener(() => ShowAlphaSkill(shapeData));

        lambdaButton.onClick.RemoveAllListeners();
        lambdaButton.onClick.AddListener(() => ShowLambdaSkill(shapeData));
    }

    // ==========================
    // アップグレードパネルを閉じる
    // ==========================
    public void CloseUpgradePanel()
    {
        gameObject.SetActive(false);
    }

    // ==========================
    // スキル説明を閉じる
    // ==========================
    public void CloseSkillDescription()
    {
        skillDescription.gameObject.SetActive(false);
        confirmButton.gameObject.SetActive(false);
    }

    // ==========================
    // スキル表示関連の処理
    // ==========================
    public void ShowOmegaSkill(ShapeBaseStats shapeBaseStats)
    {
        confirmButton.gameObject.SetActive(true);
        skillDescription.gameObject.SetActive(true);
        DebugUtility.Log("ShowOmegaSkillメソッドが呼ばれました");
        ShowSkill(
            desc: shapeBaseStats.omegaSkillDescription,
            skillCost: shapeBaseStats.omegaSkillCost,
            isUpgraded: shapeBaseStats.IsOmegaUpgrade,
            action: () =>
            {
                if (player.currencyManager.Currency < shapeBaseStats.omegaSkillCost)
                {
                    DebugUtility.Log("通貨が足りません！");
                    return;
                }
                player.SpendCurrency(shapeBaseStats.omegaSkillCost);
                shapeBaseStats.ActivateOmegaSkill();
                ChangeButtonAction(shapeBaseStats.omegaSkillCost, shapeBaseStats.IsOmegaUpgrade, () => { });
            });
    }

    public void ShowAlphaSkill(ShapeBaseStats shapeBaseStats)
    {
        confirmButton.gameObject.SetActive(true);
        skillDescription.gameObject.SetActive(true);
        ShowSkill(
            desc: shapeBaseStats.alphaSkillDescription,
            skillCost: shapeBaseStats.alphaSkillCost,
            isUpgraded: shapeBaseStats.isAlphaUpgrade,
            action: () =>
            {
                if (player.currencyManager.Currency < shapeBaseStats.alphaSkillCost)
                {
                    DebugUtility.Log("通貨が足りません！");
                    return;
                }
                player.SpendCurrency(shapeBaseStats.alphaSkillCost);
                shapeBaseStats.ActivateAlphaSkill();
                ChangeButtonAction(shapeBaseStats.alphaSkillCost, shapeBaseStats.isAlphaUpgrade, () => { });
            });
    }

    public void ShowLambdaSkill(ShapeBaseStats shapeBaseStats)
    {
        confirmButton.gameObject.SetActive(true);
        skillDescription.gameObject.SetActive(true);
        ShowSkill(
            desc: shapeBaseStats.lambdaSkillDescription,
            skillCost: shapeBaseStats.lambdaSkillCost,
            isUpgraded: shapeBaseStats.isLambdaUpgrade,
            action: () =>
            {
                if (player.currencyManager.Currency < shapeBaseStats.lambdaSkillCost)
                {
                    DebugUtility.Log("通貨が足りません！");
                    return;
                }
                player.SpendCurrency(shapeBaseStats.lambdaSkillCost);
                shapeBaseStats.ActivateLambdaSkill();
                ChangeButtonAction(shapeBaseStats.lambdaSkillCost, shapeBaseStats.isLambdaUpgrade, () => { });
            });
    }

    // ==========================
    // スキルを表示
    // ==========================
    private void ShowSkill(string desc, float skillCost, bool isUpgraded, UnityAction action)
    {
        Text SkillDescription = skillDescription.GetComponentInChildren<Text>();
        if (SkillDescription == null)
        {
            DebugUtility.LogError("Textコンポーネントが見つかりません！");
            return;
        }
        SkillDescription.text = desc;
        ChangeButtonAction(skillCost, isUpgraded, action);
    }

    // ==========================
    // ボタンのアクションを変更
    // ==========================
    private void ChangeButtonAction(float skillCost, bool isUpgraded, UnityAction action)
    {
        if (confirmButton != null)
        {
            Text confirmButtonText = confirmButton.GetComponentInChildren<Text>();
            if (confirmButtonText != null)
            {
                if (isUpgraded)
                {
                    confirmButtonText.text = "✔";
                    confirmButton.onClick.RemoveAllListeners(); // 何も起こらないようにする
                }
                else if (skillCost == 0)
                {
                    confirmButtonText.text = "NONE";
                    confirmButton.onClick.RemoveAllListeners(); // 何も起こらないようにする
                }
                else
                {
                    confirmButtonText.text = skillCost.ToString();
                    confirmButton.onClick.RemoveAllListeners();
                    confirmButton.onClick.AddListener(action);
                }
            }
        }
        else
        {
            DebugUtility.LogError("confirmButtonが設定されていません！");
        }
    }
}
