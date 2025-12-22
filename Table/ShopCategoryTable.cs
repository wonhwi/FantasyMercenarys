using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShopCategoryTable : LazySingleton<ShopCategoryTable>, ITableFactory
{
  /// <summary>
  /// key = 상점 인덱스
  /// </summary>
  public Dictionary<int, List<ShopCategoryData>> dictShopCategoryData = new Dictionary<int, List<ShopCategoryData>>();

  public void Load()
  {
    dictShopCategoryData.Clear();

    string shopCategoryJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.ShopCategory, out shopCategoryJson))
    {
      List<ShopCategoryData> shopCategoryDataList = JsonConvert.DeserializeObject<List<ShopCategoryData>>(shopCategoryJson);
      foreach (var data in shopCategoryDataList)
      {
        if (!dictShopCategoryData.ContainsKey(data.shopIdx))
        {
          dictShopCategoryData[data.shopIdx] = new List<ShopCategoryData>();
        }

        dictShopCategoryData[data.shopIdx].Add(data);

      }

      Debug.Log("ShopCategory Table Load Success");
    }
  }



  public List<ShopCategoryData> GetShopCategoryDataList(int shopIndex)
  {
    if (dictShopCategoryData.ContainsKey(shopIndex))
      return dictShopCategoryData[shopIndex];
    else
      return null;
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }
}
