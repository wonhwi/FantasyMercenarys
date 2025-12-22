using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStatManager : LazySingleton<PlayerStatManager>
{
  #region Player Stat

  /// <summary>
  /// 최종 스텟
  /// </summary>
  public Dictionary<StatType, float> playerTotalStats = new Dictionary<StatType, float>();

  /// <summary>
  /// 보너스 스텟
  /// </summary>
  private Dictionary<StatType, float> bonusStats = new Dictionary<StatType, float>();


  //직업 스텟 [Base]
  //직업 패시브 스킬의 StatType이 체력, 공격력 방어력일 경우는 합산이기 때문에 합쳐줄거야
  public Dictionary<StatType, float> jobStats = new Dictionary<StatType, float>();

  //장비 스텟 [Base]
  public Dictionary<StatType, float> equipmentStats = new Dictionary<StatType, float>();


  //동료, 스킬 보유 스텟 [Bonus]
  public Dictionary<StatType, float> hasPartnerItemStats = new Dictionary<StatType, float>();
  public Dictionary<StatType, float> hasSkillItemStats = new Dictionary<StatType, float>();

  //직업 패시브 스킬 스텟 [Bonus]
  public Dictionary<StatType, float> jobPassiveStats = new Dictionary<StatType, float>();


  //가호석상 스텟 [Bonus]
  public Dictionary<StatType, float> blessingStats = new Dictionary<StatType, float>();

  //수레 레벨에 따른 스텟 [Bonus]
  public Dictionary<StatType, float> cartStats = new Dictionary<StatType, float>();

  //상인 협회 [Bonus]
  public Dictionary<StatType, float> merchantGuildStats = new Dictionary<StatType, float>();

  //버프 스텟을 제외한 총 합 스텟 전투력
  private int totalStatPower;

  public void UpdatePlayerTotalStats()
  {
    InitStat();

    //Base Stat
    //직업 스텟
    StatUtility.CombineStatDic(playerTotalStats, jobStats);

    //장비 스텟
    StatUtility.CombineStatDic(playerTotalStats, equipmentStats);

    //Bonus Stat
    //동료 아이템 보유 효과 스텟
    StatUtility.CombineStatDic(bonusStats, hasPartnerItemStats);

    //스킬 아이템 보유 효과 스텟
    StatUtility.CombineStatDic(bonusStats, hasSkillItemStats);

    //가호 석상 스텟
    StatUtility.CombineStatDic(bonusStats, blessingStats);

    //수레 스텟
    StatUtility.CombineStatDic(bonusStats, cartStats);

    //직업 패시브 스텟
    StatUtility.CombineStatDic(bonusStats, jobPassiveStats);

    //상인 협회 스텟
    StatUtility.CombineStatDic(bonusStats, merchantGuildStats);

    //최종 스텟 계산
    StatUtility.CalculateTotalStat(playerTotalStats, bonusStats);

    //this.totalStatPower = StatUtility.GetStatTotalPower(playerTotalStats);

    InGameEventManager.TriggerEvent(InGameEventType.UpdatePlayerStat);

    
  }

  private void InitStat()
  {
    playerTotalStats.Clear();
    bonusStats.Clear();

    StatType[] playerStats = ConstantManager.STAT_TYPES_PLAYER_ALL_STAT;

    for (int i = 0; i < playerStats.Length; i++)
    {
      StatType statType = playerStats[i];

      playerTotalStats[statType] = 0f;
    }
  }

  /// <summary>
  /// 직업 스텟 업데이트
  /// 1. 로그인 시 업데이트 진행
  /// 2. 직업 교체 API 실행 이후 업데이트 진행
  /// </summary>
  /// <param name="jobData"></param>
  public void UpdateJobStats()
  {
    jobStats.Clear();
    jobPassiveStats.Clear();

    var playerJobStats = NewInGameManager.getInstance.player.statBuffController.GetUnitStats();
    var playerPassiveStats = NewInGameManager.getInstance.player.statBuffController.GetPassiveStats();
    var passiveAddType = ConstantManager.STAT_TYPES_PASSIVE_SKILL_ADD;

    foreach (var kvp in playerJobStats)
    {
      StatType statType = kvp.Key;
      float statValue = kvp.Value;

      jobStats.Add(statType, statValue);
    }

    foreach (var kvp in playerPassiveStats)
    {
      StatType statType = kvp.Key;
      float statValue = kvp.Value;

      //패시브 스킬 중 체 공 방 타입일경우 직업 스텟에 합산을 해줘야 합니다.
      if(passiveAddType.Contains(statType))
      {
        if (!jobStats.ContainsKey(statType))
          jobStats.Add(statType, statValue);
        else
          jobStats[statType] += statValue;
      }
      else
      {
        jobPassiveStats.Add(statType, statValue);
      }
    }

    UpdatePlayerTotalStats();
  }

  /// <summary>
  /// 장비 스텟 총합
  /// 이 함수는 현재 장착중인 장비들이 Update되어었을때 실행 해주는 함수
  /// 1. LoadUserInfo 이후 실행 (현재 프리셋에 장착되어있는 장비 기반 스텟 구성 해야되기 때문)  - LoadUserInfo 시 실행
  /// 2. 아이템 뽑기 이후 장착시 Update                                                     -  LampGacha에 추가 완료
  /// 플레이어 총 스텟 연산 2번에 해당
  /// </summary>
  /// <returns></returns>
  public void UpdateEquipmentStats()
  {
    equipmentStats.Clear();

    int findKey = GameDataManager.getInstance.equipPresetMap.FirstOrDefault(n => n.Value.isSelect).Key;

    if (findKey.Equals(-1))
      return;

    long[] equipmentIndexList = GameDataManager.getInstance.equipPresetMap.FirstOrDefault(n => n.Value.isSelect).Value.mountSlot;

    for (int i = 0; i < equipmentIndexList.Length; i++)
    {
      long equipIndex = equipmentIndexList[i];

      if (!equipIndex.Equals(0))
      {
        InvenData _invenData = GameDataManager.getInstance.dictEquip[equipIndex];

        if (_invenData != null)
        {
          foreach (var statData in _invenData.statDataList)
          {
            StatType statType = (StatType)statData.statType;

            if (equipmentStats.ContainsKey(statType))
              equipmentStats[statType] += statData.statValue;
            else
              equipmentStats.Add(statType, statData.statValue);
          }
        }

      }
    }

    UpdatePlayerTotalStats();
  }

  /// <summary>
  /// 스킬, 동료 보유효과
  /// 1. 아이템 획득 시 실행
  /// 2. 아이템 강화 시 실행
  /// 3. 아이템 합성 시 실행
  /// </summary>
  public void UpdateHasItemStats()
  {
    hasPartnerItemStats.Clear();
    hasSkillItemStats.Clear();

    float hasPartnerItemStatValue = PartnerTable.getInstance.GetHasPartnersStat();
    float hasSkillItemStatValue = SkillTable.getInstance.GetHasSkillsStat();

    StatType[] statTypes = ConstantManager.STAT_TYPES_ITEM_HAS_VALUE;

    //동료, 스킬
    for (int i = 0; i < statTypes.Length; i++)
    {
      StatType statType = statTypes[i];

      hasPartnerItemStats.Add(statType, hasPartnerItemStatValue);
      hasSkillItemStats.Add  (statType, hasSkillItemStatValue);
    }

    UpdatePlayerTotalStats();
  }

  /// <summary>
  /// 가호석상 스텟 업데이트
  /// 1. 로그인 시 스텟 업데이트
  /// 2. 가호석상 팝업 나갈때 프리셋 저장 API 실행 시
  /// </summary>
  /// <param name="blessingPresetData"></param>
  public void UpdateBlessingStats(BlessingPresetData blessingPresetData)
  {
    blessingStats.Clear();

    for (int i = 0; i < blessingPresetData.blessingDataList.Count; i++)
    {
      BlessingData blessingData = blessingPresetData.blessingDataList[i];

      if(blessingData.blessingType != 0)
      {
        //기존 스텟 퍼센트 증가량이 1,2,3 으로 되어있음
        StatType statType = StatUtility.GetConvertBonusStatType((StatType)blessingData.blessingType);

        if(!blessingStats.ContainsKey(statType))
          blessingStats.Add(statType, blessingData.blessingValue);
        else
          blessingStats[statType] += blessingData.blessingValue;
      }
    }

    UpdatePlayerTotalStats();
  }

  /// <summary>
  /// 수레 스텟 업데이트
  /// 1. 로그인 시 스텟 업데이트
  /// 2. 수레 레벨업 시 스텟 업데이트
  /// </summary>
  /// <param name="cartData"></param>
  public void UpdateCartStats(CartData cartData)
  {
    cartStats.Clear();

    cartStats.Add(StatType.HealthPer,      cartData.bonusHp);
    cartStats.Add(StatType.AttackPowerPer, cartData.bonusAtk);
    cartStats.Add(StatType.DefensePer,     cartData.bonusDef);

    UpdatePlayerTotalStats();
  }


  /// <summary>
  /// 상인협회 스텟 업데이트
  /// 1. 로그인 시 스텟 업데이트
  /// 2. 상인협회 개별 스킬 업데이트 시
  /// </summary>
  public void UpdateMerchantGuildStats()
  {
    merchantGuildStats.Clear();

    ContentMerchantGuild contents = GameDataManager.getInstance.userContentsData.merchantGuild;

    merchantGuildStats.Add(StatType.HealthPer,             contents.GetSkillBuffTotalValue(MerchantGuildSkillType.Health));
    merchantGuildStats.Add(StatType.AttackPowerPer,        contents.GetSkillBuffTotalValue(MerchantGuildSkillType.AttackPower));
    merchantGuildStats.Add(StatType.DefensePer,            contents.GetSkillBuffTotalValue(MerchantGuildSkillType.Defense));
    merchantGuildStats.Add(StatType.PartnerAttackSpeed,    contents.GetSkillBuffTotalValue(MerchantGuildSkillType.PartnerAttackSpeed));
    merchantGuildStats.Add(StatType.PartnerDamageIncrease, contents.GetSkillBuffTotalValue(MerchantGuildSkillType.PartnerDamageIncrease));
    merchantGuildStats.Add(StatType.SkillDamageIncrease,   contents.GetSkillBuffTotalValue(MerchantGuildSkillType.SkillDamageIncrease));

    UpdatePlayerTotalStats();
  }

  #endregion
}
