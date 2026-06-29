using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Music Playlist")]
    [SerializeField] private AudioClip[] music1Playlist;
    [SerializeField] private AudioClip[] music2Playlist;

    [HideInInspector] public bool canPlaySounds = true;

    private List<int> playedIndexes = new List<int>();
    private AudioClip[] lastPlaylist;
    private float defaultMusicVolume;
    private float defaultSfxVolume;
    private bool lastSoundState;
    private bool overrideMusic = false;
    private bool isFadingOutEnding = false;
    private Tween musicFadeTween;
    private bool isTrackPlaying = false;
    private bool isMusicPaused = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        defaultMusicVolume = musicSource.volume;
        defaultSfxVolume = sfxSource.volume;
        lastSoundState = canPlaySounds;
        PlayNextTrack();
    }

    private void Update()
    {
        if (!overrideMusic && isTrackPlaying && !musicSource.isPlaying && !isMusicPaused && !musicFadeTween.IsActive())
        {
            PlayNextTrack();
        }

        if (canPlaySounds != lastSoundState)
        {
            ApplySoundState();
            lastSoundState = canPlaySounds;
        }

        if (overrideMusic && musicSource.clip != null && isTrackPlaying && !isMusicPaused)
        {
            float timeLeft = musicSource.clip.length - musicSource.time;
            if (timeLeft <= 1.5f && !isFadingOutEnding)
            {
                isFadingOutEnding = true;
                FadeMusic(0f, 1.5f);
            }
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            if (musicSource.isPlaying)
            {
                musicSource.Pause();
                isMusicPaused = true;
            }
        }
        else
        {
            if (isMusicPaused)
            {
                musicSource.UnPause();
                isMusicPaused = false;
            }
        }
    }

    private void OnApplicationPause(bool isPaused)
    {
        if (isPaused)
        {
            if (musicSource.isPlaying)
            {
                musicSource.Pause();
                isMusicPaused = true;
            }
        }
        else
        {
            if (isMusicPaused)
            {
                musicSource.UnPause();
                isMusicPaused = false;
            }
        }
    }

    private void ApplySoundState()
    {
        if (!canPlaySounds)
        {
            FadeMusic(0f, 0.5f);
            sfxSource.volume = 0f;
        }
        else
        {
            FadeMusic(defaultMusicVolume, 0.5f);
            sfxSource.volume = defaultSfxVolume;
        }
    }

    public void PlayNextTrack()
    {
        AudioClip[] musicPlaylist = null;

        if (PrestigeManager.Instance.currentRegion == 0)
            musicPlaylist = music1Playlist;
        else if (PrestigeManager.Instance.currentRegion == 1)
            musicPlaylist = music2Playlist;
        else
            musicPlaylist = music1Playlist;

        if (musicPlaylist == null || musicPlaylist.Length == 0) return;

        // Reset play history whenever we switch to a different playlist
        // (fixes region 2 OST not playing, since old indexes from the
        // previous playlist were blocking valid picks in the new one)
        if (musicPlaylist != lastPlaylist)
        {
            playedIndexes.Clear();
            lastPlaylist = musicPlaylist;
        }

        if (playedIndexes.Count >= musicPlaylist.Length)
            playedIndexes.Clear();

        int newIndex;
        do
        {
            newIndex = Random.Range(0, musicPlaylist.Length);
        }
        while (playedIndexes.Contains(newIndex));

        playedIndexes.Add(newIndex);
        musicSource.clip = musicPlaylist[newIndex];
        musicSource.loop = false;
        musicSource.volume = 0f;
        musicSource.Play();
        isTrackPlaying = true;
        FadeMusic(defaultMusicVolume, 1f);
    }

    public void PlayOverrideMusic(AudioClip clip)
    {
        if (clip == null) return;
        overrideMusic = true;
        isFadingOutEnding = false;
        FadeMusic(0f, 1f, () =>
        {
            musicSource.clip = clip;
            musicSource.loop = false;
            musicSource.volume = 0f;
            musicSource.Play();
            isTrackPlaying = true;
            FadeMusic(0.35f, 1.5f);
        });
    }

    public void StopOverrideMusic()
    {
        overrideMusic = false;
        isFadingOutEnding = false;
        isTrackPlaying = false;
        PlayNextTrack();
    }

    private void FadeMusic(float targetVolume, float duration, TweenCallback onComplete = null)
    {
        musicFadeTween?.Kill();
        musicFadeTween = musicSource
            .DOFade(targetVolume, duration)
            .SetEase(Ease.InOutSine)
            .OnComplete(onComplete);
    }

    public void PlaySfxSound(AudioClip clip, float volume = 1f, float minPitch = 0.95f, float maxPitch = 1.05f)
    {
        if (clip == null || !canPlaySounds) return;
        float randomPitch = Random.Range(minPitch, maxPitch);
        sfxSource.pitch = randomPitch;
        sfxSource.PlayOneShot(clip, volume);
        sfxSource.pitch = 1f;
    }
}