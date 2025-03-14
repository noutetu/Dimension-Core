using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
    [Header ("BGMリスト")]
    [SerializeField] AudioClip mainBGM;
    [SerializeField] AudioClip bossBGM;
    [SerializeField] AudioClip gameOverBGM;
    [SerializeField] AudioClip clearBGM;

    private AudioSource audioSource;
    public static BGMManager Instance { get; private set; }

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
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayMainBGM();
    }
    // ===========================================
    // BGM再生
    // ===========================================
    public void PlayMainBGM()
    {
        audioSource.clip = mainBGM;
        audioSource.Play();
    }
    public void PlayBossBGM()
    {
        audioSource.clip = bossBGM;
        audioSource.Play();
    }
    public void PlayGameOverBGM()
    {
        audioSource.clip = gameOverBGM;
        audioSource.Play();
    }
    public void PlayGameClearBGM()
    {
        audioSource.clip = clearBGM;
        audioSource.Play();
    }
    public void StopBGM()
    {
        audioSource.Stop();
    }

    public void PlayBattleBGM()
    {
        DimensionData dimensionData = DimensionManager.Instance.GetCurrentDimension();
        AudioClip bgm = dimensionData.backgroundMusic;
        audioSource.clip = bgm;
        audioSource.Play();
    }
}
