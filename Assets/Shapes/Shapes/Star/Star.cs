using System.Collections.Generic;
using UnityEngine;
// ==========================
// Starクラス(敵との衝突時にShootingStarを発射)
// ==========================
public class Star : Shape
{
    [Header("ShootingStarプレハブ")]
    [SerializeField] ShootingStar shootingStarPrefab;
    
    int shootingStarCount = 3;      // 発射するShootingStarの数
    float shootingStarAtk = 100;    // ShootingStarの攻撃力

    // 発射したShootingStarのリスト
    public List<ShootingStar> shootingStars = new List<ShootingStar>();

    // ==========================
    // 敵との衝突時の処理
    // ==========================
    protected override void OnEnemyCollision(Shape other)
    {
        // 通常の敵との衝突時の処理
        base.OnEnemyCollision(other);
        // ShootingStarを発射
        Shoot();
    }

    // ==========================
    // ShootingStarを発射
    // ==========================
    private void Shoot()
    {
        DebugUtility.Log("StarのShoot!!!!!!");
        // ループでShootingStarを生成
        for (int i = 0; i < shootingStarCount; i++)
        {
            // アップグレード適用可能か
            bool isUpgraded = !IsEnemy && baseStats.IsOmegaUpgrade;
            // ShootingStarを生成
            ShootingStar shootingStar = Instantiate(shootingStarPrefab, transform.position, Quaternion.identity);
            // 初期化
            shootingStar.Initialize(shootingStarAtk, IsEnemy, this, isUpgraded);
            // リストに追加
            shootingStars.Add(shootingStar);
        }
    }

    // ==========================
    // 破壊時の処理
    // ==========================
    public override void OnDestroyed()
    {
        // 通常の破壊処理
        base.OnDestroyed();
        // 発射したShootingStarを破棄
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
        // アルファアップグレード適用可能なら
        if (baseStats.IsAlphaUpgrade)
        {
            // shootingStarの攻撃力を増加
            shootingStarAtk = 200;
        }
    }

    // ==========================
    // Lambdaスキル
    // ==========================
    public override void ActivateLambdaSkill()
    {
        // ラムダアップグレード適用可能なら
        if (baseStats.IsLambdaUpgrade)
        {
            // 発射するShootingStarの数を増加
            shootingStarCount = 5;
        }
    }
}
