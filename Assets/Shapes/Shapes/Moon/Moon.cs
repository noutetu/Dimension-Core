using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System.Linq;
// ==========================
// 月の特性: 月のフェーズによって効果が変化する
// 新月: 友軍の回避力が上昇
// 半月: 友軍の攻撃力が上昇 
// 満月: 友軍の体力が回復
// ==========================
public class Moon : Shape
{
    [Header("新月状態のゲームオブジェクト")]
    [SerializeField] private GameObject newMoonObject;      
    [Header("半月状態のゲームオブジェクト")]
    [SerializeField] private GameObject halfMoonObject;     
    [Header("満月状態のゲームオブジェクト")]
    [SerializeField] private GameObject fullMoonObject;     

    private enum MoonPhase { NewMoon, HalfMoon, FullMoon }  // 月のフェーズ
    private MoonPhase currentPhase = MoonPhase.NewMoon;     // 初期状態を新月に設定

    private PolygonCollider2D parentCollider; // 親のPolygonCollider2D

    private float phaseDuration = 10f;      // 各状態の持続時間（秒）
    float evasionBuffValue = 20f;           // 回避力のバフ値
    float damageBuffValue = 200f;           // 攻撃力のバフ値
    float healValue = 80;                   // 回復量

    // ==========================
    // 初期化
    // ==========================
    public override void Initialize()
    {
        // 通常の初期化処理
        base.Initialize();
        // PolygonCollider2Dを取得
        parentCollider = GetComponent<PolygonCollider2D>();
        // 初期状態を新月に設定
        SetMoonCollision(newMoonObject); 
        // 月のサイクルを開始
        StartMoonCycle();
    }

    // ==========================
    // 月のサイクルの開始
    // ==========================
    private void StartMoonCycle()
    {
        // 一定時間ごとに月のフェーズを変更
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
            // 新月の場合は半月に変更
            case MoonPhase.NewMoon:
                currentPhase = MoonPhase.HalfMoon;  // 半月に変更
                SetMoonCollision(halfMoonObject);   // 半月のコライダーに設定
                ApplyHalfMoonEffects();             // 半月の効果を適用
                break;
            // 半月の場合は満月に変更
            case MoonPhase.HalfMoon:                
                currentPhase = MoonPhase.FullMoon;  // 満月に変更
                SetMoonCollision(fullMoonObject);   // 満月のコライダーに設定
                ApplyFullMoonEffects();             // 満月の効果を適用
                break;
            // 満月の場合は新月に変更
            case MoonPhase.FullMoon:
                currentPhase = MoonPhase.NewMoon;   // 新月に変更
                SetMoonCollision(newMoonObject);    // 新月のコライダーに設定
                ApplyNewMoonEffects();              // 新月の効果を適用   
                break;
        }
    }

    // ==========================
    // 月のフェーズのアクティベート
    // ==========================
    private void SetMoonCollision(GameObject activeObject)
    {
        // 月のフェーズに応じてアクティブなゲームオブジェクトを切り替え
        newMoonObject.SetActive(activeObject == newMoonObject);
        halfMoonObject.SetActive(activeObject == halfMoonObject);
        fullMoonObject.SetActive(activeObject == fullMoonObject);

        // 現在の月の状態が持つPolygonCollider2Dの形状を親のPolygonCollider2Dにコピー
        PolygonCollider2D activeCollider = activeObject.GetComponent<PolygonCollider2D>();
        if (activeCollider != null && parentCollider != null)
        {
            // 既存のパスをクリア
            parentCollider.pathCount = 0;
            // 新しいパスを設定
            parentCollider.pathCount = activeCollider.pathCount;
            // パスの数だけ繰り返し
            for (int i = 0; i < activeCollider.pathCount; i++)
            {
                // パスを取得
                Vector2[] path = activeCollider.GetPath(i);
                // スケールを考慮したパスを作成
                Vector2[] scaledPath = new Vector2[path.Length];
                for (int j = 0; j < path.Length; j++)
                {
                    // スケールを考慮して形状を調整
                    scaledPath[j] = Vector2.Scale(path[j], activeObject.transform.localScale);
                }
                // 親のPolygonCollider2Dにパスを設定
                parentCollider.SetPath(i, scaledPath);
                // 親のPolygonCollider2Dをアクティブにする
                activeCollider.enabled = false;
            }
        }
        // 敵だった場合の色変更
        SetColor();
    }

    // ==========================
    // 新月の効果を適用
    // ==========================
    private void ApplyNewMoonEffects()
    {
        // 味方の図形を取得
        List<Shape> activeShapes = GetAllFriendShapes();

        foreach (Shape shape in activeShapes)
        {
            // 回避力のバフを適用
            shape.CombatHandler.TakeFlatBuffForBattle(StatType.Evasion, evasionBuffValue, phaseDuration);
        }
    }

    // ==========================
    // 半月の効果を適用
    // ==========================
    private void ApplyHalfMoonEffects()
    {
        // 味方の図形を取得
        List<Shape> activeShapes = GetAllFriendShapes();

        foreach (Shape shape in activeShapes)
        {
            // 攻撃力のバフを適用
            shape.CombatHandler.TakeFlatBuffForBattle(StatType.Attack, damageBuffValue, phaseDuration);
        }
    }

    // ==========================
    // 満月の効果を適用
    // ==========================
    private void ApplyFullMoonEffects()
    {
        // 味方の図形を取得
        List<Shape> activeShapes = GetAllFriendShapes();

        foreach (Shape shape in activeShapes)
        {
            // 体力を回復
            shape.CombatHandler.TakeHealOverTime(healValue, phaseDuration, 1f);
        }
    }

    // ==========================
    //　味方のShapeを取得
    // ==========================
    private List<Shape> GetAllFriendShapes()
    {
        // アクティブ図形リストを作成
        List<Shape> activeShapes;

        // 敵の場合
        if (IsEnemy)
        {
            // 敵ジェネレーターを取得
            EnemyGenerator enemyGenerator = FindObjectOfType<EnemyGenerator>();
            //　アクティブな敵のリストを取得
            activeShapes = new List<Shape>(enemyGenerator.activeEnemies);
            // アクティブな図形リストを返す
            return activeShapes;
        }
        // 味方の場合
        // プレイヤーを取得
        Player player = FindObjectOfType<Player>();
        // アクティブな図形リストを取得
        activeShapes = player.shapeManager.activeShapes.ToList();
        // アクティブな図形リストを返す
        return activeShapes;
    }
    // ==========================
    // 敵だった場合の色変更  
    // ==========================
    private void SetColor()
    {
        if (IsEnemy)
        {
            
            // 赤色に変更
            GetComponentInChildren<SpriteRenderer>().color = Color.red;
        }
    }
    // ==========================
    // Alphaスキル
    // ==========================
    public override void ActivateAlphaSkill()
    {
        // Alphaスキルがアクティブの場合
        if (BaseStats.IsAlphaUpgrade)
        {
            // 月のフェーズの持続時間を短縮
            phaseDuration -= 5f;
        }
    }

    // ==========================
    // Lambdaスキル
    // ==========================
    public override void ActivateLambdaSkill()
    {
        // Lambdaスキルがアクティブの場合
        if (baseStats.IsLambdaUpgrade)
        {
            // 月のフェーズの持続時間を延長
            phaseDuration += 5f;
        }
    }
}
