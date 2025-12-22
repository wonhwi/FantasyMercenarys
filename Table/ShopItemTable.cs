using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShopItemTable : LazySingleton<ShopItemTable>, ITableFactory
{
  //key = shopItemIdx
  public Dictionary<int, ShopItemData> dictShopItemData = new Dictionary<int, ShopItemData>();

  //key = categoryIdx
  public Dictionary<int, List<ShopItemData>> dictShopItemCategoryData = new Dictionary<int, List<ShopItemData>>();

  public void Load()
  {
    dictShopItemCategoryData.Clear();

    string shopItemJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.ShopItem, out shopItemJson))
    {
      List<ShopItemData> shopItemDataList = JsonConvert.DeserializeObject<List<ShopItemData>>(shopItemJson);

      foreach (var data in shopItemDataList)
      {
        dictShopItemData[data.shopItemIdx] = data;
      }

      foreach (var data in shopItemDataList)
      {
        if (!dictShopItemCategoryData.ContainsKey(data.categoryIdx))
        {
          dictShopItemCategoryData[data.categoryIdx] = new List<ShopItemData>();
        }

        dictShopItemCategoryData[data.categoryIdx].Add(data);

      }

      Debug.Log("ShopItem Table Load Success");
    }
  }

  public List<ShopItemData> GetShopCategoryItemDataList(int categoryIndex)
  {
    if (dictShopItemCategoryData.ContainsKey(categoryIndex))
      return dictShopItemCategoryData[categoryIndex];
    else
      return null;
  }

  public ShopItemData GetShopItemData(int shopItemIdx)
  {
    if (dictShopItemData.ContainsKey(shopItemIdx))
      return dictShopItemData[shopItemIdx];
    else
      return default;
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }
}
