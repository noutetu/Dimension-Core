using UnityEngine;
using System.Collections.Generic;

// ==========================
/// 次元の基本情報を保持するクラス
// ==========================
[CreateAssetMenu(fileName = "NewDimension", menuName = "Game/Dimension Data")]
public class DimensionData : ScriptableObject
{
    [Header("次元の基本情報")]
    public string dimensionName;
    // この次元が出現するのに必要なステージ数
    public int requiredStage;

    [Header("出現しやすい図形リスト")]
    public List<ShapeBaseStats> possibleShapes;

    [Header("環境エフェクト")]
    public AudioClip backgroundMusic;

    // ==========================
    /// 次元が現在の難易度で出現可能かを判定
     // ==========================
    public bool CanAppear()
    {
        bool canAppear = StageManager.Instance.GetCurrentStage() == requiredStage;
        return canAppear;
    }
}
