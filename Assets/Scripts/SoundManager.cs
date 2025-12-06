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
    FootstepsKnight,
    Ice

}

[System.Serializable]
public class SoundEntry
{
    public Sounds sound;
    public AudioSource source;
    public float start;
    public float cutoff;
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; [SerializeField]
    private List<SoundEntry> soundEntries = new List<SoundEntry>();

    private Dictionary<Sounds, SoundEntry> _soundLibrary;

    private void Awake()
    {
        if (instance == null) { instance = this; }
        else { Destroy(gameObject); }

        _soundLibrary = new Dictionary<Sounds, SoundEntry>();
        foreach (var entry in soundEntries)
            _soundLibrary[entry.sound] = entry;
    }

    public void PlaySound(Sounds sound)
    {
        var values = _soundLibrary[sound];
        var source = values.source;
        source.time = values.start;
        StartCoroutine(StopAfterSeconds(values.source, values.cutoff));
    }
    public void PlaySound(Sounds sound, int nTimes)
    {
        var values = _soundLibrary[sound];
        var source = values.source;
        source.time = values.start;
        StartCoroutine(StopAfterSeconds(values.source, values.cutoff));
    }

    public void StartBGM()
    {
        var entry = _soundLibrary[Sounds.background];
        entry.source.Play();
    }

    public IEnumerator PlayNTimes(Sounds sound, int n)
    {
        for (int i = 0; i < n; i++)
        {
            PlaySound(sound);
            yield return new WaitForSeconds(_soundLibrary[sound].cutoff == 0 ? _soundLibrary[sound].source.clip.length : _soundLibrary[sound].cutoff );
        }
    }

    private IEnumerator StopAfterSeconds(AudioSource src, float dur)
    {
        yield return new WaitForSeconds(dur);
        src.Stop();
    }
}
