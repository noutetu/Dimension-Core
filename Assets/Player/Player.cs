using UnityEngine;
using System.Collections.Generic;
using UniRx;
using System;
// =====================
// プレイヤークラス
// =====================
public class Player : MonoBehaviour
{
    public PlayerCurrencyManager currencyManager; // 通貨マネージャー
    public PlayerShapeManager shapeManager; // 図形マネージャー
    public PlayerBuffManager buffManager; // バフマネージャー

    // ==========================
    // 初期化
    // ==========================
    private void Start()
    {
        // コンポーネントの取得
        currencyManager = GetComponentInChildren<PlayerCurrencyManager>();
        shapeManager = GetComponentInChildren<PlayerShapeManager>();
        buffManager = GetComponentInChildren<PlayerBuffManager>();
    }

    // ==========================
    // 通貨
    // ==========================
    public void EarnCurrency(float amount)
    {
        currencyManager.EarnCurrency(amount);
    }

    public void SpendCurrency(float amount)
    {
        currencyManager.SpendCurrency(amount);
    }

    // ==========================
    // 図形
    // ==========================
    public void AddShape(ShapeBaseStats baseStats) // 修正: BaseStats -> baseStats
    {
        shapeManager.AddShape(baseStats);
    }

    public void RemoveShape(ShapeBaseStats shape)
    {
        shapeManager.RemoveShape(shape);
    }

    public List<Shape> SpawnShapesForBattle(Transform[] spawnPoints)
    {
        return shapeManager.SpawnShapesForBattle(spawnPoints);
    }
    // ==========================
    // バフ
    // ==========================
    // 攻撃を強化
    public void EnhancePowerByPercentage(float percentage)
    {
        buffManager.EnhancePowerByPercentage(percentage);
    }

    // 攻撃を減少
    public void ReducePowerByPercentage(float percentage)
    {
        buffManager.ReducePowerByPercentage(percentage);
    }

    // 速度を強化
    public void EnhanceSpeedByPercentage(float percentage)
    {
        buffManager.EnhanceSpeedByPercentage(percentage);
    }

    // 速度を減少
    public void ReduceSpeedByPercentage(float percentage)
    {
        buffManager.ReduceSpeedByPercentage(percentage);
    }

    // 回避を強化
    public void EnhanceEvasionByPercentage(float percentage)
    {
        buffManager.EnhanceEvasionByPercentage(percentage);
    }

    // 回避を減少
    public void ReduceEvasionByPercentage(float percentage)
    {
        buffManager.ReduceEvasionByPercentage(percentage);
    }

    // クリティカルを強化
    public void EnhanceCriticalByPercentage(float percentage)
    {
        buffManager.EnhanceCriticalByPercentage(percentage);
    }

    // クリティカルを減少
    public void ReduceCriticalByPercentage(float percentage)
    {
        buffManager.ReduceCriticalByPercentage(percentage);
    }
    // ==========================
    // 戦闘終了処理
    // ==========================
    public void EndBattle()
    {
        // 戦闘終了時に通貨を獲得
        currencyManager.EarnCurrency();
        // フェーズ5の場合、追加で通貨を獲得
        if (StageManager.Instance.GetCurrentPhase() == 5)
        {
            Observable.Timer(TimeSpan.FromSeconds(1.5f)).Subscribe(_ =>
            {
                currencyManager.EarnCurrency(150);
            }).AddTo(this);
        }
        // アクティブ図形リストをクリア
        shapeManager.ClearActiveShapes();
    }

}
