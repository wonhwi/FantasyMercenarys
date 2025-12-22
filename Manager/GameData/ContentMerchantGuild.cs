using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentMerchantGuild
{
  //해당 타입 분리 int = skillIdx
  public Dictionary<MerchantGuildSkillType, List<int>> dictSkillTypeInfo = new Dictionary<MerchantGuildSkillType, List<int>>();
  public Dictionary<int, MerchantGuildSkillData> dictSkillInfo = new Dictionary<int, MerchantGuildSkillData>();

  public bool isUpgrade         { get; private set; }
  public int upgradeSkillIdx    { get; private set; }
  public long upgradeCompleteAt { get; private set; }


  public bool SetIsUpgrade(bool isUpgrade) => this.isUpgrade = isUpgrade;
  public int SetUpgradeSkillIdx(int upgradeSkillIdx) => this.upgradeSkillIdx = upgradeSkillIdx;
  public long SetUpgradeCompleteAt(long upgradeCompleteAt) => this.upgradeCompleteAt = upgradeCompleteAt;

  /// <summary>
  /// 로그인 시 실행
  /// </summary>
  public void SetSkillData(List<MerchantGuildSkillData> skillList)
  {
    foreach (var skillData in skillList)
    {
      dictSkillInfo[skillData.skillIdx] = skillData;

      UpdateSkillTypeData(skillData.skillIdx);
    }

    PlayerStatManager.getInstance.UpdateMerchantGuildStats();
  }

  /// <summary>
  /// API 호출 시 업데이트 함수
  /// </summary>
  /// <param name="recruitData"></param>
  public void UpdateSkillData(MerchantGuildSkillData skillData)
  {
    dictSkillInfo[skillData.skillIdx] = skillData;

    UpdateSkillTypeData(skillData.skillIdx);

    PlayerStatManager.getInstance.UpdateMerchantGuildStats();
  }

  private void UpdateSkillTypeData(int skillIdx)
  {
    MerchantSkillTree skillTree = MerchantGuildTable.getInstance.GetSkillTree(skillIdx);

    MerchantGuildSkillType skillType = (MerchantGuildSkillType)skillTree.skillBuffType;

    if (!dictSkillTypeInfo.ContainsKey(skillType))
    {
      dictSkillTypeInfo.Add(skillType, new List<int>());
    }

    List<int> skillTypeList = dictSkillTypeInfo[skillType];

    if (!skillTypeList.Contains(skillIdx))
      skillTypeList.Add(skillIdx);
  }

  public bool HasSkill(int skillIdx)
  {
    return dictSkillInfo.ContainsKey(skillIdx);
  }

  public float GetSkillBuffTotalValue(MerchantGuildSkillType merchantSkillType)
  {
    float value = 0f;

    if(dictSkillTypeInfo.ContainsKey(merchantSkillType))
    {
      List<int> skillTypeList = dictSkillTypeInfo[merchantSkillType];

      for (int i = 0; i < skillTypeList.Count; i++)
      {
        int skillIdx = skillTypeList[i];

        MerchantGuildSkillData merchantGuildSkillData = dictSkillInfo[skillIdx];
        
        int skillLv = merchantGuildSkillData.skillLv;

        MerchantSkillLevelup skillLvUpData = MerchantGuildTable.getInstance.GetSkillLevelup(skillIdx, skillLv);

        value += skillLvUpData.skillValue;
      }
    }

    Debug.Log($"Merchant Skill TotalValue {merchantSkillType} / {value}");
    
    return value;
  }

}
