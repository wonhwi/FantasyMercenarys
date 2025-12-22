using Assets.ZNetwork.Manager;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;

public class SpineLayerTable : LazySingleton<SpineLayerTable>, ITableFactory
{
  public Dictionary<int, SpineLayerData> dictSpineLayerData = new Dictionary<int, SpineLayerData>();

  public void Load()
  {
    dictSpineLayerData.Clear();
    string spineLayerJson;
    if (TableManager.Instance._tmpTableDataMap.TryGetValue(MetaTableType.SpineLayer, out spineLayerJson))
    {
      List<SpineLayerData> spineLayerDataList = JsonConvert.DeserializeObject<List<SpineLayerData>>(spineLayerJson);
      foreach (var data in spineLayerDataList)
      {
        if (!dictSpineLayerData.ContainsKey(data.spineLayerIdx))
        {
          dictSpineLayerData.Add(data.spineLayerIdx, data);
        }
        else
          Debug.Log("SpineLayer Table Load Error Index : " + data.spineLayerIdx);
      }
      Debug.Log("SpineLayer Table Load Success");
    }
  }

  public SpineLayerData GetSpineLayerData(string spineSkin)
  {
    return dictSpineLayerData.Values.Where(n => n.spineSkin == spineSkin).FirstOrDefault();
  }

  public void Reload()
  {
    throw new System.NotImplementedException();
  }
}
