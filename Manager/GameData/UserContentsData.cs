using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserContentsData
{
  public long researchFunds; //연구 비

  public List<int> dailyPopupList = new List<int>();
  
  public ContentPrayerCenter prayerCenter = new ContentPrayerCenter();
  public ContentBlessingStatue blessingStatue = new ContentBlessingStatue();
  public ContentAdventureGuild adventureGuild = new ContentAdventureGuild();
  public ContentPartnerSkillCraft partnerSkillCraft = new ContentPartnerSkillCraft();
  public ContentPartnerRecruit partnerRecruit = new ContentPartnerRecruit();
  public ContentMerchantGuild merchantGuild = new ContentMerchantGuild();
  public ContentHopeCenter hopeCenter = new ContentHopeCenter();
  public ContentBreedingGround breedingGround = new ContentBreedingGround();
  public ContentCart contentCart = new ContentCart();
  
  public void UpdateResearchFunds(long itemCount)
  {
    this.researchFunds = itemCount;

    CurrencyManager.getInstance.SetCurrencyData(ConstantManager.ITEM_CURRENCY_RESEARCH_FUNDS, researchFunds);
  }

  public void LoadDailyPopupInfoList(List<int> opneDailyPopupList)
  {
    dailyPopupList.AddRange(opneDailyPopupList);
  }

  public bool RemoveDailyPopupInfoList(DailyPopupType popupType)
  {
    int findIdx = -1;

    for (int i = 0; i < dailyPopupList.Count; i++)
    {
      if (dailyPopupList[i] == (int)popupType)
      {
        findIdx = i; 
        break;
      }
    }

    if (findIdx != -1)
    {
      dailyPopupList.Remove(findIdx);
      return true;
    }

    Debug.LogError("정상적으로 삭제 못함");
    return false;
  }

  public bool IsValidDailyPopupType(DailyPopupType popupType)
  {
    for (int i = 0; i < dailyPopupList.Count; i++)
    {
      if (dailyPopupList[i] == (int)popupType)
      {
        return true;
      }
    }

    return false;
  }
}
