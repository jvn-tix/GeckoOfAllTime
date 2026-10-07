using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("UI Sounds")]
    [SerializeField] private AudioClip clickSfx;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float bgmVolume = 0.5f;
    [SerializeField, Range(0f, 1f)] private float sfxVolume = 1f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        bgmSource.loop = true;
        bgmSource.volume = bgmVolume;
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volumeScale = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume * volumeScale);
    }

    public void PlayClick()
    {
        PlaySFX(clickSfx);
    }
}