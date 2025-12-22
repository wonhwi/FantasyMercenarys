using Assets.ZNetwork;
using Assets.ZNetwork.Manager;
using FantasyMercenarys.Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static RestPacket;

public class LampLvInfoTable : LazySingleton<LampLvInfoTable>, ITableFactory
{
    public Dictionary<int, LampLvInfoData> dictLampLvInfoData = new Dictionary<int, LampLvInfoData>();

    public void Load()
    {
        dictLampLvInfoData.Clear();

        string lampLvInfoJson;
        if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.LampLvInfo, out lampLvInfoJson))
        {
            List<LampLvInfoData> lampLvDataList = JsonConvert.DeserializeObject<List<LampLvInfoData>>(lampLvInfoJson);
            foreach (var data in lampLvDataList)
            {
                if (!dictLampLvInfoData.ContainsKey(data.shopLvIdx))
                {
                    dictLampLvInfoData.Add(data.shopLvIdx, data);
                }
                else
                    Debug.Log("LampLvInfo Table Load Error Index : " + data.shopLvIdx);
            }
            Debug.Log("LampLvInfo Table Load Success");


        }

        
    Debug.Log("LampGachaWeightData Table Load Success");
    }


    public LampLvInfoData GetEquipmentGachaLevelData(int _key)
    {
        if (dictLampLvInfoData.ContainsKey(_key))
        {
            return dictLampLvInfoData[_key];
        }
        else
        {
            Debug.Log($"No LampLvInfo Data key : {_key}");
            throw new System.NotImplementedException();
        }
    }

  public List<InvenData> PickLampItemList(int shopLvIdx, int pickCount)
  {
    if (!dictLampLvInfoData.ContainsKey(shopLvIdx) || !dictLampLvInfoData.ContainsKey(shopLvIdx))
      throw new ArgumentException($"{shopLvIdx}: Incorrect ShopIdx.");

    List<InvenData> pickItemList = new List<InvenData>();
    var filteredItems = GachaWeightTable.getInstance.GetGachaWeightDataList(shopLvIdx);

    MNMRandom random = GameDataManager.getInstance.GetLampRandom();

    for (int i = 0; i < pickCount; i++)
    {
      float randomValue = (float)random.Next(0, 10000) * 0.01f;

      //기획서에 계정 레벨 +-10 이라고 명시가 되어있음
      int itemLv = GameDataManager.getInstance.userInfoModel.GetPlayerLv() + random.Next(-10, 10);
      itemLv = Math.Max(itemLv, 1);

      float cumulativeWeight = 0;
      foreach (var item in filteredItems)
      {
        cumulativeWeight += item.weight;
        if (randomValue <= cumulativeWeight)
        {
          var pickedItem = EquipmentItemTable.getInstance.RewardEquipInfo(item.targetIdx, itemLv);
          pickItemList.Add(pickedItem);
          break;
        }
      }
    }
    return pickItemList;
  }

  public void Reload()
    {
        throw new System.NotImplementedException();
    }
}
