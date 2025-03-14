using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 図形のレアリティやレアリティに応じた図形を返すクラス
/// </summary>
public class RarityManager : MonoBehaviour
{
    public static RarityManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 指定されたレアリティの図形リストを取得
    /// </summary>
    /// <param name="rarity">取得する図形のレアリティ</param>
    /// <returns>指定されたレアリティの図形リスト</returns>
    public List<ShapeBaseStats> GetShapesByRarity(Rarity rarity)
    {
        return ShapeDex.Instance.GetAllShapes().Where(s => s.Rarity == rarity).ToList();
    }

    /// <summary>
    /// 指定されたレアリティ以上のランダムな図形を取得
    /// </summary>
    /// <param name="minRarity">最低レアリティ</param>
    /// <returns>ランダムに選ばれた図形</returns>
    public ShapeBaseStats GetRandomShapeOfRarity(Rarity minRarity)
    {
        List<ShapeBaseStats> allShapes = ShapeDex.Instance.GetAllShapes();
        if (allShapes == null || allShapes.Count == 0) return null;

        List<ShapeBaseStats> filtered = allShapes.FindAll(s => (int)s.Rarity >= (int)minRarity);
        if (filtered.Count == 0) return null;

        int index = Random.Range(0, filtered.Count);
        return filtered[index];
    }

    /// <summary>
    /// 選択モードに応じてレアリティを決定
    /// </summary>
    /// <param name="mode">選択モード</param>
    /// <returns>決定されたレアリティ</returns>
    public Rarity DetermineRarityByMode(SelectionMode mode)
    {
        float randomValue = Random.value;

        switch (mode)
        {
            case SelectionMode.OnlyCommon:
                return Rarity.Common;
            case SelectionMode.OnlyRare:
                return Rarity.Rare;
            case SelectionMode.OnlyLegendary:
                return Rarity.Legendary;
            case SelectionMode.ManyCommon:
                return randomValue < 0.8f ? Rarity.Common : randomValue < 0.99f ? Rarity.Rare : Rarity.Legendary;
            case SelectionMode.ManyRare:
                return randomValue < 0.2f ? Rarity.Common : randomValue < 0.9f ? Rarity.Rare : randomValue < 0.99f ? Rarity.Legendary : Rarity.Common;
            case SelectionMode.ManyLegendary:
                return randomValue < 0.1f ? Rarity.Common : randomValue < 0.4f ? Rarity.Rare : Rarity.Legendary;
            default:
                return Rarity.Common;
        }
    }

    /// <summary>
    /// 難易度に応じてレアリティを決定
    /// </summary>
    /// <param name="difficulty">難易度</param>
    /// <param name="randomValue">ランダム値</param>
    /// <returns>決定されたレアリティ</returns>
    public Rarity GetRarityByDifficulty(int difficulty, float randomValue)
    {
        var (commonWeight, rareWeight, legendaryWeight) = GetRarityWeightsByDifficulty(difficulty);

        if (randomValue < commonWeight) return Rarity.Common;
        if (randomValue < commonWeight + rareWeight) return Rarity.Rare;
        return Rarity.Legendary;
    }

    /// <summary>
    /// 難易度に応じたレアリティの重みを取得
    /// </summary>
    /// <param name="difficulty">難易度</param>
    /// <returns>レアリティの重み</returns>
    private (float commonWeight, float rareWeight, float legendaryWeight) GetRarityWeightsByDifficulty(int difficulty)
    {
        return difficulty switch
        {
            1 => (0.9f, 0.1f, 0.0f),
            2 => (0.8f, 0.2f, 0.0f),
            3 => (0.7f, 0.3f, 0.0f),
            4 => (0.6f, 0.35f, 0.05f),
            5 => (0.5f, 0.4f, 0.1f),
            6 => (0.4f, 0.5f, 0.1f),
            7 => (0.3f, 0.5f, 0.2f),
            8 => (0.2f, 0.4f, 0.4f),
            9 => (0.1f, 0.3f, 0.6f),
            10 => (0.0f, 0.2f, 0.8f),
            _ => (1.0f, 0.0f, 0.0f),
        };
    }
}
