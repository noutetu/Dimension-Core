using UnityEngine;
using System.Collections.Generic;

// =====================
//  味方図形の生成を行うクラス
// =====================
public class FriendGenerator : MonoBehaviour
{
    [SerializeField] private Transform[] friendSpawnPoints; // 味方の生成位置リスト

    // =====================
    // 味方を生成
    // =====================
    public List<Shape> GenerateFriends(Player player)
    {
        if (player == null)
        {
            DebugUtility.LogError("Player が null です。");
            return null;
        }

        if (player.shapeManager.OwnedShapes.Count == 0)
        {
            DebugUtility.LogWarning("プレイヤーが図形を所持していません。");
            return null;
        }

        // 図形を生成し、リストに追加
        List<Shape> spawnedShapes = player.SpawnShapesForBattle(friendSpawnPoints);

        return spawnedShapes;
    }
}
