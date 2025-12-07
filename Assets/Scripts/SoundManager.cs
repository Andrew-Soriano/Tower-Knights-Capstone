using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Sounds
{
    Lightning,
    MainMenu,
    background,
    ArrowLaunch,
    ArrowImpact,
    Explode,
    BombLaunch,
    Bones,
    CatapultLaunch,
    FootstepsMarch,
    Ice

}

[System.Serializable]
public class SoundEntry
{
    public Sounds sound;
    public AudioClip clip;
    public float start;
    public float cutoff;
    [HideInInspector] public AudioSource source;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    private Dictionary<Sounds, SoundEntry> _soundLibrary;
    private List<AudioSource> _sfxPool = new();
    private AudioSource _bgmSource;

    [SerializeField] private List<SoundEntry> soundEntries = new();
    [SerializeField] private int sfxPoolSize = 10;

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else 
        { 
            Destroy(gameObject); ;
            return;
        }

        _bgmSource = gameObject.AddComponent<AudioSource>();
        _bgmSource.loop = true;

        _soundLibrary = new Dictionary<Sounds, SoundEntry>();
        foreach (var entry in soundEntries)
            _soundLibrary[entry.sound] = entry;

        for (int i = 0; i < sfxPoolSize; i++)
        {
            AudioSource sfx = gameObject.AddComponent<AudioSource>();
            _sfxPool.Add(sfx);
        }

        PlayBGM(Sounds.MainMenu);
    }

    public void PlayBGM(Sounds bgm)
    {
        if (!_soundLibrary.TryGetValue(bgm, out var entry))
        {
            Debug.LogWarning($"SoundManager: BGM {bgm} not found!");
            return;
        }

        if (_bgmSource.isPlaying && _bgmSource.clip == entry.clip)
            return;

        _bgmSource.clip = entry.clip;
        _bgmSource.time = entry.start;
        _bgmSource.Play();
    }

    public void StopBGM()
    {
        _bgmSource.Stop();
        _bgmSource.clip = null;
    }

    public void PlaySFX(Sounds sound)
    {
        if (!_soundLibrary.TryGetValue(sound, out var entry))
        {
            Debug.LogWarning($"SoundManager: SFX {sound} not found!");
            return;
        }

        AudioSource available = _sfxPool.Find(s => !s.isPlaying);
        if (available == null)
            return;

        available.clip = entry.clip;
        available.time = entry.start;
        available.Play();

        if (entry.cutoff > 0)
            StartCoroutine(StopAfterSeconds(available, entry.cutoff));
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
    }

    private IEnumerator StopAfterSeconds(AudioSource src, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (src != null) src.Stop();
    }
}
