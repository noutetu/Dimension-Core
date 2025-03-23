using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// ==========================
// 図形のレアリティを管理するクラス
// ==========================
public class RarityManager : MonoBehaviour
{
    public static RarityManager Instance { get; private set; }

    private List<ShapeBaseStats> commonShapes = new List<ShapeBaseStats>();
    private List<ShapeBaseStats> rareShapes = new List<ShapeBaseStats>();
    private List<ShapeBaseStats> superRareShapes = new List<ShapeBaseStats>();

    // ==========================
    // Awake
    // ==========================
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeShapesList();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // ==========================
    // 図形のリストを初期化
    // ==========================
    private void InitializeShapesList()
    {
        commonShapes = ShapeDex.Instance.GetAllShapes().Where(shape => shape.rarity == Rarity.星1).ToList();
        rareShapes = ShapeDex.Instance.GetAllShapes().Where(shape => shape.rarity == Rarity.星2).ToList();
        superRareShapes = ShapeDex.Instance.GetAllShapes().Where(shape => shape.rarity == Rarity.星3).ToList();
        // それぞれのリストの要素数をログに出す
        Debug.Log($"星1の図形の数：{commonShapes.Count}");
        Debug.Log($"星2の図形の数：{rareShapes.Count}");
        Debug.Log($"星3の図形の数：{superRareShapes.Count}");
    }
    // ==========================
    // レアリティを受け取って、そのレアリティの図形をランダムに返すメソッド
    // ==========================
    public ShapeBaseStats GetRandomShapeByRarity(Rarity rarity)
    {
        return rarity switch
        {
            Rarity.星1 => commonShapes[Random.Range(0, commonShapes.Count)],
            Rarity.星2 => rareShapes[Random.Range(0, rareShapes.Count)],
            Rarity.星3 => superRareShapes[Random.Range(0, superRareShapes.Count)],
            _ => null
        };
    }

    // ==========================
    // SelectionModeから図形のレアリティを返すメソッド
    // ==========================
    public Rarity GetRarityBySelectionMode(SelectionMode selectionMode)
    {
        return selectionMode switch
        {
            SelectionMode.OnlyCommon => Rarity.星1, // 常に星1
            SelectionMode.OnlyRare => Rarity.星2, // 常に星2
            SelectionMode.OnlyLegendary => Rarity.星3, // 常に星3
            SelectionMode.ManyCommon => GetRandomRarityByWeights(70f, 25f, 5f), // 70% 星1, 25% 星2, 5% 星3
            SelectionMode.ManyRare => GetRandomRarityByWeights(30f, 60f, 10f), // 60% 星2, 30% 星1, 10% 星3
            SelectionMode.ManyLegendary => GetRandomRarityByWeights(10f, 50f, 40f), // 50% 星2, 40% 星3, 10% 星1
            _ => Rarity.星1 // デフォルトは星1
        };
    }

    // ==========================
    // それぞれのレアリティの出る確率からランダムにレアリティを返すメソッド
    // ==========================
    private Rarity GetRandomRarityByWeights(float w1, float w2, float w3)
    {
        float total = w1 + w2 + w3;
        float rand = Random.value * total;

        return rand switch
        {
            _ when rand < w1 => Rarity.星1,         // 星一の確率
            _ when rand < w1 + w2 => Rarity.星2,    // 星二の確率
            _ => Rarity.星3                         // 星三の確率
        };
    }
} 
