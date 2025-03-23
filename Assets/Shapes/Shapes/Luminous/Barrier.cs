using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// ==========================
// Lumiousのバリア
// ==========================
public class Barrier : MonoBehaviour
{
    private bool isParentEnemy;             // 親オブジェクトが敵かどうか
    private Shape parentShape;              // 親オブジェクトのShapeコンポーネント

    private float attackBuffAmount;         // バリアの攻撃力バフの値(実数値)
    private float buffCooldown = 2f;        // バフのクールダウン時間
    private float buffDuration = 10f;       // バフの持続時間
    
    private HashSet<Shape> buffedShapes;    // バフを適用したShapeのリスト

    // ==========================
    // 初期化
    // ==========================
    private void OnEnable()
    {
        // バフリストの初期化
        buffedShapes = new HashSet<Shape>();
        // 親オブジェクトのisEnemyフラグを取得
        parentShape = GetComponentInParent<Shape>();
        if (parentShape != null)
        {
            // 親オブジェクトが敵かどうかを取得
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
            // バフの持続時間後にバフを削除
            StartCoroutine(RemoveBuffAfterCooldown(shape));
        }

        // 親オブジェクトが敵でなく、親オブジェクトのLambdaスキルがアップグレードされている場合
        if (!isParentEnemy && parentShape.BaseStats.IsLambdaUpgrade)
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
        shape.CombatHandler.TakeFlatBuffForBattle(StatType.Attack, attackBuffAmount, buffDuration);
        // バフリストに追加
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
