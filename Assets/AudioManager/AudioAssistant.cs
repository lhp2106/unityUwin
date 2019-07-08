using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(AudioListener))]
[RequireComponent(typeof(AudioSource))]
public class AudioAssistant : MonoBehaviour
{
    #region Sinleton

    private static AudioAssistant instance;

    public static AudioAssistant Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<AudioAssistant>();
            }
            return instance;
        }
    }

    #endregion

    public AudioSource music;
    public AudioSource sfx;

    public float musicVolume
    {
        get
        {
            if (!PlayerPrefs.HasKey("Music Volume"))
                return 0.5f;
            return PlayerPrefs.GetFloat("Music Volume");
        }
        set
        {
            PlayerPrefs.SetFloat("Music Volume", value);
        }
    }

    public float sfxVolume
    {
        get
        {
            if (!PlayerPrefs.HasKey("SFX Volume"))
                return 1f;
            return PlayerPrefs.GetFloat("SFX Volume");
        }
        set
        {
            PlayerPrefs.SetFloat("SFX Volume", value);
        }
    }

    private const string SETTING_SOUND_KEY = "setting_sound";

    public List<MusicTrack> tracks = new List<MusicTrack>();
    public List<Sound> sounds = new List<Sound>();

    static List<string> mixBuffer = new List<string>();
    static float mixBufferClearDelay = 0.05f;

    public bool isMuteMusic = false;
    public bool isMuteSound = false;

    public bool quiet_mode = false;
    public AudioClip currentTrack;

    // setting
    public SettingSound settingSound;
    public Dictionary<int, SettingSoundItem> dictSettingSound;

    private float MUSIC_VOLUME_MAX = 1;

    #region Implement Unity

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);

        Init();

    }

    private void OnDestroy()
    {
        SaveSettingSound();
    }

    public void Init()
    {
        AudioSource[] sources = GetComponents<AudioSource>();
        music = sources[0];
        music.loop = true;
        sfx = sources[1];

        // Initialize
        sfxVolume = sfxVolume;
        musicVolume = musicVolume;

        ChangeMusicVolume(musicVolume);
        ChangeSFXVolume(sfxVolume);

        StartCoroutine(MixBufferRoutine());

        //PlayerPrefs.DeleteKey("MuteMusic");
        //PlayerPrefs.DeleteKey("MuteSound");

        isMuteMusic = PlayerPrefs.GetInt("MuteMusic") == 1;
        isMuteSound = PlayerPrefs.GetInt("MuteSound") == 1;

        if (PlayerPrefs.HasKey(SETTING_SOUND_KEY))
        {
            string data = PlayerPrefs.GetString(SETTING_SOUND_KEY);
            try
            {
                settingSound = JsonUtility.FromJson<SettingSound>(data);
            }
            catch
            {
                settingSound = new SettingSound
                {
                    setting = new List<SettingSoundItem>()
                };
                SaveSettingSound();
            }
        }
        else
        {
            settingSound = new SettingSound
            {
                setting = new List<SettingSoundItem>()
            };
            SaveSettingSound();
        }
        dictSettingSound = settingSound.GetDictionary();
    }

    #endregion

    private Sound GetSoundByName(string name)
    {
        return sounds.Find(x => x.name == name);
    }

    // Coroutine responsible for limiting the frequency of playing sounds
    private IEnumerator MixBufferRoutine()
    {
        float time = 0;

        while (true)
        {
            time += Time.unscaledDeltaTime;
            yield return 0;
            if (time >= mixBufferClearDelay)
            {
                mixBuffer.Clear();
                time = 0;
            }
        }
    }

    // Launching a music track
    public void PlayMusic(string trackName)
    {
        AudioClip musicBG = null;

        foreach (MusicTrack track in tracks)
            if (track.name == trackName)
                musicBG = track.track;

        VKDebug.LogColorRed(musicBG.name);

        StartCoroutine(instance.CrossFade(musicBG));
    }

    // Launching a music track
    public void PlayMusic(AudioClip track)
    {
        StartCoroutine(instance.CrossFade(track));
    }

    // A smooth transition from one to another music
    private IEnumerator CrossFade(AudioClip to, float delay = .3f)
    {
        if (to != null)
        {
            currentTrack = to;
        }

        if (music.clip != null)
        {
            while (delay > 0)
            {
                music.volume = delay * musicVolume * MUSIC_VOLUME_MAX;
                delay -= Time.unscaledDeltaTime;
                yield return 0;
            }
        }
        music.clip = to;
        if (to == null || isMuteMusic)
        {
            music.Stop();
            yield break;
        }
        delay = 0;
        if (!music.isPlaying) music.Play();
        while (delay < 0.3f)
        {
            music.volume = delay * musicVolume * MUSIC_VOLUME_MAX;
            delay += Time.unscaledDeltaTime;
            yield return 0;
        }
        music.volume = musicVolume * MUSIC_VOLUME_MAX;
    }

    // A single sound effect
    public void Shot(string clip)
    {
        Sound sound = instance.GetSoundByName(clip);

        if (isMuteSound)
        {
            return;
        }

        if (sound != null && !mixBuffer.Contains(clip))
        {
            if (sound.clips.Count == 0) return;
            mixBuffer.Add(clip);

            //int random = Random.Range(0, sound.name);
            VKDebug.LogColorRed("Name sound play", sound.name);
            sfx.PlayOneShot(sound.clips[0]);
        }
    }

    public void Shot(AudioClip clip)
    {
        if (isMuteSound)
        {
            return;
        }

        if (clip != null)
        {
            sfx.PlayOneShot(clip);
        }
    }

    // Turn on/off music
    public void MuteMusic()
    {
        isMuteMusic = !isMuteMusic;
        PlayerPrefs.SetInt("MuteMusic", isMuteMusic ? 1 : 0);
        PlayerPrefs.Save();
        PlayMusic(isMuteMusic ? null : currentTrack);
    }

    public void MuteSound()
    {
        isMuteSound = !isMuteSound;
        PlayerPrefs.SetInt("MuteSound", isMuteSound ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void MuteMusic(bool isMute)
    {
        isMuteMusic = isMute;
        PlayMusic(isMuteMusic ? null : currentTrack);
    }

    public void MuteSound(bool isMute)
    {
        isMuteSound = isMute;
    }

    public void ChangeMusicVolume(float v)
    {
        musicVolume = v;
        music.volume = musicVolume * MUSIC_VOLUME_MAX;
    }

    public void ChangeSFXVolume(float v)
    {
        sfxVolume = v;
        sfx.volume = sfxVolume;
    }

    // Play GAME
    // Launching a music track
    public void PlayMusicGame(int gameId, AudioClip track)
    {
        SettingSoundItem item = GetSettingSound(gameId);
        StartCoroutine(CrossFadeGame(track, item.isMuteMusic));
    }

    // A smooth transition from one to another music
    private IEnumerator CrossFadeGame(AudioClip to, bool mute, float delay = .3f)
    {
        if (to != null)
        {
            currentTrack = to;
        }

        if (music.clip != null)
        {
            while (delay > 0)
            {
                music.volume = delay * musicVolume * MUSIC_VOLUME_MAX;
                delay -= Time.unscaledDeltaTime;
                yield return 0;
            }
        }
        music.clip = to;
        if (to == null || mute)
        {
            music.Stop();
            yield break;
        }
        delay = 0;
        if (!music.isPlaying) music.Play();
        while (delay < 0.3f)
        {
            music.volume = delay * musicVolume * MUSIC_VOLUME_MAX;
            delay += Time.unscaledDeltaTime;
            yield return 0;
        }
        music.volume = musicVolume * MUSIC_VOLUME_MAX;
    }

    // A single sound effect
    public void PlaySoundGame(int gameId, AudioClip clip)
    {
        SettingSoundItem item = GetSettingSound(gameId);

        if (item.isMuteSound)
        {
            return;
        }

        if (clip != null)
        {
            sfx.PlayOneShot(clip);
        }
    }

    // On/off by game
    public void MuteMusicGame(int gameId)
    {
        SettingSoundItem item = GetSettingSound(gameId);
        item.isMuteMusic = !item.isMuteMusic;

        StartCoroutine(CrossFadeGame(item.isMuteMusic ? null : currentTrack, item.isMuteMusic));
        SaveSettingSound();
    }


    public void MuteSoundGame(int gameId)
    {
        SettingSoundItem item = GetSettingSound(gameId);
        item.isMuteSound = !item.isMuteSound;

        SaveSettingSound();
    }

    // Sound game
    public SettingSoundItem GetSettingSound(int gameId, bool mute = false)
    {
        if (!dictSettingSound.ContainsKey(gameId))
        {
            dictSettingSound.Add(gameId, new SettingSoundItem
            {
                gameId = gameId,
                isMuteMusic = mute,
                isMuteSound = mute
            });

            SaveSettingSound();
        }
        else if (mute) // mute setting for minigame
        {
            dictSettingSound[gameId].isMuteMusic = mute;
            dictSettingSound[gameId].isMuteSound = mute;
        }
        return dictSettingSound[gameId];
    }

    public void SaveSettingSound()
    {
        if (dictSettingSound == null)
        {
            return;
        }

        settingSound.Reload(dictSettingSound);
        PlayerPrefs.SetString(SETTING_SOUND_KEY, JsonUtility.ToJson(settingSound));
        PlayerPrefs.Save();
    }

    [System.Serializable]
    public class MusicTrack
    {
        public string name;
        public AudioClip track;
    }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public List<AudioClip> clips = new List<AudioClip>();
    }
}


[System.Serializable]
public class SettingSound
{
    public List<SettingSoundItem> setting;

    public void Reload(Dictionary<int, SettingSoundItem> dict)
    {
        setting = new List<SettingSoundItem>();
        foreach (var item in dict)
        {
            setting.Add(item.Value);
        }
    }

    public Dictionary<int, SettingSoundItem> GetDictionary()
    {
        Dictionary<int, SettingSoundItem> dictSettingSound = new Dictionary<int, SettingSoundItem>();

        foreach (var item in setting)
        {
            dictSettingSound.Add(item.gameId, item);
        }

        return dictSettingSound;
    }
}

[System.Serializable]
public class SettingSoundItem
{
    public int gameId;
    public bool isMuteSound;
    public bool isMuteMusic;
}
