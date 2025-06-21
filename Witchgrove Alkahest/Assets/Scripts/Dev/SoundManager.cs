using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unified Sound Manager allowing playback by name.
/// Supports one background music AudioSource and pooled SFX AudioSources.
/// Assign sound entries in inspector, then call PlaySound("name").
/// </summary>
public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [System.Serializable]
    public class SoundEntry
    {
        [Tooltip("Unique identifier for this sound")]
        public string name;
        [Tooltip("Audio clip to play")]     
        public AudioClip clip;
        [Tooltip("Volume (0-1)")]
        [Range(0f,1f)] public float volume = 1f;
    }

    [Header("Sound Library")]
    [Tooltip("List of named sounds")]
    [SerializeField] private SoundEntry[] soundEntries;
    
    [Header("BG Music library")]
    [Tooltip("List of bg music files")]
    [SerializeField] private SoundEntry[] bgMusicEntries;

    [Header("Music Settings")]
    [Tooltip("AudioSource for background music")]    
    [SerializeField] private AudioSource musicSource;

    [Header("SFX Pool Settings")]
    [Tooltip("Initial number of pooled SFX AudioSources")]
    [SerializeField] private int initialSfxPoolSize = 10;
    private List<AudioSource> sfxPool;

    // Internal  sound lookup dictionary
    private Dictionary<string, SoundEntry> soundDict;
    // Internal  bg music lookup dictionary
    private Dictionary<string, SoundEntry> bgMusicDict;

    void Awake()
    {
        // Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Build sound lookup
        soundDict = new Dictionary<string, SoundEntry>(soundEntries.Length);
        foreach (var entry in soundEntries)
        {
            if (entry == null || string.IsNullOrEmpty(entry.name) || entry.clip == null)
                continue;
            soundDict[entry.name] = entry;
        }
        // Build bg music lookup
        bgMusicDict = new Dictionary<string, SoundEntry>(bgMusicEntries.Length);
        foreach (var entry in bgMusicEntries)
        {
            if (entry == null || string.IsNullOrEmpty(entry.name) || entry.clip == null)
                continue;
            bgMusicDict[entry.name] = entry;
        }

        // Initialize SFX pool
        sfxPool = new List<AudioSource>(initialSfxPoolSize);
        for (int i = 0; i < initialSfxPoolSize; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            sfxPool.Add(src);
        }
    }

    private void Start()
    {
        PlayMusic("MeadowLvl", true);
    }

    /// <summary>
    /// Play a named sound as one-shot SFX.
    /// </summary>
    public void PlaySound(string soundName)
    {
        if (!soundDict.TryGetValue(soundName, out var entry))
        {
            Debug.LogWarning($"Sound '{soundName}' not found in SoundManager library.");
            return;
        }
        var src = sfxPool.Find(s => !s.isPlaying) ?? ExpandSfxPool(1)[0];
        src.volume = entry.volume;
        src.PlayOneShot(entry.clip);
    }

    /// <summary>
    /// Play background music clip.
    /// </summary>
    public void PlayMusic(string soundName, bool loop = true)
    {
        if (musicSource == null)
            return;
        if (!bgMusicDict.TryGetValue(soundName, out var entry))
        {
            Debug.LogWarning($"Music '{soundName}' not found in SoundManager library.");
            return;
        }
        musicSource.clip = entry.clip;
        musicSource.volume = entry.volume;
        musicSource.loop = loop;
        musicSource.Play();
    }

    /// <summary>
    /// Stop background music immediately.
    /// </summary>
    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    /// <summary>
    /// Expands the SFX pool by count and returns new sources.
    /// </summary>
    private List<AudioSource> ExpandSfxPool(int count)
    {
        var newList = new List<AudioSource>(count);
        for (int i = 0; i < count; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.loop = false;
            sfxPool.Add(src);
            newList.Add(src);
        }
        return newList;
    }
}
