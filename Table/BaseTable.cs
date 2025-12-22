using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseTable : LazySingleton<BaseTable>, ITableFactory
{
  public Dictionary<int, BaseData> dictBaseData = new Dictionary<int, BaseData>();

  public void Load()
  {
    dictBaseData.Clear();

    string baseJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.Base, out baseJson))
    {
      List<BaseData> baseDataList = JsonConvert.DeserializeObject<List<BaseData>>(baseJson);
      foreach (var data in baseDataList)
      {
        if (!dictBaseData.ContainsKey(data.baseIdx))
        {
          dictBaseData.Add(data.baseIdx, data);
        }
        else
          Debug.Log("Base Table Load Error Index : " + data.baseIdx);
      }
      Debug.Log("Base Table Load Success");
    }

    //Debug.LogError(JsonConvert.SerializeObject(dictBaseData, Formatting.Indented));
  }

  public BaseData GetBaseData(int _key)
  {
    if (dictBaseData.ContainsKey(_key))
    {
      return dictBaseData[_key];
    }
    else
    {
      Debug.Log($"No Base Data Key : {_key}");
      return default;
      throw new System.NotImplementedException();
    }
  }

  public float GetBaseValue(int baseIdx)
  {
    BaseData baseData = GetBaseData(baseIdx);

    if (!baseData.dataUnit.Equals((int)BaseDataUnit.Integer))
      return baseData.value * 0.01f;

    return baseData.value;
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }
}
