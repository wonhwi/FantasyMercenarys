using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class EquipmentMappingTable : LazySingleton<EquipmentMappingTable>, ITableFactory
{
  public Dictionary<int, List<EquipmentMappingData>> dictEquipmentMappingData = new Dictionary<int, List<EquipmentMappingData>>();

  public void Load()
  {
    dictEquipmentMappingData.Clear();

    string equipmentMappingJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.EquipmentMapping, out equipmentMappingJson))
    {
      List<EquipmentMappingData> equipmentMappingDataList = JsonConvert.DeserializeObject<List<EquipmentMappingData>>(equipmentMappingJson);
      foreach (var data in equipmentMappingDataList)
      {
        if (!dictEquipmentMappingData.ContainsKey(data.equipItemIdx))
          dictEquipmentMappingData.Add(data.equipItemIdx, new List<EquipmentMappingData>());

        dictEquipmentMappingData[data.equipItemIdx].Add(data);
      }
      Debug.Log("EquipmentMapping Table Load Success");

      Debug.LogError(JsonConvert.SerializeObject(dictEquipmentMappingData, Formatting.Indented));
    }
  }

  public List<EquipmentMappingData> GetEquipmentMappingDataList(int itemIdx)
  {
    if (dictEquipmentMappingData.ContainsKey(itemIdx))
    {
      return dictEquipmentMappingData[itemIdx];
    }
    else
    {
      Debug.Log($"No EquipmentMapping Data key : {itemIdx}");
      return null;
    }
  }

  public EquipmentMappingData GetEquipmentMappingData(int itemIdx)
  {
    List<EquipmentMappingData> equipmentMappingDataList = GetEquipmentMappingDataList(itemIdx);

    int jobIdx = GameDataManager.getInstance.userInfoModel.GetJobCode();

    if (equipmentMappingDataList != null)
    {
      int listCount = equipmentMappingDataList.Count;
      
      for (int i = 0; i < listCount; i++)
      {
        if (equipmentMappingDataList[i].jobIdx == jobIdx)
          return equipmentMappingDataList[i];
      }


    }

    Debug.Log($"{itemIdx} 의 Mapping Data가 없습니다");
    return default;
  }


  public void Reload()
  {
    throw new System.NotImplementedException();
  }
}
