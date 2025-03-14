using UnityEngine;
using System.Collections.Generic;
using UniRx;
using System.Linq;

public class PlayerShapeManager : MonoBehaviour
{
    public ReactiveCollection<ShapeBaseStats> OwnedShapes { get; private set; } = new ReactiveCollection<ShapeBaseStats>();
    public ReactiveCollection<Shape> activeShapes = new ReactiveCollection<Shape>();

    // ==== 図形の永続強化値 ====
    private float AtkBuffValue = 0; // 攻撃力バフの値
    private float SpdBuffValue = 0; // 速度バフの値
    private float EvasionBuffValue = 0; // 回避バフの値
    private float CritBuffValue = 0; // クリティカルバフの値

    private void Start()
    {
        ObserveOwnedShapes();
    }

    private void ObserveOwnedShapes()
    {
        // 所持図形数が0以下になったらゲームオーバー
        OwnedShapes.ObserveCountChanged()
            .Where(count => count <= 0)
            .Delay(System.TimeSpan.FromSeconds(1.2f))
            .Subscribe(_ => FindAnyObjectByType<GameFlowManager>().GameOver())
            .AddTo(this);
    }

    /// <summary>
    /// 新しい図形を追加する（Shape から）
    /// </summary>
    public void AddShape(ShapeBaseStats BaseStats)
    {
        if (BaseStats == null) return;

        OwnedShapes.Add(BaseStats);
        ShapeDex.Instance.DiscoverShape(BaseStats.ShapeName); // 図鑑に登録

        DebugUtility.Log($"図形 {BaseStats.ShapeName} を追加しました。（現在の所有数: {OwnedShapes.Count}）");
    }

    /// <summary>
    /// 指定した ShapeInstance を削除する
    /// </summary>
    public void RemoveShape(ShapeBaseStats shape)
    {
        OwnedShapes.Remove(shape);
    }

    /// <summary>
    /// 戦闘時に ShapeInstance から Shape を生成
    /// </summary>
    public List<Shape> SpawnShapesForBattle(Transform[] spawnPoints)
    {
        List<Shape> spawnedShapes = new List<Shape>();

        int spawnCount = Mathf.Min(OwnedShapes.Count, spawnPoints.Length); // 生成数を制限
        int index = 0;

        foreach (var instance in OwnedShapes)
        {
            if (index >= spawnCount) break;

            Transform spawnPoint = spawnPoints[index]; // 生成位置を取得
            GameObject shapeObject = Instantiate(instance.ShapePrefab, spawnPoint.position, spawnPoint.rotation);
            Shape shapeComponent = shapeObject.GetComponent<Shape>();

            if (shapeComponent != null)
            {
                shapeComponent.IsEnemy = false; // プレイヤーの図形として設定
                DebugUtility.Log($"Shape {instance.name} を生成しました。");
                // イベントバフを適用
                shapeComponent.Stats.ApplyStatusModifierByEvent
                (
                    0,                  // 体力バフの値
                    AtkBuffValue,       // 攻撃力バフの値
                    SpdBuffValue,       // 速度バフの値
                    CritBuffValue,      // クリティカルバフの値
                    EvasionBuffValue    // 回避バフの値
                );
                shapeComponent.Initialize();
                spawnedShapes.Add(shapeComponent);
                activeShapes.Add(shapeComponent); // アクティブシェイプリストに追加
                // HPが0以下になったらリストから削除
                shapeComponent.Stats.CurrentHP
                    .Where(hp => hp <= 0)
                    .Subscribe(_ =>
                    {
                        OwnedShapes.Remove(instance); // HPが0以下になったら所有リストから削除
                        activeShapes.Remove(shapeComponent); // HPが0以下になったらアクティブリストから削除
                    }).AddTo(shapeComponent);
            }

            index++;
        }

        return spawnedShapes; // 生成された Shape のリストを返す
    }

    /// <summary>
    /// 戦闘終了時にアクティブシェイプリストをクリア
    /// </summary>
    public void ClearActiveShapes()
    {
        Shape[] activeShapesArray = activeShapes.ToArray();

        foreach (Shape shape in activeShapesArray)
        {
            if (shape == null || shape.IsEnemy) continue;

            // Shapeがまだ破壊されていないなら
            if (shape != null)
            {
                activeShapes.Remove(shape); // アクティブシェイプリストから削除
                shape.OnDestroyed();
                Destroy(shape.gameObject);
            }
        }

        activeShapes.Clear(); // アクティブシェイプリストをクリア
    }
}
