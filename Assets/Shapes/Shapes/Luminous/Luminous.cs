using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Luminous : Shape
{
    [SerializeField] private Barrier barrier;
    float barrierCooldown = 20f;
    float barrierDuration = 10f;

    // バリアの攻撃力バフの値(実数値)
    float attackBuffAmount = 200f;

    // ==========================
    // 初期化
    // ==========================
    public override void Initialize()
    {
        base.Initialize();
        if (barrier == null)
        {
            DebugUtility.LogError("Barrierコンポーネントが見つかりません。");
            return;
        }
        barrier.SetBuffParameters(attackBuffAmount);
        StartCoroutine(BarrierCycle());
    }
    // ==========================
    // バリアサイクルの開始
    // ==========================
    private IEnumerator BarrierCycle()
    {
        while (true)
        {
            // バリアを有効化
            barrier.gameObject.SetActive(true);
            yield return new WaitForSeconds(barrierDuration);
            
            // バリアを無効化
            barrier.gameObject.SetActive(false);
            yield return new WaitForSeconds(barrierCooldown);
        }
    }
    // ==========================
    // Omegaスキル
    // ==========================
    public override void ActivateOmegaSkill()
    {
        if(baseStats.IsOmegaUpgrade && !IsEnemy)
        {
            barrierCooldown = 15f;
        }
    }
    // ==========================
    // Alphaスキル
    // ==========================
    public override void ActivateAlphaSkill()
    {
        if(baseStats.isAlphaUpgrade)
        {
            attackBuffAmount = 300f;
        }
    }
}
