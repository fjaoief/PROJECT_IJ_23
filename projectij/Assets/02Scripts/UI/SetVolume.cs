using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    private AudioMixer mixer;
    private Slider slider;

    private void Start() {
        mixer = SoundManager.Instance.GameSound;
        slider = gameObject.GetComponent<Slider>();
        if (gameObject.name == "BGM_Slider")
            slider.value = SoundManager.Instance.masterVolumeBGM;
        else if (gameObject.name == "SFX_Slider")
            slider.value = SoundManager.Instance.masterVolumeSFX;
    }

    public void SetBGMLevel(float sliderValue)
    {
        Debug.Log(sliderValue);
        mixer.SetFloat("BGMvol", Mathf.Log10(sliderValue) * 20);
        SoundManager.Instance.masterVolumeBGM = sliderValue;
    }

    public void SetSFXLevel(float sliderValue)
    {
        mixer.SetFloat("SFXvol", Mathf.Log10(sliderValue) * 20); 
        SoundManager.Instance.masterVolumeSFX = sliderValue;
    }
        
}
