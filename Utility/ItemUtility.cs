using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ItemUtility
{

  public static IEnumerable<long> GetInvenIndexList(List<InvenData> invenDataList)
  {
    foreach (var invenData in invenDataList)
    {
      yield return invenData.invenIdx;
    }
  }

  public static IEnumerable<int> GetItemKeys<T>(Dictionary<int, T> itemDict)
  {
    foreach (var item in itemDict)
    {
      yield return item.Key;
    }
  }

  public static IEnumerable<int> GetItemKeysByMatch<T>(Dictionary<int, T> itemDict, System.Predicate<T> match = null)
  {
    if (match == null)
    {
      foreach (var item in itemDict)
      {
        yield return item.Key;
      }
    }
    else
    {
      foreach (var item in itemDict)
      {
        if (match(item.Value))
          yield return item.Key;
      }
    }
  }

  #region 아이템 강화, 합성
  /// <summary>
  /// 단순 레벨 값 출력
  /// </summary>
  /// <param name="invenData"></param>
  /// <returns></returns>
  public static string GetItemLevelBoxText(InvenData invenData)
  {
    if (IsMaxItemLevel(invenData))
      return ConstantManager.ITEM_MAX_LEVEL_TEXT;

    return $"{invenData.itemLv}";
  }

  /// <summary>
  /// Lv. 포함 레벨 출력
  /// </summary>
  /// <param name="invenData"></param>
  /// <returns></returns>
  public static string GetItemLevelText(InvenData invenData)
  {
    if (IsMaxItemLevel(invenData))
      return ConstantManager.ITEM_MAX_LEVEL_TEXT;

    return $"Lv.{invenData.itemLv}";
  }

  /// <summary>
  /// 아이템 최대 레벨 여부 확인
  /// </summary>
  /// <param name="invenData"></param>
  /// <returns></returns>
  public static bool IsMaxItemLevel(InvenData invenData)
  {
    int itemLevel = invenData.itemLv;

    if (invenData.itemGroup.Equals((int)ItemGroup.Skill))
    {
      IdleSkillData skillData = SkillTable.getInstance.GetSkillData(SkillCategory.Item, invenData.itemIdx);

      if (skillData.maxLevel == itemLevel)
        return true;
    }
    else if (invenData.itemGroup.Equals((int)ItemGroup.Partner))
    {
      PartnerData partnerData = PartnerTable.getInstance.GetPartnerData(invenData.itemIdx);

      if (partnerData.maxLevel == itemLevel)
        return true;
    }

    return false;
  }

  /// <summary>
  /// 아이템 강화 가능 여부 판단
  /// </summary>
  /// <param name="invenData"></param>
  /// <returns></returns>
  public static bool IsUseEnhanceItem(InvenData invenData)
  {
    int itemLevel = invenData.itemLv;
    long itemCount = invenData.itemCount;

    int targetCount = GetEnhanceTargetCount(invenData);

    if (invenData.itemGroup.Equals((int)ItemGroup.Skill))
    {
      IdleSkillData skillData = SkillTable.getInstance.GetSkillData(SkillCategory.Item, invenData.itemIdx);

      if (skillData.IsNull())
        return false;

      if (!skillData.isLevelUp)
        return false;
    }
    else if (invenData.itemGroup.Equals((int)ItemGroup.Partner))
    {
      PartnerData partnerData = PartnerTable.getInstance.GetPartnerData(invenData.itemIdx);

      if (partnerData.IsNull())
        return false;
    }

    if (IsMaxItemLevel(invenData))
      return false;

    //레벨업 하는데 필요 갯수
    if (itemCount >= GetEnhanceTargetCount(invenData))
      return true;

    return false;
  }


  /// <summary>
  /// 아이템 강화 가능 목표 갯수
  /// </summary>
  /// <param name="invenData"></param>
  /// <returns></returns>
  public static int GetEnhanceTargetCount(InvenData invenData)
  {
    int targetCount = 0;

    int itemLevel = invenData.itemLv;

    if (invenData.itemGroup.Equals((int)ItemGroup.Skill))
    {
      IdleSkillData skillData = SkillTable.getInstance.GetSkillData(SkillCategory.Item, invenData.itemIdx);

      targetCount = (int)Mathf.Round(
        itemLevel *
        BaseTable.getInstance.GetBaseValue(ConstantManager.BASE_INDEX_SKILL_ENHANCE_MULTIPLY) *
        skillData.levelUpReqIncrease
        );
    }
    else if (invenData.itemGroup.Equals((int)ItemGroup.Partner))
    {
      PartnerData partnerData = PartnerTable.getInstance.GetPartnerData(invenData.itemIdx);

      targetCount = (int)Mathf.Round(
        itemLevel *
        BaseTable.getInstance.GetBaseValue(ConstantManager.BASE_INDEX_PARTNER_ENHANCE_MULTIPLY) *
        partnerData.levelUpReqIncrease
        );

    }

    targetCount = Mathf.Clamp(targetCount, 1, targetCount);

    return targetCount;
  }

  public static bool IsUseCombineItem(InvenData invenData)
  {
    int itemLevel = invenData.itemLv;
    long itemCount = invenData.itemCount;



    //최대 레벨이 현재 레벨인지 판단
    if (IsMaxItemLevel(invenData))
    {
      int combineCount = GetCombineTargetCount(invenData);
      bool IsNextGradeConbineable = IsNextGradeCombinable(invenData);

      //합성 갯수가 0이면 합성이 되면 안됨
      if (combineCount == 0)
        return false;

      //다음 등급의 아이템 합성 가능 한게 하나도 없으면 false
      //if (!IsNextGradeConbineable)
      //  return false;

      //현재 내가 가지고 있는 아이템의 갯수가 최소 합성 갯수 이상 이어야함
      if (itemCount >= combineCount)
        return true;
    }

    return false;
  }

  /// <summary>
  /// 아이템 합성 가능 목표 갯수
  /// </summary>
  /// <param name="invenData"></param>
  /// <returns></returns>
  public static int GetCombineTargetCount(InvenData invenData)
  {
    int targetCount = 0;

    int itemLevel = invenData.itemLv;
    long itemCount = invenData.itemCount;

    if (invenData.itemGroup.Equals((int)ItemGroup.Skill))
    {
      IdleSkillData skillData = SkillTable.getInstance.GetSkillData(SkillCategory.Item, invenData.itemIdx);

      targetCount = skillData.combineReqValue;
    }
    else if (invenData.itemGroup.Equals((int)ItemGroup.Partner))
    {
      PartnerData partnerData = PartnerTable.getInstance.GetPartnerData(invenData.itemIdx);

      targetCount = partnerData.combineReqValue;
    }

    return targetCount;

  }

  public static bool IsNextGradeCombinable(InvenData invenData)
  {
    bool canCombinable = false;

    if (invenData.itemGroup.Equals((int)ItemGroup.Skill))
    {
      IdleSkillData skillData = SkillTable.getInstance.GetSkillData(SkillCategory.Item, invenData.itemIdx);


      SkillTable.getInstance.dictGradeCombineCount.TryGetValue((ItemGradeType)skillData.skillGrade + 1, out int gradeCount);

      if (gradeCount > 0)
        canCombinable = true;
    }
    else if (invenData.itemGroup.Equals((int)ItemGroup.Partner))
    {
      PartnerData partnerData = PartnerTable.getInstance.GetPartnerData(invenData.itemIdx);

      PartnerTable.getInstance.dictGradeCombineCount.TryGetValue((ItemGradeType)partnerData.partnerGrade + 1, out int gradeCount);

      if (gradeCount > 0)
        canCombinable = true;
    }


    return canCombinable;

  }
  #endregion

  #region ItemSlot
  public static string GetItemName(int itemIdx)
  {
    ItemData itemData = ItemTable.getInstance.GetItemData(itemIdx);

    string itemName = itemData.itemRecordCd;

    if (itemData.itemGroup == (int)ItemGroup.Equipment)
    {
      EquipmentItemData equipmentItemData = EquipmentItemTable.getInstance.GetEquipmentItemData(itemData.itemIdx);

      //무기 라면 MappingData로 사용해
      if (equipmentItemData.mountType == (int)EquipmentMountType.Weapon)
      {
        EquipmentMappingData mappingData = EquipmentMappingTable.getInstance.GetEquipmentMappingData(itemData.itemIdx);

        itemName = mappingData.equipRecordCd;
      }
    }

    return LanguageTable.getInstance.GetLanguage(itemName);
  }

  public static string GetItemIcon(int itemIdx)
  {
    ItemData itemData = ItemTable.getInstance.GetItemData(itemIdx);

    string itemIcon = itemData.iconImage;

    if (itemData.itemGroup == (int)ItemGroup.Equipment)
    {
      EquipmentItemData equipmentItemData = EquipmentItemTable.getInstance.GetEquipmentItemData(itemData.itemIdx);

      //무기 라면 MappingData로 사용해
      if (equipmentItemData.mountType == (int)EquipmentMountType.Weapon)
      {
        EquipmentMappingData mappingData = EquipmentMappingTable.getInstance.GetEquipmentMappingData(itemData.itemIdx);

        itemIcon = mappingData.iconImage;
      }
    }

    return itemIcon;
  }

  #endregion
}
