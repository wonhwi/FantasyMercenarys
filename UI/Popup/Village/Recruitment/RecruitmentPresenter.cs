using Cysharp.Threading.Tasks;
using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RestPacket;

public class RecruitmentPresenter
{
  private readonly RecruitmentModel model;
  private readonly RecruitmentView view;

  public RecruitmentPresenter(RecruitmentModel model, RecruitmentView view)
  {
    this.model = model;
    this.view = view;

    Initialize();
    InitViewData();
  }

  #region Init
  private void Initialize()
  {
    view.InitUnlockSlot();
  }

  private void InitViewData()
  {
    for (int i = 0; i < view.RecruitSlots.Length; i++)
    {
      int slotIndex = i + 1;

      var slotData = model.GetPartnerExploreSlotData(slotIndex);

      RecruitmentSlot slot = view.RecruitSlots[i];

      slot.view  = view;
      slot.model = model;

      slot.InitExploreSlotData(slotData);
      slot.InitSlotEvent();

      slot.OnLoadInven = view.OnLoadInven;

      view.OnCloseCountDown += slot.InitCountDown;
    }
  }
  #endregion


  public void SetData()
  {
    UpdateRecruitSlots();
  }

  private void UpdateRecruitSlots()
  {
    Dictionary<int, UserRecruitInfoDTO> userRecruitInfoDataDict = model.GetRecruitInfoDict();

    foreach (var recruitInfoData in userRecruitInfoDataDict)
    {
      UpdateRecruitSlot(recruitInfoData.Value);
    }
  }

  private void UpdateRecruitSlot(UserRecruitInfoDTO recruitInfo)
  {
    int slotIndex = recruitInfo.slotNum - 1;
    int totalSecend = CodeUtility.GetTotalSeconds(recruitInfo.endDt);

    bool isRemainTime = totalSecend > 0;

    var recruitSlot = GetRecruitSlot(slotIndex);

    recruitSlot.InitSlot();

    recruitSlot.OnExploreStart    = () => OnRecruitStart(recruitInfo.slotNum);
    recruitSlot.OnExploreCancel   = () => OnRecruitCancel(recruitInfo.slotNum);
    recruitSlot.OnExploreComplete = () => OnRecruitComplete(recruitInfo.slotNum);

    recruitSlot.SetViewData();

    recruitSlot.SetData(recruitInfo, isRemainTime);

    if(isRemainTime)
      recruitSlot.SetCountDown(totalSecend);

    UpdateMountItem();
  }

  private RecruitmentSlot GetRecruitSlot(int slotIndex)
  {
    return view.RecruitSlots[slotIndex];
  }

  private void UpdateMountItem()
  {
    model.ClearMountList();

    for (int i = 0; i < view.RecruitSlots.Length; i++)
    {
      using (var enumerator = view.RecruitSlots[i].GetInvenItemList().GetEnumerator())
      {
        while (enumerator.MoveNext())
        {
          model.AddMountList(enumerator.Current);
        }
      }

    }
  }


  #region API
  /// <summary>
  /// 동료 모집소 모집 시작
  /// </summary>
  /// <param name="slotNo">슬롯 번호</param>
  /// <param name="mountPartnerList">장착한 동료 InvenIdx List</param>
  private async void OnRecruitStart(int slotNo)
  {
    Debug.Log("OnRecruitStart");

    int slotIndex = slotNo - 1;

    RecruitmentSlot slot = GetRecruitSlot(slotIndex);

    await APIManager.getInstance.REQ_RecruitmentStart<RES_RecruitmentStart>(
      slotNo,
      slot.GetInvenIndexList(),
      (responseResult) => {
        UpdateRecruitSlot(responseResult.userRecruitInfoDTO);
      }
    );

  }

  /// <summary>
  /// 동료 모집소 취소
  /// </summary>
  /// <param name="slotNo">슬롯 번호</param>
  /// <param name="updateStatus">변경할 상태</param>
  private async void OnRecruitCancel(int slotNo)
  {
    UIUtility.ShowNotificationThreePopup(
      "알림",
      "탐험 취소 또는 제작 가속을 하시겠습니까?",

      confirmAction: async () =>
      {

        await UniTask.DelayFrame(1);

        UIUtility.ShowNotificationPopup(
        "알림",
        "탐험을 취소 하시겠습니까?",
        () =>
        {
          Debug.Log($"OnRecruitCancel");

          OnRecruitUpdateStatus(slotNo, 0);
        }
        );

      },
      subAction: async () => {
        var popup = await NewUIManager.getInstance.Show<EquipmentFastTimePopupController>("FantasyMercenary/Popup/Equipment/EquipmentFastTimePopup");
        popup.SetLeftTime(10000);
      },
      confirmText: "탐험 취소",
      subText: "가속",
      cancelText: "닫기"
      );

  }

  /// <summary>
  /// 동료 모집소 보상 획득
  /// </summary>
  /// <param name="slotNo">슬롯 번호</param>
  /// <param name="updateStatus">변경할 상태</param>
  private async void OnRecruitComplete(int slotNo)
  {
    Debug.Log($"OnRecruitComplete");

    OnRecruitUpdateStatus(slotNo, 1);
  }

  /// <summary>
  /// 동료 모집소 업데이트
  /// </summary>
  /// <param name="slotNo">슬롯 번호</param>
  /// <param name="updateStatus">변경할 상태</param>
  private async void OnRecruitUpdateStatus(int slotNo, int updateStatus)
  {
    await APIManager.getInstance.REQ_RecruitmentUpdateStatus<RES_RecruitmentUpdateStatus>(
      slotNo,
      updateStatus,
      (responseResult) => {

        UpdateRecruitSlot(responseResult.userRecruitInfoDTO);

        if(responseResult.rewardList != null && responseResult.rewardList.Count > 0)
          UIUtility.ShowRewardItemPopup(responseResult.rewardList);
      }
    );
  }

  #endregion

}
