using Assets.ZNetwork.Data;
using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class GameDataManager : LazySingleton<GameDataManager>
{
    public UserInfoModel userInfoModel = new UserInfoModel();
    public UserShopData userShopData = new UserShopData();
    public UserContentsData userContentsData = new UserContentsData();
    public UserCostumeData costumeData = new UserCostumeData();
    
    public bool isLogin = false;

    private string accountId;
    private string authToken;

    public int currentEquipPresetNum = 1;//장비 프리셋 기본 번호
    public int currentSkillPresetNum = 1;//스킬 프리셋 번호
    private int currentPartnerPresetNum = 1; //파트너 프리셋 번호
    public int waveTotalGold = 0;

    public Dictionary<long, InvenData> dictEquip = new Dictionary<long, InvenData>();
    public Dictionary<long, InvenData> dictPending = new Dictionary<long, InvenData>();
    public Dictionary<long, InvenData> dictSkill = new Dictionary<long, InvenData>();
    public Dictionary<long, InvenData> dictPartner = new Dictionary<long, InvenData>();
    public Dictionary<int, InvenData> dictConsume = new Dictionary<int, InvenData>();


    public Dictionary<int, PresetData> equipPresetMap = new Dictionary<int, PresetData>(); //장비
    public Dictionary<int, PresetData> skillPresetMap = new Dictionary<int, PresetData>(); //스킬
    public Dictionary<int, PresetData> partnerPresetMap = new Dictionary<int, PresetData>(); //동료

    

    public int stageIndex { get; set; }
    public int chapterIndex { get; set; }
    public int regionIndex { get; set; }

    public int currentWave { get; set; }
    public int currentStage { get; set; }
    public int currentChapter { get; set; }
    public int currentRegion { get; set; }

    public bool isRepeatBattle;
    public bool isBossWave;
    public bool isRequestWaveEndPacket;
    public bool isEquipGachaLeveling;

    public long levelUpTimeLeft;

    public int middleBossWave;
    public int stageBossWave;
    public int equipmentGachaCount; // 장비 뽑기 수량
    public float clearLimitTime;

    private MNMRandom _lampRandom;

    #region UserInfoData
    public void SetAccountId(string _accountId)
    {
        accountId = _accountId;
    }
    public void SetAuthToken(string _authToken)
    {
        authToken = _authToken;
    }

    public void SetUserInfoData(UserInfoData userInfoData)
    {
        this.userInfoModel.userInfoData = userInfoData;
    }

    public void SetCostumeData(UserCostumeData userCostumeData)
    {
        costumeData = userCostumeData;
    }

    public void SetEquipData(List<InvenData> equipList)
    {
        dictEquip.Clear();

        foreach (var item in equipList)
        {
            // Assume InvenData has a property 'Id' to be used as the key
            dictEquip[item.invenIdx] = item;
        }
    }

    public void SetSkillData(List<InvenData> skillList)
    {
        dictSkill.Clear(); // 기존 데이터 초기화

        foreach (var item in skillList)
        {
            dictSkill[item.invenIdx] = item; // invenIdx를 키로 사용
        }
    }

    public void SetPartnerData(List<InvenData> partnerList)
    {
        dictPartner.Clear(); // 기존 데이터 초기화

        foreach (var item in partnerList)
        {
            dictPartner[item.invenIdx] = item; // invenIdx를 키로 사용
        }
    }

  public void SetConsumeData(List<InvenData> consumeDataList)
  {
    dictConsume.Clear(); // 기존 데이터 초기화

    foreach (var comsumeData in consumeDataList)
      UpdateConsumeData(comsumeData);
  }

  public void UpdateConsumeData(InvenData consumeData)
  {
    long itemCount = consumeData.itemCount;

    if (itemCount != 0)
      dictConsume[consumeData.itemIdx] = consumeData; // itemIdx를 키로 사용
    else
      dictConsume.Remove(consumeData.itemIdx);

    CurrencyManager.getInstance.SetCurrencyData(consumeData.itemIdx, consumeData.itemCount);
  }

    public void PresetDataList(List<PresetData> presetList)
    {

        foreach (var presetData in presetList)
        {
            PresetType presetType = (PresetType)presetData.presetType;

            switch (presetType)
            {
                case PresetType.Equipment:
                    if (!equipPresetMap.ContainsKey(presetData.presetNumber))
                    {
                        equipPresetMap.Add(presetData.presetNumber, presetData);
                    }

                    break;
                case PresetType.Skill:
                    if (!skillPresetMap.ContainsKey(presetData.presetNumber))
                    {
                        skillPresetMap.Add(presetData.presetNumber, presetData);
                    }
                    break;
                case PresetType.Partner:
                    if (!partnerPresetMap.ContainsKey(presetData.presetNumber))
                    {
                        partnerPresetMap.Add(presetData.presetNumber, presetData);
                    }
                    break;
            }
        }
    }


  public void UpdateInvenData(List<InvenData> invenDataList)
  {
    if (invenDataList == null)
      return;

    int dataCount = invenDataList.Count;

    for (int i = 0; i < dataCount; i++)
    {
      if (invenDataList[i] == null)
        continue;

      InvenData invenData = invenDataList[i];

      long invenIdx = invenData.invenIdx;
      int itemGroup = invenData.itemGroup;
      long itemCount = invenData.itemCount;

      bool isPartnerSkill = itemGroup == (int)ItemGroup.Partner || itemGroup == (int)ItemGroup.Skill;

      var targetMap = GetTargetMap((ItemGroup)itemGroup);

      if(targetMap != null)
      {
        if (itemCount != 0 || isPartnerSkill)
          targetMap[invenIdx] = invenData;
        else
          targetMap.Remove(invenIdx);



      }
      else
      {
        switch ((ItemGroup)itemGroup)
        {
          case ItemGroup.Gold:
            userInfoModel.SetGold(invenData.itemCount);
            
            break;
          case ItemGroup.Gem:
            userInfoModel.SetGem(invenData.itemCount);
            break;
          case ItemGroup.BlessPoint:
            userInfoModel.SetBlessingPoint(invenData.itemCount);
            break;



          case ItemGroup.Ticket_Equipment:
          case ItemGroup.Ticket_Partner:
          case ItemGroup.Ticket_Skill:
          case ItemGroup.LotteryTicket:
          case ItemGroup.CartEnhance:
            UpdateConsumeData(invenData);
            break;
          default:
            break;
        }
      }
    }
  }

  /// <summary>
  /// 소모 재화 보유량 반환
  /// </summary>
  /// <param name="itemIdx"></param>
  /// <returns></returns>
  public long GetConsumeValue(int itemIdx)
  {
    if (dictConsume.ContainsKey(itemIdx))
    {
      return dictConsume[itemIdx].itemCount;
    }
    else
    {
      ItemData itemData = ItemTable.getInstance.GetItemData(itemIdx);

      return (ItemGroup)itemData.itemGroup switch
      {
        ItemGroup.Gold => userInfoModel.GetGold(),
        ItemGroup.Gem => userInfoModel.GetGem(),
        ItemGroup.BlessPoint => userInfoModel.GetBlessingPoint(),
        ItemGroup.ResearchFunds => userContentsData.researchFunds,
        _ => 0,
      };
    }
  }
    public void UpdatePreset(PresetData presetData)
    {
        if (presetData == null)
            return;

        if (!Enum.IsDefined(typeof(PresetType), presetData.presetType))
            return;

        PresetType presetType = (PresetType)presetData.presetType;
        if (presetData.presetNumber < 0)
            return;
        switch (presetType)
        {
            case PresetType.Equipment:
                this.equipPresetMap[presetData.presetNumber] = presetData;
                break;
            case PresetType.Skill:
                this.skillPresetMap[presetData.presetNumber] = presetData;
                break;
            case PresetType.Partner:
                this.partnerPresetMap[presetData.presetNumber] = presetData;
                break;
        }
    }
    public string GetAccountID()
    {
        return accountId;
    }
    public string GetAuthToken()
    {
        return authToken;
    }
    public void Logout()
    {
        isLogin = false;
        // 유저 정보
        userInfoModel.userInfoData = new UserInfoData();
    }

    public void SetSkillPresetNum(int presetNum)
    {
        this.currentSkillPresetNum = presetNum;
    }

    public void SetPartnerPresetNum(int presetNum)
    {
        this.currentPartnerPresetNum = presetNum;
    }
    public void SetEquipPresetNum(int presetNum)
    {
        this.currentEquipPresetNum = presetNum;
    }

    #endregion
    public void SetBossWaveData()
    {
        WaveData data = WaveTable.getInstance.GetWaveData(IdleStageTable.getInstance.GetStageData(stageIndex).waveType);
        if (data.waveType != 0) // 해당 StageIdx에 해당하는 waveData가 존재 할때..
        {
            middleBossWave = data.middleBossWave;
            if (data.bossWave == 1)
            {
                stageBossWave = data.waveNum;
            }
        }

        if (data.timeType != 0) // 해당 waveData의 클리어 제한 시간 타입이 존재 한다면..
        {
            clearLimitTime = data.clearLimitTime;
        }
    }

    public MNMRandom GetLampRandom()
    {
        return _lampRandom;
    }
    public void SetLampRandom(long seed)
    {
        _lampRandom = new MNMRandom(seed);
    }

    public void UpdateLampRandom(long seed)
    {
        _lampRandom.UpdateSeed(seed);
    }


    public List<InvenData> GetPresetInvenDataList(PresetData presetData)
    {
        return GetPresetInvenDataList((PresetType)presetData.presetType, presetData.mountSlot);
    }

    public List<InvenData> GetPresetInvenDataList(PresetType presetType)
    {
        PresetData presetData = GetTargetPreset(presetType);

        return GetPresetInvenDataList(presetType, presetData.mountSlot);
    }

    public List<InvenData> GetPresetInvenDataList(PresetType presetType, long[] mountSlotIndexs)
    {
        List<InvenData> invenDataList = new List<InvenData>();
        Dictionary<long, InvenData> itemMap = GetTargetMap(presetType);

        for (int i = 0; i < mountSlotIndexs.Length; i++)
        {
            long mountSlotIndex = mountSlotIndexs[i];

            if (mountSlotIndex != 0)
            {
                invenDataList.Add(itemMap[mountSlotIndex]);
            }
            else
            {
                invenDataList.Add(null);
            }
        }
        return invenDataList;
    }

  public Dictionary<long, InvenData> GetTargetMap(ItemGroup itemGroup) => itemGroup switch
  {
    ItemGroup.Equipment  => dictEquip,
    ItemGroup.Partner    => dictPartner,
    ItemGroup.Skill      => dictSkill,
    _ => null
  };

    public Dictionary<long, InvenData> GetTargetMap(PresetType presetType) => presetType switch
    {
        PresetType.Equipment => dictEquip,
        PresetType.Partner => dictPartner,
        PresetType.Skill => dictSkill,
    };

    public PresetData GetTargetPreset(PresetType presetType) => presetType switch
    {
        PresetType.Equipment => equipPresetMap[currentEquipPresetNum],
        PresetType.Partner => partnerPresetMap[currentPartnerPresetNum],
        PresetType.Skill => skillPresetMap[currentSkillPresetNum],

    };

}