using UnityEngine;
using System.Collections.Generic;

public class Triangle : Shape
{
    // ==========================
    // Omegaスキル
    // ==========================
    public override void ActivateOmegaSkill()
    {
        // 敵は強化を受けない
        if(IsEnemy)
        {
            DebugUtility.Log("敵は強化を受けない");
            return;
        }
        
        if (!baseStats.IsOmegaUpgrade){return;}
        Stats.ApplyStatusModifierByUpgrade(0,0,0,10,10);
    }

    // ==========================
    // Alphaスキル
    // ==========================
    public override void ActivateAlphaSkill()
    {
        // 敵は強化を受けない
        if(IsEnemy)return;
        if (!baseStats.isAlphaUpgrade){return;}
        
        Stats.ApplyStatusModifierByUpgrade(0,100,0,20,0);    
    }

    // ==========================
    // Lambdaスキル
    // ==========================
    public override void ActivateLambdaSkill()
    {
        // 敵は強化を受けない
        if(IsEnemy)return;
        if (!baseStats.isLambdaUpgrade){return;}
        
        Stats.ApplyStatusModifierByUpgrade(0,0,4,0,20);
    }
}
