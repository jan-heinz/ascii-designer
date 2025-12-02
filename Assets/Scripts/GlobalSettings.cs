using UnityEngine;
using UnityEngine.UI;

public class GlobalSettings : MonoBehaviour
{
    public Slider BGMVolumeSlider;
    public Slider SFXVolumeSlider;

    void Start()
    {
        BGMVolumeSlider.value = AudioManager.Instance.GetMusicVolume();
        SFXVolumeSlider.value = AudioManager.Instance.GetSFXVolume();
          
        BGMVolumeSlider.onValueChanged.AddListener(OnBGMVolumeChanged);
        SFXVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }

    void OnBGMVolumeChanged(float newValue)
    {
        AudioManager.Instance.SetMusicVolume(newValue);
    }
    
    void OnSFXVolumeChanged(float newValue)
    {
        AudioManager.Instance.SetSFXVolume(newValue);
    }
}

