using UnityEngine;
using UnityEngine.UI;
// =====================
// プレイヤーの通貨を管理するクラス
// =====================
public class PlayerCurrencyManager : MonoBehaviour
{
    [SerializeField] private GameObject playerCurrencyUI; // 通貨UI
    private Text currencyText; // 通貨テキスト

    private BattleCoinReward battleCoinReward; // 戦闘報酬の通貨量を管理するクラス
    public float Currency { get; private set; } // 通貨

    // =====================
    // 初期化
    // =====================
    private void Start()
    {
        Initialize();
    }
    // =====================
    // 初期化処理
    // =====================
    private void Initialize()
    {
        Currency = 800; // 初期通貨
        // 通貨UIのテキストを取得
        currencyText = playerCurrencyUI.GetComponentInChildren<Text>();
        currencyText.text = Currency.ToString();
        // 戦闘報酬の通貨量を管理するクラスを取得
        battleCoinReward = playerCurrencyUI.GetComponentInChildren<BattleCoinReward>();
    }
    // =====================
    // 値を受け取って通貨を増やす
    // =====================
    public void EarnCurrency(float amount)
    {
        // 増えた通貨を表示
        battleCoinReward.ShowReward($"+{amount}", true);
        // 通貨を増やす
        Currency += amount;
        currencyText.text = Currency.ToString();
    }
    // =====================
    // 難易度に応じて通貨を増やす（戦闘終了時の処理）
    // =====================
    public void EarnCurrency()
    {
        // 戦闘報酬の通貨量を取得
        int amount = battleCoinReward.GetReward();
        // 増えた通貨を表示
        battleCoinReward.ShowReward($"+{amount}", true);
        // 通貨を増やす
        Currency += amount;
        currencyText.text = Currency.ToString();
    }
    // =====================
    // 通貨を減らす
    // =====================
    public void SpendCurrency(float amount)
    {
        // 減った通貨を表示
        battleCoinReward.ShowReward($"-{amount}", false);
        // 通貨を減らす
        // 通貨が足りない場合は0にする
        if (Currency >= amount)
        {
            Currency -= amount;
            currencyText.text = Currency.ToString();
        }
        else
        {
            Currency = 0;
            currencyText.text = Currency.ToString();
        }
    }
}
