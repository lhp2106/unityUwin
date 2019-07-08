using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AudioController : MonoBehaviour {

    public const string PATH = "Sound/";

    #region Properties

    public List<AudioClip> audios;
    public AudioSource mAudio;

    public VKObjectPoolManager poolAudioItem;
    private List<AudioClip> audioCaches;
    #endregion

    #region Singleton
    private static AudioController instance;

    public static AudioController Instance
    {
        get
        {
            if (instance == null)
            {
                instance = GameObject.FindObjectOfType<AudioController>();
            }
            return instance;
        }
    }

    void Awake()
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

        audioCaches = new List<AudioClip>();
        DontDestroyOnLoad(this.gameObject);
    }

    #endregion

    #region Method
    public void PlayAudio(string name)
    {
        if (Database.Instance.localData.isOpenSound)
        {
            AudioClip clip = GetAudioClip(name);
            if (clip != null)
            {
                AudioItem item = poolAudioItem.BorrowObject<AudioItem>();
                item.PlayAudio(clip, poolAudioItem);
            }
        }
    }

    public void PlayAudio(AudioClip clip)
    {
        if (Database.Instance.localData.isOpenSound)
        {
            if (clip != null)
            {
                AudioItem item = poolAudioItem.BorrowObject<AudioItem>();
                item.PlayAudio(clip, poolAudioItem);
            }
        }
    }

    private AudioClip GetAudioClip(string name)
    {
        AudioClip aClip =  audioCaches.FirstOrDefault(a => a.name.Equals(name));

        if (aClip == null)
        {
            AudioClip clip = Resources.Load("Sound/" + name) as AudioClip;

            if (clip != null)
            {
                clip.name = name;
                aClip = clip;
                audioCaches.Add(clip);
            }
        }

        return aClip;
    }

    public void ClearAudioCache()
    {
        audioCaches.Clear();
    }
    #endregion

}
