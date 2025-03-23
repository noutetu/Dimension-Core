using UnityEngine;
using System.Collections.Generic;
using UniRx;

// ===========================================
// 敵の生成クラス
// ===========================================
public class EnemyGenerator : MonoBehaviour
{
    [Header("敵の生成場所")]
    [SerializeField] private Transform[] enemySpawnPoints; // 敵が生成されるポイントの配列

    [Header("敵のパターン")]
    [SerializeField] private List<EnemyPattern> enemyPatterns; // Unity Inspector上で設定する敵のパターンリスト

    [Header("ボス限定図形")]
    [SerializeField] private ShapeBaseStats bossShape; // ボス図形

    // 生きている敵のリスト
    public ReactiveCollection<Shape> activeEnemies = new ReactiveCollection<Shape>(); // 敵のリスト

    // バフ関連の変数
    private float hpBuff = 0; // 体力バフ
    private float attackBuff = 0; // 攻撃力バフ
    private float speedBuff = 0; // 速度バフ
    private float evasionBuff = 0; // 回避バフ
    private float criticalBuff = 0; // クリティカルバフ

    // ===========================================
    // 初期化
    // ===========================================
    private void Start()
    {
        // 敵が全滅したことを監視
        activeEnemies.ObserveCountChanged()
            .Where(count => count == 0) // 敵の数が 0 になったとき
            .Delay(System.TimeSpan.FromSeconds(1.2f)) // 1.2秒の遅延
            .Subscribe(_ => OnAllEnemiesDefeated()) // 全ての敵が倒されたときの処理を実行
            .AddTo(this); // このオブジェクトに購読を追加
    }

    // ===========================================
    // 敵をまとめて生成
    // ===========================================
    public void GenerateAllEnemies()
    {
        DebugUtility.Log("敵を生成します！");
        
        // ボスフェーズの場合、指定された敵を生成
        if (StageManager.Instance.GetCurrentPhase() == 5)
        {
            DebugUtility.Log("ボス図形を生成します！");
            GenerateSpecifiedEnemies();
            return;
        }
        // 現在のゲーム難易度に基づく合計レアリティ
        int totalRarity = GameDifficultyManager.Instance.TotalRarity; 
        // 合計レアリティに基づいてランダムに敵を取得
        List<ShapeBaseStats> enemies = GetRandomEnemiesWithTotalRarity(totalRarity); 

        if (enemies == null || enemies.Count == 0)
        {
            DebugUtility.LogWarning("敵の生成に失敗しました！敵リストが空です。");
            return;
        }
        
        int enemyIndex = 0;
        // 敵の生成
        foreach (var enemy in enemies)
        {
            if (enemyIndex < enemySpawnPoints.Length)
            {
                if (GenerateEnemy(enemySpawnPoints[enemyIndex], enemy))
                {
                    enemyIndex++;
                }
            }
        }

        ResetEnemyBuff(); // 敵のバフをリセット
        DebugUtility.Log($"Generated {enemyIndex} enemies."); // 生成された敵の数をログに出力
    }

    // ===========================================
    // 指定された敵の生成
    // ===========================================
    public void GenerateSpecifiedEnemies()
    {   // 現在のステージのボスパターンを取得
        List<ShapeBaseStats> specifiedEnemies = EnemyPatternList.Instance.GetCurrentStageBossPattern(); // 現在のステージのボスパターンを取得
        // 敵の生成
        int enemyIndex = 0;
        foreach (var enemy in specifiedEnemies)
        {
            if (enemyIndex < enemySpawnPoints.Length)
            {
                GenerateEnemy(enemySpawnPoints[enemyIndex], enemy);
                enemyIndex++;
            }
        }

        ResetEnemyBuff(); // 敵のバフをリセット
    }

    // ===========================================
    // 敵の生成処理
    // ===========================================
    private bool GenerateEnemy(Transform spawnPoint, ShapeBaseStats enemyStats)
    {   // 生成する敵のプレハブを取得
        GameObject prefabToSpawn = enemyStats.ShapePrefab; 
        if (prefabToSpawn == null)
        {
            DebugUtility.LogWarning("敵のプレハブが設定されていません！");
            return false;
        }
        // 敵を生成
        GameObject enemy = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation); 
        // 生成された敵のShapeコンポーネントを取得
        Shape shape = enemy.GetComponent<Shape>(); 
        // バフを適用
        if (shape != null)
        {
            shape.IsEnemy = true; // 敵フラグを設定
            shape.Stats.ApplyEnemyEnhance(
                hpBuff,  // 体力
                attackBuff, // 攻撃力
                speedBuff, // 速度
                evasionBuff,  // 回避
                criticalBuff // クリティカル
            );
            // 敵の初期化
            shape.Initialize(); 
            // 敵をアクティブリストに追加
            activeEnemies.Add(shape); 

            // 敵が破壊されたときのイベントを購読
            shape.Stats.CurrentHP
                .Where(hp => hp <= 0) // HPが0以下になったとき
                .Subscribe(_ => activeEnemies.Remove(shape)) // 敵をアクティブリストから削除
                .AddTo(shape); // この敵オブジェクトに購読を追加
            return true;
        }
        return false;
    }

    // ===========================================
    // 合計レアリティから生成する敵を選択
    // ===========================================
    private List<ShapeBaseStats> GetRandomEnemiesWithTotalRarity(int totalRarity)
    {
        DebugUtility.Log($"敵の合計レアリティ: {totalRarity}");
        // 現在の次元データを取得
        DimensionData currentDimension = DimensionManager.Instance.GetCurrentDimension(); 
        if (currentDimension == null || currentDimension.possibleShapes.Count == 0)
        {
            DebugUtility.LogWarning("現在の次元の敵リストが空です！");
            return null;
        }
        // 全ての図形リストを
        List<ShapeBaseStats> allShapes = ShapeDex.Instance.GetAllShapes(); 
         // 選択された図形リスト
        List<ShapeBaseStats> selectedShapes = new List<ShapeBaseStats>();
        // 現在の次元で利用可能な図形リスト
        List<ShapeBaseStats> dimensionShapes = currentDimension.possibleShapes; 
        // 合計レアリティに達するまで図形を選択
        int currentTotalRarity = 0;
        while (currentTotalRarity < totalRarity && allShapes.Count > 0)
        {   
            // ランダムに図形を選択
            ShapeBaseStats randomShape = GetRandomShape(dimensionShapes, allShapes); 
            if (randomShape == bossShape)
            {
                continue;
            }
            // 図形のレアリティ値を取得
            int shapeRarityValue = (int)randomShape.rarity; 

            if (currentTotalRarity + shapeRarityValue <= totalRarity)
            {
                // 図形を選択リストに追加
                selectedShapes.Add(randomShape); 
                // 現在の合計レアリティを更新
                currentTotalRarity += shapeRarityValue; 
            }
        }
        // 選択された図形リストを返す
        return selectedShapes;
    }

    // ===========================================
    // ランダムな図形の選択
    // ===========================================
    private ShapeBaseStats GetRandomShape(List<ShapeBaseStats> dimensionShapes, List<ShapeBaseStats> allShapes)
    {
        // 50%の確率で次元図形を選択
        if (Random.value < 0.5f && dimensionShapes.Count > 0)
        {
            return dimensionShapes[Random.Range(0, dimensionShapes.Count)]; 
        }
        // 50%の確率で全図形リストから選択
        else
        {
            return allShapes[Random.Range(0, allShapes.Count)]; 
        }
    }

    // ===========================================
    // 全ての敵が倒されたときの処理
    // ===========================================
    private void OnAllEnemiesDefeated()
    {
        DebugUtility.Log("全ての敵が倒された！");
        // バトルフェーズを終了
        GameFlowManager gameFlowManager = FindObjectOfType<GameFlowManager>(); 
        if (gameFlowManager != null)
        {
            gameFlowManager.EndBattlePhase(); 
        }
    }

    // ===========================================
    // 敵のバフ設定
    // ===========================================
    public void SetEnemyBuff(float hp = 0, float atk = 0, float spd = 0, float evasion = 0, float crit = 0)
    {
        hpBuff += hp; // 体力バフを追加
        attackBuff += atk; // 攻撃力バフを追加
        speedBuff += spd; // 速度バフを追加
        evasionBuff += evasion; // 回避バフを追加
        criticalBuff += crit; // クリティカルバフを追加
    }

    // ===========================================
    // 敵のバフリセット
    // ===========================================
    public void ResetEnemyBuff()
    {
        hpBuff = 0; // 体力バフをリセット
        attackBuff = 0; // 攻撃力バフをリセット
        speedBuff = 0; // 速度バフをリセット
        evasionBuff = 0; // 回避バフをリセット
        criticalBuff = 0; // クリティカルバフをリセット
    }
}
