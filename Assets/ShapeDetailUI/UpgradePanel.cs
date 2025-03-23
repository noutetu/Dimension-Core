using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
// ==========================
// 図形のアップグレードを表示するパネル
// ==========================
public class UpgradePanel : MonoBehaviour
{
    [Header("スキルボタン")]
    [SerializeField] private Button omegaButton;  // オメガ図形の詳細情報を表示するボタン
    [SerializeField] private Button alphaButton;  // アルファ図形の詳細情報を表示するボタン
    [SerializeField] private Button lambdaButton;  // ベータ図形の詳細情報を表示するボタン

    [Header("価格テキスト")]
    [SerializeField] private Text omegaCostText;  // オメガ図形の価格を表示するテキスト
    [SerializeField] private Text alphaCostText;  // アルファ図形の価格を表示するテキスト
    [SerializeField] private Text lambdaCostText;  // ベータ図形の価格を表示するテキスト

    [Header("スキル説明")]
    [SerializeField] private GameObject skillDescription;  // スキルの説明

    [Header("確認ボタン")]
    [SerializeField] private Button confirmButton;  // ボタンを押した時の処理を確認するためのボタン

    [Header("図形名テキスト")]
    [SerializeField] private Text detailNameText;  // 図形の名前を表示するテキスト
    [Header("閉じるボタン")]
    [SerializeField] private Button closeButton;  // 図形の詳細画面を閉じるボタン

    Player player; // プレイヤー

    // ==========================
    // 図形の詳細を設定
    // ==========================
    public void SetShapeDetail(ShapeBaseStats shapeData, UnityAction unityAction)
    {
        player = FindObjectOfType<Player>();

        if (closeButton != null)
        {
            // 閉じるボタンのクリックイベントを設定
            closeButton.onClick.AddListener(unityAction);
        }
        // スキル説明を閉じる
        CloseSkillDescription();

        DebugUtility.Log($"図形の詳細を表示: {shapeData.name}");
        if (detailNameText != null)
        {
            // 図形の名前を表示
            detailNameText.text = shapeData.ShapeName;
        }
        else
        {
            DebugUtility.LogError("Textコンポーネント（NameText）が見つかりません！");
        }

        // ボタンのクリックイベントと金額を設定
        // オメガボタン
        omegaButton.onClick.RemoveAllListeners();
        omegaButton.onClick.AddListener(() => ShowOmegaSkill(shapeData));
        omegaCostText.text = shapeData.omegaSkillCost.ToString();
        // アルファボタン
        alphaButton.onClick.RemoveAllListeners();
        alphaButton.onClick.AddListener(() => ShowAlphaSkill(shapeData));
        alphaCostText.text = shapeData.alphaSkillCost.ToString();
        // ラムダボタン
        lambdaButton.onClick.RemoveAllListeners();
        lambdaButton.onClick.AddListener(() => ShowLambdaSkill(shapeData));
        lambdaCostText.text = shapeData.lambdaSkillCost.ToString();
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
    // スキル表示の設定
    // ==========================
    public void ShowOmegaSkill(ShapeBaseStats shapeBaseStats)
    {
        ShowSkill(
            // オメガスキルの説明
            skillDescription: shapeBaseStats.omegaSkillDescription,
            // オメガスキルの価格
            skillCost: shapeBaseStats.omegaSkillCost,
            // オメガスキルがアップグレードされているかどうか
            isUpgraded: shapeBaseStats.IsOmegaUpgrade,
            // オメガボタン
            button: omegaButton,
            // 他のボタンの配列
            otherButtons: new Button[] { alphaButton, lambdaButton },
            // オメガスキルの処理
            activateSkill: shapeBaseStats.ActivateOmegaSkill
        );
    }

    public void ShowAlphaSkill(ShapeBaseStats shapeBaseStats)
    {
        ShowSkill(
            // アルファスキルの説明
            skillDescription: shapeBaseStats.alphaSkillDescription,
            // アルファスキルの価格
            skillCost: shapeBaseStats.alphaSkillCost,
            // アルファスキルがアップグレードされているかどうか
            isUpgraded: shapeBaseStats.IsAlphaUpgrade,
            // アルファボタン
            button: alphaButton,
            // 他のボタンの配列
            otherButtons: new Button[] { omegaButton, lambdaButton },
            // アルファスキルの処理
            activateSkill: shapeBaseStats.ActivateAlphaSkill
        );
    }

    public void ShowLambdaSkill(ShapeBaseStats shapeBaseStats)
    {
        ShowSkill(
            // ラムダスキルの説明
            skillDescription: shapeBaseStats.lambdaSkillDescription,
            // ラムダスキルの価格
            skillCost: shapeBaseStats.lambdaSkillCost,
            // ラムダスキルがアップグレードされているかどうか
            isUpgraded: shapeBaseStats.IsLambdaUpgrade,
            // ラムダボタン
            button: lambdaButton,
            // 他のボタンの配列
            otherButtons: new Button[] { omegaButton, alphaButton },
            // ラムダスキルの処理
            activateSkill: shapeBaseStats.ActivateLambdaSkill
        );
    }

    // ==========================
    // スキルを表示
    // ==========================
    private void ShowSkill(
        string skillDescription,    // スキルの説明
        float skillCost,            // スキルの価格
        bool isUpgraded,            // スキルがアップグレードされているかどうか
        Button button,              // ボタン
        Button[] otherButtons,      // 他のボタンの配列
        UnityAction activateSkill)  // スキルの処理
    {
        //　確認ボタンとスキル説明を表示
        confirmButton.gameObject.SetActive(true);
        this.skillDescription.gameObject.SetActive(true);
        // 押されたボタンの色を変更
        button.GetComponent<Image>().color = Color.green;
        // 他のボタンの色をリセット
        foreach (var otherButton in otherButtons)
        {
            otherButton.GetComponent<Image>().color = Color.white;
        }
        Text skillDescriptionText = this.skillDescription.GetComponentInChildren<Text>();
        if (skillDescriptionText == null)
        {
            DebugUtility.LogError("Textコンポーネントが見つかりません！");
            return;
        }
        // スキル説明を表示
        skillDescriptionText.text = skillDescription;
        // 確認ボタンのアクションを変更
        ChangeActionOnConfirmButton(skillCost, isUpgraded, () =>
        {
            if (player.currencyManager.Currency < skillCost)
            {
                DebugUtility.Log("通貨が足りません！");
                return;
            }
            // 通貨を消費
            player.SpendCurrency(skillCost);
            // スキルをアクティブ化
            activateSkill();
            // 確認ボタンのアクションを変更
            ChangeActionOnConfirmButton(skillCost, true, () => { });
        });
    }

    // ==========================
    // 確認ボタンのアクションを変更
    // ==========================
    private void ChangeActionOnConfirmButton(float skillCost, bool isUpgraded, UnityAction action)
    {
        if (confirmButton != null)
        {
            // 確認ボタンのテキストを取得
            Text confirmButtonText = confirmButton.GetComponentInChildren<Text>();
            if (confirmButtonText != null)
            {
                // スキルがアップグレードされている場合
                if (isUpgraded)
                {
                    confirmButtonText.color = Color.green; // テキストの色を緑にする
                    confirmButtonText.text = "✔";          // チェックマークを表示
                    confirmButton.onClick.RemoveAllListeners(); // 何も起こらないようにする
                }
                // スキルの価格が0の場合
                else if (skillCost == 0)
                {
                    confirmButtonText.color = Color.gray; // テキストの色を灰色にする
                    confirmButtonText.text = "なし";        // 「なし」を表示
                    confirmButton.onClick.RemoveAllListeners(); // 何も起こらないようにする
                }
                else
                {
                    confirmButtonText.color = Color.white;  // テキストの色を白にする
                    confirmButtonText.text = "購入";        // 「購入」を表示
                    confirmButton.onClick.RemoveAllListeners(); // リスナーを削除
                    confirmButton.onClick.AddListener(action);  // アクションを追加
                }
            }
        }
        else
        {
            DebugUtility.LogError("confirmButtonが設定されていません！");
        }
    }
}
