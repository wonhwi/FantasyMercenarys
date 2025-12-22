using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class PartnerExploreSlotTable : LazySingleton<PartnerExploreSlotTable>, ITableFactory
{
  public Dictionary<int, PartnerExploreSlotData> dictPartnerExploreSlotData = new Dictionary<int, PartnerExploreSlotData>();

  public void Load()
  {
    dictPartnerExploreSlotData.Clear();
    string exploreDataJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.PartnerExploreSlot, out exploreDataJson))
    {
      List<PartnerExploreSlotData> exploreSlotDataList = JsonConvert.DeserializeObject<List<PartnerExploreSlotData>>(exploreDataJson);
      foreach (var data in exploreSlotDataList)
      {
        if (!dictPartnerExploreSlotData.ContainsKey(data.exploreSlot))
        {
          dictPartnerExploreSlotData.Add(data.exploreSlot, data);
        }

        dictPartnerExploreSlotData[data.exploreSlot] = data;

      }
      Debug.Log("ExploreSlot Table Load Success");
    }
  }

  public PartnerExploreSlotData GetPartnerExploreSlotData(int slotNum)
  {
    if (dictPartnerExploreSlotData.ContainsKey(slotNum))
      return dictPartnerExploreSlotData[slotNum];
    else
      return default;
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }
}
