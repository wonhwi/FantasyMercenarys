using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RestPacket;

public class HopeCenterPresenter
{
  private HopeCenterModel model;
  private HopeCenterView view;

  public HopeCenterPresenter(HopeCenterModel model, HopeCenterView view, HopeCenterUIPopup popUp)
  {
    this.model = model;
    this.view = view;


    view.OnClosePopup += () =>
    {
      view.DisableCurrency();
      popUp.Hide();
    };

    InitEvent();
  }

  private void InitEvent()
  {
    var enumerator = model.GetMetaLotteryTicketKeyData().GetEnumerator();

    while (enumerator.MoveNext())
    {
      int lotteryIdx = enumerator.Current;

      var slot = GetUISlot(lotteryIdx);

      view.OnClosePopup += slot.InitCountDown;
    }
  }


  public void SetData()
  {
    view.EnableCurrency();

    OnHopeCenterLoad();
  }


  private void SetLotteryData()
  {
    var enumerator = model.GetMetaLotteryTicketKeyData().GetEnumerator();

    while (enumerator.MoveNext())
    {
      int lotteryIdx = enumerator.Current;

      SetLotterySlot(lotteryIdx);
      SetLotteryPurchase(lotteryIdx);
      SetLotteryConsume(lotteryIdx);
      SetLotteryCountDown(lotteryIdx);

    }
  }

  private void SetLotterySlot(int lotteryIdx)
  {
    LotteryTicketData lotteryTicketData = model.GetLotteryTicketData(lotteryIdx);

    var slot = GetUISlot(lotteryIdx);

    slot.SetLotteryNameText(lotteryTicketData.lotteryRecordCd);
    slot.SetLotteryImage(lotteryTicketData.lotteryIconImage);
    slot.SetLotteryRewardText(lotteryTicketData.descRecordCd);
  }

  private void SetLotteryPurchase(int lotteryIdx)
  {
    LotteryTicketData lotteryTicketData = model.GetLotteryTicketData(lotteryIdx);

    HopeCenterLotteryData userLotteryData = model.GetUserLotteryData(lotteryIdx);

    var slot = GetUISlot(lotteryIdx);

    slot.SetPurchaseLimitText($"{userLotteryData.lotteryCount}/{lotteryTicketData.lotteryMaxCount}");

    slot.OnPurchase = () => OnScratchLottery(lotteryIdx);
  }

  private void SetLotteryConsume(int lotteryIdx)
  {
    LotteryTicketData lotteryTicketData = model.GetLotteryTicketData(lotteryIdx);

    var slot = GetUISlot(lotteryIdx);

    int consumeTypeCount = 0;

    if (lotteryTicketData.lotteryCurrency1 != 0)
      consumeTypeCount++;

    if (lotteryTicketData.lotteryCurrency2 != 0)
      consumeTypeCount++;

    ConsumeRequirement[] requirements = new ConsumeRequirement[consumeTypeCount];

    if (lotteryTicketData.lotteryCurrency1 != 0)
    {
      requirements[0] = new ConsumeRequirement() { 
        itemIdx = lotteryTicketData.lotteryCurrency1, 
        itemCount = lotteryTicketData.lotteryCurrency1Value 
      };
    }

    if (lotteryTicketData.lotteryCurrency2 != 0)
    {
      requirements[1] = new ConsumeRequirement()
      {
        itemIdx = lotteryTicketData.lotteryCurrency2,
        itemCount = lotteryTicketData.lotteryCurrency2Value
      };
    }

    slot.SetConsumeData(requirements);
  }

  private void SetLotteryCountDown(int lotteryIdx)
  {
    LotteryTicketData lotteryTicketData = model.GetLotteryTicketData(lotteryIdx);

    HopeCenterLotteryData userLotteryData = model.GetUserLotteryData(lotteryIdx);

    var slot = GetUISlot(lotteryIdx);

    TimeSpan timeDifference = DateTime.UtcNow.AddHours(9) - userLotteryData.lastChargeTime;

    int lotteryRechargeSecond = lotteryTicketData.lotteryRechargeMinutes * 60;

    int totalSeconds = 0;
    
    if(lotteryRechargeSecond != 0)
    {
      totalSeconds = lotteryRechargeSecond - (int)timeDifference.TotalSeconds % lotteryRechargeSecond;
    }
    else
    {
      totalSeconds = -(int)timeDifference.TotalSeconds;
    }

    bool useCountDown = totalSeconds > 0 && userLotteryData.lotteryCount < lotteryTicketData.lotteryMaxCount;

    slot.InitCountDown();

    slot.OnCountComplete = useCountDown ? OnHopeCenterLoad : null;

    if (useCountDown)
    {
      slot.SetCountDown(totalSeconds);
    }
      
  }


  /// <summary>
  /// 아이템 슬롯 반환 함수
  /// </summary>
  /// <param name="slotIdx"></param>
  /// <returns></returns>
  private HopeCenterUISlot GetUISlot(int lotteryIdx)
  {
    int slotIndex = model.GetDataIndex(lotteryIdx);

    return view.HopeUISlots[slotIndex];
  }

  #region API
  public async void OnHopeCenterLoad()
  {
    await APIManager.getInstance.REQ_HopeCenterLoad<RES_HopeCenterLoad>((responseResult) =>
    {
      SetLotteryData();
    });
  }

  public async void OnScratchLottery(int lotteryIdx)
  {
    HopeCenterLotteryData userLotteryData = model.GetUserLotteryData(lotteryIdx);
    
    var slot = GetUISlot(lotteryIdx);

    bool isEnought = slot.GetConsumeIsEnought();
    bool isSoldOut = userLotteryData.lotteryCount < 1;

    if (!isEnought)
    {
      UIUtility.ShowToastMessagePopup("재화 부족");
      return;
    }

    if(isSoldOut)
    {
      UIUtility.ShowToastMessagePopup("재고 부족");
      return;
    }

    await APIManager.getInstance.REQ_HopeCenterRcv<RES_HopeCenterRcv>(lotteryIdx, (responseResult) =>
    {
      view.SetAnimation();
      SetLotteryPurchase(lotteryIdx);

      SetLotteryCountDown(lotteryIdx);

      view.OnScratchComplete = () =>
      {
        UIUtility.ShowRewardItemPopup(responseResult.rewardList);
      };
    });
  }
  #endregion

}
