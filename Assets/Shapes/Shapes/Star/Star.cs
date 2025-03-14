using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : Shape
{
    // ShootingStarのプレハブ
    [Header("ShootingStar")]
    [SerializeField] ShootingStar shootingStarPrefab;
    int shootingStarCount = 3;
    float shootingStarAtk = 100;

    public List<ShootingStar> shootingStars = new List<ShootingStar>();

    // ==========================
    // 敵との衝突時の処理
    // ==========================
    protected override void OnEnemyCollision(Shape other)
    {
        base.OnEnemyCollision(other);
        Shoot();
    }

    // ==========================
    // ShootingStarを発射
    // ==========================
    private void Shoot()
    {
        DebugUtility.Log("StarのShoot！！！！！！！！！！！");
        for (int i = 0; i < shootingStarCount; i++)
        {
            bool isUpgraded = !IsEnemy && baseStats.IsOmegaUpgrade;
            ShootingStar shootingStar = Instantiate(shootingStarPrefab, transform.position, Quaternion.identity);
            shootingStar.Initialize(shootingStarAtk, IsEnemy, this, isUpgraded);
            shootingStars.Add(shootingStar);
        }
    }

    // ==========================
    // 破壊時の処理
    // ==========================
    public override void OnDestroyed()
    {
        base.OnDestroyed();
        var copy = new List<ShootingStar>(shootingStars);
        foreach (var shootingStar in copy)
        {
            if (shootingStar != null) shootingStar.DestroyShootingStar();
        }
    }

    // ==========================
    // Alphaスキル
    // ==========================
    public override void ActivateAlphaSkill()
    {
        if (baseStats.isAlphaUpgrade)
        {
            shootingStarAtk = 200;
        }
    }

    // ==========================
    // Lambdaスキル
    // ==========================
    public override void ActivateLambdaSkill()
    {
        if (baseStats.isLambdaUpgrade)
        {
            shootingStarCount = 6;
        }
    }
}
