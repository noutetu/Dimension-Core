using System.Collections.Generic;
using UnityEngine;
// ================================
// 敵の出現パターンリスト
// ================================
public class EnemyPatternList : MonoBehaviour
{   // シングルトン
    public static EnemyPatternList Instance { get; private set; }

    [Header("敵のパターン")]
    [SerializeField] private List<EnemyPattern> enemyPatterns;
    
    // ================================
    // 初期化
    // ================================
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンをまたいでも維持
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // ================================
    // 現在のステージのボスパターンを取得
    // ================================
    public List<ShapeBaseStats> GetCurrentStageBossPattern()
    {   // ステージ番号を取得
        int index = StageManager.Instance.GetCurrentStage() - 1;
        if (index < 0 || index >= enemyPatterns.Count)
        {
            DebugUtility.LogError("指定されたステージのボスパターンが見つかりません！");
            return null;
        }
        // ボスのパターンを返す
        return enemyPatterns[index].enemies;
    }
}
