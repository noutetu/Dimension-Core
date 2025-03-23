using UnityEngine;
// ==========================================
// バトル中のエフェクト表示を管理するクラス
// ==========================================
public class ShapeEffectHandler : MonoBehaviour
{
    // パーティクルシステム
    private ParticleSystem particle;
    // ================================
    // 初期化
    // ================================
    private void Awake()
    {
        // パーティクルシステムを取得
        particle = GetComponent<ParticleSystem>();
    }
    // ================================
    // エフェクト再生
    // ================================
    public void PlayEffect(Vector2 position)
    {
        transform.position = position; // 位置を設定
        particle.Play(); // パーティクルを再生
    }
}
