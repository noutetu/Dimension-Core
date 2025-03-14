using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heart : Shape
{
    int healValue = 10;

    // ==========================
    // 味方との衝突処理
    // ==========================
    protected override void OnFriendCollision(Shape other)
    {
        // otherの最大体力20%回復したい
        float heal = other.BaseStats.BaseHP * healValue / 100;
        other.CombatHandler.TakeHeal(heal);
    }
    // ==========================
    // 初期化
    // ==========================
    public override void Initialize()
    {
        base.Initialize();
        if (baseStats.IsOmegaUpgrade && !IsEnemy)
        {
            // 999秒間毎秒30回復
            CombatHandler.TakeHealOverTime(30, 999,5);
        }
    }
    // ==========================
    // Alphaスキル
    // ==========================
    public override void ActivateAlphaSkill()
    {
        if(baseStats.isAlphaUpgrade)
        {
            healValue = 30;
        }
    }
}
