using UnityEngine;

public class PlayerBuffManager : MonoBehaviour
{
    public float AtkBuffValue { get; private set; } = 0; // 攻撃力バフの値
    public float SpdBuffValue { get; private set; } = 0; // 速度バフの値
    public float EvasionBuffValue { get; private set; } = 0; // 回避バフの値
    public float CritBuffValue { get; private set; } = 0; // クリティカルバフの値

    // 攻撃力を指定%分増加するメソッド
    public void EnhancePowerByPercentage(float percentage)
    {
        float buffValue = percentage / 100;
        AtkBuffValue += buffValue;
    }

    // 攻撃力を指定%分減少するメソッド
    public void ReducePowerByPercentage(float percentage)
    {
        float debuffValue = percentage / 100;
        AtkBuffValue -= debuffValue;
    }

    // 速度を指定%分増加するメソッド
    public void EnhanceSpeedByPercentage(float percentage)
    {
        float buffValue = percentage / 100;
        SpdBuffValue += buffValue;
    }

    // 速度を指定%分減少するメソッド
    public void ReduceSpeedByPercentage(float percentage)
    {
        float value = percentage / 100;
        SpdBuffValue -= value;
    }

    // 回避率を指定%分増加するメソッド
    public void EnhanceEvasionByPercentage(float percentage)
    {
        float value = percentage;
        EvasionBuffValue += value;
    }

    // 回避率を指定%分減少するメソッド
    public void ReduceEvasionByPercentage(float percentage)
    {
        float value = percentage;
        EvasionBuffValue -= value;
    }

    // クリティカル率を指定%分増加するメソッド
    public void EnhanceCriticalByPercentage(float percentage)
    {
        float value = percentage;
        CritBuffValue += value;
    }

    // クリティカル率を指定%分減少するメソッド
    public void ReduceCriticalByPercentage(float percentage)
    {
        float value = percentage;
        CritBuffValue -= value;
    }
}
