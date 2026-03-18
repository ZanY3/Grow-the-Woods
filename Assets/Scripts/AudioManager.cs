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
    [SerializeField] private AudioClip[] musicPlaylist;

    [HideInInspector] public bool canPlaySounds = true;

    private List<int> playedIndexes = new List<int>();

    private float defaultMusicVolume;
    private float defaultSfxVolume;

    private bool lastSoundState;
    private bool overrideMusic = false;
    private bool isFadingOutEnding = false;

    private Tween musicFadeTween;

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
        if (!overrideMusic && !musicSource.isPlaying)
        {
            PlayNextTrack();
        }

        if (canPlaySounds != lastSoundState)
        {
            ApplySoundState();
            lastSoundState = canPlaySounds;
        }

        if (overrideMusic && musicSource.clip != null && musicSource.isPlaying)
        {
            float timeLeft = musicSource.clip.length - musicSource.time;

            if (timeLeft <= 1.5f && !isFadingOutEnding)
            {
                isFadingOutEnding = true;
                FadeMusic(0f, 1.5f);
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

    private void PlayNextTrack()
    {
        if (musicPlaylist == null || musicPlaylist.Length == 0) return;

        if (playedIndexes.Count >= musicPlaylist.Length)
        {
            playedIndexes.Clear();
        }

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

            FadeMusic(0.35f, 1.5f);
        });
    }

    public void StopOverrideMusic()
    {
        overrideMusic = false;
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
