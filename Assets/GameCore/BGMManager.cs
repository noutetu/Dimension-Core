using UnityEngine;
// ===========================================
// BGM管理クラス
// ===========================================
public class BGMManager : MonoBehaviour
{
    [Header ("BGMリスト")]
    [SerializeField] AudioClip mainBGM;
    [SerializeField] AudioClip bossBGM;
    [SerializeField] AudioClip gameOverBGM;
    [SerializeField] AudioClip clearBGM;

    private AudioSource audioSource;// オーディオソース

    public static BGMManager Instance { get; private set; } // シングルトン

    // ===========================================
    // 初期化
    // ===========================================
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    // ===========================================
    // 初期化
    // ===========================================
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayMainBGM();
    }
    // ===========================================
    // メインBGM再生    
    // ===========================================
    public void PlayMainBGM()
    {
        audioSource.clip = mainBGM;
        audioSource.Play();
    }
    // ===========================================
    // ボスBGM再生
    // ===========================================
    public void PlayBossBGM()
    {
        audioSource.clip = bossBGM;
        audioSource.Play();
    }
    // ===========================================
    // ゲームオーバーBGM再生
    // ===========================================
    public void PlayGameOverBGM()
    {
        audioSource.clip = gameOverBGM;
        audioSource.Play();
    }
    // ===========================================
    // クリアBGM再生
    // ===========================================
    public void PlayGameClearBGM()
    {
        audioSource.clip = clearBGM;
        audioSource.Play();
    }
    // ===========================================
    // BGM停止
    // ===========================================
    public void StopBGM()
    {
        audioSource.Stop();
    }
    // ===========================================
    // バトルBGM再生
    // ===========================================
    public void PlayBattleBGM()
    {   // 現在の次元を取得
        DimensionData dimensionData = DimensionManager.Instance.GetCurrentDimension();
        // 次元に応じたBGMを再生
        AudioClip bgm = dimensionData.backgroundMusic;
        audioSource.clip = bgm;
        audioSource.Play();
    }
}
