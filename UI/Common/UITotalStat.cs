using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using FantasyMercenarys.Data;
using Newtonsoft.Json;

/// <summary>
/// 총 보유효과
/// </summary>
public class UITotalStat : MonoBehaviour
{
  public TextMeshProUGUI totalStatText;

  /// <summary>
  /// 아이템 팝업창을 열었을때 (스킬, 파트너) Total 총 보유량을 출력해 주는 부분
  /// </summary>
  /// <param name="itemGroup"></param>
  public void SetItemGroupHasValue(ItemGroup itemGroup)
  {
    float itemHasValue = 0f;

    if (itemGroup.Equals(ItemGroup.Skill))
      itemHasValue = SkillTable.getInstance.GetHasSkillsStat();
    else if (itemGroup.Equals(ItemGroup.Partner))
      itemHasValue = PartnerTable.getInstance.GetHasPartnersStat();


    totalStatText.text = $"체력 + {itemHasValue:F2}%<br>공격력 + {itemHasValue:F2}%<br>방어력 + {itemHasValue:F2}%";
  }

  /// <summary>
  /// 내가 획득하지 못한 아이템에 대해서 보유량 값 반환 및 출력
  /// </summary>
  /// <param name="itemData"></param>
  public void SetItemHasValue(ItemData itemData)
  {
    float itemHasValue = 0f;

    ItemGroup itemGroup = (ItemGroup)itemData.itemGroup;

    if (itemGroup.Equals(ItemGroup.Skill))
      itemHasValue = SkillTable.getInstance.GetSkillHasValue(itemData.itemIdx, 0);
    else if (itemGroup.Equals(ItemGroup.Partner))
      itemHasValue = PartnerTable.getInstance.GetPartnerHasValue(itemData.itemIdx, 0);

    totalStatText.text = $"기본 체력, 공격력, 방어력 + {itemHasValue:F2}%";

  }

  /// <summary>
  /// 내가 획득한 아이템에 대해서 보유량 값 반환 및 출력
  /// </summary>
  /// <param name="invenData"></param>
  public void SetItemHasValue(InvenData invenData)
  {
    float itemHasValue = 0f;

    ItemGroup itemGroup = (ItemGroup)invenData.itemGroup;

    if (itemGroup.Equals(ItemGroup.Skill))
      itemHasValue = SkillTable.getInstance.GetSkillHasValue(invenData.itemIdx, invenData.itemLv);
    else if (itemGroup.Equals(ItemGroup.Partner))
      itemHasValue = PartnerTable.getInstance.GetPartnerHasValue(invenData.itemIdx, invenData.itemLv);

    totalStatText.text = $"기본 체력, 공격력, 방어력 + {itemHasValue:F2}%";
  }

}
