using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

/// <summary>
/// 전반적인 CountDown을 사용하는 데 사용되는 모듈화된 스크립트
/// </summary>
public class BundleCountDownText : MonoBehaviour
{
  /// <summary>
  /// InitCount 시 Text가 출력되는 방식
  /// </summary>
  public enum DefaultTextType
  {
    Empty = 0, //표시 없고 빈값
    Time = 1,  //00:00:00 형태로 출력
  }

  /// <summary>
  /// CountDown이 마무리 되었을때 출력되는 방식
  /// </summary>
  private enum FinishTextType
  {
    Empty = 0,       //표시 없고 빈값
    FinishText = 1,  //완료 표시
    FinishTime = 2,  //00:00:00 형태로 출력
  }

  [SerializeField] private DefaultTextType defaultType = DefaultTextType.Empty;
  [SerializeField] private FinishTextType textType = FinishTextType.Empty;
  [SerializeField] private TextMeshProUGUI countDownText;
  [SerializeField] private GameObject countDownGroup;

  [SerializeField] private Color defaultColor = Color.gray;
  [SerializeField] private Color countDownColor = Color.white;

  public bool isInitActive = false;  //초기화 시 CountText 활성화/비활성화 여부
  public bool isFinishAlive = false; //카운트 다운 완료 시 Text 활성화/비활성화 여부

  private Coroutine timerCoroutine;

  public Action OnComplete;

  public void InitCountDown()
  {
    StopCountDown();

    SetInitText();
    SetTextActive(isInitActive);
  }


  public void SetCountDown(int remainSeconds)
  {
    if (remainSeconds > 0)
      timerCoroutine = StartCoroutine(OnCountDown(remainSeconds));
    else
      OnComplete?.Invoke();
  }

  /// <summary>
  /// 초기화시 Text 출력 방식
  /// </summary>
  public void SetInitText()
  {
    if (defaultType == DefaultTextType.Empty)
      countDownText.text = string.Empty;
    else if (defaultType == DefaultTextType.Time)
    {
      countDownText.color = defaultColor;
      countDownText.text = FormatUtility.FormatHHMMSS(0);
    }
  }

  public void SetText(string timeText)
  {
    countDownText.text = timeText;
  }

  /// <summary>
  /// CountDown 마무리 시 출력되는 방식
  /// </summary>
  public void SetFinishText()
  {
    if (textType == FinishTextType.Empty)
      countDownText.text = string.Empty;
    else if (textType == FinishTextType.FinishText)
      countDownText.text = "완료";
    else if (textType == FinishTextType.FinishTime)
    {
      countDownText.color = defaultColor;
      countDownText.text = FormatUtility.FormatHHMMSS(0);
    }
  }

  public void SetTextActive(bool isActive)
  {
    countDownGroup.SetActive(isActive);
  }
  
  public void StopCountDown()
  {
    if (timerCoroutine != null)
    {
      StopCoroutine(timerCoroutine);
      timerCoroutine = null;
    }
  }

  private IEnumerator OnCountDown(int remainSeconds)
  {
    SetTextActive(true);

    countDownText.color = countDownColor;

    int time = remainSeconds;
    while (time >= 0)
    {
      SetText(FormatUtility.FormatHHMMSS(time));
      yield return YieldInstructionCache.WaitForSeconds(1f);
      time--;
    }

    SetFinishText();

    SetTextActive(isFinishAlive);

    OnComplete?.Invoke();
  }
}
