using UnityEngine;
using System.Collections.Generic;

public class DimensionManager : MonoBehaviour
{
    public static DimensionManager Instance { get; private set; }

    [Header("次元データ")]
    [SerializeField] private List<DimensionData> allDimensions; // すべての次元データ
    private DimensionData currentDimension; // 現在選ばれている次元

    private void Awake()
    {
        // シングルトンの適用
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // シーンを跨いでも維持
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 現在の次元を取得
    /// </summary>
    public DimensionData GetCurrentDimension()
    {
        return currentDimension;
    }

    // 次元を選択
    public void SelectRandomDimension(DimensionData dimension)
    {
        currentDimension = dimension;
    }

    /// <summary>
    /// ランダムな次元を返す
    /// </summary>
    public DimensionData ReturnRandomDimension(int currentStage, DimensionData firstDimension = null)
    {
        // 現在のステージに出現可能な次元をフィルタリング
        List<DimensionData> availableDimensions = allDimensions.FindAll(d => d.requiredStage <= currentStage);

        // 1. もし availableDimensions が空なら何も返せない
        if (availableDimensions.Count == 0)
        {
            DebugUtility.LogWarning("No dimensions are available for the current stage.");
            return null;
        }

        // 2. 引数 firstDimension が null の場合 → ランダムに返す
        if (firstDimension == null)
        {
            return availableDimensions[Random.Range(0, availableDimensions.Count)];
        }

        // 3. firstDimension が指定されている場合
        // 「違う次元を返したい」ので、フィルタリングして除外を試みる
        List<DimensionData> filtered = availableDimensions.FindAll(d => d != firstDimension);

        // 4. フィルタ結果が空でないなら「違う次元」の中から返す
        if (filtered.Count > 0)
        {
            return filtered[Random.Range(0, filtered.Count)];
        }
        else
        {
            // 「異なる次元」が存在しない場合の fallback
            // 例: 同じ次元を返すか、nullを返すなど、プロジェクト方針で決める
            DebugUtility.LogWarning("No alternative dimension found. Returning the same dimension.");
            return firstDimension;
        }
    }
}
