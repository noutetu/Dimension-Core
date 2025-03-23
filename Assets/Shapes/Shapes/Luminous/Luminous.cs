using System.Collections;
using UnityEngine;
// ==========================
// Luminousクラス(一定周期でバリアをはり、衝突した味方の攻撃力を上昇させる)
// ==========================
public class Luminous : Shape
{
    [Header("バリア")]
    [SerializeField] private Barrier barrier;

    float barrierCooldown = 20f;   // バリアのクールダウン時間
    float barrierDuration = 10f;   // バリアの持続時間

    float attackBuffAmount = 200f; // バリアの攻撃力バフの値(実数値)

    // ==========================
    // 初期化
    // ==========================
    public override void Initialize()
    {
        // 通常の初期化処理
        base.Initialize();
        if (barrier == null)
        {
            DebugUtility.LogError("Barrierコンポーネントが見つかりません。");
            return;
        }
        // バリアの初期化
        barrier.SetBuffParameters(attackBuffAmount);
        // バリアサイクルの開始
        StartCoroutine(BarrierCycle());
    }
    // ==========================
    // バリアサイクルの開始
    // ==========================
    private IEnumerator BarrierCycle()
    {
        // 生きている間は繰り返す
        while (!IsDead)
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
        // Omegaスキルがアクティブなら
        if (baseStats.IsOmegaUpgrade && !IsEnemy)
        {
            // バリアのクールダウン時間を短縮
            barrierCooldown = 15f;
        }
    }
    // ==========================
    // Alphaスキル
    // ==========================
    public override void ActivateAlphaSkill()
    {
        // Alphaスキルがアクティブなら
        if (baseStats.IsAlphaUpgrade)
        {
            // バリアの攻撃力バフの値を上昇
            attackBuffAmount = 300f;
        }
    }
}
