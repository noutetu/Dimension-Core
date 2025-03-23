using System.Collections.Generic;
using UnityEngine;
// ================================
// 敵のパターンクラス
// ================================
[CreateAssetMenu(fileName = "EnemyPattern", menuName = "ScriptableObjects/EnemyPattern", order = 1)]
public class EnemyPattern : ScriptableObject
{
    [Header("パターン名")]
    public string patternName;

    [Header("敵のパターン")]
    public List<ShapeBaseStats> enemies;
}
