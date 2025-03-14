using UnityEngine;
using System.Collections.Generic;
using UniRx;
using System;

public class Player : MonoBehaviour
{
    public PlayerCurrencyManager currencyManager;
    public PlayerShapeManager shapeManager;
    public PlayerBuffManager buffManager;

    // ==========================
    // 初期化
    // ==========================
    private void Start()
    {
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
    public void AddShape(ShapeBaseStats BaseStats)
    {
        shapeManager.AddShape(BaseStats);
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
    public void EnhancePowerByPercentage(float percentage)
    {
        buffManager.EnhancePowerByPercentage(percentage);
    }

    public void ReducePowerByPercentage(float percentage)
    {
        buffManager.ReducePowerByPercentage(percentage);
    }

    public void EnhanceSpeedByPercentage(float percentage)
    {
        buffManager.EnhanceSpeedByPercentage(percentage);
    }

    public void ReduceSpeedByPercentage(float percentage)
    {
        buffManager.ReduceSpeedByPercentage(percentage);
    }

    public void EnhanceEvasionByPercentage(float percentage)
    {
        buffManager.EnhanceEvasionByPercentage(percentage);
    }

    public void ReduceEvasionByPercentage(float percentage)
    {
        buffManager.ReduceEvasionByPercentage(percentage);
    }

    public void EnhanceCriticalByPercentage(float percentage)
    {
        buffManager.EnhanceCriticalByPercentage(percentage);
    }

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
        if(StageManager.Instance.GetCurrentPhase() == 5)
        {
            Observable.Timer(TimeSpan.FromSeconds(1.5f)).Subscribe(_ =>
            {
                currencyManager.EarnCurrency(150);
            }).AddTo(this);
        }

        shapeManager.ClearActiveShapes();
    }

}
