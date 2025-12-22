using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BreedingTable : LazySingleton<BreedingTable>, ITableFactory
{
  public Dictionary<int, BreedingCreatureData> dictBreedingCreatureData = new Dictionary<int, BreedingCreatureData>();
  public Dictionary<int, BreedingResearchData> dictBreedingResearchData = new Dictionary<int, BreedingResearchData>();


  public void Load()
  {
    dictBreedingCreatureData.Clear();
    dictBreedingResearchData.Clear();

    string breedingCreatureJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.BreedingCreatureInfo, out breedingCreatureJson))
    {
      List<BreedingCreatureData> breedingCreatureDataList = JsonConvert.DeserializeObject<List<BreedingCreatureData>>(breedingCreatureJson);
      foreach (var data in breedingCreatureDataList)
      {
        dictBreedingCreatureData[data.creatureIdx] = data;
      }
      Debug.Log("BreedingCreatureInfo Table Load Success");
    }

    string breedingResearchJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.BreedingResearchInfo, out breedingResearchJson))
    {
      List<BreedingResearchData> breedingResearchDataList = JsonConvert.DeserializeObject<List<BreedingResearchData>>(breedingResearchJson);
      foreach (var data in breedingResearchDataList)
      {
        dictBreedingResearchData[data.vaultLevel] = data;
      }
      Debug.Log("BreedingResearch Table Load Success");

    }
  }

  public Dictionary<int, BreedingCreatureData> GetBreedingCreatureDic()
  {
    return dictBreedingCreatureData;
  }

  public BreedingCreatureData GetCreatureData(int creatureIdx)
  {
    if (dictBreedingCreatureData.ContainsKey(creatureIdx))
      return dictBreedingCreatureData[creatureIdx];
    else
      return default;
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }
}
