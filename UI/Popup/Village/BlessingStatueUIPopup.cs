using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using static RestPacket;
using Assets.ZNetwork.Data;
using Newtonsoft.Json;
using System;
using Cysharp.Threading.Tasks;

//1. 슬롯 고정 상태 2. 슬롯 변동 가능 상태로 변경 3.프리셋 선택
public enum PresetUpdateType
{
  LockSlot = 1,     //BlessStatueSlotType = 3 인 상태
  UnlockSlot = 2,   //BlessStatueSlotType = 2 인 상태
  Preset = 3
}

public class BlessingStatueUIPopup : UIBaseController
{
  private UserInfoModel userInfo;
  private UserContentsData userContentsData;

  [Header("우측 버프 슬롯 정보")]
  public BlessStatueSlot[] blessSlotList;

  public BundleUnlockDropdown bundleDropdown;
  public BundleUnlockSlot bundleSlot;

  public Button useBlessingButton;

  [Header("좌측 UI")]
  public BundleContentLvInfo BundleBlessingLvInfo;
  public BundleCurrency bundleCurrency;

  public Button blessingLvInfoBtn;
  
  public TextMeshProUGUI totalCostText;
  public Button closeButton;

  [Header("가호 석상 레벨 별 정보")]
  public BlessStatueLvInfo blessStatueLvInfo;
  

  private int blessCost = 0;
  private int presetIndex = -1;

  protected override void Awake()
  {
    base.Awake();

    this.userContentsData = GameDataManager.getInstance.userContentsData;
    this.userInfo = GameDataManager.getInstance.userInfoModel;

    bundleDropdown.OnSelect = OnValueChangePreset;

    bundleSlot.SetSlot(blessSlotList);

    bundleDropdown.SetData(SlotUnlockContentType.BlessingStatuePreset);
    bundleSlot.SetData(SlotUnlockContentType.BlessingStatueSlot);

    blessingLvInfoBtn.onClick.AddListener(() => blessStatueLvInfo.SetLvData(userContentsData.blessingStatue.GetBlessingLv()));
    useBlessingButton.onClick.AddListener(OnClickUseBlessing);
    closeButton.onClick.AddListener(Hide);

    InitBlessSlotEvent();

  }

  private void InitBlessSlotEvent()
  {
    for (int i = 0; i < blessSlotList.Length; i++)
    {
      int index = i + 1;

      blessSlotList[i].OnClickBlessable     = () => BlessingStatueSlotOpen(this.presetIndex, index);
      blessSlotList[i].OnClickUnlock        = () => BlessingStatuePresetUpdate(PresetUpdateType.UnlockSlot, index);
      blessSlotList[i].OnClickLock          = () => BlessingStatuePresetUpdate(PresetUpdateType.LockSlot  , index);

    }
  }

  /// <summary>
  /// 신앙심 포인트는 나갔다 들어올때 갱신해주는 방식으로
  /// </summary>
  public void Open()
  {
    bundleCurrency.Enable();

    bundleSlot.UpdateData();


    SetBlessPresetSlot();
    SetBlessStatue(
      userContentsData.blessingStatue.GetBlessingLv(), 
      (int)userContentsData.blessingStatue.GetUseBlessingPoints()
    );
  }


  /// <summary>
  /// 프리셋 Dropdown 추가
  /// </summary>
  /// <param name="blessingPresetDataList"></param>
  private void SetBlessPresetSlot()
  {
    var blessingPresetMap = userContentsData.blessingStatue.blessingPresetMap;

    bundleDropdown.SetDropdown(blessingPresetMap.Select(n => $"프리셋 {n.Key}").ToList());

    bundleDropdown.OnValueChange(blessingPresetMap.Where(n => n.Value.isSelect).FirstOrDefault().Key - 1);
  }

  /// <summary>
  /// 가호 석상 축복사용에 필요한 비용 업데이트
  /// </summary>
  private void UpdateBlessingCost()
  {
    int activeSlotCount = 0;

    foreach (var blessSlot in blessSlotList)
    {
      if(blessSlot.blessStatueSlotType == BlessStatueSlotType.Unlock)
        activeSlotCount++;
    }

    if(activeSlotCount > 0)
    {
      blessCost = (int)BaseTable.getInstance.GetBaseValue(ConstantManager.BASE_INDEX_BLEESING_STATUE_SLOT_BLESSING_COST + activeSlotCount - 1);
    }
    else
    {
      blessCost = ConstantManager.DATA_NONE_INTEGER_VALUE;
    }

    totalCostText.text = blessCost.ToString();
  }

  /// <summary>
  /// 가호 석상 축복 사용
  /// </summary>
  public void OnClickUseBlessing()
  {
    bool isEnoughPoint    = blessCost > userInfo.GetBlessingPoint();
    bool isValidReadySlot = IsActiveSlotValid(BlessStatueSlotType.Blessable);
    bool isValidUnlock    = !IsActiveSlotValid(BlessStatueSlotType.Unlock);
    bool isValidHighGrade = IsValidHighGradeSlot();

    if (isEnoughPoint)
    {
      UIUtility.ShowToastMessagePopup("신앙심 부족");
      return;
    }


    if (isValidReadySlot)
    {
      UIUtility.ShowToastMessagePopup("활성화 대기 상태 해제");
      return;
    }


    if (isValidUnlock)
    {
      UIUtility.ShowToastMessagePopup("최소 1개 슬롯 잠금 해제");
      return;
    }

    //SSS 등급 이상의 옵션이 있는지 판단
    if(isValidHighGrade)
    {
      //일일 알림 활성화 리스트에 있는지 판단
      bool isValidDailyPopup = GameDataManager.getInstance.userContentsData.IsValidDailyPopupType(DailyPopupType.BlessingStatue_Statue);

      if (isValidDailyPopup)
      {
        //해당 팝업 활성화 시키고, Action에 RemoveDailyPopup 실행, OnUseBlessing() 실행
        //UIUtility.ShowNotificationPopup();

      }
    }

    OnUseBlessing();
  }

  private async void OnUseBlessing()
  {
    await APIManager.getInstance.REQ_UseBlessing<RES_UseBlessing>(this.presetIndex, (responseResult) =>
    {
      SetBlessStatue(responseResult.blessingLv, (int)responseResult.useBlessingPoints);
      userContentsData.blessingStatue.UpdateBlessingPreset(this.presetIndex, responseResult.blessingDataList);

      UpdateSlot();

    });
  }

  /// <summary>
  /// 활성화 된 슬롯이 하나라도 있는지 판단 로직
  /// </summary>
  /// <returns></returns>
  private bool IsActiveSlotValid(BlessStatueSlotType slotType)
  {
    foreach (var blessSlot in blessSlotList)
    {
      if (blessSlot.blessStatueSlotType == slotType)
        return true;
    }

    return false;
  }

  private bool IsValidHighGradeSlot()
  {
    foreach (var blessSlot in blessSlotList)
    {
      if (blessSlot.slotGrade >= (int)GradeType.SSS)
        return true;
    }

    return false;
  }

  /// <summary>
  /// 유저 가호 석상 프리셋 데이터 기반 슬롯 데이터 변경
  /// </summary>
  private void UpdateSlot()
  {
    for (int i = 0; i < blessSlotList.Length; i++)
    {
      blessSlotList[i].SetBlessSlotData(userContentsData.blessingStatue.blessingPresetMap[presetIndex].blessingDataList[i]);
    }
  }


  /// <summary>
  /// 슬롯 오픈 시
  /// </summary>
  /// <param name="presetNum"></param>
  /// <param name="slotNum"></param>
  public async void BlessingStatueSlotOpen(int presetNum, int slotNum)
  {
    //Debug.LogError($"{presetNum}/{slotNum}");
    await APIManager.getInstance.REQ_BlessingStatueSlotOpen<RES_BlessingStatueSlotOpen>(presetNum, slotNum, (responseResult) => {
      userContentsData.blessingStatue.UpdateBlessingData(presetNum, slotNum, responseResult.updateBlessingData);
      UpdateSlot();
      UpdateBlessingCost();
    });
  }

  /// <summary>
  /// 해당 프리셋 잠금, 등등 상태 변경
  /// </summary>
  /// <param name="type">슬롯 고정, 슬롯 변동 가능</param>
  /// <param name="slotNum"></param>
  public async void BlessingStatuePresetUpdate(PresetUpdateType type, int slotNum)
  {
    //Debug.LogError(this.presetIndex);

    await APIManager.getInstance.REQ_BlessingStatuePresetUpdate<RES_BlessingStatuePresetUpdate>((int)type, this.presetIndex, slotNum, (responseResult) =>
    {
      userContentsData.blessingStatue.UpdateBlessingSlotData((int)type, this.presetIndex, slotNum);
      UpdateSlot();
      UpdateBlessingCost();
    });
  }


  /// <summary>
  /// 프리셋 드롭다운 선택 시 아래 함수 실행
  /// API 정상 수신 후 해당 데이터로 변경
  /// </summary>
  /// <param name="index"></param>
  public async void OnValueChangePreset(int index)
  {
    presetIndex = index + 1;

    UpdateSlot();
    UpdateBlessingCost();

  }


  /// <summary>
  /// 좌측 가호석상 정보 설정
  /// </summary>
  /// <param name="statueLv"></param>
  /// <param name="curExp"></param>
  private void SetBlessStatue(int statueLv, int curExp)
  {
    int statueMaxLevel = GachaWeightTable.getInstance.GetGachaMaxLevel(ShopCategoryType.BlessStatue);
    

    if(statueLv < statueMaxLevel)
      BundleBlessingLvInfo.SetData(ShopCategoryType.BlessStatue, statueLv, curExp);
    else
      BundleBlessingLvInfo.SetMaxData(statueMaxLevel);
  }


  public async override void Hide()
  {
    base.Hide();

    await APIManager.getInstance.REQ_BlessingStatuePresetUpdate<RES_BlessingStatuePresetUpdate>((int)PresetUpdateType.Preset, this.presetIndex, 0, (_) => {
      userContentsData.blessingStatue.UpdateBlessingPresetApply(this.presetIndex);
    });

    bundleCurrency.Disable();


    presetIndex = -1;
    blessCost = 0;
  }


  

}
