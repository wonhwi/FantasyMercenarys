using DG.Tweening;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;

public enum AudioType
{
  BGM = 1,
  SFX = 2,
}

public class NewSoundManager : SingletonTemplate<NewSoundManager>
{
  public class AudioSourceQue
  {
    public int soundIdx;
    public string soundName;
    public AudioSource source;

    public AudioSourceQue()
    {
      soundIdx = 0;
      soundName = "";
      source = null;
    }

    public void Stop()
    {
      soundIdx = 0;
      soundName = "";
      if (source != null)
        source.Stop();
    }

    public bool IsPlaying()
    {
      if ((source != null) && source.isPlaying)
        return true;

      return false;
    }
  }

  [Header("AudioMixer")]
  public AudioMixer audioMixer;

  [Header("BGM")]
  public AudioSource BGMAudioSource;
  private string BGMInGameStageSoundName = ""; // 인게임 BGM

  private string currentLoopBGMname = "";



  [Header("SFX")]
  //Const 옮기자
  //추후 Priority 작업도 진행 할 예정
  private List<AudioSourceQue> sfxSourceList = new List<AudioSourceQue>();
  private Queue<AudioSourceQue> sfxSourcePool = new Queue<AudioSourceQue>();

  //오디오 타입에 따라 출력 딜레이 값 적용

  private float sfxDelayTime = 0.1f;  //sfx 최소 딜레이 시간 (재 출력 시간)
  private float sfxPlayTime;          //sfx 실행 시간

  //public TMPro.TextMeshProUGUI QueueCount;
  //플레이 가능한 요소를 찾아서 플레이 Max개 만큼 실행이 되고있으면 오디오를 씹어버려
  public void InitDefaultSetting()
  {
    sfxSourcePool.Clear();

    this.SetSoundPrefab();

    this.SetMuteAudio(AudioType.BGM, IsMuteAudio(AudioType.BGM));
    this.SetMuteAudio(AudioType.SFX, IsMuteAudio(AudioType.SFX));

    //On 이면 데시벨 20 Off면 -80 
    //사운드 설정
    //audioMixer.SetFloat("Master",        CalculationDecibel(masterValue));
    //audioMixer.SetFloat("MasterBGM",     CalculationDecibel(bgmValue));
    //audioMixer.SetFloat("MasterSFX",     CalculationDecibel(sfxValue));
  }

  private void SetSoundPrefab()
  {
    GameObject prefabBGM = NewResourceManager.getInstance.LoadResource<GameObject>(NewResourcePath.PREFAB_SOUND_BGM);
    GameObject prefabSFX = NewResourceManager.getInstance.LoadResource<GameObject>(NewResourcePath.PREFAB_SOUND_SFX);

    foreach (AudioType audioType in Enum.GetValues(typeof(AudioType)))
    {
      switch (audioType)
      {
        case AudioType.SFX:
          for (int i = 0; i < ConstantManager.SFX_MAX_SIZE; i++)
          {
            GameObject sfxObject = Instantiate(prefabSFX, this.transform);
            AudioSourceQue audioSourceQue = new AudioSourceQue();
            audioSourceQue.source = sfxObject.GetComponent<AudioSource>();
            sfxSourceList.Add(audioSourceQue);
            sfxSourcePool.Enqueue(audioSourceQue);
          }
          break;

        case AudioType.BGM:
          BGMAudioSource = Instantiate(prefabBGM, this.transform).GetComponent<AudioSource>();
          break;
      }
    }
  }

  public bool PlayBGMSound(string soundName, bool useFadeOut = true)
  {
    if(currentLoopBGMname.Equals(soundName))
      return false;

    this.currentLoopBGMname = soundName;

    SoundData soundData = SoundTable.getInstance.GetSoundData(soundName);

    if (useFadeOut)
    {
      StartCoroutine(CorStartFadeBGM(soundData));
    }
    else
    {
      BGMAudioSource.Stop();
      BGMAudioSource.clip = NewResourceManager.getInstance.LoadAudioClip(NewResourcePath.PATH_SOUND_BGM, soundName);
      BGMAudioSource.volume = soundData.volume;
      BGMAudioSource.Play();
    }
    

    return true;
  }

  public bool PlayLobbyBGM(string soundName)
  {
    BGMAudioSource.Stop();
    BGMAudioSource.clip = NewResourceManager.getInstance.LoadAudioClip(NewResourcePath.PATH_SOUND_BGM, soundName);
    BGMAudioSource.Play();

    return true;
  }


  public bool PlaySFXSound(string soundName)
  {
    //현재 해당 사운드 타입이 정해진 Delay시간이 지났다고 판단되면 통과 추후 작업
    //if (Time.realtimeSinceStartup < (sfxDelayTime + sfxPlayTime))
    //  return false;
    //
    //sfxPlayTime = Time.realtimeSinceStartup;
    if (soundName.Equals(ConstantManager.DATA_NONE_STRING_VALUE))
      return false;

    SoundData soundData = SoundTable.getInstance.GetSoundData(soundName);

    if (soundData.IsNull())
    {
      Debug.Log($"사운드 데이터가 존재하지 않습니다. {soundData.soundIdx} 인덱스");
      return false;
    }

    AudioSourceQue audioSourceQue = GetAudioSourceQue();

    if(audioSourceQue == null)
    {
      Debug.Log($"현재 활성화 된 오디오가 {ConstantManager.SFX_MAX_SIZE} 개를 넘어서 출력하지 않습니다.");
      return false;
    }
    
    

    audioSourceQue.soundName = soundName;
    audioSourceQue.source.clip = NewResourceManager.getInstance.LoadAudioClip(NewResourcePath.PATH_SOUND_SFX, soundData.soundName);
    audioSourceQue.source.volume = soundData.volume;
    audioSourceQue.source.Play();

    StartCoroutine(CorReturnToPool(audioSourceQue));

    return true;
  }

  public bool PlayUISFXSound(string soundName)
  {
    if (soundName.Equals(ConstantManager.DATA_NONE_STRING_VALUE))
      return false;

    AudioSourceQue audioSourceQue = GetAudioSourceQue();

    if (audioSourceQue == null)
    {
      Debug.Log($"현재 활성화 된 오디오가 {ConstantManager.SFX_MAX_SIZE} 개를 넘어서 출력하지 않습니다.");
      return false;
    }

    audioSourceQue.soundName = soundName;
    audioSourceQue.source.clip = NewResourceManager.getInstance.LoadAudioClip(NewResourcePath.PATH_SOUND_UI_SFX, soundName);
    audioSourceQue.source.volume = 1f;
    audioSourceQue.source.Play();

    StartCoroutine(CorReturnToPool(audioSourceQue));

    return true;
  }


  public void StopSound(string clipName)
  {
    sfxSourceList.Find(n => n.soundName.Equals(clipName)).Stop();

  }

  public IEnumerator CorStartFadeBGM(SoundData soundData, float duration = 1.5f)
  {
    float currentTime = 0;
    float currentVol = BGMAudioSource.volume;
    float targetVolume = soundData.volume;
    //startAction?.Invoke();

    while (currentTime < duration)
    {
      currentTime += Time.deltaTime;

      BGMAudioSource.volume = Mathf.Lerp(currentVol, 0f, currentTime / duration);
      yield return null;
    }

    currentTime = 0f;

    BGMAudioSource.clip = NewResourceManager.getInstance.LoadAudioClip(NewResourcePath.PATH_SOUND_BGM, soundData.soundName);
    BGMAudioSource.Play();

    while (currentTime < duration)
    {
      currentTime += Time.deltaTime;

      BGMAudioSource.volume = Mathf.Lerp(0f, targetVolume, currentTime / duration);
      yield return null;
    }


    BGMAudioSource.volume = targetVolume;
    //endAction?.Invoke();
  }


  public bool SetBGMSoundInGameStage(string soundName)
  {
    BGMInGameStageSoundName = soundName;

    return true;
  }

  public bool PlayBGMSoundInGameStage()
  {
    //PlayBGMSoundName(BGMInGameStageSoundName);

    return true;
  }


  private AudioSourceQue GetAudioSourceQue()
  {
    if (sfxSourcePool.Count > 0)
    {
      return sfxSourcePool.Dequeue();

    }
    else
    {
      return null;
    }
  }

  private IEnumerator CorReturnToPool(AudioSourceQue audioSourceQue)
  {
    yield return new WaitUntil(() => !audioSourceQue.IsPlaying());
    //Debug.LogError("audioSourceQue Queue로 다시 들어왔어");
    sfxSourcePool.Enqueue(audioSourceQue);
  }

  /// <summary>
  /// 오디오 음소거 활성화/비활성화 여부 확인
  /// </summary>
  /// <param name="audioType">오디오 타입</param>
  /// <returns></returns>
  public bool IsMuteAudio(AudioType audioType)
  {
    bool isMute = PlayerPrefs.GetInt($"OffMaster{audioType}Volume", 1) == 1 ? false : true;

    return isMute;
  }

  public void SetMuteAudio(AudioType audioType, bool isMute)
  {
    int value = !isMute ? 1 : 0;
    audioMixer.SetFloat($"Master{audioType}", CalculationDecibel(value));

    PlayerPrefs.SetInt($"OffMaster{audioType}Volume", value);
  }

  /// <summary>
  /// 데시벨 계산  0 ~ 1 값으로 데시벨 계산
  /// </summary>
  /// <returns></returns>
  public float CalculationDecibel(float value)
  {
    float dBValue = 0f;

    if (value.Equals(0f))
    {
      dBValue = Mathf.Log10(0.0001f) * 20f;
    }
    else
    {
      dBValue = Mathf.Log10(value * 10f) * 20f;
    }

    return dBValue;
  }



  #region Public Function


  #endregion

  #region BGM
  private void PauseBGM(bool pause)
  {
    if (BGMAudioSource != null && pause)
    {
      BGMAudioSource.Pause();
    }
    else if (BGMAudioSource != null && !pause)
    {
      BGMAudioSource.Play();
    }

  }

  #endregion

  #region SFX

  #endregion

  /// <summary>
  /// BGM Fade In Out으로 변경되는 효과
  /// </summary>
  public void CorBGMFadeChange()
  {

  }

  string[] TestClipList = new string[] { "sfx_attack_bow_02", "sfx_attack_bow_06", "sfx_attack_bow_10" };

#if UNITY_EDITOR
  //Clipping 방지 코드 추후 추가 예정
  public void Update()
  {

    if(Input.GetKeyDown(KeyCode.K))
    {
      PlaySFXSound(TestClipList[UnityEngine.Random.Range(0, TestClipList.Length)]);
    }


    //if(QueueCount != null)
    //  QueueCount.text = $"활성화 된 SFX SoundCont {sfxSourcePool.Count}";
  }
#endif
  //void OnApplicationFocus(bool hasFocus)
  //{
  //  PauseBGM(!hasFocus);
  //}


  #region Obsolete
  /// <summary>
  /// Slide 에서 사용하는 함수 추후 볼륨 제어 기획이 생길때 쓸 놈
  /// </summary>
  /// <param name="audioType">설정 하려는 오디오 타입</param>
  /// <param name="value">0 ~ 1 사이의 값</param>
  /// <returns></returns>
  public void SetVolume(AudioType audioType, float value)
  {
    float volume = Mathf.Round(value * 100f) * 0.01f;

    float decibelValue = CalculationDecibel(volume);
    Debug.Log(decibelValue);
    PlayerPrefs.SetFloat($"{audioType}Volume", volume);
    audioMixer.SetFloat($"Master", decibelValue);
  }

  #endregion
}
