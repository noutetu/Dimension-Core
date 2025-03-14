using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatternList : MonoBehaviour
{
    public static EnemyPatternList Instance { get; private set; }
    [Header("敵のパターン")]
    [SerializeField] private List<EnemyPattern> enemyPatterns; // Unity Inspector上で設定する敵のパターンリスト

    public List<ShapeBaseStats> GetCurrentStageBossPattern()
    {
        int index = StageManager.Instance.GetCurrentStage() - 1;
        if (index < 0 || index >= enemyPatterns.Count)
        {
            DebugUtility.LogError("指定されたステージのボスパターンが見つかりません！");
            return null;
        }
        return enemyPatterns[index].enemies;
    }

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
}
