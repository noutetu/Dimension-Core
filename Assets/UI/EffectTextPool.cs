using UnityEngine;
using System.Collections.Generic;

public class EffectTextPool : MonoBehaviour
{
    // シングルトンインスタンス
    public static EffectTextPool Instance { get; private set; }

    // エフェクトテキストのプレハブ
    [SerializeField] public EffectText effectTextPrefab;
    // 初期プールサイズ
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
            EffectText effectText = Instantiate(effectTextPrefab, transform);
            effectText.gameObject.SetActive(false);
            pool.Enqueue(effectText);
        }
    }

    // ==========================
    // プールから取得
    // ==========================
    public EffectText GetFromPool()
    {
        EffectText effectText = pool.Count > 0 
        // カウントが 0 より大きい場合は、プールから取得
        ? pool.Dequeue()
        // そうでない場合は、新しく生成
        : Instantiate(effectTextPrefab, transform);

        effectText.gameObject.SetActive(true);
        return effectText;
    }

    // ==========================
    // プールに戻す
    // ==========================
    public void ReturnToPool(EffectText effectText)
    {
        effectText.ResetEffectText();  // 位置や透明度をリセット
        effectText.gameObject.SetActive(false);  // 非表示にする
        pool.Enqueue(effectText);  // プールへ追加
    }
}
