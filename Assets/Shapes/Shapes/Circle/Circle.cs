
// ==========================
// Circle
// ==========================
public class Circle : Shape
{
    // ==========================
    // Alphaスキル
    // ==========================
    public override void ActivateAlphaSkill()
    {
        // 敵は強化を受けない
        if (IsEnemy) return;
        if (!baseStats.IsAlphaUpgrade) { return; }
        // ステータスを上昇
        Stats.ApplyStatusModifierByUpgrade(
            hpBuff: 500,
            atkBuff: 200,
            spdBuff: 4,
            critBuff: 20f,
            evaBuff: 20f
        );

        DebugUtility.Log("円のステータスが大きく上昇した");
    }
}
