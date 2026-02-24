using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioClip arenaMusic;
    public float musicVolume = 0.4f;

    [Header("Player SFX")]
    public AudioClip swingSound;
    public AudioClip hitSound;
    public AudioClip blockSound;
    public AudioClip perfectParrySound;
    public AudioClip syncBurstSound;
    public AudioClip flipSound;
    public AudioClip deathSound;

    [Header("Enemy SFX")]
    public AudioClip enemyHitSound;
    public AudioClip enemyDeathSound;
    public AudioClip rushersGrowl;

    [Header("UI SFX")]
    public AudioClip waveStartSound;
    public AudioClip waveClearSound;
    public AudioClip upgradeSelectSound;
    public AudioClip marketBuySound;

    private AudioSource _musicSource;
    private AudioSource _sfxSource;

    void Awake()
    {
        Instance = this;
        AudioSource[] sources = GetComponents<AudioSource>();
        if (sources.Length >= 2)
        {
            _musicSource = sources[0];
            _sfxSource = sources[1];
        }
        else
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
            _sfxSource = gameObject.AddComponent<AudioSource>();
        }

        _musicSource.loop = true;
        _musicSource.volume = musicVolume;
        _musicSource.playOnAwake = false;
    }

    void Start()
    {
        if (arenaMusic != null)
        {
            _musicSource.clip = arenaMusic;
            _musicSource.Play();
        }
    }

    public void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        _sfxSource.PlayOneShot(clip, volume);
    }

    public void PlayFlip() => Play(flipSound, 0.7f);
    public void PlaySwing() => Play(swingSound, 0.6f);
    public void PlayHit() => Play(hitSound, 1f);
    public void PlayBlock() => Play(blockSound, 0.8f);
    public void PlayPerfectParry() => Play(perfectParrySound, 1f);
    public void PlaySyncBurst() => Play(syncBurstSound, 1f);
    public void PlayEnemyHit() => Play(enemyHitSound, 0.7f);
    public void PlayEnemyDeath() => Play(enemyDeathSound, 0.8f);
    public void PlayWaveStart() => Play(waveStartSound, 0.9f);
    public void PlayWaveClear() => Play(waveClearSound, 0.9f);
    public void PlayUpgradeSelect() => Play(upgradeSelectSound, 0.8f);
    public void PlayMarketBuy() => Play(marketBuySound, 0.8f);
    public void PlayDeath() => Play(deathSound, 1f);
}