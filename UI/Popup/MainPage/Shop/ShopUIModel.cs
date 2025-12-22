using FantasyMercenarys.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopUIModel
{
  private UserShopData userShopData = GameDataManager.getInstance.userShopData;
  private ShopCategoryTable shopCategoryTable => ShopCategoryTable.getInstance;
  private ShopItemTable shopItemTable => ShopItemTable.getInstance;


  public List<ShopCategoryData> GetShopCategoryDataList(int shopIdx)
  {
    return shopCategoryTable.GetShopCategoryDataList(shopIdx);
  }

  public List<ShopItemData> GetShopCategoryItemDataList(int categoryIdx)
  {
    return shopItemTable.GetShopCategoryItemDataList(categoryIdx);
  }

  public ShopItemData GetShopItemData(int shopItemIdx)
  {
    return shopItemTable.GetShopItemData(shopItemIdx);
  }

  public ShopInfoData GetShopInfoData(ShopCategoryType shopCategoryType)
  {
    return userShopData.GetShopInfoData(shopCategoryType);
  }

  public bool IsEnoughItem(int itemIdx, long itemCount)
  {
    return GameDataManager.getInstance.GetConsumeValue(itemIdx) >= itemCount;
  }

  /// <summary>
  /// 상점 타입에 따른 소모되는 재화 아이템 인덱스 반환
  /// </summary>
  public int GetConsumeItemIdx(ShopCategoryType shopCategoryType) => shopCategoryType switch
  {
    ShopCategoryType.Skill          => ConstantManager.ITEM_CURRENCY_TICKET_SKILL,
    ShopCategoryType.Partner        => ConstantManager.ITEM_CURRENCY_TICKET_PARTNER,
    ShopCategoryType.LimitedProduct => ConstantManager.ITEM_CURRENCY_GEM,
    ShopCategoryType.DiaProduct     => ConstantManager.ITEM_CURRENCY_GEM,
  };

}
