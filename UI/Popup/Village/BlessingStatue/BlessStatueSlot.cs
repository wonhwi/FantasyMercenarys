using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FantasyMercenarys.Data;
using System;

public enum BlessStatueSlotType
{
  ConditionLock = 0,   //잠김 (해금 조건 달성안되어 있어 아예 잠금)
  Blessable = 1,       //대기 (버튼을 눌러서 축복을 받을 수 있는 상태)
  Unlock = 2,          //변동 (이미 버프가 있는 상태 변동 -> 고정으로 상태 변환 가능)
  Lock = 3,            //고정 (이미 버프가 있는 상태 고정 -> 변동으로 상태 변환 가능)
}


public class BlessStatueSlot : UIUnlockSlot
{
  public BlessStatueSlotType blessStatueSlotType;

  [Header("슬롯 상태별 활성화/비활성화 하는 게임오브젝트")]
  public GameObject bundleConditionLock;      //조건 충족해야해
  public GameObject bundleBlessable;          //버튼을 눌러서 속성 얻어야 할때

  public GameObject bundleActive;
  public GameObject bundleUnLock;
  public GameObject bundleLock;


  [Header("버튼")]
  public Button blessableButton;
  public Button lockButton;
  public Button unlockButton;

  [Header("등급/효과")]
  public Image gradeImage;
  public TextMeshProUGUI blessText;

  public int slotGrade;

  public Action OnClickBlessable;
  public Action OnClickLock;
  public Action OnClickUnlock;


  protected override void Awake()
  {
    base.Awake();

    blessableButton.onClick.AddListener(OnClickBlessable.Invoke);

    lockButton.onClick.AddListener(  () => {
      SetBlessStatueSlot(BlessStatueSlotType.Lock);
      OnClickLock?.Invoke();
      });

    unlockButton.onClick.AddListener(() => { 
      SetBlessStatueSlot(BlessStatueSlotType.Unlock);
      OnClickUnlock?.Invoke();
      });
  }

  public void SetBlessSlotData(BlessingData blessingData)
  {
    BlessStatueSlotType type = (BlessStatueSlotType)blessingData.activeStatus;

    //잠금 상태 일때 내 레벨이 충족하면 type을 바꿔줘서 실행해줘
    if(type == BlessStatueSlotType.ConditionLock)
    {
      if (!IsLocked())
        type = BlessStatueSlotType.Blessable;
    }

    this.SetBlessStatueSlot(type);

    if(type == BlessStatueSlotType.Unlock || type == BlessStatueSlotType.Lock)
    {
      this.SetData(blessingData);
    }
  }

  public void SetBlessStatueSlot(BlessStatueSlotType type)
  {
    this.blessStatueSlotType = type;

    switch (type)
    {
      case BlessStatueSlotType.ConditionLock:
        bundleConditionLock.SetActive(true);

        bundleActive.SetActive(false);
        bundleBlessable.SetActive(false);
        break;
      case BlessStatueSlotType.Blessable:
        bundleBlessable.SetActive(true);

        bundleActive.SetActive(false);
        bundleConditionLock.SetActive(false);
        break;
      case BlessStatueSlotType.Unlock:
        bundleActive.SetActive(true);
        bundleUnLock.SetActive(true);
        bundleLock.SetActive(false);

        bundleBlessable.SetActive(false);
        bundleConditionLock.SetActive(false);
        break;
      case BlessStatueSlotType.Lock:
        bundleActive.SetActive(true);
        bundleUnLock.SetActive(false);
        bundleLock.SetActive(true);

        bundleBlessable.SetActive(false);
        bundleConditionLock.SetActive(false);
        break;
    }
  }

  private void SetData(BlessingData blessingData)
  {
    slotGrade = blessingData.grade;

    gradeImage.sprite = NewResourceManager.getInstance.LoadGradeSpirte(blessingData.grade);
    gradeImage.SetNativeSize();

    blessText.text = $"{FormatUtility.GetStatTypeNameBlessing(blessingData.blessingType)} {blessingData.blessingValue:F2} %";

  }

}
