using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RestPacket;
using SlotState = MailboxView.SlotState;

public class MailboxPresenter
{
  private MailboxModel model;
  private MailboxView view;
  private MailboxUIPopup popUp;

  [Header("Pool")]
  private MailboxSlot mailboxSlotPrefab;
  private readonly List<MailboxSlot> slotPool = new();
  private const int InitialPoolSize = 20;

  public MailboxPresenter(MailboxModel model, MailboxView view, MailboxUIPopup popUp)
  {
    this.model = model;
    this.view = view;
    this.popUp = popUp;

    InitEvent();
    InitSlotPool();
  }

  private void InitEvent()
  {
    view.OnCloseClicked = () => {
      popUp.Hide();
    };

    view.OnMailSlotClicked = OnReadMail;

    view.OnClaimAllRewards = OnClaimAllMail;
    view.OnDeleteReadMail = OnMailDelete;

    view.OnClaimMail = OnClaimMail;
  }

  #region Pool
  private void InitSlotPool()
  {
    mailboxSlotPrefab = Resources.Load<MailboxSlot>(NewResourcePath.PREFAB_UI_MAILBOX_SLOT);

    for (int i = slotPool.Count; i < InitialPoolSize; i++)
      slotPool.Add(CreateSlot());
  }

  private MailboxSlot CreateSlot()
  {
    var slot = GameObject.Instantiate(mailboxSlotPrefab, view.contentsParent);
    slot.gameObject.SetActive(false);
    return slot;
  }

  private void ReturnAllSlots()
  {
    foreach (var slot in slotPool)
      slot.gameObject.SetActive(false);
  }

  private void EnsureSlotCount(int count)
  {
    while (slotPool.Count < count)
      slotPool.Add(CreateSlot());
  }
  #endregion

  #region API

  /// <summary>
  /// 메일 정보 Load
  /// </summary>
  public async void OnMailLoad()
  {
    Debug.Log("OnMailLoad");

    await APIManager.getInstance.REQ_MailLoad<RES_MailLoad>((responseResult) =>
    {
      model.LoadMailDataList(responseResult.mailDataList);

      EnsureSlotCount(responseResult.mailDataList.Count);
      view.SetSlots(slotPool, model.mailDataList);

    });
  }

  /// <summary>
  /// 메일 상세보기
  /// </summary>
  /// <param name="mailIdx"></param>
  public async void OnReadMail(long mailIdx)
  {
    Debug.Log("OnReadMail");

    MailData mailData = model.GetMailData(mailIdx);
    
    //만약 이미 읽은 상태라면 그냥 활성화 시키자
    SlotState slotState = (SlotState)mailData.mailState;

    if (slotState == SlotState.NewMail)
    {
      await APIManager.getInstance.REQ_MailDetail<RES_MailDetail>(mailIdx, (responseResult) =>
      {
        model.UpdateMailData(responseResult.mailData);

        mailData = responseResult.mailData;
      });
    }

    view.ShowDetail(mailData);
    view.SetSlots(slotPool, model.mailDataList);
  }

  /// <summary>
  /// 메일 보상 전체 수령
  /// </summary>
  /// <param name="isBulkReceive"></param>
  /// <param name="mailIdx"></param>
  public async void OnClaimAllMail()
  {
    Debug.Log("OnClaimAllMail");

    await APIManager.getInstance.REQ_ClaimAllMail<RES_MailRewardRcv>((responseResult) =>
    {
      model.LoadMailDataList(responseResult.mailDataList);

      view.SetSlots(slotPool, model.mailDataList);

      UIUtility.ShowMailRewardItemPopup(responseResult.rewardList);
    });
  }

  /// <summary>
  /// 메일 보상 개별 수령
  /// </summary>
  /// <param name="isBulkReceive"></param>
  /// <param name="mailIdx"></param>
  public async void OnClaimMail(long mailIdx)
  {
    Debug.Log("OnClaimMail");

    await APIManager.getInstance.REQ_ClaimMail<RES_MailRewardRcv>(mailIdx, (responseResult) =>
    {
      model.LoadMailDataList(responseResult.mailDataList);

      view.SetSlots(slotPool, model.mailDataList);

      UIUtility.ShowMailRewardItemPopup(responseResult.rewardList);

      view.mailDetail.Hide();
    });
  }

  /// <summary>
  /// 수령한 모든 메일 삭제
  /// </summary>
  public async void OnMailDelete()
  {
    Debug.Log("OnMailDelete");

    await APIManager.getInstance.REQ_MailDelete<RES_MailDelete>((responseResult) =>
    {
      model.LoadMailDataList(responseResult.mailDataList);

      view.SetSlots(slotPool, model.mailDataList);
    });
  }

  #endregion

}
