using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    private float _sfxVolume=1.0f;
    private float _musicVolume=1.0f;
    [Serializable]
    public class SFX
    {
        public string name;
        public AudioClip clip;
        [Range(0,1)] public float volume=1.0f;
    }
    [Serializable]
    public class Music
    {
        public string name;
        public AudioClip clip;
        [Range(0, 1)] public float volume = 1.0f;
    }

    [SerializeField] private SFX[] _soundEffects;
    [SerializeField] private Music[] _musics;
    [SerializeField] private int _poolSize = 5;
    private List<AudioSource> _sources = new List<AudioSource>();
    Queue<AudioSource> _usedSource=new Queue<AudioSource>();
    AudioSource _musicSource;
    private Dictionary<string, AudioClip> _sfxDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> _musicDictionary = new Dictionary<string, AudioClip>();
    private float _volume = 1.0f;

    public static SoundManager instance { get; private set; }
    public float SfxVolume { get => _sfxVolume; set
        {
            if (value > 1.0f) value = 1.0f;
            else if (value < .0f) value = .0f;
            _sfxVolume = value;
        }
    }
    public float MusicVolume { get => _musicVolume; set
        {
            if(value>1.0f) value=1.0f;
            else if(value<.0f) value=.0f;
            _musicVolume = value;
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }
        for (int i = 0; i < _poolSize; i++)
        {
            AudioSource audioAource = gameObject.AddComponent<AudioSource>();
            _sources.Add(audioAource);
        }
        DontDestroyOnLoad(this.gameObject);
        foreach (SFX sfx in _soundEffects)
        {
            _sfxDictionary[sfx.name] = sfx.clip;
        }
        foreach (Music music in _musics)
        {
            _sfxDictionary[music.name] = music.clip;
        }
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = true ;
    }
    private AudioSource GetAvailableSource()
    {
        foreach (AudioSource source in _sources)
        {
            if (!source.isPlaying)
            {
                if (_usedSource.Contains(source))
                {
                    RemoveSource(source);
                }
                _usedSource.Enqueue(source);
                return source;
            }
        }
        AudioSource returnSource = _usedSource.Dequeue();
        _usedSource.Enqueue(returnSource);
        return returnSource;
    }
    public void RemoveSource(AudioSource source)
    {
        _usedSource = new Queue<AudioSource>(_usedSource.Where(p => p != source));
        return;
    }
    public void  PlaySound(string soundName)
    {
        if (!_sfxDictionary.TryGetValue(soundName, out AudioClip clip))
        {
            Debug.LogWarning($"pas de  son au nom : '{soundName} trouvé !");
            return;
        }
        AudioSource audioSource = GetAvailableSource();
        if (audioSource == null) return;
        float soudVolume = 1*_sfxVolume;
        foreach (SFX sound in _soundEffects) 
        {
            if (sound.name == soundName)
            {
                soudVolume = sound.volume;
                break;
            }
        }
        //float finalVolume = soundVolume * global sfx Volume; à faire dans un truc d'option i guess;
        audioSource.clip = clip;
        audioSource.volume = soudVolume;
        audioSource.Play();
    }
    public void StopSound(string soundName)
    {
        foreach(AudioSource source in _usedSource)
        {
            if(source.name == soundName)
            {
                source.Stop();
                RemoveSource(source);
            }
        }
    }

    public void PlayMusic(string name)
    {

        if (!_musicDictionary.TryGetValue(name, out AudioClip clip))
        {
            Debug.LogWarning($"pas de  music au nom : '{name} trouvé !");
            return;
        }
        float soudVolume = 1*_musicVolume;
        foreach (Music music in _musics)
        {
            if (music.name == name)
            {
                soudVolume = music.volume;
                break;
            }
        }
        //float finalVolume = soundVolume * global sfx Volume; à faire dans un truc d'option i guess;
        _musicSource.clip = clip;
        _musicSource.volume = soudVolume;
        _musicSource.Play();
    }
    
}
