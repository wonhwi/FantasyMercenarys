using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ShopTable : LazySingleton<ShopTable>, ITableFactory
{
  public Dictionary<int, ShopData> dictShopData = new Dictionary<int, ShopData>();

  public void Load()
  {
    dictShopData.Clear();

    string shopJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.Shop, out shopJson))
    {
      List<ShopData> shopDataList = JsonConvert.DeserializeObject<List<ShopData>>(shopJson);
      foreach (var data in shopDataList)
      {
        if (!dictShopData.ContainsKey(data.shopIdx))
        {
          dictShopData.Add(data.shopIdx, data);
        }
        else
          Debug.Log("Shop Table Load Error Index : " + data.shopIdx);
      }
      Debug.Log("Shop Table Load Success");
    }
  }

  public ShopData GetShopData(int _key)
  {
    if (dictShopData.ContainsKey(_key))
    {
      return dictShopData[_key];
    }
    else
    {
      Debug.Log($"No Shop Data key : {_key}");
      return default;
    }
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }
}
