using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource BGMSource;
    [SerializeField] private AudioSource SFXSource;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
    }

    public void PlayMusic(AudioClip clip, float volume = 1f)
    {
        BGMSource.clip = clip;
        BGMSource.volume = volume;
        BGMSource.Play();
    }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        SFXSource.PlayOneShot(clip, volume);
    }

    public void StopMusic()
    {
        BGMSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        BGMSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        SFXSource.volume = volume;
    }
}