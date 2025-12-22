using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserShopData
{
  private Dictionary<ShopCategoryType, ShopInfoData> userShopInfoData = new Dictionary<ShopCategoryType, ShopInfoData>();

  public ShopInfoData GetShopInfoData(ShopCategoryType shopCategoryType)
  {
    if (userShopInfoData.ContainsKey(shopCategoryType))
      return userShopInfoData[shopCategoryType];

    return null;
  }

  public void SetUserShopInfoData(List<ShopInfoData> shopInfoDataList)
  {
    for (int i = 0; i < shopInfoDataList.Count; i++)
    {
      UpdateUserShopInfoData(shopInfoDataList[i]);
    }
  }

  public void UpdateUserShopInfoData(ShopInfoData shopInfoData)
  {
    ShopCategoryType shopCategoryType = (ShopCategoryType)shopInfoData.shopCatIdx;

    if (userShopInfoData.ContainsKey(shopCategoryType))
      userShopInfoData[shopCategoryType] = shopInfoData;
    else
      userShopInfoData.Add(shopCategoryType, shopInfoData);
  }
}
