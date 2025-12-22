using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantGuildModel
{
  private MerchantGuildTable merchantGuildTable => MerchantGuildTable.getInstance;
  private ContentMerchantGuild contentModel => GameDataManager.getInstance.userContentsData.merchantGuild;

  #region Table Data

  /// <summary>
  /// 상인 협회 진입 시 스킬들 테이블 메타데이터 기반 설정 용도로 사용
  /// </summary>
  /// <returns></returns>
  public IEnumerable<int> GetMetaSkillTreeKeyData()
  {
    return merchantGuildTable.dictSkillTreeData.Keys;
  }

  /// <summary>
  /// skillIdx 기반 메타 데이터 반환
  /// </summary>
  /// <param name="skillIdx"></param>
  /// <returns></returns>
  public MerchantSkillTree GetSkillTreeData(int skillIdx)
  {
    return merchantGuildTable.dictSkillTreeData[skillIdx];
  }

  /// <summary>
  /// 다음 레벨 정보 반환 - (비용, 증가 수치) 참조 용도
  /// </summary>
  /// <param name="skillIdx"></param>
  /// <param name="skillLv"></param>
  /// <returns></returns>
  public MerchantSkillLevelup GetSkillLevelUpData(int skillIdx, int skillLv)
  {
    List<MerchantSkillLevelup> merchantSkillLevelupList = merchantGuildTable.dictSkillLevelUpData[skillIdx];

    int findIdx = merchantSkillLevelupList.FindIndex(n => n.skillLevel == skillLv);

    if (findIdx != -1)
      return merchantSkillLevelupList[findIdx];
    else
      return default;
  }
  /// <summary>
  /// 최대 레벨 아이템인지 판단 로직
  /// </summary>
  /// <param name="skillIdx"></param>
  /// <param name="skillLv"></param>
  /// <returns></returns>
  public bool IsMaxLvItem(int skillIdx, int skillLv)
  {
    List<MerchantSkillLevelup> merchantSkillLevelupList = merchantGuildTable.dictSkillLevelUpData[skillIdx];

    int lastIdx = merchantSkillLevelupList.Count - 1;

    int maxLv = merchantSkillLevelupList[lastIdx].skillLevel;

    if (maxLv == skillLv)
      return true;
    else
      return false;
  }

  /// <summary>
  /// 선행 조건 만족 여부 판단
  /// </summary>
  public bool IsConditionValid(int skillIdx)
  {
    MerchantSkillTree skillTree = GetSkillTreeData(skillIdx);

    int preSkillIdx1 = skillTree.preSkillIdx1;
    int pre1ReqLv    = skillTree.pre1ReqLv;

    int preSkillIdx2 = skillTree.preSkillIdx2;
    int pre2ReqLv    = skillTree.pre2ReqLv;

    bool condition1 = IsConditionValidItem(preSkillIdx1, pre1ReqLv);
    bool condition2 = IsConditionValidItem(preSkillIdx2, pre2ReqLv);

    return condition1 && condition2;

    bool IsConditionValidItem(int conditionSkillIdx, int conditionSkillLv)
    {
      if (conditionSkillIdx == 0)
        return true;

      int conditionLv = GetHasItemLv(conditionSkillIdx);

      //해당 아이템을 가지고 있고 목표 레벨보다 높을경우
      if (conditionLv >= conditionSkillLv)
        return true;
      else
        return false;
    }
  }

  

  /// <summary>
  /// 메타데이터 기반 Index 순서반환해서 재 사용
  /// </summary>
  /// <param name="skillIdx"></param>
  /// <returns></returns>
  public int GetDataIndex(int skillIdx)
  {
    //해당 스킬 Idx가 0이면
    if (skillIdx == 0)
      return 0;

    int index = 0;

    //스킬 정보가 없으면 결과적으로 0 Return
    foreach (var skillInfo in merchantGuildTable.dictSkillTreeData)
    {
      if (skillInfo.Key == skillIdx)
      {
        return index;
      }
      index++;
    }

    return index;
  }

  #endregion

  #region Content
  /// <summary>
  /// 해당 아이템을 유저가 습득한지 여부 판단
  /// </summary>
  /// <param name="skillIdx"></param>
  /// <returns></returns>
  public bool HasItem(int skillIdx)
  {
    if (contentModel.dictSkillInfo.Count == 0)
      return false;

    return contentModel.dictSkillInfo.ContainsKey(skillIdx);
  }

  /// <summary>
  /// 해당 아이템을 습득했으면 레벨 반환, 습득 안했으면 0 반환
  /// </summary>
  /// <param name="skillIdx"></param>
  /// <returns></returns>
  public int GetHasItemLv(int skillIdx)
  {
    if(HasItem(skillIdx))
      return contentModel.dictSkillInfo[skillIdx].skillLv;

    return 0;
  }
  
  /// <summary>
  /// 현재 연구중인 아이템인지 판단
  /// </summary>
  /// <param name="skillIdx"></param>
  /// <returns></returns>
  public bool IsResearchItem(int skillIdx)
  {
    return GetUpgradeSkillIdx() == skillIdx;
  }

  /// <summary>
  /// 팝업 진입 시 유저의 가장 우측 상단에 있는 아이템 자동 선택 용도
  /// 해당 아이템 SlotIndex 반환
  /// </summary>
  /// <returns></returns>
  public int GetLastSkillSlotIndex()
  {
    int itemIdx = 0;

    int skillColumn = 1;
    int skillOrder = 1;

    var enumerator = contentModel.dictSkillInfo.Keys.GetEnumerator();

    while (enumerator.MoveNext())
    {
      int skillIdx = enumerator.Current;

      MerchantSkillTree skillTreeData = GetSkillTreeData(skillIdx);

      bool isNewHighestSlot = skillTreeData.skillColumn > skillColumn;
      bool isSameSlotButHigherOrder =
          skillTreeData.skillColumn == skillColumn && skillTreeData.columnOrder < skillOrder;

      if (isNewHighestSlot)
      {
        skillColumn = skillTreeData.skillColumn;
        skillOrder = skillTreeData.columnOrder;
        itemIdx = skillIdx;
      }
      else if (isSameSlotButHigherOrder)
      {
        skillOrder = skillTreeData.columnOrder;
        itemIdx = skillIdx;
      }
    }

    return GetDataIndex(itemIdx);
  }

  /// <summary>
  /// 유저가 소유하고 있는 아이템들 ItemIdx 들을 반환
  /// </summary>
  /// <returns></returns>
  public IEnumerable<int> GetUserHasSkillKeyData()
  {
    return contentModel.dictSkillInfo.Keys;
  }



  public bool GetIsUpgrade() => contentModel.isUpgrade;
  public int GetUpgradeSkillIdx() => contentModel.upgradeSkillIdx;
  public long GetUpgradeCompleteAt() => contentModel.upgradeCompleteAt;


  #endregion
}
