using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentPartnerSkillCraft
{
  public Dictionary<ItemGroup, List<PSCraftData>> dictPSCraftData = new Dictionary<ItemGroup, List<PSCraftData>>();

  public List<PSCraftData> GetCraftData(ItemGroup itemGroup)
  {
    return dictPSCraftData[itemGroup];
  }

  /// <summary>
  /// 로그인 시 실행
  /// </summary>
  public void SetCraftInfoData(ItemGroup itemGroup, List<PSCraftData> psCraftDataList)
  {
    if (dictPSCraftData.ContainsKey(itemGroup))
      dictPSCraftData[itemGroup] = psCraftDataList;
    else
      dictPSCraftData.Add(itemGroup, psCraftDataList);

  }

  public void UpdateCraftInfoData(PSCraftData craftData)
  {

    ItemGroup itemGroup = (ItemGroup)craftData.craftType;

    int findIndex = dictPSCraftData[itemGroup].FindIndex(n => n.slotNo == craftData.slotNo);

    if(findIndex != -1)
    {
      dictPSCraftData[itemGroup][findIndex] = craftData;
    }
  }

}
