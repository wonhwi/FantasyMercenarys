using Assets.ZNetwork.Data;
using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RestPacket;

public class AdventureGuildPresenter
{
  private readonly AdventureGuildQuestTable tableModel;
  private readonly ContentAdventureGuild runtimeModel;
  private readonly AdventureGuildUIPopup popupRoot;
  private readonly AdventureGuildView view;

  private readonly int coolTime;

  public AdventureGuildPresenter(AdventureGuildUIPopup popupRoot, AdventureGuildView view)
  {
    this.tableModel   = AdventureGuildQuestTable.getInstance;
    this.runtimeModel = GameDataManager.getInstance.userContentsData.adventureGuild;

    this.popupRoot = popupRoot;
    this.view = view;


    this.coolTime = (int)BaseTable.getInstance.GetBaseValue(ConstantManager.BASE_INDEX_ADVENTURE_GUILD_REFRESH_COOLTIME);

    Initialize();
  }

  public void Initialize()
  {
    view.OnCloseButtonClicked = popupRoot.Hide;
  }


  public void SetData()
  {
    UpdateQuestSlots();
    UpdateGuildInfo();   
  }

  /// <summary>
  /// 모험가 길드에 있는 UI 정보들을 업데이트 하는 함수
  /// 슬롯 상태에 따른 값을 입력해야 UpdateQuestSlots 먼저 실행
  /// </summary>
  private void UpdateGuildInfo()
  {
    int guildLv  = runtimeModel.AdventureGuildLv;
    int guildExp = runtimeModel.AdventureGuildExp;
    int maxRequestAcceptCoutn = runtimeModel.MaxRequestAcceptCount;
    int dailyRequestAcceptableCount  = runtimeModel.DailyAcceptableCount;
    int acceptQuestCount = GetAcceptQuestCount();

    (int minGrade, int maxGrade) = GachaWeightTable.getInstance.GetGachaGradeRange(
        (int)ShopLvIndex.AdventureGuild + guildLv);

    view.SetGradeRangeText(minGrade, maxGrade);
    view.SetGuildLvInfo(guildLv, guildExp);
    view.SetQuestCountInfo(dailyRequestAcceptableCount, maxRequestAcceptCoutn, acceptQuestCount);
  }

  private void UpdateQuestSlots()
  {
    Dictionary<int, GuildQuestInfoData> guildQuestInfoDataDict = runtimeModel.GetQeustInfoDict();

    foreach (var questInfo in guildQuestInfoDataDict)
    {
      UpdateQuestSlot(questInfo.Value);
    }
  }

  private void UpdateQuestSlot(GuildQuestInfoData questInfo)
  {
    int slotIndex = questInfo.questOrder - 1;

    var questSlot = GetQuestSlot(slotIndex);
    int remainingTime = CodeUtility.GetTotalSeconds(questInfo.questAcceptedTime) + coolTime;

    questSlot.OnAcceptAction   = () => OnQuestAccept(questInfo);
    questSlot.OnCompleteAction = () => OnQuestComplete(questInfo);
    questSlot.OnCancelAction   = () => OnQuestCancel(questInfo);
    questSlot.OnRefreshAction  = () => OnQuestRefresh(questInfo);
    
    questSlot.SetData(questInfo);
    questSlot.SetRefreshCoolTime(remainingTime);
  }

  private AdventureGuildQuestSlot GetQuestSlot(int slotIndex)
  {
    return view.QuestSlotArray[slotIndex];
  }

  /// <summary>
  /// 현재 진행 중인 퀘스트 슬롯 갯수 반환
  /// </summary>
  /// <returns></returns>
  private int GetAcceptQuestCount()
  {
    int acceptCount = 0;

    for (int i = 0; i < view.QuestSlotArray.Length; i++)
    {
      if(view.QuestSlotArray[i].IsAcceptState())
        acceptCount++;
    }

    return acceptCount;
  }

  #region Button API

  private async void OnQuestAccept(GuildQuestInfoData questInfo)
  {
    if(runtimeModel.DailyAcceptableCount <= 0)
    {
      UIUtility.ShowToastMessagePopup("더 이상 의뢰 수락 불가");
      return;
    }

    Debug.Log("OnQuestAccept");

    await APIManager.getInstance.REQ_AdventureGuildQuestStatusEditor<RES_AdventureGuildQuestStatusEditor>(
      questInfo.questOrder, 
      (int)AdventureGuildQuestEventType.Accept,
      (responseResult) => {
        UpdateQuestSlot(responseResult.questInfoData);
        UpdateGuildInfo();
      }
    );
  }

  private async void OnQuestComplete(GuildQuestInfoData questInfo)
  {
    Debug.Log("OnQuestComplete");

    await APIManager.getInstance.REQ_AdventureGuildQuestReward<RES_AdventureGuildQuestReward>(
      questInfo.questOrder,
      (responseResult) => {
        view.ShowReward(responseResult.rewardList);
        UpdateQuestSlot(responseResult.questInfoData);
        UpdateGuildInfo();
      }
    );
  }

  private async void OnQuestCancel(GuildQuestInfoData questInfo)
  {
    Debug.Log("OnQuestCancel");

    await APIManager.getInstance.REQ_AdventureGuildQuestStatusEditor<RES_AdventureGuildQuestStatusEditor>(
      questInfo.questOrder,
      (int)AdventureGuildQuestEventType.Cancel,
      (responseResult) => { 
        UpdateQuestSlot(responseResult.questInfoData);
        UpdateGuildInfo();
      }
    );
  }

  private async void OnQuestRefresh(GuildQuestInfoData questInfo)
  {
    Debug.Log("OnQuestRefresh");

    await APIManager.getInstance.REQ_AdventureGuildQuestStatusEditor<RES_AdventureGuildQuestStatusEditor>(
      questInfo.questOrder,
      (int)AdventureGuildQuestEventType.Refresh,
      (responseResult) => { 
        UpdateQuestSlot(responseResult.questInfoData);
        UpdateGuildInfo();
      }
    );
  }
  #endregion
}
