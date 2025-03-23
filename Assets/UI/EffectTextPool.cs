using UnityEngine;
using System.Collections.Generic;
// ==========================
// エフェクトテキストプール
// ==========================
public class EffectTextPool : MonoBehaviour
{
    // シングルトンインスタンス
    public static EffectTextPool Instance { get; private set; }

    [Header("エフェクトテキストのプレハブ")]
    [SerializeField] public EffectText effectTextPrefab;
    [Header(" 初期プールサイズ")]
    [SerializeField] private int initialPoolSize = 10;

    // エフェクトテキストのプール
    private Queue<EffectText> pool = new Queue<EffectText>();

    // ==========================
    // 初期化
    // ==========================
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            // プールの初期化
            InitializePool();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // ==========================
    // プールの初期化
    // ==========================
    private void InitializePool()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            // エフェクトテキストのインスタンスを生成
            EffectText effectText = Instantiate(effectTextPrefab, transform);
            // 非表示にする
            effectText.gameObject.SetActive(false);
            // プールに追加
            pool.Enqueue(effectText);
        }
    }

    // ==========================
    // プールから取得
    // ==========================
    public EffectText GetFromPool()
    {
        // プールにオブジェクトがある場合
        if (pool.Count > 0)
        {
            // プールから取得
            EffectText effectText = pool.Dequeue();
            // 表示する
            effectText.gameObject.SetActive(true);
            // オブジェクトを返す
            return effectText;
        }
        // プールサイズを超えていない場合
        else if (pool.Count + 1 <= initialPoolSize)
        {
            // 新しく生成
            EffectText effectText = Instantiate(effectTextPrefab, transform);
            // 表示する
            effectText.gameObject.SetActive(true);
            // オブジェクトを返す
            return effectText;
        }
        else
        {
            return null; // プールサイズを超えた場合は null を返す
        }
    }

    // ==========================
    // プールに戻す
    // ==========================
    public void ReturnToPool(EffectText effectText)
    {
        // 位置や透明度をリセット
        effectText.ResetEffectText();  
        // 非表示にする
        effectText.gameObject.SetActive(false);  
        // プールへ追加
        pool.Enqueue(effectText);  
    }
}
