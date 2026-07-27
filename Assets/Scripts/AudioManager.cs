using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Background Music Clips")]
    public AudioClip mainMenuBGM;
    public AudioClip inGameBGM;

    [Header("Sound Effects Clips")]
    public AudioClip jumpSFX;
    public AudioClip dashSFX;
    public AudioClip wallSlideSFX;
    public AudioClip checkpointSFX;
    public AudioClip finishSFX;
    public AudioClip hurtSFX;
    public AudioClip dieSFX;
    public AudioClip buttonClickSFX;

    private void Awake()
    {
        // Singleton pattern: Memastikan hanya ada 1 AudioManager dan tidak hancur saat ganti scene
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Auto create AudioSource jika belum dipasang di Inspector
        if (bgmSource == null) bgmSource = gameObject.AddComponent<AudioSource>();
        if (sfxSource == null) sfxSource = gameObject.AddComponent<AudioSource>();

        bgmSource.loop = true;
    }

    private void Start()
    {
        PlayMainMenuBGM();
    }

    // --- BGM CONTROLS ---

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null) return;
        if (bgmSource.clip == clip && bgmSource.isPlaying) return;

        bgmSource.Stop();
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlayMainMenuBGM()
    {
        PlayBGM(mainMenuBGM);
    }

    public void PlayInGameBGM()
    {
        PlayBGM(inGameBGM);
    }

    public void StopBGM()
    {
        bgmSource.Stop();
    }

    // --- SFX CONTROLS ---

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayJumpSFX() => PlaySFX(jumpSFX);
    public void PlayDashSFX() => PlaySFX(dashSFX);
    public void PlayWallSlideSFX() => PlaySFX(wallSlideSFX);
    public void PlayCheckpointSFX() => PlaySFX(checkpointSFX);
    public void PlayFinishSFX() => PlaySFX(finishSFX);
    public void PlayHurtSFX() => PlaySFX(hurtSFX);
    public void PlayDieSFX() => PlaySFX(dieSFX);
    public void PlayButtonClickSFX() => PlaySFX(buttonClickSFX);
}
