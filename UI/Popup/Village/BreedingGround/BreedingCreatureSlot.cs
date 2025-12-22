using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class BreedingCreatureSlot : MonoBehaviour
{
  public enum SlotState
  {
    Lock = -1,   //미해금 상태
    Empty = 0,
    Mount = 1,
  }

  [SerializeField] private BundleRewardPoint bundleRewardPoint;
  [SerializeField] private BundleGaugeText bundleGaugeText;

  [SerializeField] private GameObject creatureRoot;
  [SerializeField] private Button creatureButton;  //크리쳐 아이콘
  [SerializeField] private Image conditionIcon;    //컨디션 상태 아이콘

  [SerializeField] private Button lockButton;
  [SerializeField] private Button slotButton;

  private int currentPoint;
  private int maxPoint;

  public Action OnClickSlot;     //인벤토리 Load
  public Action OnUnMount;       //연구비 획득, 장착해제
  public Action OnLock;

  private Coroutine trackCoroutine;

  private void Awake()
  {
    slotButton.onClick.AddListener(()     => OnClickSlot?.Invoke());
    creatureButton.onClick.AddListener(() => OnUnMount?.Invoke());
    lockButton.onClick.AddListener(()     => OnLock?.Invoke());

  }


  /// <summary>
  /// 슬롯 상태 설정
  /// </summary>
  /// <param name="slotState"></param>
  public void SetSlot(SlotState slotState)
  {
    lockButton.gameObject.SetActive(slotState == SlotState.Lock);
    creatureRoot.SetActive(slotState == SlotState.Mount);
  }

  /// <summary>
  /// 데이터 기반 이미지, 기타 설정
  /// </summary>
  /// <param name="creatureIdx"></param>
  /// <param name="conditionType"></param>
  public void SetData(string creatureIcon, int conditionType, int maxPoint)
  {
    this.maxPoint = maxPoint;

    creatureButton.image.sprite = NewResourceManager.getInstance.LoadSprite(NewResourcePath.PATH_UI_ICON_BREEDING_GROUND, creatureIcon);
    conditionIcon.sprite = NewResourceManager.getInstance.LoadSprite(NewResourcePath.PATH_UI_ICON_BREEDING_GROUND, $"fm_condition_{conditionType}");
  }

  /// <summary>
  /// 생명체가 획득한 수치 량 출력
  /// </summary>
  public void SetPointGauge(int currentPoint)
  {
    this.currentPoint = currentPoint;

    bundleGaugeText.SetGaugeTextData(Mathf.Clamp(currentPoint, 0, maxPoint), maxPoint);
  }

  public void SetRewardPoint(int point)
  {
    bundleRewardPoint.SetPoint(point);
  }


  //이거 배치했을때 코루틴 실행
  //배치 시간을 남은 초로 환산한다음  내가 몇초바다 한바퀴도는지 시간을 응용하자
  public void StartTrackCoroutine(int gainPerSecond)
  {
    trackCoroutine = StartCoroutine(TrackRewardPoint(gainPerSecond));
  }

  private IEnumerator TrackRewardPoint(int gainPerSecond)
  {
    //Debug.LogError("초당 획득 수치 " + gainPerSecond);
    //Debug.LogError("분당 획득 수치 " + gainPerSecond * 60);

    while (true)
    {
      yield return YieldInstructionCache.WaitForSeconds(1f);

      int point = this.currentPoint + gainPerSecond;

      SetPointGauge(point);

      
    }

  }

  public void InitCreatureSlot()
  {
    if (trackCoroutine != null)
    {
      StopCoroutine(trackCoroutine);
      trackCoroutine = null;
    }
  }

}
