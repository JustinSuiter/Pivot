using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Music")]
    public AudioClip[] musicTracks;
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
    private int _lastTrackIndex = -1;

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

        _musicSource.loop = false;
        _musicSource.playOnAwake = false;

        _musicSource.volume = PlayerPrefs.GetFloat("MusicVolume", 0.4f);
        _sfxSource.volume = PlayerPrefs.GetFloat("SFXVolume", 1f);
    }

    void Start()
    {
        PlayRandomTrack();
    }

    void Update()
    {
        if (!_musicSource.isPlaying)
            PlayRandomTrack();
    }

    void PlayRandomTrack()
    {
        if (musicTracks == null || musicTracks.Length == 0) return;

        int index;
        do
        {
            index = Random.Range(0, musicTracks.Length);
        }
        while (index == _lastTrackIndex && musicTracks.Length > 1);

        _lastTrackIndex = index;
        _musicSource.clip = musicTracks[index];
        _musicSource.Play();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        if (_musicSource != null)
            _musicSource.volume = volume;
        PlayerPrefs.SetFloat("MusicVolume", volume);
        PlayerPrefs.Save();
    }

    public void SetSFXVolume(float volume)
    {
        if (_sfxSource != null)
            _sfxSource.volume = volume;
        PlayerPrefs.SetFloat("SFXVolume", volume);
        PlayerPrefs.Save();
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