using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NewDimension", menuName = "Game/Dimension Data")]
public class DimensionData : ScriptableObject
{
    [Header("次元の基本情報")]
    public string dimensionName; // 次元の名前
    public int requiredStage; // この次元が出現するための必要難易度

    [Header("出現しやすい図形リスト")]
    public List<ShapeBaseStats> possibleShapes; // ここに登場する図形のリスト

    [Header("環境エフェクト")]
    public Sprite backgroundSprite; // 次元の背景（オプション）
    public AudioClip backgroundMusic; // 次元のBGM（オプション）

    /// <summary>
    /// 次元が現在の難易度で出現可能かを判定
    /// </summary>
    public bool CanAppear()
    {
        bool canApper = StageManager.Instance.GetCurrentStage() == requiredStage;
        return canApper;
    }
}
