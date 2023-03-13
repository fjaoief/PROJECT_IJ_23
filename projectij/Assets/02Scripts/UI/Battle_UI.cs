using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;

public class Battle_UI : MonoBehaviour
{
    private AudioSource[] audiosources;

    private void Awake()
    {
        audiosources = SoundManager.Instance.GetComponents<AudioSource>();
    }

    public void clickPauseBtn()
    {
        GameObject pausebtn = EventSystem.current.currentSelectedGameObject;
        TextMeshProUGUI btn_name = pausebtn.transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>();
        if (btn_name.text == "Pause")
        {
            StageManager.Instance.setPause(0);
            // 소리 pause
            foreach (AudioSource a in audiosources) {
                if (a.outputAudioMixerGroup.name == "BGM")
                    a.Pause();
            }
            btn_name.text = "Resume";
        }
        else if (btn_name.text == "Resume")
        {
            StageManager.Instance.setPause(1);
            foreach (AudioSource a in audiosources) {
                if (a.outputAudioMixerGroup.name == "BGM")
                    a.Play();
            }
            btn_name.text = "Pause";
        }
    }

}
