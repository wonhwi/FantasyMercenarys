using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RestPacket;
using SlotState = BreedingCreatureSlot.SlotState;

public class BreedingGroundPresenter
{
  private BreedingGroundModel model;
  private BreedingGroundView view;
  private BreedingGroundUIPopup popUp;


  public BreedingGroundPresenter(BreedingGroundModel model, BreedingGroundView view, BreedingGroundUIPopup popUp)
  {
    this.model = model;
    this.view = view;
    this.popUp = popUp;

    InitEvent();
  }

  #region Init
  private void InitEvent()
  {
    view.OnClosePopup += () =>
    {
      view.InitCreature();
      view.InitCreatureSlot();
      view.InitObject();
      popUp.Hide();
    };

    view.OnUpgradeVault = () =>
    {
      if (!view.GetConsumeIsEnought())
        UIUtility.ShowToastMessagePopup("강화 재료가 부족합니다");
      else
        OnVaultUpgrade();
    };

    view.OnBoostVault = async () => {
      var popup = await NewUIManager.getInstance.Show<EquipmentFastTimePopupController>("FantasyMercenary/Popup/Equipment/EquipmentFastTimePopup");
      popup.SetLeftTime(10000);
    };
    view.OnMaxVault = () => UIUtility.ShowToastMessagePopup("이미 최고 레벨의 금고입니다.");

    view.OnCloseInven = popUp.OnCloseInven;
    view.OnViewInven = SetInventorySelect;
    view.OnSelectInven = (int itemIdx) => OnSlotMount(view.selectSlotIdx, itemIdx);
    view.OnSelect = () => {

      int selectItemIdx = popUp.GetSelectItemIdx();

      CreatureData creatureData = model.GetCreatureData(selectItemIdx);

      bool hasItem = creatureData != null;
      
      if (hasItem)
      {

        bool isMount = model.IsMountSlot(selectItemIdx);

        if(!isMount)
          view.OnSelectInven?.Invoke(selectItemIdx);
        else
          UIUtility.ShowToastMessagePopup("이미 배치된 생명체입니다");
      }
      else
      {
        UIUtility.ShowToastMessagePopup("소유하고 있지 않은 생명체 입니다");
      }
      
    };


    int creatureSlotCount = view.CreatureSlots.Length;

    for (int i = 0; i < creatureSlotCount; i++)
    {
      int index = i;

      int slotIdx = index + 1;

      var creatureSlot = GetCreatureSlot(index);

      creatureSlot.OnClickSlot = () => { 
        popUp.OnLoadInven();
        view.selectSlotIdx = slotIdx;
      };

      creatureSlot.OnUnMount = () => OnSlotUnMount(slotIdx);
      creatureSlot.OnLock = () => InitSlotUnLockEvent(index);

    }

  }

  private void InitSlotUnLockEvent(int slotIdx)
  {
    if (slotIdx == 0)
      return;

    var unLockData = SlotUnlockConditionTable.getInstance.GetSlotUnlockConditionData(SlotUnlockContentType.BreedingGrounds, slotIdx + 1);

    if(unLockData.unlockConditionType == SlotUnlockConditionType.ResearchBuy)
    {
      UIUtility.ShowNotificationConsumePopup(
        "알림", 
        "연구비를 소모하여 슬롯 해금하시겠습니까?", 
        ConstantManager.ITEM_CURRENCY_RESEARCH_FUNDS, 
        unLockData.unLockValue, 
        () => OnSlotOpen(slotIdx + 1));
    }
    else if(unLockData.unlockConditionType == SlotUnlockConditionType.MerchantGuild)
    {
      var skillTreeData = MerchantGuildTable.getInstance.dictSkillTreeData[unLockData.unLockValue];

      UIUtility.ShowToastMessagePopup($"{skillTreeData.skillDesc} 연구 시 해제");
    }
  }
  #endregion

  private void SetInventorySelect(int itemIdx)
  {
    ItemData itemData = ItemTable.getInstance.GetItemData(itemIdx);

    BreedingCreatureData creatureMetaData = model.GetMetaCreatureData(itemData.itemIdx);

    CreatureData creatureData = model.GetCreatureData(itemData.itemIdx);

    bool hasCreature = creatureData != null;

    view.SetInventoryIcon(itemData.iconImage);
    view.SetInventoryDetail(creatureMetaData);
    view.SetInventoryConditionActive(hasCreature);

    if (hasCreature)
    {
      view.SetInventoryCondition(creatureData.conditionValue);
    }
  }


  public void SetData()
  {
    OnBreedingGroundsLoad();
  }

  public async void OnBreedingGroundsLoad()
  {
    await APIManager.getInstance.REQ_BreedingGroundsLoad<RES_BreedingGroundsLoad>((_) =>
    {
      SetCreatureSlots();
      UpdateResearchPerText();
      SetResearchPoint();
      SetVault();

      view.RunCoroutine(OnCreateGroundObject());
    });
  }

  public void SetCreatureSlots()
  {
    int[] slotList = model.GetSlotList();

    for (int i = 0; i < slotList.Length; i++)
    {
      int slotIdx = i;
      int slotValue = slotList[i];

      SetCreatureSlot(slotIdx, slotValue);
    }
  }

  /// <summary>
  /// 하단 생명체 슬롯 UI 설정
  /// </summary>
  /// <param name="slotIdx"></param>
  /// <param name="slotValue"></param>
  public void SetCreatureSlot(int slotIdx, int slotValue)
  {
    SlotState slotState = SlotState.Lock;

    if (slotValue == 0)
      slotState = SlotState.Empty;
    else if (slotValue > 0)
      slotState = SlotState.Mount;

    var creatureSlot = GetCreatureSlot(slotIdx);      //하단 UI Slot
    var creature = GetGroundCreture(slotIdx);         //운동장 도는 크리쳐

    creatureSlot.InitCreatureSlot();
    creatureSlot.SetSlot(slotState);

    //현재 생명체가 장착되어있는 상태
    if(slotState == SlotState.Mount)
    {
      int creatureIdx = slotValue;

      CreatureData creatureData = model.GetCreatureData(creatureIdx);

      int conditionType = creatureData.conditionValue;

      BreedingCreatureData creatureMetaData = BreedingTable.getInstance.GetCreatureData(creatureIdx);

      //트랙 길이
      int trackLength = model.GetTrackLength();

      //크리쳐 속도
      int creatureSpeed = creatureMetaData.creatureSpeed;

      //크리쳐 컨디션 배율
      float conditionValue = model.GetConditionValue(conditionType);

      //한 바퀴 보상 메타 값
      int trackRewardValue = model.GetRewardTrackCycle();

      //상인협회 생명체 속도 증가량
      float merchantCreatureSpeedPercent = model.GetSkillTypeValue(MerchantGuildSkillType.CreatureSpeedUp) * 0.01f;

      //상인협회 연구비 획득 증가량
      float merchantReseachGainPercent = model.GetSkillTypeValue(MerchantGuildSkillType.ResearchGainUp) * 0.01f;

      //크리쳐 최종 속도
      int totalSpeed = Mathf.FloorToInt(trackLength / ((creatureSpeed * conditionValue) * (1f + merchantCreatureSpeedPercent)));
      
      //트랙 한 바퀴 돌았을때 획득 값 
      int gainTrackCycle = Mathf.FloorToInt((trackRewardValue * (1f + merchantReseachGainPercent) * 100f) * 0.01f);

      //초당 획득 값
      int gainPerSecond = Mathf.FloorToInt(gainTrackCycle / totalSpeed);

      creatureSlot.SetData(creatureMetaData.creatureIcon, conditionType, creatureMetaData.maxResearchPoints);
      creatureSlot.SetPointGauge(creatureData.getPoint);
      creatureSlot.StartTrackCoroutine(gainPerSecond);

      Debug.LogError($"크리쳐 스피드 {creatureSpeed}, 컨디션 {conditionValue}, 상인협회 속도 증가량 {merchantCreatureSpeedPercent * 100f}%");
      Debug.LogError($"크리쳐 최종 속도{(creatureSpeed * conditionValue) * (1f + merchantCreatureSpeedPercent)}");
      Debug.LogError($"트랙 길이{trackLength}, 한 바퀴 도는데 걸리는 시간{totalSpeed}");
      Debug.LogError($"한 바퀴 보상 값 {trackRewardValue}, 상인협회 보상 증가량 {merchantReseachGainPercent * 100f}%");
      Debug.LogError($"트랙 한 바퀴 돌았을떄 최종 보상 값 : {gainTrackCycle}, 초당 획득 값 {gainPerSecond}, 분당 획득 값 {gainPerSecond * 60}");


      creature.SetCreatureImage(creatureMetaData.creatureIcon);
      creature.SetCreautureAnimation(totalSpeed);
    }
    else
    {
      creature.InitCreature();
    }

  }

  private void UpdateResearchPerText()
  {
    int[] slotList = model.GetSlotList();

    float totalGainPerMinute = 0;

    for (int i = 0; i < slotList.Length; i++)
    {
      int slotValue = slotList[i];

      //슬롯이 잠금이거나, 비어있지 않으면
      if(slotValue != -1 && slotValue != 0)
      {
        int itemIdx = slotValue;

        CreatureData creatureData = model.GetCreatureData(itemIdx);

        int trackLength = model.GetTrackLength();

        //크리쳐 컨디션 배율
        float conditionValue = model.GetConditionValue(creatureData.conditionValue);

        float merchantCreatureSpeedPercent = model.GetSkillTypeValue(MerchantGuildSkillType.CreatureSpeedUp) * 0.01f;
        float merchantReseachGainPercent = model.GetSkillTypeValue(MerchantGuildSkillType.ResearchGainUp) * 0.01f;

        int gainTrackCycle = Mathf.RoundToInt((model.GetRewardTrackCycle() * (1f + merchantReseachGainPercent) * 100f) * 0.01f);

        //크리쳐 속도
        float creatureSpeed = trackLength / ((model.GetMetaCreatureData(itemIdx).creatureSpeed * conditionValue) * (1f + merchantCreatureSpeedPercent));

        //초당 획득 값
        float gainPerSecond = gainTrackCycle / creatureSpeed;
        //분당 획득 값
        float gainPerMinute = gainPerSecond * 60;


        totalGainPerMinute += gainPerMinute;
      }

    }

    view.SetResearchPerText((int)totalGainPerMinute);
  }

  public void SetResearchPoint()
  {
    int vaultLv = model.GetVaultLevel();

    BreedingResearchData breedingResearchData = model.GetVaultResearchData(vaultLv);

    int maxResearchPoints = breedingResearchData.maxResearchPoints;

    int merchantResearchLimit = (int)model.GetSkillTypeValue(MerchantGuildSkillType.ResearchMaxLimitUp);

    view.SetResearchPoint(maxResearchPoints + merchantResearchLimit);
  }

  public void SetVault()
  {
    int vaultLv = model.GetVaultLevel();
    int vaultMaxLv = model.GetValueMaxLevel();

    bool isMaxLv = vaultLv == vaultMaxLv;
    bool isUpgrade = model.GetIsUpgrade();

    SetVaultState(isUpgrade, isMaxLv);

    if (!isMaxLv)
    {
      if(!isUpgrade)
      {
        BreedingResearchData breedingResearchData = model.GetVaultResearchData(vaultLv + 1);

        int upgradeCost = breedingResearchData.upgradeCost;

        ConsumeRequirement requirement = new ConsumeRequirement()
        {
          itemIdx = ConstantManager.ITEM_CURRENCY_RESEARCH_FUNDS,
          itemCount = upgradeCost
        };

        view.SetConsumeData(requirement, breedingResearchData.researchTime);

      }
      else
      {
        SetVaultCountDown();
      }
    }
    
  }

  /// <summary>
  /// 금고 버튼 활성화/비활성화
  /// </summary>
  public void SetVaultState(bool isUpgrade, bool isVaultLvMax)
  {
    view.SetVaultState(isUpgrade, isVaultLvMax);
  }

  private void SetVaultCountDown()
  {
    int totalSeconds = CodeUtility.GetTotalSeconds(model.GetUpgradeCompleteAt());

    view.bundleCountDownText.OnComplete = async () =>
    {
      view.upgradingButton.gameObject.SetActive(false);

      await APIManager.getInstance.REQ_BreedingGroundsLoad<RES_BreedingGroundsLoad>((_) =>
      {
        SetResearchPoint();
        SetVault();
      });
    };
    


    view.bundleCountDownText.InitCountDown();

    view.bundleCountDownText.SetCountDown(totalSeconds);
  }


  private IEnumerator OnCreateGroundObject()
  {
    float createIntervalTime = model.GetObjectCreateInterval();

    int createLimit = model.GetObjectCreateLimit();

    float time = 0f;

    while(true)
    {
      if(time >= createIntervalTime)
      {
        bool isDailyPointMax = model.IsDailyPointMax();

        if (!isDailyPointMax)
        {
          int activeCount = view.GetActiveObject();

          if (activeCount < createLimit)
          {
            //생성하기
            GroundObject groundObject = view.GetGroundObject();

            GroundObjectType groundObjectType = model.GetObjectType();

            int objectIdx = groundObject.GetSiblingIndex();

            groundObject.SetData(groundObjectType);

            groundObject.OnPointGain = () => OnPointGain(objectIdx, groundObjectType);
          }
        }
        else
        {
          Debug.Log("일일 획득 포인트 달성으로 아무것도 하지 않음");
        }

        time = 0f;
      }
      yield return YieldInstructionCache.WaitForSeconds(0.1f);
      time += 0.1f;
    }

  }

  private GroundCreature GetGroundCreture(int slotIdx)
  {
    return view.GroundCreatures[slotIdx];
  }

  private BreedingCreatureSlot GetCreatureSlot(int slotIdx)
  {
    return view.CreatureSlots[slotIdx];
  }

  private GroundObject GetGroundObject(int slotIdx)
  {
    return view.GroundObjects[slotIdx];
  }


  #region API
  /// <summary>
  /// 사육장 슬롯 오픈
  /// </summary>
  public async void OnSlotOpen(int slotNo)
  {
    int slotIdx = slotNo - 1;

    await APIManager.getInstance.REQ_BreedingSlotOpen<RES_BreedingSlotAction>(slotNo, (responseResult) =>
    {
      SetCreatureSlot(slotIdx, 0);
      SetResearchPoint();
    });
  }

  /// <summary>
  /// 사육장 슬롯 장착
  /// </summary>
  public async void OnSlotMount(int slotNo, int creatureIdx)
  {
    int slotIdx = slotNo - 1;

    await APIManager.getInstance.REQ_BreedingSlotMount<RES_BreedingSlotAction>(slotNo, creatureIdx, (responseResult) =>
    {
      CreatureData creatureData = responseResult.creatureData;

      //슬롯 장착
      if(creatureData != null)
      {
        SetCreatureSlot(slotIdx, creatureData.creatureIdx);
      }

      popUp.OnCloseInven();
      UpdateResearchPerText();
    });
  }

  /// <summary>
  /// 사육장 슬롯 해제
  /// </summary>
  public async void OnSlotUnMount(int slotNo)
  {
    int slotIdx = slotNo - 1;

    await APIManager.getInstance.REQ_BreedingPointGain<RES_BreedingPointGain>(4, slotNo, (responseResult) =>
    {
      var creatureSlot = GetCreatureSlot(slotIdx);

      SetCreatureSlot(slotIdx, 0);

      if(responseResult.rewardPoint > 0)
        creatureSlot.SetRewardPoint(responseResult.rewardPoint);

      UpdateResearchPerText();
      SetResearchPoint();
    });
  }


  /// <summary>
  /// 사육장 오브젝트 클릭으로 포인트 획득
  /// </summary>
  public async void OnPointGain(int objectIndx, GroundObjectType groundObjectType)
  {
    await APIManager.getInstance.REQ_BreedingPointGain<RES_BreedingPointGain>(5, (int)groundObjectType, (responseResult) =>
    {
      GroundObject groundObject = GetGroundObject(objectIndx);

      groundObject.SetRewardPoint(responseResult.rewardPoint);

      SetResearchPoint();
      SetVault();
    });
  }

  /// <summary>
  /// 사육장 금고 업그레이드
  /// </summary>
  public async void OnVaultUpgrade()
  {
    int updateLv = model.GetVaultLevel() + 1;

    await APIManager.getInstance.REQ_BreedingGroundsUpgrade<RES_BreedingGroundsUpgrade>(updateLv, (responseResult) =>
    {
      SetVault();
      SetResearchPoint();
    });
  }
  #endregion

}
