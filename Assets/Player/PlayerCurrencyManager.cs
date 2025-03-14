using UnityEngine;
using UnityEngine.UI;

public class PlayerCurrencyManager : MonoBehaviour
{
    [SerializeField] private GameObject playerCurrencyUI;
    private Text currencyText;
    private BattleCoinReward battleCoinReward;

    public float Currency { get; private set; }

    private void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        Currency = 800; // 初期通貨
        currencyText = playerCurrencyUI.GetComponentInChildren<Text>();
        currencyText.text = Currency.ToString();
        battleCoinReward = playerCurrencyUI.GetComponentInChildren<BattleCoinReward>();
    }
    // 値を受け取って通貨を増やす
    public void EarnCurrency(float amount)
    {
        battleCoinReward.ShowReward($"+{amount}", true);
        Currency += amount;
        currencyText.text = Currency.ToString();
    }
    // 何度に応じて通貨を増やす（戦闘終了時の処理）
    public void EarnCurrency()
    {
        int amount = battleCoinReward.GetReward();
        battleCoinReward.ShowReward($"+{amount}", true);
        Currency += amount;
        currencyText.text = Currency.ToString();
    }

    public void SpendCurrency(float amount)
    {
        battleCoinReward.ShowReward($"-{amount}", false);

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
