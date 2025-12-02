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

    public void PlayMusic(AudioClip clip)
    {
        BGMSource.clip = clip;
        BGMSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        SFXSource.PlayOneShot(clip, SFXSource.volume);
    }

    public void StopMusic()
    {
        BGMSource.Stop();
    }

    public float GetMusicVolume()
    {
        return BGMSource.volume;
    }   

    public float GetSFXVolume()
    {
        return SFXSource.volume;
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