using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyPattern", menuName = "ScriptableObjects/EnemyPattern", order = 1)]
public class EnemyPattern : ScriptableObject
{
    [Header("パターン名")]
    public string patternName;

    [Header("敵のパターン")]
    public List<ShapeBaseStats> enemies;
}
