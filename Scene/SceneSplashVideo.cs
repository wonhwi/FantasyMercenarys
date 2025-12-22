using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class SceneSplashVideo : MonoBehaviour
{
  public VideoPlayer videoPlayer; // 비디오 플레이어 컴포넌트
  public string nextSceneName; // 전환할 씬 이름

  void Start()
  {
    Screen.sleepTimeout = SleepTimeout.NeverSleep;

    // 비디오 플레이어에서 비디오가 끝날 때 호출되는 이벤트 등록
    videoPlayer.loopPointReached += OnVideoFinished;

    // 비디오 재생 시작
    videoPlayer.Play();
  }

  // 비디오가 끝났을 때 호출되는 메서드
  void OnVideoFinished(VideoPlayer videoPlayer)
  {
    // 다음 씬으로 전환
    //Debug.Log($"{nextSceneName}으로 이동해버렷");
    SceneManager.LoadScene(nextSceneName);
  }
}
