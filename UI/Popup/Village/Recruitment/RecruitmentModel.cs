using Cysharp.Threading.Tasks;
using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static RestPacket;

public class RecruitmentModel
{
  private PartnerExploreSlotTable exploreSlotModel => PartnerExploreSlotTable.getInstance;
  private ContentPartnerRecruit runtimeModel => GameDataManager.getInstance.userContentsData.partnerRecruit;

  public PartnerExploreSlotData GetPartnerExploreSlotData(int slotIndex)
  {
    return exploreSlotModel.GetPartnerExploreSlotData(slotIndex);
  }

  public Dictionary<int, UserRecruitInfoDTO> GetRecruitInfoDict()
  {
    return runtimeModel.GetRecruitInfoDict();
  }

  public void ClearMountList()
  {
    runtimeModel.mountSlotList.Clear();
  }

  public void AddMountList(int itemIdx)
  {
    if(!IsMountItem(itemIdx))
      runtimeModel.mountSlotList.Add(itemIdx);
  }

  public void RemoveMountList(int itemIdx)
  {
    if(IsMountItem(itemIdx))
      runtimeModel.mountSlotList.Remove(itemIdx);
  }

  public bool IsMountItem(int itemIdx)
  {
    return runtimeModel.mountSlotList.FindIndex(n => n == itemIdx) != -1;
  }

}
