using UnityEngine;
using System.Collections.Generic;
using UniRx;
using System.Linq;

// =====================
// プレイヤーの図形管理クラス
// =====================
public class PlayerShapeManager : MonoBehaviour
{
    // 所持図形リスト
    public ReactiveCollection<ShapeBaseStats> OwnedShapes { get; private set; } = new ReactiveCollection<ShapeBaseStats>();
    // アクティブ図形リスト
    public ReactiveCollection<Shape> activeShapes = new ReactiveCollection<Shape>();

    // ==== 図形の永続強化値 ====
    private float AtkBuffValue = 0; // 攻撃力バフの値
    private float SpdBuffValue = 0; // 速度バフの値
    private float EvasionBuffValue = 0; // 回避バフの値
    private float CritBuffValue = 0; // クリティカルバフの値
    // =====================
    // 初期化
    // =====================
    private void Start()
    {
        // 所持図形の監視
        ObserveOwnedShapes();
    }

    // =====================
    // 所持図形の監視
    // =====================
    private void ObserveOwnedShapes()
    {
        // 所持図形数が0以下になったらゲームオーバー
        OwnedShapes.ObserveCountChanged()
            .Where(count => count <= 0)
            .Delay(System.TimeSpan.FromSeconds(1.2f))
            .Subscribe(_ => FindAnyObjectByType<GameFlowManager>().GameOver())
            .AddTo(this);
    }


    // =====================
    /// 新しい図形を追加する（Shape から）
    // =====================
    public void AddShape(ShapeBaseStats BaseStats)
    {
        if (BaseStats == null) return;
        // 所持図形リストに追加
        OwnedShapes.Add(BaseStats);
        // 図鑑に登録
        ShapeDex.Instance.DiscoverShape(BaseStats.ShapeName); 
        
        DebugUtility.Log($"図形 {BaseStats.ShapeName} を追加しました。（現在の所有数: {OwnedShapes.Count}）");
    }

    // =====================
    /// 指定した図形を削除する
    // =====================
    public void RemoveShape(ShapeBaseStats shape)
    {
        OwnedShapes.Remove(shape);
    }

    // =====================
    /// 戦闘時に図形を生成
    // =====================
    public List<Shape> SpawnShapesForBattle(Transform[] spawnPoints)
    {
        // 生成した図形リスト
        List<Shape> spawnedShapes = new List<Shape>();
        // 生成数を制限
        int spawnCount = Mathf.Min(OwnedShapes.Count, spawnPoints.Length); 
        // 所持図形リストから図形を生成
        int index = 0;
        foreach (var instance in OwnedShapes)
        {
            if (index >= spawnCount) break;
            // 生成位置を取得
            Transform spawnPoint = spawnPoints[index];
            // 図形を生成
            GameObject shapeObject = Instantiate(instance.ShapePrefab, spawnPoint.position, spawnPoint.rotation);
            // 図形コンポーネントを取得
            Shape shapeComponent = shapeObject.GetComponent<Shape>();
            // 図形コンポーネントがあれば
            if (shapeComponent != null)
            {
                // プレイヤーの図形として設定
                shapeComponent.IsEnemy = false;
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
                // 初期化
                shapeComponent.Initialize();
                // リストに追加
                spawnedShapes.Add(shapeComponent);
                activeShapes.Add(shapeComponent);
                // HPが0以下になった時の処理
                shapeComponent.Stats.CurrentHP
                    .Where(hp => hp <= 0)
                    .Subscribe(_ =>
                    {
                        // 所有リストから削除
                        OwnedShapes.Remove(instance);
                        // アクティブリストから削除
                        activeShapes.Remove(shapeComponent);
                    }).AddTo(shapeComponent);
            }

            index++;
        }

        return spawnedShapes;
    }

    // =====================
    /// 戦闘終了時にアクティブシェイプリストをクリア
    // =====================
    public void ClearActiveShapes()
    {
        // アクティブシェイプリストを配列に変換
        Shape[] activeShapesArray = activeShapes.ToArray();
        // アクティブシェイプリストをクリア
        foreach (Shape shape in activeShapesArray)
        {
            if (shape == null || shape.IsEnemy) continue;

            // Shapeがまだ破壊されていないなら
            if (shape != null)
            {
                // アクティブシェイプリストから削除
                activeShapes.Remove(shape);
                // 破壊処理
                shape.OnDestroyed();
                Destroy(shape.gameObject);
            }
        }
        // アクティブシェイプリストをクリア
        activeShapes.Clear();
    }
}
