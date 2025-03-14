using UnityEngine;
using System.Collections.Generic;
using UniRx;

public class EnemyGenerator : MonoBehaviour
{
    [Header("敵の生成場所")]
    [SerializeField] private Transform[] enemySpawnPoints;

    [Header("敵のパターン")]
    [SerializeField] private List<EnemyPattern> enemyPatterns; // Unity Inspector上で設定する敵のパターンリスト

    [Header("ボス限定図形")]
    [SerializeField] private ShapeBaseStats bossShape; // ボス図形

    // 生きている敵のリスト
    public ReactiveCollection<Shape> activeEnemies = new ReactiveCollection<Shape>(); // 敵のリスト

    // バフ関連の変数
    private float hpbuff = 0;
    private float atkbuff = 0;
    private float spdBuffValue = 0;
    private float evasionBuffValue = 0;
    private float critBuffValue = 0;

    private void Start()
    {
        // 敵が全滅したことを監視
        activeEnemies.ObserveCountChanged()
            .Where(count => count == 0) // 敵の数が 0 になったとき
            .Delay(System.TimeSpan.FromSeconds(1.2f))
            .Subscribe(_ => OnAllEnemiesDefeated())
            .AddTo(this);
    }

    // ===========================================
    // 敵の生成
    // ===========================================
    public void GenerateAllEnemies()
    {
        DebugUtility.Log("敵を生成します！");
        
        // ボスフェーズの場合、指定された敵を生成
        if (StageManager.Instance.GetCurrentPhase() == 5)
        {
            DebugUtility.Log("ボス図形を生成します！");            GenerateSpecifiedEnemies();
            return;
        }

        int totalRarity = GameDifficultyManager.Instance.TotalRarity;
        List<ShapeBaseStats> enemies = GetRandomEnemiesWithTotalRarity(totalRarity);

        if (enemies == null || enemies.Count == 0)
        {
            DebugUtility.LogWarning("敵の生成に失敗しました！敵リストが空です。");
            return;
        }

        int enemyIndex = 0;

        foreach (var enemy in enemies)
        {
            if (enemyIndex < enemySpawnPoints.Length)
            {
                if (GenerateEnemy(enemySpawnPoints[enemyIndex], enemy))
                {
                    enemyIndex++;
                }
                else
                {
                    DebugUtility.LogWarning("敵の生成に失敗しました！プレハブが設定されていない可能性があります。");
                }
            }
            else
            {
                DebugUtility.LogWarning("敵の生成ポイントが不足しています。");
            }
        }

        ResetEnemyBuff();
        DebugUtility.Log($"Generated {enemyIndex} enemies.");    }

    public void GenerateSpecifiedEnemies()
    {
        DebugUtility.Log("敵を生成します！");
        List<ShapeBaseStats> specifiedEnemies = EnemyPatternList.Instance.GetCurrentStageBossPattern();
        int enemyIndex = 0;

        foreach (var enemy in specifiedEnemies)
        {
            if (enemyIndex < enemySpawnPoints.Length)
            {
                GenerateEnemy(enemySpawnPoints[enemyIndex], enemy);
                enemyIndex++;
            }
        }

        ResetEnemyBuff();
        DebugUtility.Log($"Generated {enemyIndex} specified enemies.");    }

    private bool GenerateEnemy(Transform spawnPoint, ShapeBaseStats enemyStats)
    {
        GameObject prefabToSpawn = enemyStats.ShapePrefab;
        if (prefabToSpawn == null)
        {
            DebugUtility.LogWarning("敵のプレハブが設定されていません！");
            return false;
        }

        GameObject enemy = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        Shape shape = enemy.GetComponent<Shape>();

        if (shape != null)
        {
            shape.IsEnemy = true;
            shape.Stats.ApplyEnemyEnhance(
                hpbuff,  // 体力
                atkbuff, // 攻撃力
                spdBuffValue, // 速度
                evasionBuffValue,  // 回避
                critBuffValue // クリティカル
            );
            shape.Initialize();
            activeEnemies.Add(shape);

            // 敵が破壊されたときのイベントを購読
            shape.Stats.CurrentHP
                .Where(hp => hp <= 0)
                .Subscribe(_ => activeEnemies.Remove(shape))
                .AddTo(shape);
            return true;
        }
        return false;
    }

    // ===========================================
    // 敵の選択
    // ===========================================
    private List<ShapeBaseStats> GetRandomEnemiesWithTotalRarity(int totalRarity)
    {
        DebugUtility.Log($"敵の合計レアリティ: {totalRarity}");
        DimensionData currentDimension = DimensionManager.Instance.GetCurrentDimension();
        if (currentDimension == null || currentDimension.possibleShapes.Count == 0)
        {
            DebugUtility.LogWarning("現在の次元の敵リストが空です！");
            return null;
        }

        List<ShapeBaseStats> allShapes = ShapeDex.Instance.GetAllShapes();
        if (allShapes == null || allShapes.Count == 0)
        {
            DebugUtility.LogWarning("全図形リストが空です！");
            return null;
        }

        List<ShapeBaseStats> selectedShapes = new List<ShapeBaseStats>();
        List<ShapeBaseStats> availableShapes = new List<ShapeBaseStats>(allShapes);
        List<ShapeBaseStats> dimensionShapes = currentDimension.possibleShapes;
        int currentTotalRarity = 0;

        while (currentTotalRarity < totalRarity && availableShapes.Count > 0)
        {
            ShapeBaseStats randomShape = GetRandomShape(dimensionShapes, availableShapes);

            if (randomShape == bossShape)
            {
                continue;
            }

            int shapeRarityValue = (int)randomShape.Rarity;

            if (currentTotalRarity + shapeRarityValue <= totalRarity)
            {
                selectedShapes.Add(randomShape);
                currentTotalRarity += shapeRarityValue;
            }
        }

        if (currentTotalRarity < totalRarity)
        {
            DebugUtility.LogWarning($"合計レアリティが指定された値に達しませんでした。TotalRarity: {totalRarity}, SelectedRarity: {currentTotalRarity}");
        }

        return selectedShapes;
    }

    /// <summary>
    /// dimensionShapes または availableShapes リストからランダムに図形を選択します。
    /// </summary>
    /// <param name="dimensionShapes">現在の次元に特有の図形のリスト。</param>
    /// <param name="availableShapes">一般的に利用可能な図形のリスト。</param>
    /// <returns>提供されたリストのいずれかからランダムに選択された <see cref="ShapeBaseStats"/> オブジェクト。</returns>
    /// <remarks>
    /// メソッドは、dimensionShapes リストが空でない場合、50% の確率でそのリストから図形を選択します。
    /// それ以外の場合は、availableShapes リストから図形を選択します。
    /// </remarks>
    private ShapeBaseStats GetRandomShape(List<ShapeBaseStats> dimensionShapes, List<ShapeBaseStats> availableShapes)
    {
        if (Random.value < 0.5f && dimensionShapes.Count > 0)
        {
            return dimensionShapes[Random.Range(0, dimensionShapes.Count)];
        }
        else
        {
            return availableShapes[Random.Range(0, availableShapes.Count)];
        }
    }

    // ===========================================
    // 敵の削除
    // ===========================================
    private void OnAllEnemiesDefeated()
    {
        DebugUtility.Log("全ての敵が倒された！");        GameFlowManager gameFlowManager = FindObjectOfType<GameFlowManager>();
        if (gameFlowManager != null)
        {
            gameFlowManager.EndBattlePhase();
        }
        else
        {
            DebugUtility.LogError("GameFlowManagerが見つかりません！");
        }
    }

    // ===========================================
    // 敵のバフ関連
    // ===========================================
    public void SetEnemyBuff(float hp = 0, float atk = 0, float spd = 0, float evasion = 0, float crit = 0)
    {
        hpbuff += hp;
        atkbuff += atk;
        spdBuffValue += spd;
        evasionBuffValue += evasion;
        critBuffValue += crit;
    }

    public void ResetEnemyBuff()
    {
        hpbuff = 0;
        atkbuff = 0;
        spdBuffValue = 0;
        evasionBuffValue = 0;
        critBuffValue = 0;
    }
}
