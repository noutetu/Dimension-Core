using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;

public class Moon : Shape
{
    private enum MoonPhase { NewMoon, HalfMoon, FullMoon }
    private MoonPhase currentPhase = MoonPhase.NewMoon; // 初期状態を新月に設定

    [SerializeField] private float phaseDuration = 10f; // 各状態の持続時間（秒）
    [SerializeField] private GameObject newMoonObject; // 新月状態のゲームオブジェクト
    [SerializeField] private GameObject halfMoonObject; // 半月状態のゲームオブジェクト
    [SerializeField] private GameObject fullMoonObject; // 満月状態のゲームオブジェクト

    private PolygonCollider2D parentCollider;

    float evasionBuffValue = 20f;
    float damageBuffValue = 200f;
    float healValue = 80;

    // ==========================
    // 初期化
    // ==========================
    public override void Initialize()
    {
        base.Initialize();
        parentCollider = GetComponent<PolygonCollider2D>();
        ActivateMoonPhase(newMoonObject); // 初期状態を新月に設定
        StartMoonCycle();
    }

    // ==========================
    // 月のサイクルの開始
    // ==========================
    private void StartMoonCycle()
    {
        Observable.Interval(System.TimeSpan.FromSeconds(phaseDuration))
            .Subscribe(_ =>
            {
                ChangePhase();
            })
            .AddTo(this);
    }

    // ==========================
    // 月のフェーズの変更
    // ==========================
    private void ChangePhase()
    {
        switch (currentPhase)
        {
            case MoonPhase.NewMoon:
                currentPhase = MoonPhase.HalfMoon;
                ActivateMoonPhase(halfMoonObject);
                ApplyHalfMoonEffects();
                break;
            case MoonPhase.HalfMoon:
                currentPhase = MoonPhase.FullMoon;
                ActivateMoonPhase(fullMoonObject);
                ApplyFullMoonEffects();
                break;
            case MoonPhase.FullMoon:
                currentPhase = MoonPhase.NewMoon;
                ActivateMoonPhase(newMoonObject);
                ApplyNewMoonEffects();
                break;
        }
    }

    // ==========================
    // 月のフェーズのアクティベート
    // ==========================
    private void ActivateMoonPhase(GameObject activeObject)
    {
        newMoonObject.SetActive(activeObject == newMoonObject);
        halfMoonObject.SetActive(activeObject == halfMoonObject);
        fullMoonObject.SetActive(activeObject == fullMoonObject);

        // 現在の月の状態が持つPolygonCollider2Dの形状を親のPolygonCollider2Dにコピー
        PolygonCollider2D activeCollider = activeObject.GetComponent<PolygonCollider2D>();
        if (activeCollider != null && parentCollider != null)
        {
            parentCollider.pathCount = 0; // 既存のパスをクリア
            parentCollider.pathCount = activeCollider.pathCount;
            for (int i = 0; i < activeCollider.pathCount; i++)
            {
                Vector2[] path = activeCollider.GetPath(i);
                Vector2[] scaledPath = new Vector2[path.Length];
                for (int j = 0; j < path.Length; j++)
                {
                    // スケールを考慮して形状を調整
                    scaledPath[j] = Vector2.Scale(path[j], activeObject.transform.localScale);
                }
                parentCollider.SetPath(i, scaledPath);
                activeCollider.enabled = false;
            }
        }
    }

    // ==========================
    // 新月の効果を適用
    // ==========================
    private void ApplyNewMoonEffects()
    {
        // 新月の効果を適用
        List<Shape> activeShapes = GetAllFriendShapes();
        
        foreach (Shape shape in activeShapes)
        {
            shape.CombatHandler.TakeFlatBuffForBattle(StatType.Evasion,evasionBuffValue,phaseDuration);
        }
    }

    // ==========================
    // 半月の効果を適用
    // ==========================
    private void ApplyHalfMoonEffects()
    {
        // 半月の効果を適用
        List<Shape> activeShapes = GetAllFriendShapes();
        
        foreach (Shape shape in activeShapes)
        {
            shape.CombatHandler.TakeFlatBuffForBattle(StatType.Attack,damageBuffValue,phaseDuration);
        }
    }

    // ==========================
    // 満月の効果を適用
    // ==========================
    private void ApplyFullMoonEffects()
    {
        // 満月の効果を適用
        List<Shape> activeShapes = GetAllFriendShapes();
        
        foreach (Shape shape in activeShapes)
        {
            shape.CombatHandler.TakeHealOverTime(healValue, phaseDuration, 1f);
        }
    }

    // ==========================
    // 友軍のShapeを取得
    // ==========================
    private List<Shape> GetAllFriendShapes()
    {
        List<Shape> activeShapes = new List<Shape>();

        if(IsEnemy)
        {
            EnemyGenerator enemyGenerator = FindObjectOfType<EnemyGenerator>();
            activeShapes = new List<Shape>(enemyGenerator.activeEnemies);
            return activeShapes;
        }
        Player player = FindObjectOfType<Player>();
        if (player != null)
        {
            activeShapes = player.shapeManager.activeShapes.ToList();
        }

        return activeShapes;
    }

    // ==========================
    // Alphaスキル
    // ==========================
    public override void ActivateAlphaSkill()
    {
        if (BaseStats.isAlphaUpgrade)
        {
            phaseDuration -= 5f;
        }
    }

    // ==========================
    // Lambdaスキル
    // ==========================
    public override void ActivateLambdaSkill()
    {
        if(baseStats.isLambdaUpgrade)
        {
            phaseDuration += 5f;
        }
    }
}
