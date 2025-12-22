using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreedingGroundModel
{
  private BreedingTable breedingTable => BreedingTable.getInstance;

  private ContentBreedingGround contentModel => GameDataManager.getInstance.userContentsData.breedingGround;
  private ContentMerchantGuild contentMerchantGuild => GameDataManager.getInstance.userContentsData.merchantGuild;

  #region Table Data

  public int GetValueMaxLevel()
  {
    int valutLv = 1;

    foreach (var kvp in breedingTable.dictBreedingResearchData)
    {
      if (kvp.Key > valutLv)
        valutLv = kvp.Key;
    }

    return valutLv;
  }

  public int GetTrackLength() => (int)BaseTable.getInstance.GetBaseValue(ConstantManager.BASE_INDEX_BREEDING_CREATURE_TRACK_LENGTH);
  public int GetRewardTrackCycle() => (int)BaseTable.getInstance.GetBaseValue(ConstantManager.BASE_INDEX_BREEDING_CREATURE_REWARD_RESEARCHFUNDS);
  public float GetObjectCreateInterval() => BaseTable.getInstance.GetBaseValue(821);
  public int GetObjectCreateLimit() => (int)BaseTable.getInstance.GetBaseValue(805);

  public float GetObjectResearchRate() => BaseTable.getInstance.GetBaseValue(822);
  public float GetObjectCanRate() => BaseTable.getInstance.GetBaseValue(823);
  public float GetObjectPoopRate() => BaseTable.getInstance.GetBaseValue(824);

  public BreedingResearchData GetVaultResearchData(int vaultLv) => breedingTable.dictBreedingResearchData[vaultLv];

  public GroundObjectType GetObjectType()
  {
    float randomValue = Random.Range(0, 100) * 0.01f;

    float researchRate = GetObjectResearchRate();
    float canRate = GetObjectCanRate() + researchRate;
    float poopRate = GetObjectPoopRate() + canRate;

    if (randomValue < researchRate)
      return GroundObjectType.Research;
    else if (randomValue < canRate)
      return GroundObjectType.Can;
    else
      return GroundObjectType.Poop;


  }

  public float GetSkillTypeValue(MerchantGuildSkillType skillType)
  {
    return contentMerchantGuild.GetSkillBuffTotalValue(skillType);
  }

  public BreedingCreatureData GetMetaCreatureData(int creatureIdx)
  {
    return breedingTable.GetCreatureData(creatureIdx);
  }

  public float GetConditionValue(int conditionType)
  {
    return BaseTable.getInstance.GetBaseValue(ConstantManager.BASE_INDEX_BREEDING_CREATURE_CONDITION + conditionType);
  }
  #endregion

  #region Content

  /// <summary>
  /// 사육장 관련 유저가 소지하고 있는 크리쳐 데이터 리스트 반환
  /// </summary>
  /// <returns></returns>
  public IEnumerable<CreatureData> GetCreatureDataList()
  {
    return contentModel.dictCreatureData.Values;
  }

  /// <summary>
  /// 사육장 관련 유저가 소지하고 있는 크리쳐 데이터 반환
  /// </summary>
  /// <returns></returns>
  public CreatureData GetCreatureData(int creatureIdx)
  {
    return contentModel.GetCreatureData(creatureIdx);
  }

  /// <summary>
  /// 사육장 관련 유저가 소지하고 있는지 판단
  /// </summary>
  /// <returns></returns>
  public bool HasCreature(int creatureIdx)
  {
    return contentModel.HasCreature(creatureIdx);
  }

  public int GetVaultLevel() => contentModel.breedingGroundsData.vaultLevel;

  

  /// <summary>
  /// 오늘 일일 획득 포인트 달성했는지 판단 로직
  /// </summary>
  /// <returns></returns>
  public bool IsDailyPointMax()
  {
    int vaultLevel = GetVaultLevel();

    int dailyPointLimit = GetVaultResearchData(vaultLevel).dailyPointLimit;

    if (contentModel.daliyObejctPoint >= dailyPointLimit)
      return true;

    return false;

  }
  public bool GetIsUpgrade() => contentModel.breedingGroundsData.isUpgrade;
  public long GetUpgradeCompleteAt() => contentModel.breedingGroundsData.upgradeCompleteAt;

  public int GetSlotState(int slotIdx) => contentModel.breedingGroundsData.slotList[slotIdx];

  public int[] GetSlotList() => contentModel.breedingGroundsData.slotList;

  public bool IsMountSlot(int creatureIdx)
  {
    bool isMount = false;

    int slotCount = contentModel.breedingGroundsData.slotList.Length;

    for (int i = 0; i < slotCount; i++)
    {
      int slotValue = GetSlotState(i);

      if(slotValue == creatureIdx)
      {
        isMount = true;
        break;
      }
    }

    return isMount;
  }

  #endregion
}
