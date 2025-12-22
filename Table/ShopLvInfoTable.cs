using Assets.ZNetwork.Manager;
using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShopLvInfoTable : LazySingleton<ShopLvInfoTable>, ITableFactory
{
  public Dictionary<int, ShopLvInfoData> dictShopLvInfoData = new Dictionary<int, ShopLvInfoData>();

  public void Load()
  {
    dictShopLvInfoData.Clear();

    string shopLvInfoJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.ShopLvInfo, out shopLvInfoJson))
    {
      List<ShopLvInfoData> shopLvInfoDataList = JsonConvert.DeserializeObject<List<ShopLvInfoData>>(shopLvInfoJson);
      foreach (var data in shopLvInfoDataList)
      {
        if (!dictShopLvInfoData.ContainsKey(data.shopLvIdx))
        {
          dictShopLvInfoData.Add(data.shopLvIdx, data);
        }
        else
          Debug.Log("ShopLvInfo Table Load Error Index : " + data.shopLvIdx);
      }
      Debug.Log("ShopLvInfo Table Load Success");
    }
  }

  public int GetCurRequireLvExp(ShopCategoryType categoryType, int level)
  {
    int currentExp = 0;

    foreach (var shopLvInfoData in dictShopLvInfoData.Values)
    {
      if (shopLvInfoData.categoryIdx == (int)categoryType && shopLvInfoData.shopLv == level)
      {
        currentExp = shopLvInfoData.requireLvExp;
      }
    }


    return currentExp;
  }

  public ShopLvInfoData GetShopLvInfoData(int _key)
  {
    if (dictShopLvInfoData.ContainsKey(_key))
    {
      return dictShopLvInfoData[_key];
    }
    else
    {
      Debug.Log($"No ShopLvInfo Data key : {_key}");
      return default;
    }
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }
}
