using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[System.Serializable]
public class Sound
{
  public string name;

  public AudioClip clip;
  public AudioMixerGroup output;

  [Range(0f, 1f)]
  public float volume = 1f;

  public bool isLoop = false;
  public AudioSource source;
}

public class SoundManager : MonoBehaviour
{
    private static SoundManager _instance;
    public Sound[] sounds;
    private static Dictionary<string, float> soundTimerDictionary;

    public static SoundManager Instance {get{return _instance;}}
    private AudioSource BGM_Source;
    public AudioMixer GameSound;

    public float masterVolumeSFX = 1f;
    public float masterVolumeBGM = 1f;

    public bool isBattleBGMended = false;

    private void Awake()
    {
        DontDestroyOnLoad(this);
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }

        soundTimerDictionary = new Dictionary<string, float>();

        foreach (Sound sound in sounds)
        {
            if (sound.source == null)
                sound.source = gameObject.AddComponent<AudioSource>();
            sound.source.clip = sound.clip;
            sound.source.outputAudioMixerGroup = sound.output;

            sound.source.volume = sound.volume;
            sound.source.loop = sound.isLoop;
        }
    }

    private void Start()
    {
        playbgm("start_bgm");
        SceneManager.sceneLoaded += PlayBGM;
        BGM_Source = transform.GetChild(0).GetComponent<AudioSource>();
    }

    public void PlayBGM(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("kk");
        if (scene.name == "BattleScene")
        {
            StartCoroutine(PlayBattleBGM());
            StartCoroutine(PlayPortalBGM());
        }
        else if (scene.name == "StartScene")
        {
            playbgm("start_bgm");
        }
    }

    IEnumerator PlayBattleBGM()
    {
        while(StageManager.Instance.player.got_portal < 1)
        {
            playbgm(GameManager.gameManager_Instance.squad[0].characterName.ToString());
            yield return new WaitUntil(()=>!BGM_Source.isPlaying);
            yield return new WaitForSecondsRealtime(60f);

            playbgm(GameManager.gameManager_Instance.squad[1].characterName.ToString());
            yield return new WaitUntil(()=>!BGM_Source.isPlaying);
            yield return new WaitForSecondsRealtime(60f);

            playbgm(GameManager.gameManager_Instance.squad[2].characterName.ToString());
            yield return new WaitUntil(()=>!BGM_Source.isPlaying);
            yield return new WaitForSecondsRealtime(60f);
        }        
    }

    IEnumerator PlayPortalBGM()
    {
        yield return new WaitUntil(()=>StageManager.Instance.player.got_portal >= 1);
        StopCoroutine(PlayBattleBGM());
        playbgm("portal");
    }

    public void Play(string name)
    {
        Sound sound = Array.Find(sounds, s => s.name == name);

        if (sound == null)
        {
            Debug.LogError("Sound " + name + " Not Found!");
            return;
        }

        if (!CanPlaySound(sound))return;

        sound.source.volume = sound.volume * masterVolumeSFX;
        sound.source.PlayOneShot(sound.clip);
    }

    private void playbgm(string name)
    {
        Sound sound = Array.Find(sounds, s => s.name == name);

        if (sound == null)
        {
            Debug.LogError("Sound " + name + " Not Found!");
            return;
        }

        if (!CanPlaySound(sound))return;

        sound.source.volume = sound.volume * masterVolumeBGM;
        sound.source.loop = sound.isLoop;
        sound.source.clip = sound.clip;
        sound.source.Play();
    }

    public void Stop(string name)
    {
        Sound sound = Array.Find(sounds, s => s.name == name);

        if (sound == null)
        {
            Debug.LogError("Sound " + name + " Not Found!");
            return;
        }

        sound.source.Stop();
    }

    private static bool CanPlaySound(Sound sound)
    {
    if (soundTimerDictionary.ContainsKey(sound.name))
    {
        float lastTimePlayed = soundTimerDictionary[sound.name];

        if (lastTimePlayed + sound.clip.length < Time.time)
        {
        soundTimerDictionary[sound.name] = Time.time;
        return true;
        }

        return false;
    }

    return true;
    }

}
