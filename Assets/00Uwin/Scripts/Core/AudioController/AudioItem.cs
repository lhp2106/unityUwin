using UnityEngine;

public class AudioItem : MonoBehaviour {

    public AudioSource mAudio;
    public VKObjectPoolManager poolManager;

    private bool isPlay = false;

    void Update()
    {
        if (isPlay)
        {
            if (!mAudio.isPlaying)
            {
                RemoveAudio();
            }
        }
    }

    public void PlayAudio(AudioClip audio, VKObjectPoolManager poolManager)
    {
        this.poolManager = poolManager;
        mAudio.clip = audio;
        mAudio.loop = false;
        mAudio.Play();

        isPlay = true;
    }

    public void RemoveAudio()
    {
        isPlay = false;
        mAudio.clip = null;
        poolManager.GiveBackObject(gameObject);
    }
}
