using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    private float attackBuffAmount;
    private bool isParentEnemy;
    private Shape parentShape;
    private HashSet<Shape> buffedShapes = new HashSet<Shape>();
    private float buffCooldown = 2f; // バフのクールダウン時間
    private float buffDuration = 10f; // バフの持続時間

    // ==========================
    // 初期化
    // ==========================
    private void OnEnable()
    {
        // 親オブジェクトのisEnemyフラグを取得
        parentShape = GetComponentInParent<Shape>();
        if (parentShape != null)
        {
            isParentEnemy = parentShape.IsEnemy;
        }
    }
    // ==========================
    // バフの設定
    // ==========================
    public void SetBuffParameters(float attackBuffAmount)
    {
        this.attackBuffAmount = attackBuffAmount;
    }
    // ==========================
    // 衝突時処理
    // ==========================
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // 衝突したオブジェクトがShapeコンポーネントを持っているか確認
        Shape shape = collision.gameObject.GetComponent<Shape>();
        
        // 衝突したShapeが存在し、親オブジェクトと同じ敵味方フラグを持っている場合
        if (shape != null && shape.IsEnemy == isParentEnemy && !buffedShapes.Contains(shape))
        {
            // 攻撃力バフを適用
            ApplyAttackBuff(shape);
            StartCoroutine(RemoveBuffAfterCooldown(shape));
        }

        // 親オブジェクトが敵でなく、親オブジェクトのLambdaスキルがアップグレードされている場合
        if (!isParentEnemy && parentShape.BaseStats.isLambdaUpgrade)
        {
            // 衝突したShapeが存在し、親オブジェクトと同じ敵味方フラグを持っている場合
            if (shape != null && shape.IsEnemy == isParentEnemy)
            {
                // 衝突したShapeを回復
                shape.CombatHandler.TakeHeal(attackBuffAmount);
            }
        }
    }
    // ==========================
    // バフの適用
    // ==========================
    private void ApplyAttackBuff(Shape shape)
    {
        // 攻撃力バフを適用
        shape.CombatHandler.TakeFlatBuffForBattle(StatType.Attack, attackBuffAmount,buffDuration);
        buffedShapes.Add(shape);
    }
    // ==========================
    // バフの削除
    // ==========================
    private IEnumerator RemoveBuffAfterCooldown(Shape shape)
    {
        yield return new WaitForSeconds(buffCooldown);
        buffedShapes.Remove(shape);
    }
}
